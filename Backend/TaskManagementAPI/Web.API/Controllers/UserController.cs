using Application.Common.DTO.Request;
using Application.Users;
using Application.Users.Create;
using Application.Users.GetAll;
using Application.Users.GetbyId;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web.API.Common.Hateoas;
using Web.API.Common.Models.Hateoas;

namespace Web.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class UsersController : APIControllerBase<
        int,
    UserResponse,
    CreateUserCommand,
    GetUserByIdQuery,
    GetAllUsersQuery,
    MediatR.IRequest<ErrorOr.ErrorOr<MediatR.Unit>>,
    NoPartialCommand<UserResponse>
>
{

    private readonly ISender _mediator;
    private readonly HateoasService _hateoasService;
    public UsersController(ISender mediator, HateoasService hateoasService) : base(
            mediator,
            hateoasService,
            allowCreate: true,
            allowGet: true,
            allowGetAll: true,
            allowUpdate: false,
            allowPartial: false,
            allowDelete: false,
            upserting: false
    )
    {
        _hateoasService = hateoasService ?? throw new ArgumentNullException(nameof(hateoasService));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    protected override string ResourceName => "Users";
    protected override string GetByIdRouteName => "GetUsers";


    [HttpPost(Name = "AddUsers")]
    [ProducesResponseType(typeof(Resource<UserResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> Create([FromBody] CreateUserCommand cmd, [FromHeader(Name = "Accept")] string accept)
    {
        return HandleCreateAsync(cmd, accept);
    }

    [ProducesResponseType(typeof(Resource<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id}", Name = "GetUsers")]
    public Task<IActionResult> GetById(int id, [FromHeader(Name = "Accept")] string accept)
    {
        return HandleGetByIdAsync(new GetUserByIdQuery(id), accept);
    }

    [HttpGet(Name = "GetUserss")]
    [ProducesResponseType(typeof(ResourceCollection<Resource<UserResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public Task<IActionResult> GetAll([FromQuery] PagedQueryParameters qp, [FromHeader(Name = "Accept")] string accept)
    {
        return HandleGetAllAsync(new GetAllUsersQuery(qp), qp, accept);
    }


    [HttpGet("list", Name = "GetUsersList")]
    [ProducesResponseType(typeof(ResourceCollection<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public Task<IActionResult> GetList(
    [FromQuery] PagedQueryParameters qp,
    [FromHeader(Name = "Accept")] string accept
    )
    {
        return HandleGetAllListAsync(new GetAllUsersQuery(qp), qp, accept);
    }



    protected override int ExtractResponseId(UserResponse response) => response.UserId;

}
