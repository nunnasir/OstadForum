using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ostad.Forum.DAL;

public class ForumDesignTimeFactory : IDesignTimeDbContextFactory<ForumDbContext>
{
    public ForumDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ForumDbContext>();
        optionsBuilder.UseSqlServer("Server=.;Database=OstadForumDb;Trusted_Connection=True;TrustServerCertificate=True;");
        return new ForumDbContext(optionsBuilder.Options);
    }
}

