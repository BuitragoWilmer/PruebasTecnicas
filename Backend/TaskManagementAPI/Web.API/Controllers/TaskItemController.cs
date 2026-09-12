using Application.Common.DTO.Request;
using Application.TaskItems;
using Application.TaskItems.Create;
using Application.TaskItems.GetAll;
using Application.TaskItems.GetById;
using Application.TaskItems.Update;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web.API.Common.Hateoas;
using Web.API.Common.Models.Hateoas;

namespace Web.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class TaskItemsController : APIControllerBase<
        int,
    TaskItemResponse,
    CreateTaskItemCommand,
    GetTaskItemByIdQuery,
    GetAllTaskItemsQuery,
    UpdateTaskItemCommand,
    NoPartialCommand<TaskItemResponse>
>
{

    private readonly ISender _mediator;
    private readonly HateoasService _hateoasService;
    public TaskItemsController(ISender mediator, HateoasService hateoasService) : base(
            mediator,
            hateoasService,
            allowCreate: true,
            allowGet: true,
            allowGetAll: true,
            allowUpdate: true,
            allowPartial: false,
            allowDelete: false,
            upserting: false
    )
    {
        _hateoasService = hateoasService ?? throw new ArgumentNullException(nameof(hateoasService));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    protected override string ResourceName => "TaskItems";
    protected override string GetByIdRouteName => "GetTaskItems";


    [HttpPost(Name = "AddTaskItems")]
    [ProducesResponseType(typeof(Resource<TaskItemResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> Create([FromBody] CreateTaskItemCommand cmd, [FromHeader(Name = "Accept")] string accept)
    {
        return HandleCreateAsync(cmd, accept);
    }

    [ProducesResponseType(typeof(Resource<TaskItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id}", Name = "GetTaskItems")]
    public Task<IActionResult> GetById(int id, [FromHeader(Name = "Accept")] string accept)
    {
        return HandleGetByIdAsync(new GetTaskItemByIdQuery(id), accept);
    }

    [HttpGet(Name = "GetTaskItemss")]
    [ProducesResponseType(typeof(ResourceCollection<Resource<TaskItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public Task<IActionResult> GetAll([FromQuery] PagedQueryParameters qp, [FromHeader(Name = "Accept")] string accept)
    {
        return HandleGetAllAsync(new GetAllTaskItemsQuery(qp), qp, accept);
    }


    [HttpGet("list", Name = "GetTaskItemsList")]
    [ProducesResponseType(typeof(ResourceCollection<TaskItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public Task<IActionResult> GetList(
    [FromQuery] PagedQueryParameters qp,
    [FromHeader(Name = "Accept")] string accept
    )
    {
        return HandleGetAllListAsync(new GetAllTaskItemsQuery(qp), qp, accept);
    }

    [HttpPut("{id}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateTaskItemCommand cmd,
            [FromHeader(Name = "Accept")] string accept
    )
    {
        if (cmd.TaskItemId != id)
        {
            var error = Error.Validation(
                    "TaskItems.UpdateInvalid",
                    "El ID en la URL no coincide con el ID del cuerpo de la petición."
            );
            return Problem(new List<Error> { error });
        }
        return await HandleUpdateAsync(cmd, accept);
    }



    protected override int ExtractResponseId(TaskItemResponse response) => response.TaskId;

}
