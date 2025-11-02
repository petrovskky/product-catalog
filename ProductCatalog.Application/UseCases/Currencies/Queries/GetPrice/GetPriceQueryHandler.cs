using System.Text.Json;
using MediatR;
using ProductCatalog.Application.Common.Results;
using ProductCatalog.Application.Common.Interfaces;

namespace ProductCatalog.Application.UseCases.Currencies.Queries.GetPrice;

public class GetPriceQueryHandler(ICurrencyApiService currencyApiService) 
    : IRequestHandler<GetRateQuery, Result<decimal>>
{
    private static readonly int[] AllowedCurrencyIds = [431];
    
    public async Task<Result<decimal>> Handle(GetRateQuery request, CancellationToken cancellationToken)
    {
        if (!AllowedCurrencyIds.Contains(request.CurId))
            return Result<decimal>.Failure("Неподдерживаемая валюта");
        
        try
        {
            return await currencyApiService.GetRateAsync(request.CurId, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            return Result<decimal>.Failure($"Ошибка сервиса валют: {ex.Message}");
        }
        catch (TaskCanceledException)
        {
            return Result<decimal>.Failure("Превышено время ожидания сервиса валют");
        }
        catch (JsonException)
        {
            return Result<decimal>.Failure("Ошибка преобразования данных от сервиса валют");
        }
        catch (Exception)
        {
            return Result<decimal>.Failure("Не удалось получить курс валюты");
        }
    }
}