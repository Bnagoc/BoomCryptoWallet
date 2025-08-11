using Api.Authentication.Services;

namespace Api.Authentication.Endpoints;

public class Login : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/login", Handle)
        .WithSummary("Logs in a user")
        .WithRequestValidation<Request>();

    public record Request(string Email);
    public record Response(Guid UserId, string Email, decimal Balance, string Token, string RefreshToken);
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty();
        }
    }

    private static async Task<Results<Ok<Response>, UnauthorizedHttpResult>> Handle(Request request, AppDbContext database, Jwt jwt,
        IRefreshTokenService refreshTokenService, CancellationToken cancellationToken)
    {
        var user = await database.Users.SingleOrDefaultAsync(x => x.Email == request.Email, cancellationToken);

        if (user is null)
        {
            return TypedResults.Unauthorized();
        }

        var token = jwt.GenerateToken(user);
        database.RefreshTokens.RemoveRange(database.RefreshTokens.Where(u => u.UserId == user.Id.ToString()).ToList());
        var refreshToken = await refreshTokenService.GenerateAsync(user.Id.ToString());
        var response = new Response(user.Id, user.Email, user.Balance, token, refreshToken);
        return TypedResults.Ok(response);
    }
}
