using FluentValidation;

namespace ProductCatalog.Api.Dtos.Users;

public class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
{
    public LoginUserRequestValidator()
    {
        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("Email обязателен для заполнения")
            .EmailAddress().WithMessage("Неверный формат email");
        
        RuleFor(u => u.Password)
            .Length(8, 20).WithMessage("Пароль должен содержать от 8 до 20 символов");
    }
}