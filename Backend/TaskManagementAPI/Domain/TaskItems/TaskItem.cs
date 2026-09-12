using Domain.Users;

namespace Domain.TaskItems;

public sealed class TaskItem : AggregateRoot
{
    private TaskItem()
    {
    }

    public TaskItem(
        int taskId,
        string title,
        string? description,
        int assignedUserId,
        string? additionalInfo,
        DateTime createdAt)
    {
        TaskId = taskId;
        Title = title;
        Description = description;
        AssignedUserId = assignedUserId;
        AdditionalInfo = additionalInfo;
        CreatedAt = createdAt;
    }

    public void Patch(
      byte statusId
      )
    {
        StatusId = statusId;
    }

    public int TaskId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int AssignedUserId { get; set; }

    public byte StatusId { get; set; }

    public string? AdditionalInfo { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Priority { get; private set; }

    public DateTime? DueDate { get; private set; }

    public User AssignedUser { get; set; } = null!;
    public TaskStatus Status { get; set; } = null!;

}

