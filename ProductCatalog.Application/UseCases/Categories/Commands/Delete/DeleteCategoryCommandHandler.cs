using MediatR;
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.Common.Results;
using ProductCatalog.Application.Interfaces.Repositories;
using ProductCatalog.Application.Interfaces.Repositories.Categories;

namespace ProductCatalog.Application.UseCases.Categories.Commands.Delete;

public class DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork,
    IUserContext userContext, ILogger<DeleteCategoryCommandHandler> logger) 
    : IRequestHandler<DeleteCategoryCommand, Result>
{
    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
            return Result.Failure("Категория не найдена");
        
        var categoryName = category.Name;

        await categoryRepository.DeleteAsync(request.Id, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation(
            "Пользователь {ActorEmail} удалил категорию '{CategoryName}' (ID: {CategoryId})",
            userContext.Email, categoryName, request.Id);
        
        return Result.Success();
    }
}