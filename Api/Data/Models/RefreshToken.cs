namespace Api.Data.Models
{
    public class RefreshToken
    {
        public Guid Id { get; private init; } = Guid.NewGuid();
        public string UserId { get; set; } = null!;
        public string Token { get; set; } = GenerateToken();
        public DateTime ExpiresAtUtc { get; set; } = DateTime.UtcNow.AddDays(7);
        public bool IsRevoked { get; set; }

        private static string GenerateToken() => Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }
}
