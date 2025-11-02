using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.Common.Results;
using ProductCatalog.Application.Common.Services;
using ProductCatalog.Application.Interfaces.Repositories;
using ProductCatalog.Application.Interfaces.Repositories.Users;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.UseCases.Users.Commands.ChangePassword;

public class ChangePasswordCommandHandler(IUserRepository userRepository, 
    IPasswordHasher<User> passwordHasher, IUnitOfWork unitOfWork,
    IUserContext userContext, ILogger<ChangePasswordCommandHandler> logger) 
    : IRequestHandler<ChangePasswordCommand, Result>
{
    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var target = await userRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (target == null)
            return Result.Failure("Пользователь не найден");
        
        if (RolePrivileges.IsAdmin(target.Role) && request.Id != userContext.Id)
            return Result.Failure("Операция недоступна");
        
        target.PasswordHash = passwordHasher.HashPassword(target, request.NewPassword);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation(
            request.Id == userContext.Id 
                ? "Пользователь {ActorName} сменил свой пароль"
                : "Пользователь {ActorName} сменил пароль пользователю {@Target}",
            userContext.Email, target.Email);
        
        return Result.Success();
    }
}