using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ProductCatalog.Api.Middleware;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails();
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        
        switch (exception)
        {
            case UnauthorizedAccessException:
                problemDetails.Status = StatusCodes.Status423Locked;
                problemDetails.Title = "Аккаунт заблокирован";
                problemDetails.Detail = "Ваш аккаунт был заблокирован. Обратитесь к администратору.";
                logger.LogWarning("Попытка входа заблокированным пользователем: {Message}", exception.Message);
                break;

            default:
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Ошибка сервера";
                problemDetails.Detail = "Произошла непредвиденная ошибка. Пожалуйста, попробуйте позже.";
                logger.LogError(exception, "Необработанное исключение: {Message}", exception.Message);
                break;
        }

        problemDetails.Extensions["traceId"] = traceId;

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}