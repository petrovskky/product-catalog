using FluentValidation;

namespace ProductCatalog.Api.Dtos.Categories;

public class CategoryRequestValidator : AbstractValidator<CategoryRequest>
{
    public CategoryRequestValidator()
    {
        RuleFor(c => c.Name)
            .Length(2, 100).WithMessage("Название категории должно содержать от 2 до 100 символов");
    }
}