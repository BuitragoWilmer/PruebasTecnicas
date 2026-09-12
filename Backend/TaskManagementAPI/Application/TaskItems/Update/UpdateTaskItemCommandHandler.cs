using Domain.Primitives;
using Domain.DomainErrors;
using AutoMapper;
using Domain.TaskItems;
using Application.TaskStatuses;

namespace Application.TaskItems.Update
{
    public sealed class UpdateTaskItemCommandHandler : IRequestHandler<UpdateTaskItemCommand, ErrorOr<Unit>>
    {
        private readonly ITaskItemRepository _taskItemRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITaskStatusRepository _TaskStatusRepository;


        public UpdateTaskItemCommandHandler(
            ITaskItemRepository taskItemRepository,
            ITaskStatusRepository taskStatusRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper
        )
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _TaskStatusRepository = taskStatusRepository ??  throw new ArgumentNullException(nameof(taskStatusRepository));
            _taskItemRepository = taskItemRepository ?? throw new ArgumentNullException(nameof(taskItemRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ErrorOr<Unit>> Handle(UpdateTaskItemCommand command, CancellationToken cancellationToken)
        {
            TaskItem? TaskItemToModified = await _taskItemRepository.GetByIdAsync(command.TaskItemId);

            if (TaskItemToModified is null)
            {
                return Errors.TaskItem.NotFound;
            }

 
            TaskStatus? taskStatus = await _TaskStatusRepository.GetByIdAsync(command.StatusId);
            if (taskStatus is null)
            {
                return Errors.TaskItem.NotFound;
            }


            TaskItemToModified.Patch(command.StatusId);

            _taskItemRepository.Update(TaskItemToModified);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Unit.Value; 
        }

    }
}

