using AutoMapper;
using Domain.DomainErrors;
using Domain.TaskItems;

namespace Application.TaskItems.GetById;

internal sealed class GetTaskItemByIdQueryHandler
    : IRequestHandler<GetTaskItemByIdQuery, ErrorOr<TaskItemResponse>>
{
    private readonly IMapper _mapper;
    private readonly ITaskItemRepository _taskItemRepository;

    public GetTaskItemByIdQueryHandler(IMapper mapper, ITaskItemRepository taskItemRepository)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _taskItemRepository = taskItemRepository ?? throw new ArgumentNullException(nameof(taskItemRepository));
    }

    public async Task<ErrorOr<TaskItemResponse>> Handle(
        GetTaskItemByIdQuery query,
        CancellationToken cancellationToken)
    {
        if (await _taskItemRepository.GetByIdAsync(query.Id) is not TaskItem taskItem)
        {
            return Errors.TaskItem.NotFound;
        }

        return _mapper.Map<TaskItemResponse>(taskItem);
    }
}