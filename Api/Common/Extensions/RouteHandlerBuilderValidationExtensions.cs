using Api.Common.Filters;

namespace Api.Common.Extensions
{
    public static class RouteHandlerBuilderValidationExtensions
    {
        public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder)
        {
            return builder
                .AddEndpointFilter<RequestValidationFilter<TRequest>>()
                .ProducesValidationProblem();
        }
    }
}
