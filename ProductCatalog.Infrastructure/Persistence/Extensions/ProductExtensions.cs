using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Application.Common;
using ProductCatalog.Application.Filters;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure.Persistence.Extensions;

public static class ProductExtensions
{
    public static IQueryable<Product> Filter(this IQueryable<Product> query, ProductFilter filter)
    {
        if (!string.IsNullOrEmpty(filter.Name))
            query = query.Where(p => EF.Functions
                .ILike(p.Name, $"%{filter.Name}%"));
        
        if (filter.CategoryId != null)
            query = query.Where(p => p.CategoryId == filter.CategoryId);
        
        if (filter.MinPrice != null)
            query = query.Where(p => p.Price >= filter.MinPrice);
        
        if (filter.MaxPrice != null)
            query = query.Where(p => p.Price <= filter.MaxPrice);
        
        return query;
    }

    public static IQueryable<Product> Sort(this IQueryable<Product> query, SortParams sortParams)
    {
        return sortParams.Direction == SortDirection.Desc 
            ? query.OrderByDescending(GetKeySelector(sortParams.OrderBy))
            : query.OrderBy(GetKeySelector(sortParams.OrderBy));
    }

    public static async Task<PaginatedResult<IEnumerable<Product>>> ToPaginatedAsync(this IQueryable<Product> query, PageParams pageParams)
    {
        var count = await query.CountAsync();
        if (count == 0)
            return new PaginatedResult<IEnumerable<Product>>([], 0);
        
        var page = pageParams.Page ?? 1;
        var pageSize = pageParams.PageSize ?? 10;
        var result = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedResult<IEnumerable<Product>>(result, count);

    }

    private static Expression<Func<Product, object>> GetKeySelector(string? orderBy)
    {
        if (string.IsNullOrEmpty(orderBy))
            return p => p.Name;

        return orderBy switch
        {
            nameof(Product.Price) => p => p.Price,
            _ => p => p.Name
        };
    }
}