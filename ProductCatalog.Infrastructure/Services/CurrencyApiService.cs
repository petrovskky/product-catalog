using System.Text.Json;
using ProductCatalog.Application.Common.Interfaces;

namespace ProductCatalog.Infrastructure.Services;

public class CurrencyApiService(HttpClient httpClient) : ICurrencyApiService
{
    public async Task<decimal> GetRateAsync(int curId, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"rates/{curId}", cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
            
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        return root.GetProperty("Cur_OfficialRate").GetDecimal();
    }
}