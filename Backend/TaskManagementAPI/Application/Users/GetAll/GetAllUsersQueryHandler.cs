
using Application.Common.DTO.Response;
using AutoMapper;



namespace Application.Users.GetAll
{
    internal sealed  class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, ErrorOr<PagedQueryResponse<UserResponse>>>
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _UserRepository;

        public GetAllUsersQueryHandler( IMapper mapper, IUserRepository UserRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _UserRepository = UserRepository ?? throw new ArgumentNullException(nameof(UserRepository));
        }
 

        public async Task<ErrorOr<PagedQueryResponse<UserResponse>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            int totalCount = await _UserRepository.GetTotalCountAsync(request.Parameters, cancellationToken);
            if (totalCount == 0)
            {
                return new PagedQueryResponse<UserResponse>(0, new List<UserResponse>());
            }

            IReadOnlyList<UserResponse> query = _mapper
                     .Map<IReadOnlyList<UserResponse>>(await _UserRepository.GetPagedAsync(request.Parameters, cancellationToken));
                
            return  new PagedQueryResponse<UserResponse>(totalCount, query);
        }
    }

}