using MediatR;
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.Common.Results;
using ProductCatalog.Application.Common.Services;
using ProductCatalog.Application.Interfaces.Repositories;
using ProductCatalog.Application.Interfaces.Repositories.Categories;
using ProductCatalog.Application.Interfaces.Repositories.Products;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.UseCases.Products.Commands.Create;

public class CreateProductCommandHandler(IProductRepository productRepository, 
    ICategoryRepository categoryRepository, IUnitOfWork unitOfWork,
    IUserContext userContext, ILogger<CreateProductCommandHandler> logger) 
    : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var categoryExists = await categoryRepository.ExistsByIdAsync(
            request.CategoryId, cancellationToken);

        if (!categoryExists)
            return Result<Guid>.Failure("Категория не найдена");
        
        var isPrivileged = RolePrivileges.HasPrivilegedAccess(request.UserRole);
        
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            CategoryId = request.CategoryId,
            Description = request.Description.Trim(),
            Price = request.Price,
            Note = request.Note?.Trim(),
            SpecialNote = isPrivileged ? request.SpecialNote?.Trim() : null
        };

        await productRepository.AddAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation(
            "Пользователь {ActorEmail} создал продукт '{ProductName}' (ID: {ProductId})",
            userContext.Email, product.Name, product.Id);
        
        return product.Id;
    }
}