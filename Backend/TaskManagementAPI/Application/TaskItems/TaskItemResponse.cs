
namespace Application.TaskItems;

public record TaskItemResponse(
    int TaskId,
    string Title,
    string? Description,
    int AssignedUserId,
    byte StatusId,
    string? AdditionalInfo,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

