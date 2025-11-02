using FluentValidation;

namespace ProductCatalog.Api.Dtos.Users;

public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("Email обязателен для заполнения")
            .EmailAddress().WithMessage("Неверный формат email");

        RuleFor(u => u.Name)
            .Length(2, 20).WithMessage("Имя должно содержать от 2 до 20 символов");

        RuleFor(u => u.Password)
            .Length(8, 20).WithMessage("Пароль должен содержать от 8 до 20 символов")
            .Matches("[A-Z]").WithMessage("Пароль должен содержать хотя бы одну заглавную букву")
            .Matches("[a-z]").WithMessage("Пароль должен содержать хотя бы одну строчную букву")
            .Matches("[0-9]").WithMessage("Пароль должен содержать хотя бы одну цифру");
    }
}