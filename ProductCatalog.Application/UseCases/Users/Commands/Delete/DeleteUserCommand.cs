using MediatR;
using ProductCatalog.Application.Common.Results;

namespace ProductCatalog.Application.UseCases.Users.Commands.Delete;

public record DeleteUserCommand(Guid Id) : IRequest<Result>;