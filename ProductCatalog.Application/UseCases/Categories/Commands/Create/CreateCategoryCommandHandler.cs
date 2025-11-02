using MediatR;
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.Common.Results;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Application.Interfaces.Repositories;
using ProductCatalog.Application.Interfaces.Repositories.Categories;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.UseCases.Categories.Commands.Create;

public class CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork,
    IUserContext userContext, ILogger<CreateCategoryCommandHandler> logger) 
    : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, 
        CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim()
        };
        
        await categoryRepository.AddAsync(category, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation(
            "Пользователь {ActorEmail} создал категорию '{CategoryName}' (ID: {CategoryId})",
            userContext.Email, category.Name, category.Id);
            
        return category.Id;
    }
}