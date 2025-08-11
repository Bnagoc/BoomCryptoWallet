namespace Api.Authentication.Endpoints;

public class RevokeRefreshToken : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/refresh/revoke", Handle)
        .WithSummary("Revoke a refresh token")
        .WithRequestValidation<Request>();

    public record Request(string RefreshToken);
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(c => c.RefreshToken)
                .NotEmpty();
        }
    }

    private static async Task<Ok> Handle(Request request, AppDbContext database, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
    {
        var refreshToken = await database.RefreshTokens.FirstOrDefaultAsync(t => t.Token == request.RefreshToken);
        
        if (refreshToken is not null)
        {
            refreshToken.IsRevoked = true;
            await database.SaveChangesAsync();
        }

        return TypedResults.Ok();
    }
}

