using Application.Common.Repositories;
using Domain.TaskItems;

namespace Application.TaskItems;

public interface ITaskItemRepository : IRepository<TaskItem, int>
{

}
