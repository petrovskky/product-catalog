using MediatR;
using ProductCatalog.Application.Common.Results;

namespace ProductCatalog.Application.UseCases.Users.Queries.Get;

public record GetUserByIdQuery(Guid id) : IRequest<Result<UserDto>>;