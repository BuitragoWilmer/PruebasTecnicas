using MediatR;

namespace Application.TaskItems.Create;

public record CreateTaskItemCommand(
    string Title,
    string? Description,
    int AssignedUserId,
    byte StatusId,
    string? AdditionalInfo) : IRequest<ErrorOr<TaskItemResponse>>;