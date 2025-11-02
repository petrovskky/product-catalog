using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}