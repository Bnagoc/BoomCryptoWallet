namespace Api.Data.Models;

public class User : IEntity
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public decimal Balance { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
