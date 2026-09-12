using AutoMapper;
using Domain.TaskItems;
using Application.TaskItems;

namespace Application.Commom.MappingProfiles;

public sealed class TaskItemProfile : Profile
{
    public TaskItemProfile()
    {
        CreateMap<TaskItem, TaskItemResponse>();
    }
}