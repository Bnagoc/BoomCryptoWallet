using System.IdentityModel.Tokens.Jwt;
using Api.Authentication.Services;
using Microsoft.IdentityModel.Tokens;

namespace Api.Authentication.Endpoints;

public class GenerateRefreshToken : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/refresh", Handle)
        .WithSummary("Generates a refresh token")
        .WithRequestValidation<Request>();

    public record Request(string Token, string RefreshToken);
    public record Response(string Token, string RefreshToken);
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(c => c.Token)
                .NotEmpty();

            RuleFor(c => c.RefreshToken)
                .NotEmpty();
        }
    }

    private static async Task<Results<Ok<Response>, UnauthorizedHttpResult>> Handle(Request request, AppDbContext database, Jwt jwt, 
        IRefreshTokenService refreshTokenService, HttpContext context, IConfiguration configuration, CancellationToken cancellationToken)
    {
        try
        {
            var principal = GetPrincipalFromExpiredToken(request.Token, jwt, configuration["Jwt:Key"]);
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null || !await refreshTokenService.ValidateAsync(request.RefreshToken))
            {
                return TypedResults.Unauthorized();
            }

            var user = await database.Users.FindAsync(int.Parse(userId));

            await refreshTokenService.RevokeAsync(request.RefreshToken);

            var newAccessToken = jwt.GenerateToken(user!);
            var newRefreshToken = await refreshTokenService.GenerateAsync(userId);

            return TypedResults.Ok(new Response(newAccessToken, newRefreshToken));
        }
        catch
        {
            return TypedResults.Unauthorized();
        }
    }

    private static ClaimsPrincipal GetPrincipalFromExpiredToken(string token, Jwt jwt, string key)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = false,
            IssuerSigningKey = JwtHelpers.SecurityKey(key),
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtToken ||
            !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}

