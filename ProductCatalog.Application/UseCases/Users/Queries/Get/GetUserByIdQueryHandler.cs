using MediatR;
using ProductCatalog.Application.Common.Results;
using ProductCatalog.Application.Interfaces.Repositories.Users;

namespace ProductCatalog.Application.UseCases.Users.Queries.Get;

public class GetUserByIdQueryHandler(IUserReadRepository userRepository) 
    : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.id, cancellationToken);
        return user == null
            ? Result<UserDto>.Failure("Пользователь не найден") 
            : new UserDto(user.Id, user.Email, user.Name, user.Role, user.IsBlocked);
    }
}