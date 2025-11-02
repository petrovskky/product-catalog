using System.Diagnostics;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.Common.Results;
using ProductCatalog.Application.Interfaces.Repositories.Users;

namespace ProductCatalog.Application.Common.Behaviors;

public class BlockUserPipelineBehavior<TRequest, TResponse>(
    IUserReadRepository userReadRepository,
    IUserContext userContext,
    ILogger<BlockUserPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated || !userContext.Id.HasValue)
            return await next(cancellationToken);
        
        var userId = userContext.Id.Value;
        var name = userContext.Name ?? "Неизвестно";
        var email = userContext.Email ?? "Неизвестно";
        var role = userContext.Role ?? "Неизвестно";

        try
        {
            var isBlocked = await userReadRepository.IsBlockedByIdAsync(userId, cancellationToken);
            
            if (isBlocked)
            {
                logger.LogWarning(
                    "Пользователь {Name} ({Email}, ID: {UserId}, Роль: {Role}) заблокирован — доступ к {RequestType} запрещён",
                    name, email, userId, role, typeof(TRequest).Name);

                throw new UnauthorizedAccessException("Пользователь заблокирован");
            }
        }
        catch (Exception ex) when (ex is not UnauthorizedAccessException)
        {
            logger.LogError(
                ex,
                "Ошибка базы данных при проверке статуса блокировки пользователя {UserId}", 
                userId);
        }

        return await next(cancellationToken);
    }
}