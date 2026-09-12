using Domain.TaskItems;

public class TaskStatus
{
    public TaskStatus()
    {
    }

    public TaskStatus(byte statusId, string statusName, ICollection<TaskItem> tasks)
    {
        StatusId = statusId;
        StatusName = statusName;
        Tasks = tasks;
    }

    public byte StatusId { get; set; }

    public string StatusName { get; set; } = string.Empty;

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    
}