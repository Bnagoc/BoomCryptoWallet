namespace Api.Clients.Endpoints;

public class CreateUser : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/", Handle)
        .WithSummary("Creates a new user")
        .WithRequestValidation<Request>();

    public record Request(string email);
    public record Response(Guid userId, string email, decimal balance);
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(c => c.email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(100);
        }
    }

    private static async Task<Results<Ok<Response>, ValidationError>> Handle(Request request, AppDbContext database, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
    {
        var isEmailTaken = await database.Users
            .AnyAsync(x => x.Email == request.email, cancellationToken);

        if (isEmailTaken)
        {
            return new ValidationError("Email is already created");
        }

        var user = new User
        {
            Email = request.email,
        };

        await database.Users.AddAsync(user, cancellationToken);
        await database.SaveChangesAsync(cancellationToken);
        var response = new Response(user.Id, user.Email, user.Balance);
        return TypedResults.Ok(response);
    }
}