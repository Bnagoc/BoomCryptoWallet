using Api.Data.Models;

namespace Api.Authentication.Endpoints;

public class ValidateRefreshToken : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/refresh/validate", Handle)
        .WithSummary("Validates a refresh token")
        .WithRequestValidation<Request>();

    public record Request(string RefreshToken);
    public record Response(bool IsValid);
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(c => c.RefreshToken)
                .NotEmpty();
        }
    }

    private static async Task<Ok<Response>> Handle(Request request, AppDbContext database, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
    {
        var refreshToken = await database.RefreshTokens.FirstOrDefaultAsync(t => t.Token == request.RefreshToken && !t.IsRevoked);
        
        if (refreshToken is null || refreshToken.ExpiresAtUtc < DateTime.UtcNow)
        {
            return TypedResults.Ok(new Response(false));
        }

        return TypedResults.Ok(new Response(true));
    }
}

