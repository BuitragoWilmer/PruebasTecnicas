using AutoMapper;
using Domain.DomainErrors;
using Domain.Users;

namespace Application.Users.GetbyId;

internal sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, ErrorOr<UserResponse>>
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _UserRepository;

    public GetUserByIdQueryHandler( IMapper mapper, IUserRepository UserRepository)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _UserRepository = UserRepository ?? throw new ArgumentNullException(nameof(UserRepository));
    }

    public async Task<ErrorOr<UserResponse>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        if (await _UserRepository.GetByIdAsync(query.Id) is not User user)
        {
            return Errors.User.Notfound;
        }

        return _mapper.Map<UserResponse>(user);
    }
}