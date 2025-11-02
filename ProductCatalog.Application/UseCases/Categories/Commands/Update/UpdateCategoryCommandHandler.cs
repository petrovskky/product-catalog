using MediatR;
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.Common.Results;
using ProductCatalog.Application.Interfaces.Repositories;
using ProductCatalog.Application.Interfaces.Repositories.Categories;

namespace ProductCatalog.Application.UseCases.Categories.Commands.Update;

public class UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork,
    IUserContext userContext, ILogger<UpdateCategoryCommandHandler> logger) 
    : IRequestHandler<UpdateCategoryCommand, Result>
{
    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
            return Result.Failure("Категория не найдена");
        
        var oldName = category.Name;
        category.Name = request.NewName.Trim();
        
        category.Name = request.NewName.Trim();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation(
            "Пользователь {ActorEmail} обновил категорию '{OldName}' -> '{NewName}' (ID: {CategoryId})",
            userContext.Email, oldName, category.Name, request.Id);
        
        return Result.Success();
    }
}