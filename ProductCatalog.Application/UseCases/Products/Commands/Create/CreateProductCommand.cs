using MediatR;
using ProductCatalog.Application.Common.Results;

namespace ProductCatalog.Application.UseCases.Products.Commands.Create;

public record CreateProductCommand(
    string UserRole, string Name, Guid CategoryId, string Description, decimal Price, 
    string? Note = null, string? SpecialNote = null) 
    : IRequest<Result<Guid>>;