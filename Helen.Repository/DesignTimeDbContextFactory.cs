using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Helen.Domain.Invites.Response; // Adjust the namespace as needed

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<HelenDbContext>
{
    public HelenDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<HelenDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=Helen;User Id=SA;Password=Password123;");

        return new HelenDbContext(optionsBuilder.Options);
    }
}
