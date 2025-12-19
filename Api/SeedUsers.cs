using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Models;
using System.Security.Cryptography;
using System.Text;

namespace Api
{
    public static class SeedUsers
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // First, update roles to match our implementation
            await UpdateRolesAsync(context);
            
            // Create stores
            await CreateStoresAsync(context);
            
            // Create users
            await CreateUsersAsync(context);
            
            // Assign users to stores
            await AssignUsersToStoresAsync(context);
        }

        private static async Task UpdateRolesAsync(ApplicationDbContext context)
        {
            // Don't clear roles - SeedRoles.cs handles role creation
            // Just ensure Admin and Customer roles exist if they don't
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
        }

        private static async Task CreateStoresAsync(ApplicationDbContext context)
        {
            // Check if the store already exists
            var existingStore = await context.Warehouses.FirstOrDefaultAsync(w => w.Name == "Online Store");
            if (existingStore != null)
            {
                return; // Store already exists
            }

            // Create only one store - Online Store
            var store = new Warehouse
            {
                Name = "Online Store",
                Address = "1000 E-Commerce Center",
                City = "Online",
                PhoneNumber = "+1-555-0104",
                ManagerName = "Online Manager",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            context.Warehouses.Add(store);
            await context.SaveChangesAsync();
        }

        private static async Task CreateUsersAsync(ApplicationDbContext context)
        {
            // Check if our test users already exist
            var existingTestUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "admin@company.com");
            if (existingTestUser != null)
            {
                return; // Test users already exist
            }

            var users = new List<User>
            {
                // Admin
                new User
                {
                    FullName = "John Admin",
                    Email = "admin@company.com",
                    PasswordHash = HashPassword("admin123"),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                
                // Customer
                new User
                {
                    FullName = "Alice Customer",
                    Email = "alice.customer@company.com",
                    PasswordHash = HashPassword("customer123"),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            context.Users.AddRange(users);
            await context.SaveChangesAsync();
        }

        private static async Task AssignUsersToStoresAsync(ApplicationDbContext context)
        {
            // Get roles
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            var customerRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Customer");

            // Get users
            var admin = await context.Users.FirstOrDefaultAsync(u => u.Email == "admin@company.com");
            var aliceCustomer = await context.Users.FirstOrDefaultAsync(u => u.Email == "alice.customer@company.com");

            // Assign roles - check if they already exist first
            var userRoles = new List<UserRole>();
            
            if (admin != null && adminRole != null)
            {
                var existingAdminRole = await context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == admin.Id && ur.RoleId == adminRole.Id);
                if (existingAdminRole == null)
                {
                    userRoles.Add(new UserRole { UserId = admin.Id, RoleId = adminRole.Id, AssignedAt = DateTime.UtcNow });
                }
            }
            
            if (aliceCustomer != null && customerRole != null)
            {
                var existingCustomerRole = await context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == aliceCustomer.Id && ur.RoleId == customerRole.Id);
                if (existingCustomerRole == null)
                {
                    userRoles.Add(new UserRole { UserId = aliceCustomer.Id, RoleId = customerRole.Id, AssignedAt = DateTime.UtcNow });
                }
            }

            if (userRoles.Any())
            {
                context.UserRoles.AddRange(userRoles);
                await context.SaveChangesAsync();
            }
        }

        private static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
