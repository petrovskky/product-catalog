using MediatR;
using ProductCatalog.Application.Common.Results;

namespace ProductCatalog.Application.UseCases.Currencies.Queries.GetPrice;

public record GetRateQuery(int CurId) : IRequest<Result<decimal>>;