
using Application.Common.DTO.Request;
using Application.Common.DTO.Response;

namespace Application.Users.GetAll;
public record GetAllUsersQuery(PagedQueryParameters Parameters) : IRequest<ErrorOr<PagedQueryResponse<UserResponse>>>;


