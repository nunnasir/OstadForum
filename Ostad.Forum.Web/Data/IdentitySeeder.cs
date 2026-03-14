using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ostad.Forum.DAL;
using Ostad.Forum.Domain.Entities;

namespace Ostad.Forum.Web.Data;

public static class IdentitySeeder
{
    public static async Task SeedAdminAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var db = scope.ServiceProvider.GetRequiredService<ForumDbContext>();

        const string adminRole = "Admin";
        const string adminEmail = "admin@ostadforum.com";
        const string adminPassword = "Admin@123";

        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        var user = await userManager.FindByEmailAsync(adminEmail);
        if (user == null)
        {
            user = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, adminRole);
            }
        }
        else if (!await userManager.IsInRoleAsync(user, adminRole))
        {
            await userManager.AddToRoleAsync(user, adminRole);
        }

        // Initial category: Technology
        const string technologyCategoryName = "Technology";
        var hasTechnology = await db.Categories.AnyAsync(c => c.Name == technologyCategoryName);
        if (!hasTechnology)
        {
            db.Categories.Add(new Category
            {
                Name = technologyCategoryName,
                Slug = "technology",
                Description = "Questions and discussions about technology.",
                CreatedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
        }

        // Initial tag: tech
        const string technologytagName = "Tech";
        var hasTag = await db.Tags.AnyAsync(c => c.Name == technologytagName);
        if (!hasTag)
        {
            db.Tags.Add(new Tag
            {
                Name = technologytagName,
                Slug = "tech",
                CreatedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
        }
    }
}

