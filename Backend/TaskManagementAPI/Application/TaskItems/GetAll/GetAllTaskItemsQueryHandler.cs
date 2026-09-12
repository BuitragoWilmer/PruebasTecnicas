using Application.Common.DTO.Response;
using AutoMapper;

namespace Application.TaskItems.GetAll;

internal sealed class GetAllTaskItemsQueryHandler
    : IRequestHandler<GetAllTaskItemsQuery, ErrorOr<PagedQueryResponse<TaskItemResponse>>>
{
    private readonly IMapper _mapper;
    private readonly ITaskItemRepository _taskItemRepository;

    public GetAllTaskItemsQueryHandler(IMapper mapper, ITaskItemRepository taskItemRepository)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _taskItemRepository = taskItemRepository ?? throw new ArgumentNullException(nameof(taskItemRepository));
    }

    public async Task<ErrorOr<PagedQueryResponse<TaskItemResponse>>> Handle(
        GetAllTaskItemsQuery request,
        CancellationToken cancellationToken)
    {
        int totalCount = await _taskItemRepository.GetTotalCountAsync(request.Parameters, cancellationToken);
        if (totalCount == 0)
        {
            return new PagedQueryResponse<TaskItemResponse>(0, new List<TaskItemResponse>());
        }

        var taskItems = await _taskItemRepository.GetPagedAsync(request.Parameters, cancellationToken);
        return new PagedQueryResponse<TaskItemResponse>(
            totalCount,
            _mapper.Map<IReadOnlyList<TaskItemResponse>>(taskItems));
    }
}