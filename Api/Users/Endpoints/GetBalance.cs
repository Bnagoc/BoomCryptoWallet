namespace Api.Clients.Endpoints;

public class GetBalance : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{userId}/balance", Handle)
        .WithSummary("Get user balance")
        .WithRequestValidation<Request>();

    public record Request(Guid userId);

    public record Response(Guid UserId, decimal balance);

    public static async Task<Response> Handle([AsParameters] Request request, AppDbContext database, CancellationToken cancellationToken)
    {
        return await database.Users
            .Where(c => c.Id == request.userId)
            .Select(p => new Response
            (
                p.Id,
                p.Balance
            )).FirstOrDefaultAsync();
    }
}
