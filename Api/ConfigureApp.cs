using Serilog;

namespace Api;

public static class ConfigureApp
{
    public static async Task Configure(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Crypto wallet API");
            c.RoutePrefix = String.Empty;
        });
        app.UseRouting();
        app.UseCors();
        app.UseAuthorization();
        app.MapEndpoints();
        await app.EnsureDatabaseCreated();
    }

    private static async Task EnsureDatabaseCreated(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
    }

}