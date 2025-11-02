using MediatR;
using ProductCatalog.Application.Common.Results;

namespace ProductCatalog.Application.UseCases.Users.Queries.List;

public record ListUserQuery() : IRequest<Result<IEnumerable<UserDto>>>;