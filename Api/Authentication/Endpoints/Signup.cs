using Api.Authentication.Services;

namespace Api.Authentication.Endpoints;

public class Signup : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/signup", Handle)
        .WithSummary("Register a new user account")
        .WithRequestValidation<Request>();

    public record Request(string Email);
    public record Response(string Token, string RefreshToken);
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(100);
        }
    }

    private static async Task<Results<Ok<Response>, ValidationError>> Handle(Request request, AppDbContext database, Jwt jwt,
        IRefreshTokenService refreshTokenService, CancellationToken cancellationToken)
    {
        var isEmailTaken = await database.Users
            .AnyAsync(x => x.Email == request.Email, cancellationToken);

        if (isEmailTaken)
        {
            return new ValidationError("Email is already taken");
        }

        var user = new User
        {
            Email = request.Email,
        };

        await database.Users.AddAsync(user, cancellationToken);
        await database.SaveChangesAsync(cancellationToken);

        var token = jwt.GenerateToken(user);
        var refreshToken = await refreshTokenService.GenerateAsync(user.Id.ToString());

        var response = new Response(token, refreshToken);
        return TypedResults.Ok(response);
    }
}