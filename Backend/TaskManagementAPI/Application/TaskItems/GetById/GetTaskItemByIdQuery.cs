namespace Application.TaskItems.GetById;

public record GetTaskItemByIdQuery(int Id) : IRequest<ErrorOr<TaskItemResponse>>;