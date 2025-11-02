using MediatR;
using ProductCatalog.Application.Common.Results;

namespace ProductCatalog.Application.UseCases.Users.Commands.Block;

public record BlockUserCommand(Guid Id) : IRequest<Result>;