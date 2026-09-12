using Application.Common.DTO.Request;

namespace Application.Common.Repositories;
public interface IRepository<TEntity, TId>
    where TEntity : class
{
    Task<TEntity> AddAsync(TEntity entity);
    void Delete(TEntity entity);
    void Update(TEntity entity);
    Task<bool> ExistsAsync(TId id);
    Task<TEntity?> GetByIdAsync(TId id);
    Task<List<TEntity>> GetAll();
    Task<int> GetTotalCountAsync(PagedQueryParameters parameters, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> GetPagedAsync(PagedQueryParameters parameters, CancellationToken cancellationToken = default);
}