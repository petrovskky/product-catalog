using MediatR;
using ProductCatalog.Application.Common.Results;

namespace ProductCatalog.Application.UseCases.Categories.Commands.Create;

public record CreateCategoryCommand(string Name) : IRequest<Result<Guid>>;