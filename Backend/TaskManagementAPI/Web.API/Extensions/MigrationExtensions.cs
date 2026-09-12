
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Web.API.Extensions;

public static class MigrationExensions
{

    public static void ApplyMigrations(this WebApplication app)
    {

        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
    }

}