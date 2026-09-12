using Microsoft.AspNetCore.Mvc;

namespace Application.Common.DTO.Request
{
    public class PagedQueryParameters
    {
        private const int MaxPageSize = 20;
        private int _pageSize = 5;
        public virtual int PageNumber { get; set; } = 1;
        public virtual string? SortBy { get; set; }
        public virtual bool SortDescending { get; set; } = false;

        public virtual int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public virtual string? SearchQuery { get; set; }

        [FromQuery(Name = "filters")]
        public Dictionary<string, string>? Filters { get; set; }
        public virtual string? IncludeFields { get; set; }                
    }
}