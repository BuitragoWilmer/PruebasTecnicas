using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Web.API.Common.Errors
{
    public class FormsApiProblemDetailsFactory : ProblemDetailsFactory
    {
        private readonly ApiBehaviorOptions _options;

        public FormsApiProblemDetailsFactory(IOptions<ApiBehaviorOptions> options)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public override ProblemDetails CreateProblemDetails(
            Microsoft.AspNetCore.Http.HttpContext httpContext,
            int? statusCode = null,
            string? title = null,
            string? type = null,
            string? detail = null,
            string? instance = null)
        {
            var problemDetails = new ProblemDetails
            {
                Status = statusCode ?? 500,
                Title = title,
                Type = type,
                Detail = detail,
                Instance = instance
            };

            ApplyProblemDetailsDefaults(httpContext, problemDetails, statusCode);

            return problemDetails;
        }

        public override ValidationProblemDetails CreateValidationProblemDetails(
            HttpContext httpContext,
            ModelStateDictionary modelStateDictionary,
            int? statusCode = null,
            string? title = null,
            string? type = null,
            string? detail = null,
            string? instance = null)
        {
            ArgumentNullException.ThrowIfNull(modelStateDictionary);

            var validationProblemDetails = new ValidationProblemDetails(modelStateDictionary)
            {
                Status = statusCode ?? 400,
                Type = type,
                Title = title,
                Detail = detail,
                Instance = instance
            };

            ApplyProblemDetailsDefaults(httpContext, validationProblemDetails, statusCode);

            return validationProblemDetails;
        }

        private void ApplyProblemDetailsDefaults(
            HttpContext httpContext,
            ProblemDetails problemDetails,
            int? statusCode)
        {
            problemDetails.Status ??= statusCode ?? 500;

            if (_options.ClientErrorMapping.TryGetValue(problemDetails.Status.Value, out var clientErrorData))
            {
                problemDetails.Title ??= clientErrorData.Title;
                problemDetails.Type ??= clientErrorData.Link;
            }

            var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
            if (traceId != null)
            {
                problemDetails.Extensions["traceId"] = traceId;
            }

            var errors = httpContext?.Items[HttpContextItemKeys.Errors] as List<Error>;

            if (errors is not null && errors.Count > 0)
            {
                problemDetails.Extensions.Add("errorCodes", errors.Select(e => e.Code));
            }
        }
    }
}