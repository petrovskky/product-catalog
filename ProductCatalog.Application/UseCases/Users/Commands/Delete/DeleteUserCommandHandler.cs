using MediatR;
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.Common.Results;
using ProductCatalog.Application.Common.Services;
using ProductCatalog.Application.Interfaces.Repositories;
using ProductCatalog.Application.Interfaces.Repositories.Users;

namespace ProductCatalog.Application.UseCases.Users.Commands.Delete;

public class DeleteUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork,
    IUserContext userContext, ILogger<DeleteUserCommandHandler> logger)
    : IRequestHandler<DeleteUserCommand, Result>
{
    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var target = await userRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (target == null)
            return Result.Failure("Пользователь не найден");
        
        if (RolePrivileges.IsAdmin(target.Role) && request.Id != userContext.Id)
            return Result.Failure("Операция недоступна");

        await userRepository.DeleteAsync(request.Id, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation(
            request.Id == userContext.Id
                ? "Пользователь {ActorEmail} удалил свой аккаунт"
                : "Пользователь {ActorEmail} удалил пользователя {TargetEmail}",
            userContext.Email, target.Email);

        return Result.Success();
    }
}