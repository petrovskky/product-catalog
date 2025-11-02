using MediatR;
using Microsoft.Extensions.Logging;
using ProductCatalog.Application.Common.Interfaces;
using ProductCatalog.Application.Common.Results;
using ProductCatalog.Application.Interfaces.Repositories;
using ProductCatalog.Application.Interfaces.Repositories.Products;

namespace ProductCatalog.Application.UseCases.Products.Commands.Delete;

public class DeleteProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork,
    IUserContext userContext, ILogger<DeleteProductCommandHandler> logger) 
    : IRequestHandler<DeleteProductCommand, Result>
{
    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
            return Result.Failure("Продукт не найден");
        
        var productName = product.Name;

        await productRepository.DeleteAsync(request.Id, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation(
            "Пользователь {ActorEmail} удалил продукт '{ProductName}' (ID: {ProductId})",
            userContext.Email, productName, request.Id);
        
        return Result.Success();
    }
}