using MediatR;
using ProductCatalog.Application.Common;
using ProductCatalog.Application.Common.Results;
using ProductCatalog.Application.Filters;

namespace ProductCatalog.Application.UseCases.Products.Queries.List;

public record ListProductQuery(string UserRole, ProductFilter ProductFilter, SortParams SortParams, 
    PageParams PageParams) 
    : IRequest<Result<PaginatedResult<IEnumerable<ProductDto>>>>;