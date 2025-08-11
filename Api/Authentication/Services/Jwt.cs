using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Api.Authentication.Services;

public class JwtOptions
{
    public required string Key { get; init; }
    public TimeSpan AccessTokenExpiry { get; set; } = TimeSpan.FromMinutes(5);
    public TimeSpan RefreshTokenExpiry { get; set; } = TimeSpan.FromDays(7);
}

public class Jwt(IOptions<JwtOptions> options)
{
    public string GenerateToken(User user)
    {
        var key = JwtHelpers.SecurityKey(options.Value.Key);
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
        
        var token = new JwtSecurityToken
        (
            claims: [new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())],
            signingCredentials: new(key, SecurityAlgorithms.HmacSha256Signature),
            expires: DateTime.UtcNow.Add(options.Value.AccessTokenExpiry)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public static class JwtHelpers
{
    public static SymmetricSecurityKey SecurityKey(string key)
        => new(Encoding.ASCII.GetBytes(key));
}

