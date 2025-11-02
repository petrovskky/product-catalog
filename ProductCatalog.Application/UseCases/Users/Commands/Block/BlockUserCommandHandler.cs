using MediatR;
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.Common.Results;
using ProductCatalog.Application.Common.Services;
using ProductCatalog.Application.Interfaces.Repositories;
using ProductCatalog.Application.Interfaces.Repositories.Users;

namespace ProductCatalog.Application.UseCases.Users.Commands.Block;

public class BlockUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork,
    IUserContext userContext, ILogger<BlockUserCommandHandler> logger) 
    : IRequestHandler<BlockUserCommand, Result>
{
    public async Task<Result> Handle(BlockUserCommand request, CancellationToken cancellationToken)
    {
        var target = await userRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (target == null)
            return Result.Failure("Пользователь не найден");
        
        if (RolePrivileges.IsAdmin(target.Role))
            return Result.Failure("Операция недоступна");
        
        var wasBlocked = target.IsBlocked;
        target.IsBlocked = !target.IsBlocked;
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            wasBlocked
                ? "Пользователь {ActorEmail} разблокировал пользователя {TargetEmail}"
                : "Пользователь {ActorEmail} заблокировал пользователя {TargetEmail}",
            userContext.Email, target.Email);
        
        return Result.Success();
    }
}