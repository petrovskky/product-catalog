namespace ProductCatalog.Api.Dtos.Products;

public record ProductRequest(string Name, Guid CategoryId, string Description, decimal Price, 
    string? Note = null, string? SpecialNote = null);