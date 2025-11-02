namespace ProductCatalog.Application.Common.Interfaces;

public interface ICurrencyApiService
{
    Task<decimal> GetRateAsync(int curId, CancellationToken cancellationToken);
}