using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.Common.Results;
using ProductCatalog.Application.Interfaces.Repositories;
using ProductCatalog.Application.Interfaces.Repositories.Users;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.UseCases.Auth.Register;

public class RegisterUserCommandHandler(
    IUserRepository userRepository, IPasswordHasher<User> passwordHasher, IUnitOfWork unitOfWork,
    ILogger<RegisterUserCommandHandler> logger) : IRequestHandler<RegisterUserCommand, Result>
{
    public async Task<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (await userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
            return Result.Failure("Пользователь с таким email уже существует");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Name = request.Name,
            Role = "User",
        };
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
        
        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation(
            "Зарегистрирован новый пользователь {UserEmail}",
            user.Email);
        
        return Result.Success();
    }
}