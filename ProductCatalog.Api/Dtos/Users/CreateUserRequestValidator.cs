using FluentValidation;

namespace ProductCatalog.Api.Dtos.Users;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("Email обязателен для заполнения")
            .EmailAddress().WithMessage("Неверный формат email");

        RuleFor(u => u.Name)
            .Length(2, 20).WithMessage("Имя должно быть от 2 до 20 символов");

        RuleFor(u => u.Role)
            .NotEmpty().WithMessage("Роль обязательна для заполнения")
            .Must(r =>
                "user".Equals(r, StringComparison.OrdinalIgnoreCase) ||
                "prouser".Equals(r, StringComparison.OrdinalIgnoreCase) ||
                "admin".Equals(r, StringComparison.OrdinalIgnoreCase))
            .WithMessage("Указанная роль не найдена");

        RuleFor(u => u.Password)
            .Length(8, 20).WithMessage("Пароль должен содержать от 8 до 20 символов")
            .Matches("[A-Z]").WithMessage("Пароль должен содержать хотя бы одну заглавную букву")
            .Matches("[a-z]").WithMessage("Пароль должен содержать хотя бы одну строчную букву")
            .Matches("[0-9]").WithMessage("Пароль должен содержать хотя бы одну цифру");
    }
}