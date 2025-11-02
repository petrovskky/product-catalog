using MediatR;
using ProductCatalog.Application.Common.Results;

namespace ProductCatalog.Application.UseCases.Categories.Queries.Get;

public record GetCategoryQuery(Guid Id) : IRequest<Result<CategoryDto>>;