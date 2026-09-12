namespace Application.Common.DTO.Request
{
    using FluentValidation;

    public class PagedQueryParametersValidator : AbstractValidator<PagedQueryParameters>
    {
        public PagedQueryParametersValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page number must be greater than or equal to 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 20)
                .WithMessage("Page size must be between 1 and 20.");

            RuleFor(x => x.SortBy)
                .Matches(@"^[A-Za-z]+$")
                .When(x => !string.IsNullOrWhiteSpace(x.SortBy))
                .WithMessage("Invalid SortBy. It must be a valid property name (e.g., 'Name').");

            RuleFor(x => x.IncludeFields)
                .Matches(@"^[A-Za-z]+(,[A-Za-z]+)*$")
                .When(x => !string.IsNullOrWhiteSpace(x.IncludeFields))
                .WithMessage("Invalid Include. It must consist of comma-separated property names using letters only (e.g., 'companies,otherRelation').");       

            RuleFor(x => x.SortDescending)
                .Must(x => x == true || x == false)
                .WithMessage("SortDescending must be either true or false.");

            RuleFor(x => x.SearchQuery)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.SearchQuery))
                .WithMessage("SearchQuery must not exceed 100 characters.");


                RuleForEach(x => x.Filters)
                    .Must(kvp =>
                        System.Text.RegularExpressions.Regex.IsMatch(kvp.Key, @"^[A-Za-z0-9]+$") &&
                        System.Text.RegularExpressions.Regex.IsMatch(kvp.Value, @"^[A-Za-z0-9]+$"))
                    . When(x => x.Filters is not null && x.Filters.Count > 0)
                    .WithMessage("Invalid Filters. Each key and value must contain only letters or digits (e.g., 'name:abc,id:123').");
        }
       
    }
}