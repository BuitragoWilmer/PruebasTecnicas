using System.Linq;
using System.Net.Http.Headers;
using Application.Common.DTO.Request;
using Application.Common.DTO.Response;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Web.API.Common.Errors;
using Web.API.Common.Hateoas;
using Web.API.Common.Models.Hateoas;

namespace Web.API.Controllers;
public abstract class APIControllerBase<
    TKey,
    TResponse,
    TCreateCmd,
    TGetByIdQry,
    TGetAllQry,
    TUpdateCmd,
    TPartialCmd
> : ControllerBase
    where TKey : notnull
    where TResponse : class
    where TCreateCmd : IRequest<ErrorOr<TResponse>>
    where TGetByIdQry : IRequest<ErrorOr<TResponse>>
    where TGetAllQry : IRequest<ErrorOr<PagedQueryResponse<TResponse>>>
    where TUpdateCmd : IRequest<ErrorOr<Unit>>
    where TPartialCmd : IRequest<ErrorOr<TResponse>>
{
    protected readonly ISender Sender;
    protected readonly HateoasService _hateoas;
    protected readonly bool Upserting;

    protected readonly string AllowedVerbs;

    protected APIControllerBase(
        ISender mediator,
        HateoasService hateoas,
        bool allowCreate,
        bool allowGet,
        bool allowGetAll,
        bool allowUpdate,
        bool allowPartial,
        bool allowDelete,
        bool upserting = false
    )
    {
        Sender = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _hateoas = hateoas ?? throw new ArgumentNullException(nameof(hateoas));
        Upserting = upserting;

        var verbs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (allowCreate) verbs.Add("POST");
        if (allowGet) verbs.Add("GET");
        if (allowGetAll) verbs.Add("GET");
        if (allowUpdate) verbs.Add("PUT");
        if (allowPartial) verbs.Add("PATCH");
        if (allowDelete) verbs.Add("DELETE");

        AllowedVerbs = string.Join(", ", verbs);
    }

    #region Endpoints

    protected async Task<IActionResult> HandleCreateAsync(
        TCreateCmd cmd,
        string acceptHeader
    )
    {
        // 1. Verificar si CREATE está permitido
        if (typeof(TCreateCmd).IsGenericType &&
            typeof(TCreateCmd).GetGenericTypeDefinition() == typeof(NoCreateCommand<>))
        {
            return BadRequest("CREATE is not supported for this entity.");
        }

        // 2. Validar header
        if (!MediaTypeHeaderValue.TryParse(acceptHeader, out _))
            return BadRequest("Invalid Accept header.");

        var result = await Sender.Send(cmd);

        return result.Match(
            response =>
            {
                var id = ExtractResponseId(response);
                var links = _hateoas.GetLinks(id.ToString()!, ResourceName);

                return CreatedAtRoute(
                    routeName: GetByIdRouteName,
                    routeValues: new { id },
                    value: new Resource<TResponse>(response, links)
                );
            },
            errors => Problem(errors)
        );
    }


    protected async Task<IActionResult> HandleGetByIdAsync(
        TGetByIdQry qry,
        string acceptHeader
    )
    {
        // 1. Verificar si esta entidad NO soporta GET BY ID
        if (typeof(TGetByIdQry).IsGenericType &&
            typeof(TGetByIdQry).GetGenericTypeDefinition() == typeof(NoGetByIdQuery<>))
        {
            return BadRequest("GET BY ID is not supported for this entity.");
        }

        // 2. Validar Accept
        if (!MediaTypeHeaderValue.TryParse(acceptHeader, out _))
            return BadRequest("Invalid Accept header.");

        var result = await Sender.Send(qry);

        return result.Match(
            response =>
            {
                var id = ExtractResponseId(response);
                var links = _hateoas.GetLinks(id.ToString()!, ResourceName);
                return Ok(new Resource<TResponse>(response, links));
            },
            errors => Problem(errors)
        );
    }

    protected async Task<IActionResult> HandleGetAllAsync(
        TGetAllQry qry,
        PagedQueryParameters pagingParams,
        string acceptHeader
    )
    {
        // 1. Detectar si esta entidad NO soporta GET ALL
        if (typeof(TGetAllQry).IsGenericType &&
            typeof(TGetAllQry).GetGenericTypeDefinition() == typeof(NoGetAllQuery<>))
        {
            return BadRequest("GET ALL is not supported for this entity.");
        }

        // 2. Validar Accept
        if (!MediaTypeHeaderValue.TryParse(acceptHeader, out _))
            return BadRequest("Invalid Accept header.");

        var result = await Sender.Send(qry);

        return result.Match(
            paged =>
            {
                if (!paged.Items.Any())
                    return NoContent();

                var resourceItems = paged.Items
                    .Select(item =>
                    {
                        var id = ExtractResponseId(item);
                        var links = _hateoas.GetLinks(id.ToString()!, ResourceName);
                        return new Resource<TResponse>(item, links);
                    })
                    .ToList();

                var totalPages = Math.Ceiling((double)paged.TotalItems / pagingParams.PageSize);
                var paginationLinks = _hateoas.GeneratePaginationLinks(
                    pagingParams.PageNumber,
                    pagingParams.PageSize,
                    totalPages,
                    ResourceName
                );

                return Ok(new ResourceCollection<Resource<TResponse>>(resourceItems, paginationLinks));
            },
            errors => Problem(errors)
        );
    }

    protected async Task<IActionResult> HandleGetAllListAsync(
        TGetAllQry qry,
        PagedQueryParameters pagingParams,
        string acceptHeader
    )
    {
        if (!MediaTypeHeaderValue.TryParse(acceptHeader, out _))
            return BadRequest("Invalid Accept header.");

        var result = await Sender.Send(qry);
        return result.Match(
            paged =>
            {
                if (!paged.Items.Any())
                    return NoContent();

                var resourceItems = paged.Items;

                var totalPages = Math.Ceiling((double)paged.TotalItems / pagingParams.PageSize);
                var paginationLinks = _hateoas.GeneratePaginationLinks(
                    pagingParams.PageNumber,
                    pagingParams.PageSize,
                    totalPages,
                    ResourceName
                );

                return Ok(new ResourceCollection<TResponse>(resourceItems, paginationLinks));
            },
            errors => Problem(errors)
        );
    }

    protected async Task<IActionResult> HandleUpdateAsync(
        TUpdateCmd cmd,
        string acceptHeader
    )
    {
        if (typeof(TUpdateCmd).IsGenericType &&
            typeof(TUpdateCmd).GetGenericTypeDefinition() == typeof(NoUpdateCommand<>))
        {
            return BadRequest("PUT is not supported for this entity.");
        }

        if (!MediaTypeHeaderValue.TryParse(acceptHeader, out _))
            return BadRequest("Invalid Accept header.");

        var result = await Sender.Send(cmd);
        return result.Match<IActionResult>(
            _ => NoContent(),
            errs => Problem(errs)
        );
    }

    protected async Task<IActionResult> HandlePartialAsync(
        TPartialCmd cmd,
        string acceptHeader
    )
    {
        // 1. Verificar si esta entidad NO soporta PATCH
        if (typeof(TPartialCmd).IsGenericType &&
            typeof(TPartialCmd).GetGenericTypeDefinition() == typeof(NoPartialCommand<>))
        {
            return BadRequest("PATCH is not supported for this entity.");
        }

        // 2. Validar el header Accept
        if (!MediaTypeHeaderValue.TryParse(acceptHeader, out _))
            return BadRequest("Invalid Accept header.");

        var result = await Sender.Send(cmd);
        return result.Match<IActionResult>(
            response =>
            {
                var id = ExtractResponseId(response);
                var links = _hateoas.GetLinks(id.ToString()!, ResourceName);
                return Ok(new Resource<TResponse>(response, links));
            },
            errs => Problem(errs)
        );
    }

    [HttpOptions]
    [AllowAnonymous]
    public IActionResult HandleOptions()
    {
        Response.Headers.Append("Allow", AllowedVerbs);
        Response.Headers.Append("Upserting", Upserting.ToString());
        return Ok();
    }

    #endregion

    #region Helpers

    protected IActionResult Problem(List<Error> errors)
    {
        HttpContext.Items[HttpContextItemKeys.Errors] = errors;

        if (errors.Count == 0)
            return Problem();


        if (errors.All(e => e.Type == ErrorType.Validation))
            return ValidationProblemFromErrors(errors);

        var firstError = errors[0];

        var statusCode = firstError.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        return Problem(
            title: firstError.Code,
            detail: firstError.Description,
            statusCode: statusCode
        );
    }

    protected IActionResult ValidationProblemFromErrors(List<Error> errors)
    {
        var modelState = new ModelStateDictionary();

        foreach (var error in errors)
        {
            modelState.AddModelError(
                error.Code,
                error.Description
            );
        }

        return base.ValidationProblem(modelState);
    }

    /// <summary>
    /// Nombre de recurso para HATEOAS.
    /// </summary>
    protected abstract string ResourceName { get; }

    /// <summary>
    /// Nombre de la ruta GetById para CreatedAtRoute.
    /// </summary>
    protected abstract string GetByIdRouteName { get; }

    /// <summary>
    /// Extrae el ID de la respuesta TResponse.
    /// </summary>
    protected abstract TKey ExtractResponseId(TResponse response);

    #endregion
}
