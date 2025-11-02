using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.Common.Results;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Application.Interfaces.Repositories.Users;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.UseCases.Auth.Login;

public class LoginUserCommandHandler(IUserReadRepository userRepository, IPasswordHasher<User> passwordHasher,
    IJwtService jwtService, ILogger<LoginUserCommandHandler> logger) 
    : IRequestHandler<LoginUserCommand, Result<string>>
{
    public async Task<Result<string>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
            return Result<string>.Failure("Неверный email или пароль");
        
        if (user.IsBlocked)
            return Result<string>.Failure("Ваш аккаунт заблокирован. Обратитесь к администратору.");

        var result = passwordHasher.VerifyHashedPassword(user,  user.PasswordHash, request.Password);

        if (result != PasswordVerificationResult.Success)
            return Result<string>.Failure("Неверный email или пароль");

        var token = jwtService.GenerateToken(user);

        logger.LogInformation(
            "Пользователь {UserEmail} вошёл в систему",
            user.Email);

        return token;
    }
}