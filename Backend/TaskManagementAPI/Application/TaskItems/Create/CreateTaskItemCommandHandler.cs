using Application.TaskItems;
using Domain.Primitives;
using Domain.TaskItems;

namespace Application.TaskItems.Create;

public sealed class CreateTaskItemCommandHandler : IRequestHandler<CreateTaskItemCommand, ErrorOr<TaskItemResponse>>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTaskItemCommandHandler(ITaskItemRepository taskItemRepository, IUnitOfWork unitOfWork)
    {
        _taskItemRepository = taskItemRepository ?? throw new ArgumentNullException(nameof(taskItemRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ErrorOr<TaskItemResponse>> Handle(
        CreateTaskItemCommand request,
        CancellationToken cancellationToken)
    {
        var taskItem = new TaskItem(
            0,
            request.Title.Trim(),
            request.Description?.Trim(),
            request.AssignedUserId,
            request.AdditionalInfo?.Trim(),
            DateTime.UtcNow)
        {
            StatusId = request.StatusId
        };

        await _taskItemRepository.AddAsync(taskItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new TaskItemResponse(
            taskItem.TaskId,
            taskItem.Title,
            taskItem.Description,
            taskItem.AssignedUserId,
            taskItem.StatusId,
            taskItem.AdditionalInfo,
            taskItem.CreatedAt,
            taskItem.UpdatedAt);
    }
}