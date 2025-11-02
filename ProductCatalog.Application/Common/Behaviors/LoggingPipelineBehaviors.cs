using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.Common.Results;

namespace ProductCatalog.Application.Common.Behaviors;

public class LoggingPipelineBehavior<TRequest, TResponse>(
    ILogger<LoggingPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private const long SlowRequestThresholdMs = 1000;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        logger.LogInformation(
            "Начало обработки запроса {RequestName} с данными {@Request}",
            requestName, 
            request);

        try
        {
            var response = await next(cancellationToken);
            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            LogResult(response, requestName, elapsedMs);
            LogIfSlow(requestName, elapsedMs);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            logger.LogError(
                ex, 
                "Необработанное исключение в запросе {RequestName} через {ElapsedMs}мс", 
                requestName, 
                elapsedMs);
            LogIfSlow(requestName, elapsedMs);
            
            throw;
        }
    }

    private void LogResult(TResponse response, string requestName, long elapsedMs)
    {
        if (response.IsSuccess)
        {
            logger.LogInformation(
                "Запрос {RequestName} успешно завершен за {ElapsedMs}мс",
                requestName, 
                elapsedMs);
        }
        else
        {
            logger.LogWarning(
                "Запрос {RequestName} завершился с ошибкой через {ElapsedMs}мс: {Error}",
                requestName,
                elapsedMs,
                response.Error);
        }
    }

    private void LogIfSlow(string requestName, long elapsedMs)
    {
        if (elapsedMs > SlowRequestThresholdMs)
        {
            logger.LogWarning(
                "Медленный запрос {RequestName} выполнялся {ElapsedMs}мс (порог: {ThresholdMs}мс)",
                requestName,
                elapsedMs,
                SlowRequestThresholdMs);
        }
    }
}
