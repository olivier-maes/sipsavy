using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SipSavy.Data;

// This factory is used by EF Core tools to create the DbContext at design time.
// It is typically used for adding new migrations.
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=54320;Username=postgres;Password=password;Database=sipsavy",
            op => { op.UseVector(); });

        return new AppDbContext(optionsBuilder.Options);
    }
}