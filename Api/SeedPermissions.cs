using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Models;

namespace Api
{
    public static class SeedPermissions
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
        var resources = new[] { "Products", "Categories", "Users", "Roles", "Inventory", "Orders", "Sales", "Warehouses", "Reports", "PromoCodes" };
        var actions = new[] { "Create", "Update", "Delete", "Read" };

        // Check if permissions already exist
        var existingPermissions = await context.Permissions.ToListAsync();
        
        if (existingPermissions.Count == 0)
        {
            // Create all permissions
            var permissions = new List<Permission>();

            foreach (var resource in resources)
            {
                foreach (var action in actions)
                {
                    permissions.Add(new Permission
                    {
                        Name = $"{resource}.{action}",
                        Description = $"Permission to {action.ToLower()} {resource.ToLower()}",
                        Resource = resource,
                        Action = action,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            context.Permissions.AddRange(permissions);
            await context.SaveChangesAsync();
            existingPermissions = await context.Permissions.ToListAsync();
        }
        else
        {
            // Add any missing permissions (e.g., if "Sales" was added later)
            var permissionsToAdd = new List<Permission>();
            
            foreach (var resource in resources)
            {
                foreach (var action in actions)
                {
                    var permissionName = $"{resource}.{action}";
                    var exists = existingPermissions.Any(p => p.Resource == resource && p.Action == action);
                    
                    if (!exists)
                    {
                        permissionsToAdd.Add(new Permission
                        {
                            Name = permissionName,
                            Description = $"Permission to {action.ToLower()} {resource.ToLower()}",
                            Resource = resource,
                            Action = action,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }
            }
            
            if (permissionsToAdd.Count > 0)
            {
                context.Permissions.AddRange(permissionsToAdd);
                await context.SaveChangesAsync();
                // Refresh existing permissions list
                existingPermissions = await context.Permissions.ToListAsync();
            }
        }

        // Always ensure SuperAdmin has all permissions
        var superAdminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "SuperAdmin");
        if (superAdminRole != null)
        {
            var existingRolePermissions = await context.RolePermissions
                .Where(rp => rp.RoleId == superAdminRole.Id)
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            var allPermissions = await context.Permissions.ToListAsync();
            var permissionsToAdd = allPermissions
                .Where(p => !existingRolePermissions.Contains(p.Id))
                .Select(p => new RolePermission
                {
                    RoleId = superAdminRole.Id,
                    PermissionId = p.Id,
                    AssignedAt = DateTime.UtcNow
                })
                .ToList();

            if (permissionsToAdd.Count > 0)
            {
                context.RolePermissions.AddRange(permissionsToAdd);
                await context.SaveChangesAsync();
            }
        }
    }
    }
}

