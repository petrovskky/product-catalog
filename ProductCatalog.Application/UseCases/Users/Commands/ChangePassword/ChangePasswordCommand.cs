using MediatR;
using ProductCatalog.Application.Common.Results;

namespace ProductCatalog.Application.UseCases.Users.Commands.ChangePassword;

public record ChangePasswordCommand(Guid Id, string NewPassword) : IRequest<Result>;