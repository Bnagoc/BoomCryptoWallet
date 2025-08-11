namespace Api.Clients.Endpoints;

public class CreateWithdraw : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/{userId}/withdraw", Handle)
        .WithSummary("Creates a new user withdraw")
        .WithRequestValidation<Request>();

    public record Request(Guid userId, int amount);
    public record Response(Guid userId, decimal newBalance);
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            
            RuleFor(c => c.amount)
                .GreaterThan(0);
        }
    }

    private static async Task<IResult> Handle(Request request, AppDbContext database, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
    {
        var user = await database.Users.SingleAsync(c => c.Id == request.userId, cancellationToken);

        if (user.Balance < request.amount)
        {
            return TypedResults.BadRequest(new { error = "Insufficient funds" });
        }

        user.Balance -= request.amount;
        await database.SaveChangesAsync(cancellationToken);

        var response = new Response(user.Id, user.Balance);
        return TypedResults.Ok(response);
    }
}