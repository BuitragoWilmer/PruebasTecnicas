using Application.Common.DTO.Request;
using Application.Common.Repositories;
using Application.TaskItems;
using Application.TaskStatuses;
using Domain.TaskItems;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class TaskStatusRepository : GenericRepository<TaskStatus, byte>, IRepository<TaskStatus, byte>, ITaskStatusRepository
    {

        public TaskStatusRepository(ApplicationDbContext context) : base(context)
        {
        }

        private static readonly HashSet<string> AllowedIncludes = new(StringComparer.OrdinalIgnoreCase)
        {
        };

 

        protected override IQueryable<TaskStatus> ApplySearch(
                IQueryable<TaskStatus> query,
                string? searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery)) return query;

            string search = searchQuery.Trim().ToLower();
            int.TryParse(search, out int searchId);
            query = query.Where(t => t.StatusName.Contains(search) || t.StatusId == searchId);

            return query;
        }

        protected override IQueryable<TaskStatus> ApplyFilters(
            IQueryable<TaskStatus> query,
            IDictionary<string, string>? filters)
        {
            if (filters is null) return query;

            foreach (var (field, value) in filters)
            {
                switch (field.Trim().ToLowerInvariant())
                {
                    case "StatusName":
                        query = query.Where(t => t.StatusName.Contains(value));
                        break;
                }
            }

            return query;
        }


        protected override IQueryable<TaskStatus> ApplyIncludes(
            IQueryable<TaskStatus> query,
            string includeFields)
        {
            var includes = includeFields
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(inc => AllowedIncludes.Contains(inc));

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }

        protected override IQueryable<TaskStatus> ApplySorting(
            IQueryable<TaskStatus> query,
            string? sortBy,
            bool sortDescending)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return sortDescending
                    ? query.OrderByDescending(ts => ts.StatusId)
                    : query.OrderBy(ts => ts.StatusId);
            }

            var field = sortBy.Trim().ToLowerInvariant();

            return field switch
            {
                _ => sortDescending
                        ? query.OrderByDescending(ts => ts.StatusId)
                        : query.OrderBy(ts => ts.StatusId)
            };
        }
        protected override IQueryable<TaskStatus> BuildFilteredQuery(
            PagedQueryParameters parameters,
            bool includeFields = true)
        {
            return base.BuildFilteredQuery(
                _context.TaskStatuses.AsNoTracking(),
                parameters,
                includeFields);
        }

    }
}