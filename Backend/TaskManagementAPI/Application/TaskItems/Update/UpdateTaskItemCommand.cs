namespace Application.TaskItems.Update;

public record UpdateTaskItemCommand 
(
    int TaskItemId,
    byte StatusId

): IRequest<ErrorOr<Unit>>;


