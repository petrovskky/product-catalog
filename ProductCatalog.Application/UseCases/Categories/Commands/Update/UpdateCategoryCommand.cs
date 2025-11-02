using MediatR;
using ProductCatalog.Application.Common.Results;

namespace ProductCatalog.Application.UseCases.Categories.Commands.Update;

public record UpdateCategoryCommand(Guid Id, string NewName) : IRequest<Result>;