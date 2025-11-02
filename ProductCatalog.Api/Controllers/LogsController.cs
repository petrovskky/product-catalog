using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProductCatalog.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class LogsController(IWebHostEnvironment env) : ControllerBase
{
    private string? _currentLogFile;
    private long _position;
    private FileStream? _stream;
    private StreamReader? _reader;
    private DateTime _lastLogCheck = DateTime.MinValue;
    private bool _noLogsMessageSent;
    
    private readonly StringBuilder _buffer = new(4096);

    private static readonly Dictionary<string, string> ActionMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["создал"] = "create",
        ["зарегистрирован"] = "create",
        ["удалил"] = "delete",
        ["обновил"] = "update",
        ["сменил"] = "update",
        ["заблокировал"] = "block",
        ["разблокировал"] = "unblock",
        ["вошел"] = "login"
    };
    
    private static readonly Regex RemoveFirstParentheses = new(@"(?<=\[[A-Z]+\]\s*)\([^)]*\)\s*", RegexOptions.Compiled);
    private static readonly Regex RemoveLogLevel = new(@"\[[A-Z]+\]\s*", RegexOptions.Compiled);
    private static readonly Regex NormalizeSpaces = new(@"\s{2,}", RegexOptions.Compiled);

    [HttpGet("stream")]
    public async Task Stream(CancellationToken cancellationToken)
    {
        Response.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";
        Response.Headers.Connection = "keep-alive";

        var logDir = Path.Combine(env.ContentRootPath, "logs");
        if (!Directory.Exists(logDir))
        {
            try 
            {
                Directory.CreateDirectory(logDir);
            }
            catch (Exception ex)
            {
                await SendEvent("error", $"Ошибка создания папки для логов: {ex.Message}", cancellationToken);
                return;
            }
        }

        try
        {
            await InitializeLogFile(logDir, 10, cancellationToken);
            
            if (!_noLogsMessageSent)
            {
                await SendEvent(
                    "info",
                    "Соединение установлено.",
                    cancellationToken);
                _noLogsMessageSent = true;
            }
            
            while (!cancellationToken.IsCancellationRequested)
            {
                await ReadNewLines(cancellationToken);
                
                if ((DateTime.UtcNow - _lastLogCheck).TotalSeconds > 15)
                {
                    _lastLogCheck = DateTime.UtcNow;
                    var latestFile = GetLatestLogFile(logDir);
                    if (latestFile != _currentLogFile)
                    {
                        await SwitchToNewFile(latestFile!, cancellationToken);
                    }
                }
                
                await Task.Delay(1500, cancellationToken);
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            await SendEvent("error", $"Ошибка сервера: {ex.Message}", cancellationToken);
        }
        finally
        {
            _reader?.Dispose();
            _stream?.Dispose();
        }
    }

    private async Task InitializeLogFile(string logDir, int tailLines, CancellationToken ct)
    {
        var latestFile = GetLatestLogFile(logDir);
        if (latestFile == null)
        {
            return;
        }

        _currentLogFile = latestFile;
        _position = 0;

        _stream = new FileStream(latestFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        _reader = new StreamReader(_stream);

        await SendTail(latestFile, tailLines, ct);
    }

    private async Task SwitchToNewFile(string newFile, CancellationToken ct)
    {
        await SendEvent("info", "===== Переключение на новый лог-файл =====", ct);

        _reader?.Dispose();
        _stream?.Dispose();

        _currentLogFile = newFile;
        _position = 0;

        _stream = new FileStream(newFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        _reader = new StreamReader(_stream);

        await SendTail(newFile, 10, ct);
    }

    private static string? GetLatestLogFile(string logDir) =>
        Directory.GetFiles(logDir, "log-*.txt")
                 .OrderByDescending(f => f)
                 .FirstOrDefault();

    private async Task SendTail(string filePath, int count, CancellationToken ct)
    {
        try
        {
            var lines = new Queue<string>(count);

            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(fs);

            string? line;
            while ((line = await reader.ReadLineAsync(ct)) != null)
            {
                if (lines.Count == count) lines.Dequeue();
                lines.Enqueue(line);
            }

            foreach (var l in lines)
                await SendFilteredEvent(l, ct);
        }
        catch { }
    }

    private async Task ReadNewLines(CancellationToken ct)
    {
        if (_reader == null || _stream == null || _currentLogFile == null) return;

        try
        {
            if (_stream.Length <= _position) return;

            _stream.Seek(_position, SeekOrigin.Begin);

            string? line;
            while ((line = await _reader.ReadLineAsync(ct)) != null)
            {
                await SendFilteredEvent(line, ct);
            }

            _position = _stream.Position;
        }
        catch (IOException) when (new FileInfo(_currentLogFile).Length < _position)
        {
            _position = 0;
        }
    }

    private async Task SendFilteredEvent(string line, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(line)) return;

        foreach (var kvp in ActionMap)
        {
            if (line.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
            {
                var formattedLine = FormatLogLine(line);
                await SendEvent(kvp.Value, formattedLine, ct);
                _noLogsMessageSent = true;
                return;
            }
        }
    }

    private static string FormatLogLine(string line)
    {
        try
        {
            var cleaned = RemoveFirstParentheses.Replace(line, "");
            cleaned = RemoveLogLevel.Replace(cleaned, "");
            cleaned = NormalizeSpaces.Replace(cleaned, " ").Trim();
            return cleaned;
        }
        catch
        {
            return line;
        }
    }

    private async Task SendEvent(string eventName, string data, CancellationToken ct)
    {
        _buffer.Append($"event: {eventName}\ndata: {data}\n\n");
        var bytes = Encoding.UTF8.GetBytes(_buffer.ToString());
        await Response.Body.WriteAsync(bytes, ct);
        await Response.Body.FlushAsync(ct);
        _buffer.Clear();
    }
}