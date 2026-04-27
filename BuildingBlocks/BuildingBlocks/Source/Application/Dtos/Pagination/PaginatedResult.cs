namespace BuildingBlocks.Source.Application.Dtos.Pagination;

public record PaginatedResult<TEntity>(
    int PageIndex,
    long PageSize,
    long Count,
    IEnumerable<TEntity> Data)
    where TEntity : class;
