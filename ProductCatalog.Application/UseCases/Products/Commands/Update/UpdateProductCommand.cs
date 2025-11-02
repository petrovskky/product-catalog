using MediatR;
using ProductCatalog.Application.Common.Results;

namespace ProductCatalog.Application.UseCases.Products.Commands.Update;

public record UpdateProductCommand(
    Guid Id, string Name, Guid CategoryId, string Description, decimal Price, 
    string? Note = null, string? SpecialNote = null) 
    : IRequest<Result>;