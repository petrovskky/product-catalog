using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.Common.Results;
using ProductCatalog.Application.Interfaces.Repositories;
using ProductCatalog.Application.Interfaces.Repositories.Users;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.UseCases.Users.Commands.Create;

public class CreateUserCommandHandler(
    IUserRepository userRepository, IPasswordHasher<User> passwordHasher, IUnitOfWork unitOfWork,
    IUserContext userContext, ILogger<CreateUserCommandHandler> logger) 
    : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateUserCommand request, 
        CancellationToken cancellationToken)
    {
        if (await userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
            return Result<Guid>.Failure("Пользователь с таким email уже существует");
        
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Name = request.Name,
            Role = request.Role,
        };
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
        
        await userRepository.AddAsync(user, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation(
            "Пользователь {ActorEmail} создал нового пользователя {TargetEmail}",
            userContext.Email, user.Email);
        
        return user.Id;
    }
}