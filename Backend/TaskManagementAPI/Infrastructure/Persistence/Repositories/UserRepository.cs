using Application.Common.DTO.Request;
using Application.Common.Repositories;
using Application.Users;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class UserRepository : GenericRepository<User, int>, IRepository<User, int>, IUserRepository
    {

        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        private static readonly HashSet<string> AllowedIncludes = new(StringComparer.OrdinalIgnoreCase)
        {
        };

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(user => user.Email == email);
        }   
 

        protected override IQueryable<User> ApplySearch(
                IQueryable<User> query,
                string? searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery)) return query;

            string search = searchQuery.Trim().ToLower();
            int.TryParse(search, out int searchId);
            query = query.Where(t => t.FullName.Contains(search) || t.UserId == searchId);

            return query;
        }

        protected override IQueryable<User> ApplyFilters(
            IQueryable<User> query,
            IDictionary<string, string>? filters)
        {
            if (filters is null) return query;

            foreach (var (field, value) in filters)
            {
                switch (field.Trim().ToLowerInvariant())
                {
                    case "FullName":
                        query = query.Where(t => t.FullName.Contains(value));
                        break;
                }
            }

            return query;
        }


        protected override IQueryable<User> ApplyIncludes(
            IQueryable<User> query,
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

        protected override IQueryable<User> ApplySorting(
            IQueryable<User> query,
            string? sortBy,
            bool sortDescending)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return sortDescending
                    ? query.OrderByDescending(ts => ts.UserId)
                    : query.OrderBy(ts => ts.UserId);
            }

            var field = sortBy.Trim().ToLowerInvariant();

            return field switch
            {
                "createdat" =>
                    sortDescending
                        ? query.OrderByDescending(ts => ts.CreatedAt)
                        : query.OrderBy(ts => ts.CreatedAt),
                _ => sortDescending
                        ? query.OrderByDescending(ts => ts.CreatedAt)
                        : query.OrderBy(ts => ts.CreatedAt)
            };
        }
        protected override IQueryable<User> BuildFilteredQuery(
            PagedQueryParameters parameters,
            bool includeFields = true)
        {
            return base.BuildFilteredQuery(
                _context.Users.AsNoTracking(),
                parameters,
                includeFields);
        }
    }
}