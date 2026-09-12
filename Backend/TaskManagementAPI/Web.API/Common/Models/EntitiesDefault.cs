using Application.Common.DTO.Response;
using ErrorOr;
using MediatR;

public sealed record NoPartialCommand<TResponse>()
    : IRequest<ErrorOr<TResponse>>;
public sealed record NoCreateCommand<TResponse>()
    : IRequest<ErrorOr<TResponse>>;
public sealed record NoGetByIdQuery<TResponse>()
    : IRequest<ErrorOr<TResponse>>;

public sealed record NoGetAllQuery<TResponse>()
    : IRequest<ErrorOr<PagedQueryResponse<TResponse>>>;

public sealed record NoUpdateCommand<TResponse>()
    : IRequest<ErrorOr<TResponse>>;



