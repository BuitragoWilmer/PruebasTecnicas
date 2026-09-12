using Application.Common.DTO.Request;
using Application.Common.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public abstract class GenericRepository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : class
{
    protected readonly ApplicationDbContext _context;

    protected GenericRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        await _context.Set<TEntity>().AddAsync(entity);
        return entity;
    }

    public virtual void Delete(TEntity entity) => _context.Set<TEntity>().Remove(entity);

    public virtual void Update(TEntity entity) => _context.Set<TEntity>().Update(entity);

    public virtual async Task<bool> ExistsAsync(TId id)
    {
        var entityType = _context.Model.FindEntityType(typeof(TEntity))!;
        var pkProperty = entityType.FindPrimaryKey()!.Properties.Single();

        return await _context.Set<TEntity>()
                            .AsNoTracking()
                            .AnyAsync(e => EF.Property<TId>(e, pkProperty.Name)!.Equals(id));
    }

    public virtual async Task<TEntity?> GetByIdAsync(TId id)
    {
        return await _context.Set<TEntity>().FindAsync(id);
    }

    public virtual async Task<List<TEntity>> GetAll()
    {
        return await _context.Set<TEntity>().AsNoTracking().ToListAsync();
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetPagedAsync(PagedQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = BuildFilteredQuery(parameters);
        var items = await ApplyPaging(query, parameters).ToListAsync(cancellationToken);
        return items;
    }

    public virtual async Task<int> GetTotalCountAsync(PagedQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryForCount = BuildFilteredQuery(parameters, includeFields: false);
        return await queryForCount.CountAsync(cancellationToken);
    }

    protected virtual IQueryable<TEntity> BuildFilteredQuery(PagedQueryParameters parameters, bool includeFields = true)
    {
        // Se sobreescritura en repositorios concretos
        return _context.Set<TEntity>().AsNoTracking();
    }

    protected IQueryable<TEntity> ApplyPaging(IQueryable<TEntity> query, PagedQueryParameters parameters)
    {

        return query.Skip((parameters.PageNumber - 1) * parameters.PageSize)
                    .Take(parameters.PageSize);
    }

    protected virtual IQueryable<TEntity> BuildFilteredQuery(
    IQueryable<TEntity> baseQuery,
    PagedQueryParameters parameters,
    bool includeFields = true)
    {
        var query = baseQuery;

        query = ApplySearch(query, parameters.SearchQuery);
        query = ApplyFilters(query, parameters.Filters);

        if (includeFields && !string.IsNullOrWhiteSpace(parameters.IncludeFields))
            query = ApplyIncludes(query, parameters.IncludeFields);

        query = ApplySorting(query, parameters.SortBy, parameters.SortDescending);

        return query;
    }


    protected virtual IQueryable<TEntity> ApplySearch(
    IQueryable<TEntity> query,
    string? searchQuery)
    {
        return query;
    }

    protected virtual IQueryable<TEntity> ApplyFilters(
        IQueryable<TEntity> query,
        IDictionary<string, string>? filters)
    {
        return query;
    }

    protected virtual IQueryable<TEntity> ApplyIncludes(
        IQueryable<TEntity> query,
        string includeFields)
    {
        return query;
    }

    protected virtual IQueryable<TEntity> ApplySorting(
        IQueryable<TEntity> query,
        string? sortBy,
        bool sortDescending)
    {
        return sortDescending
            ? query.OrderByDescending(e => EF.Property<object>(e, "Id"))
            : query.OrderBy(e => EF.Property<object>(e, "Id"));
    }


}