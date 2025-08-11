namespace Api.Authentication.Services;

public interface IRefreshTokenService
{
    Task<string> GenerateAsync(string userId);
    Task<bool> ValidateAsync(string refreshToken);
    Task RevokeAsync(string refreshToken);
}

public class RefreshTokenService(AppDbContext dbContext) : IRefreshTokenService
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<string> GenerateAsync(string userId)
    {
        var token = new RefreshToken
        {
            UserId = userId,
        };

        await _dbContext.RefreshTokens.AddAsync(token);
        await _dbContext.SaveChangesAsync();

        return token.Token;
    }

    public async Task<bool> ValidateAsync(string refreshToken)
    {
        var token = await _dbContext.RefreshTokens.FirstOrDefaultAsync(t => t.Token.Contains(refreshToken) && !t.IsRevoked);

        if (token is null || token.ExpiresAtUtc < DateTime.UtcNow)
        {
            return false;
        }

        return true;
    }

    public async Task RevokeAsync(string refreshToken)
    {
        var token = _dbContext.RefreshTokens.FirstOrDefault(t => t.Token == refreshToken);
        if (token != null)
        {
            token.IsRevoked = true;
        }

        await _dbContext.SaveChangesAsync();
    }
}