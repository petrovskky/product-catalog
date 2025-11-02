using MediatR;
using ProductCatalog.Application.Common.Results;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.UseCases.Products.Queries.Get;

public record GetProductQuery(Guid Id, string UserRole) : IRequest<Result<ProductDto>>;