using Application.Common.DTO.Request;
using Application.Common.Repositories;
using Application.TaskItems;
using Domain.TaskItems;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class TaskItemRepository : GenericRepository<TaskItem, int>, IRepository<TaskItem, int>, ITaskItemRepository
    {

        public TaskItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        private static readonly HashSet<string> AllowedIncludes = new(StringComparer.OrdinalIgnoreCase)
        {
        };

 

        protected override IQueryable<TaskItem> ApplySearch(
                IQueryable<TaskItem> query,
                string? searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery)) return query;

            string search = searchQuery.Trim().ToLower();
            int.TryParse(search, out int searchId);
            query = query.Where(t => t.Title.Contains(search) || t.TaskId == searchId);

            return query;
        }

        protected override IQueryable<TaskItem> ApplyFilters(
            IQueryable<TaskItem> query,
            IDictionary<string, string>? filters)
        {
            if (filters is null) return query;

            foreach (var (field, value) in filters)
            {
                switch (field.Trim().ToLowerInvariant())
                {
                    case "title":
                        query = query.Where(t => t.Title.Contains(value));
                        break;
                    case "statusid":
                        query = query.Where(t => t.StatusId == byte.Parse(value));
                        break;
                }
            }

            return query;
        }


        protected override IQueryable<TaskItem> ApplyIncludes(
            IQueryable<TaskItem> query,
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

        protected override IQueryable<TaskItem> ApplySorting(
            IQueryable<TaskItem> query,
            string? sortBy,
            bool sortDescending)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return sortDescending
                    ? query.OrderByDescending(ts => ts.TaskId)
                    : query.OrderBy(ts => ts.TaskId);
            }

            var field = sortBy.Trim().ToLowerInvariant();

            return field switch
            {
                "priority" =>
                    sortDescending
                        ? query.OrderByDescending(ts => ts.Priority)
                        : query.OrderBy(ts => ts.Priority),
                _ => sortDescending
                        ? query.OrderByDescending(ts => ts.CreatedAt)
                        : query.OrderBy(ts => ts.CreatedAt)
            };
        }
        protected override IQueryable<TaskItem> BuildFilteredQuery(
            PagedQueryParameters parameters,
            bool includeFields = true)
        {
            return base.BuildFilteredQuery(
                _context.TaskItems.AsNoTracking(),
                parameters,
                includeFields);
        }

    }
}