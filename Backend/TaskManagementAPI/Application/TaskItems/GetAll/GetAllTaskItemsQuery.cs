using Application.Common.DTO.Request;
using Application.Common.DTO.Response;

namespace Application.TaskItems.GetAll;

public record GetAllTaskItemsQuery(PagedQueryParameters Parameters)
    : IRequest<ErrorOr<PagedQueryResponse<TaskItemResponse>>>;