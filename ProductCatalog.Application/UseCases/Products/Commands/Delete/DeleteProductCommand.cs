using MediatR;
using ProductCatalog.Application.Common.Results;

namespace ProductCatalog.Application.UseCases.Products.Commands.Delete;

public record DeleteProductCommand(Guid Id) : IRequest<Result>;