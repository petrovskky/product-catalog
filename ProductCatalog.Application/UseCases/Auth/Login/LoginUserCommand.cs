using MediatR;
using ProductCatalog.Application.Common.Results;

namespace ProductCatalog.Application.UseCases.Auth.Login;

public record LoginUserCommand(string Email, string Password) : IRequest<Result<string>>;