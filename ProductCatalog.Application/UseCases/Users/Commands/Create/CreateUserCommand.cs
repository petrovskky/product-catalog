using MediatR;
using ProductCatalog.Application.Common.Results;

namespace ProductCatalog.Application.UseCases.Users.Commands.Create;

public record CreateUserCommand(string Email, string Name, string Role, string Password) 
    : IRequest<Result<Guid>>;