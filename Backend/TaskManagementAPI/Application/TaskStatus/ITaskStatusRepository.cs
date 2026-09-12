using Application.Common.Repositories;
using Domain.TaskItems;

namespace Application.TaskStatuses;

public interface ITaskStatusRepository : IRepository<TaskStatus, byte>
{

}
