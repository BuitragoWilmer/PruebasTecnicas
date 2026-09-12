using MediatR;

namespace Application.Users.Create;

public  record CreateUserCommand(
	string FullName,
	string Email) : IRequest<ErrorOr<UserResponse>>;
