using MediatR;
using ProductCatalog.Application.Common.Results;
using ProductCatalog.Application.Interfaces.Repositories.Users;

namespace ProductCatalog.Application.UseCases.Users.Queries.List;

public class ListUserQueryHandler(IUserReadRepository userRepository)
    : IRequestHandler<ListUserQuery, Result<IEnumerable<UserDto>>>
{
    public async Task<Result<IEnumerable<UserDto>>> Handle(ListUserQuery request, 
        CancellationToken cancellationToken)
    {
        var users = await userRepository.GetAllAsync(cancellationToken);
        
        return users
            .Select(u => new UserDto(u.Id, u.Email, u.Name, u.Role, u.IsBlocked))
            .ToList();
    }
}