namespace Api.Clients.Endpoints;

public class CreateDeposit : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/{userId}/deposit", Handle)
        .WithSummary("Creates a new user deposit")
        .WithRequestValidation<Request>();

    public record Request(Guid userId, decimal amount);
    public record Response(Guid userId, decimal newBalance);
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(c => c.amount)
                .GreaterThan(0);
        }
    }

    private static async Task<Ok<Response>> Handle(Request request, AppDbContext database, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
    {
        var user = await database.Users.SingleAsync(c => c.Id == request.userId, cancellationToken);

        user.Balance += request.amount;
        await database.SaveChangesAsync(cancellationToken);

        var response = new Response(user.Id, user.Balance);
        return TypedResults.Ok(response);
    }
}