namespace ProductCatalog.Application.UseCases.Products;

public record ProductDto(Guid Id, string Name, Guid CategoryId, string CategoryName, string Description, 
    decimal Price, string? Note = null, string? SpecialNote = null);