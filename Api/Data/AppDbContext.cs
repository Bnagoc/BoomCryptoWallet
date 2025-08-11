namespace Api.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureUsersTable(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private static void ConfigureUsersTable(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<User>();

            builder.HasIndex(x => x.Email)
                .IsUnique();
        }

    }
}
