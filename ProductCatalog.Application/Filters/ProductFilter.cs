namespace ProductCatalog.Application.Filters;

public class ProductFilter
{
    public string? Name { get; set; }
    public Guid? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}