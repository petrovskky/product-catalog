using FluentValidation;

namespace ProductCatalog.Api.Dtos.Products;

public class ProductRequestValidator : AbstractValidator<ProductRequest>
{
    public ProductRequestValidator()
    {
        RuleFor(p => p.Name)
            .Length(2, 100).WithMessage("Название продукта должно содержать от 2 до 100 символов");
    
        RuleFor(p => p.CategoryId)
            .NotEqual(Guid.Empty).WithMessage("Идентификатор категории обязателен");
    
        RuleFor(p => p.Description)
            .Length(2, 300).WithMessage("Описание должно содержать от 2 до 300 символов");
    
        RuleFor(p => p.Price)
            .GreaterThan(0).WithMessage("Цена должна быть больше 0")
            .LessThanOrEqualTo(1000000).WithMessage("Цена должна быть меньше или равна 1000000");
    
        RuleFor(p => p.Note)
            .Length(2, 200).WithMessage("Примечание должно содержать от 2 до 200 символов");
    
        RuleFor(p => p.SpecialNote)
            .Length(2, 200).WithMessage("Специальное примечание должно содержать от 2 до 200 символов");
    }
}