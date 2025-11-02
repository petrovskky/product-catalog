using MediatR;
using ProductCatalog.Application.Common.Results;

namespace ProductCatalog.Application.UseCases.Categories.Commands.Delete;

public record DeleteCategoryCommand(Guid Id) : IRequest<Result>;