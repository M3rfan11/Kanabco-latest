using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Models;

namespace Api
{
    public static class SeedRoles
    {
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Always ensure SuperAdmin role exists
        var superAdminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "SuperAdmin");
        if (superAdminRole == null)
        {
            superAdminRole = new Role 
            { 
                Name = "SuperAdmin", 
                Description = "Super Administrator with full system access including role and permission management",
                CreatedAt = DateTime.UtcNow
            };
            context.Roles.Add(superAdminRole);
            await context.SaveChangesAsync();
        }

        // Create other default roles if they don't exist
        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
        if (adminRole == null)
        {
            adminRole = new Role 
            { 
                Name = "Admin", 
                Description = "Administrator with full system access for managing products, categories, and orders",
                CreatedAt = DateTime.UtcNow
            };
            context.Roles.Add(adminRole);
            await context.SaveChangesAsync();
        }

        var customerRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Customer");
        if (customerRole == null)
        {
            customerRole = new Role 
            { 
                Name = "Customer", 
                Description = "Customer role for browsing products and placing orders",
                CreatedAt = DateTime.UtcNow
            };
            context.Roles.Add(customerRole);
            await context.SaveChangesAsync();
        }

        // ALWAYS ensure admin@example.com has SuperAdmin role
        var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "admin@example.com");
        if (adminUser != null)
        {
            var superAdminForUser = await context.Roles.FirstOrDefaultAsync(r => r.Name == "SuperAdmin");
            if (superAdminForUser != null)
            {
                // Check if user already has SuperAdmin role
                var existingUserRole = await context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == adminUser.Id && ur.RoleId == superAdminForUser.Id);
                
                if (existingUserRole == null)
                {
                    // User doesn't have SuperAdmin role - add it
                    var userRole = new UserRole
                    {
                        UserId = adminUser.Id,
                        RoleId = superAdminForUser.Id,
                        AssignedAt = DateTime.UtcNow
                    };

                    context.UserRoles.Add(userRole);
                    await context.SaveChangesAsync();
                }
                // If role already exists, we're good - no need to do anything
            }
        }
        
        // Also assign SuperAdmin to the first user if admin@example.com doesn't exist (fallback)
        var firstUser = await context.Users.FirstOrDefaultAsync();
        if (firstUser != null && firstUser.Email != "admin@example.com")
        {
            var superAdminForUser = await context.Roles.FirstOrDefaultAsync(r => r.Name == "SuperAdmin");
            if (superAdminForUser != null)
            {
                // Check if user already has SuperAdmin role
                var hasSuperAdmin = await context.UserRoles
                    .AnyAsync(ur => ur.UserId == firstUser.Id && ur.RoleId == superAdminForUser.Id);
                
                if (!hasSuperAdmin)
                {
                    var userRole = new UserRole
                    {
                        UserId = firstUser.Id,
                        RoleId = superAdminForUser.Id,
                        AssignedAt = DateTime.UtcNow
                    };

                    context.UserRoles.Add(userRole);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
    }
}




