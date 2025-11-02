using MediatR;
using ProductCatalog.Application.Common.Results;

namespace ProductCatalog.Application.UseCases.Categories.Queries.List;

public record ListCategoryQuery() : IRequest<Result<IEnumerable<CategoryDto>>>;