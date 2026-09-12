
namespace Application.Users.GetbyId;

public record GetUserByIdQuery(int Id) : IRequest<ErrorOr<UserResponse>>;