using Web.API.Common.Models.Hateoas;

namespace Web.API.Common.Hateoas
{
    public class HateoasService
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HateoasService(LinkGenerator baseUrl, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _linkGenerator = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
        }

        public List<Link> GetLinks(string id, string entityPrefix)
        {
            var httpContext = _httpContextAccessor.HttpContext!;
            return new List<Link>
            {
                new Link(
                     _linkGenerator.GetUriByName(httpContext, $"Get{entityPrefix}s", new { id })!,
                    "self",
                    "GET"
                ),
                  new Link(
                _linkGenerator.GetUriByName(httpContext, $"Update{entityPrefix}s", new { id })!,
                    "update",
                    "PUT"
                ),
                new Link(
                    _linkGenerator.GetUriByName(httpContext, $"PartiallyUpdate{entityPrefix}s", new { id })!,
                    "patch",
                    "PATCH"
                ),
                new Link(
                    _linkGenerator.GetUriByName(httpContext, $"Add{entityPrefix}s", new { })!,
                    "create",
                    "POST"
                )
            };
        }

        public List<Link> GeneratePaginationLinks(int page, int pageSize, double totalPages, string entityPrefix)
        {
            var httpContext = _httpContextAccessor.HttpContext!;
            var links = new List<Link>
            {
                new Link(
                    _linkGenerator.GetUriByName(httpContext, $"Get{entityPrefix}s", new { })!,
                    "self",
                    "GET"
                ),

            };

            if (page < totalPages)
            {
                links.Add(
                    new Link(
                        _linkGenerator.GetUriByName(httpContext, $"Get{entityPrefix}s", new { page = page + 1, pageSize })!,
                        "next",
                        "GET"
                ));
            }

            if (page > 1)
            {
                links.Add(new Link
                (
                    _linkGenerator.GetUriByName(httpContext, $"Get{entityPrefix}s", new { page = page - 1, pageSize })!,
                    "prev",
                    "GET"
                ));
            }

            links.Add(
                new Link
                (
                    _linkGenerator.GetUriByName(httpContext, $"Get{entityPrefix}s", new { page = 1, pageSize })!,
                    "first",
                    "GET"
                )
            );

            links.Add(
                new Link
                (
                    _linkGenerator.GetUriByName(httpContext, $"Get{entityPrefix}s", new { page = totalPages, pageSize })!,
                    "last",
                    "GET"
                )
            );

            return links;
        }

    }
}