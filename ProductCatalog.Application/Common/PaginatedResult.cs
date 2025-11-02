namespace ProductCatalog.Application.Common;

public class PaginatedResult<T>(T data, int totalCount)
{
    public T Data { get; } = data;
    public int TotalCount { get; } = totalCount;
}