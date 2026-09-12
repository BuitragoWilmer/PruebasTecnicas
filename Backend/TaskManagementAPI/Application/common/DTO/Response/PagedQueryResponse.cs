namespace Application.Common.DTO.Response;

public record PagedQueryResponse<T>(
    int TotalItems,
    IReadOnlyList<T> Items
);