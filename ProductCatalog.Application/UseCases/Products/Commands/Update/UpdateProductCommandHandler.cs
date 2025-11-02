using MediatR;
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.Common.Results;
using ProductCatalog.Application.Interfaces.Repositories;
using ProductCatalog.Application.Interfaces.Repositories.Categories;
using ProductCatalog.Application.Interfaces.Repositories.Products;

namespace ProductCatalog.Application.UseCases.Products.Commands.Update;

public class UpdateProductCommandHandler(IProductRepository productRepository,
    ICategoryRepository categoryRepository, IUnitOfWork unitOfWork,
    IUserContext userContext, ILogger<UpdateProductCommandHandler> logger)  
    : IRequestHandler<UpdateProductCommand, Result>
{
    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.ExistsByIdAsync(request.CategoryId, 
            cancellationToken);
        if (!category)
            return Result.Failure("Категория не найдена");
        
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
            return Result.Failure("Продукт не найден");
        
        var oldName = product.Name;
        
        product.Name = request.Name?.Trim() ?? product.Name;
        product.Description = request.Description?.Trim() ?? product.Description;
        product.Price = request.Price;
        product.CategoryId = request.CategoryId;
        product.Note = request.Note?.Trim();
        product.SpecialNote = request.SpecialNote?.Trim();

        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation(
            "Пользователь {ActorEmail} обновил продукт '{OldName}' (ID: {ProductId})",
            userContext.Email, oldName, request.Id);
        
        return Result.Success();
    }
}