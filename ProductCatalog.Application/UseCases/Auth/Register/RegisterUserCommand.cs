using MediatR;
using ProductCatalog.Application.Common.Results;

namespace ProductCatalog.Application.UseCases.Auth.Register;

public record RegisterUserCommand(
    string Email, string Name, string Password) : IRequest<Result>;