using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Models;

namespace Api
{
    public static class SeedProducts
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Create categories first
            await CreateCategoriesAsync(context);
            
            // Create products (with error handling to prevent API startup failure)
            try
            {
                await CreateProductsAsync(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not update products during seed: {ex.Message}");
                Console.WriteLine("Products can be updated manually through the admin interface.");
            }
            
            // Create product inventories for each store
            try
            {
                await CreateProductInventoriesAsync(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not create product inventories: {ex.Message}");
            }
        }

        private static async Task CreateCategoriesAsync(ApplicationDbContext context)
        {
            // Define the furniture categories we want
            var furnitureCategories = new Dictionary<string, string>
            {
                { "SEATING", "Be comfortable. Seat at home!" },
                { "SOFAS", "Create the perfect lounge area!" },
                { "ROOMS", "Complete room solutions for modern living!" },
                { "TABLES", "Surfaces that define your space!" },
                { "HOME DECORS", "Touches that add character!" },
                { "New Arrivals", "Fresh designs for modern spaces!" },
                { "Best Selling", "Customer favorites and top picks!" }
            };
            
            // Get all existing categories
            var existingCategories = await context.Categories.ToListAsync();
            
            // Update or create furniture categories
            foreach (var (name, description) in furnitureCategories)
            {
                var existing = existingCategories.FirstOrDefault(c => 
                    c.Name.Equals(name, StringComparison.OrdinalIgnoreCase) ||
                    (name == "New Arrivals" && c.Name == "NEW ARRIVALS") ||
                    (name == "Best Selling" && c.Name == "BEST SELLING"));
                
                if (existing != null)
                {
                    // Update existing category
                    existing.Name = name;
                    existing.Description = description;
                    existing.IsActive = true;
                    existing.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    // Create new category
                    var category = new Category
                    {
                        Name = name,
                        Description = description,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    context.Categories.Add(category);
                }
            }
            
            // Deactivate old perfume categories that don't match furniture categories
            var perfumeCategoryNames = new[] { "Body Sprays", "Luxury Perfumes", "Men Fragrances", "Unisex Scents", "Women Perfumes" };
            foreach (var category in existingCategories)
            {
                if (!furnitureCategories.ContainsKey(category.Name) && 
                    perfumeCategoryNames.Contains(category.Name))
                {
                    category.IsActive = false;
                    category.UpdatedAt = DateTime.UtcNow;
                }
            }
            
            await context.SaveChangesAsync();
            return;

            var categories = new List<Category>
            {
                new Category
                {
                    Name = "SEATING",
                    Description = "Be comfortable. Seat at home!",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Name = "SOFAS",
                    Description = "Create the perfect lounge area!",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Name = "ROOMS",
                    Description = "Complete room solutions for modern living!",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Name = "TABLES",
                    Description = "Surfaces that define your space!",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Name = "HOME DECORS",
                    Description = "Touches that add character!",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Name = "New Arrivals",
                    Description = "Fresh designs for modern spaces!",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Best Selling",
                    Description = "Customer favorites and top picks!",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        private static async Task CreateProductsAsync(ApplicationDbContext context)
        {
            // Ensure categories exist first
            await CreateCategoriesAsync(context);
            
            var existingProducts = await context.Products.ToListAsync();
            var existingProductCount = existingProducts.Count;
            
            // If products exist, update them instead of deleting (to avoid foreign key constraints)
            if (existingProducts.Any())
            {
                // Update existing products with furniture data
                var furnitureProducts = await GetFurnitureProductsListAsync(context);
                
                // Update products one at a time to handle errors gracefully
                int updatedCount = 0;
                for (int i = 0; i < Math.Min(existingProducts.Count, furnitureProducts.Count); i++)
                {
                    try
                    {
                        var existing = existingProducts[i];
                        var furniture = furnitureProducts[i];
                        
                        // Update essential visible fields (name, description, image)
                        existing.Name = furniture.Name;
                        existing.Description = furniture.Description;
                        existing.ImageUrl = furniture.ImageUrl;
                        existing.Price = furniture.Price;
                        existing.IsActive = true;
                        existing.UpdatedAt = DateTime.UtcNow;
                        
                        // Save each product individually
                        await context.SaveChangesAsync();
                        updatedCount++;
                    }
                    catch (Exception ex)
                    {
                        // Skip problematic products and continue
                        Console.WriteLine($"Warning: Could not update product {i}: {ex.Message}");
                        context.ChangeTracker.Clear(); // Clear tracking to avoid conflicts
                    }
                }
                
                Console.WriteLine($"Updated {updatedCount} products with furniture data.");
                return;
            }
            
            // If no products exist, create new ones
            var products = await GetFurnitureProductsListAsync(context);
            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
        
        private static async Task<List<Product>> GetFurnitureProductsListAsync(ApplicationDbContext context)
        {
            var seatingCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "SEATING");
            var sofasCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "SOFAS");
            var roomsCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "ROOMS");
            var tablesCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "TABLES");
            var homeDecorsCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "HOME DECORS");
            var newArrivalsCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "New Arrivals");
            var bestSellingCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Best Selling");

            var products = new List<Product>
            {
                // SEATING Category
                new Product
                {
                    Name = "Modern Accent Chair",
                    Description = "Elegant accent chair with premium fabric upholstery and solid wood legs. Perfect for living rooms and reading nooks.",
                    Price = 12500.00m,
                    Unit = "piece",
                    SKU = "SEAT-001",
                    Brand = "Kanabco",
                    Weight = 18.5m,
                    Dimensions = "85x90x95 cm",
                    ImageUrl = "https://images.unsplash.com/photo-1506439773649-6e0eb8cfb237?w=600&h=600&fit=crop",
                    CategoryId = seatingCategory?.Id ?? 1,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Executive Office Chair",
                    Description = "Ergonomic office chair with lumbar support and adjustable height. Ideal for home offices and workspaces.",
                    Price = 8500.00m,
                    Unit = "piece",
                    SKU = "SEAT-002",
                    Brand = "Kanabco",
                    Weight = 15.2m,
                    Dimensions = "65x65x120 cm",
                    ImageUrl = "https://images.unsplash.com/photo-1586953208448-b95a79798f07?w=600&h=600&fit=crop",
                    CategoryId = seatingCategory?.Id ?? 1,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Dining Chair Set (4)",
                    Description = "Set of 4 modern dining chairs with comfortable padded seats and sturdy construction.",
                    Price = 15000.00m,
                    Unit = "set",
                    SKU = "SEAT-003",
                    Brand = "Kanabco",
                    Weight = 28.0m,
                    Dimensions = "45x50x95 cm (each)",
                    ImageUrl = "https://images.unsplash.com/photo-1506439773649-6e0eb8cfb237?w=600&h=600&fit=crop",
                    CategoryId = seatingCategory?.Id ?? 1,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Recliner Armchair",
                    Description = "Comfortable recliner armchair with footrest and premium leather upholstery.",
                    Price = 22000.00m,
                    Unit = "piece",
                    SKU = "SEAT-004",
                    Brand = "Kanabco",
                    Weight = 35.0m,
                    Dimensions = "100x95x110 cm",
                    ImageUrl = "https://images.unsplash.com/photo-1549497538-303791108f95?w=600&h=600&fit=crop",
                    CategoryId = seatingCategory?.Id ?? 1,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Bar Stool (Set of 2)",
                    Description = "Modern bar stools with adjustable height and swivel function. Perfect for kitchen islands.",
                    Price = 12000.00m,
                    Unit = "set",
                    SKU = "SEAT-005",
                    Brand = "Kanabco",
                    Weight = 16.0m,
                    Dimensions = "40x40x110 cm (each)",
                    ImageUrl = "https://images.unsplash.com/photo-1506439773649-6e0eb8cfb237?w=600&h=600&fit=crop",
                    CategoryId = seatingCategory?.Id ?? 1,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                // SOFAS Category
                new Product
                {
                    Name = "3-Seater Modern Sofa",
                    Description = "Contemporary 3-seater sofa with premium fabric and comfortable cushions. Perfect for modern living rooms.",
                    Price = 35000.00m,
                    Unit = "piece",
                    SKU = "SOFA-001",
                    Brand = "Kanabco",
                    Weight = 85.0m,
                    Dimensions = "220x95x85 cm",
                    ImageUrl = "https://images.unsplash.com/photo-1555041469-a586c61ea9bc?w=600&h=600&fit=crop",
                    CategoryId = sofasCategory?.Id ?? 2,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "L-Shaped Sectional Sofa",
                    Description = "Spacious L-shaped sectional sofa with chaise lounge. Ideal for large living spaces.",
                    Price = 55000.00m,
                    Unit = "piece",
                    SKU = "SOFA-002",
                    Brand = "Kanabco",
                    Weight = 120.0m,
                    Dimensions = "280x180x85 cm",
                    ImageUrl = "https://images.unsplash.com/photo-1555041469-a586c61ea9bc?w=600&h=600&fit=crop",
                    CategoryId = sofasCategory?.Id ?? 2,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "2-Seater Loveseat",
                    Description = "Compact 2-seater loveseat with elegant design. Perfect for smaller spaces or as an accent piece.",
                    Price = 25000.00m,
                    Unit = "piece",
                    SKU = "SOFA-003",
                    Brand = "Kanabco",
                    Weight = 55.0m,
                    Dimensions = "160x90x85 cm",
                    ImageUrl = "https://images.unsplash.com/photo-1555041469-a586c61ea9bc?w=600&h=600&fit=crop",
                    CategoryId = sofasCategory?.Id ?? 2,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Leather Sofa Set",
                    Description = "Premium leather 3-seater sofa with matching armchairs. Luxurious and durable.",
                    Price = 75000.00m,
                    Unit = "set",
                    SKU = "SOFA-004",
                    Brand = "Kanabco",
                    Weight = 150.0m,
                    Dimensions = "220x95x85 cm (sofa)",
                    ImageUrl = "https://images.unsplash.com/photo-1555041469-a586c61ea9bc?w=600&h=600&fit=crop",
                    CategoryId = sofasCategory?.Id ?? 2,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Convertible Sofa Bed",
                    Description = "Versatile sofa that converts to a comfortable bed. Perfect for guest rooms and small apartments.",
                    Price = 32000.00m,
                    Unit = "piece",
                    SKU = "SOFA-005",
                    Brand = "Kanabco",
                    Weight = 75.0m,
                    Dimensions = "200x95x85 cm",
                    ImageUrl = "https://images.unsplash.com/photo-1555041469-a586c61ea9bc?w=600&h=600&fit=crop",
                    CategoryId = sofasCategory?.Id ?? 2,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                // TABLES Category
                new Product
                {
                    Name = "Dining Table (6-Seater)",
                    Description = "Elegant dining table with solid wood top and modern base. Seats 6 comfortably.",
                    Price = 28000.00m,
                    Unit = "piece",
                    SKU = "TBL-001",
                    Brand = "Kanabco",
                    Weight = 65.0m,
                    Dimensions = "200x100x75 cm",
                    ImageUrl = "https://images.unsplash.com/photo-1586023492125-27b2c045efd7?w=600&h=600&fit=crop",
                    CategoryId = tablesCategory?.Id ?? 4,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Coffee Table",
                    Description = "Modern coffee table with glass top and metal base. Perfect centerpiece for living rooms.",
                    Price = 8500.00m,
                    Unit = "piece",
                    SKU = "TBL-002",
                    Brand = "Kanabco",
                    Weight = 25.0m,
                    Dimensions = "120x60x45 cm",
                    ImageUrl = "https://images.unsplash.com/photo-1586023492125-27b2c045efd7?w=600&h=600&fit=crop",
                    CategoryId = tablesCategory?.Id ?? 4,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Office Desk",
                    Description = "Spacious office desk with drawers and cable management. Ideal for home offices.",
                    Price = 15000.00m,
                    Unit = "piece",
                    SKU = "TBL-003",
                    Brand = "Kanabco",
                    Weight = 45.0m,
                    Dimensions = "150x75x75 cm",
                    ImageUrl = "https://images.unsplash.com/photo-1586023492125-27b2c045efd7?w=600&h=600&fit=crop",
                    CategoryId = tablesCategory?.Id ?? 4,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Side Table Set (2)",
                    Description = "Set of 2 matching side tables with storage. Perfect for sofas and beds.",
                    Price = 6500.00m,
                    Unit = "set",
                    SKU = "TBL-004",
                    Brand = "Kanabco",
                    Weight = 18.0m,
                    Dimensions = "50x50x55 cm (each)",
                    ImageUrl = "https://images.unsplash.com/photo-1586023492125-27b2c045efd7?w=600&h=600&fit=crop",
                    CategoryId = tablesCategory?.Id ?? 4,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Console Table",
                    Description = "Elegant console table with drawers. Perfect for entryways and hallways.",
                    Price = 12000.00m,
                    Unit = "piece",
                    SKU = "TBL-005",
                    Brand = "Kanabco",
                    Weight = 30.0m,
                    Dimensions = "140x40x85 cm",
                    ImageUrl = "https://images.unsplash.com/photo-1586023492125-27b2c045efd7?w=600&h=600&fit=crop",
                    CategoryId = tablesCategory?.Id ?? 4,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                // HOME DECORS Category
                new Product
                {
                    Name = "Modern Floor Lamp",
                    Description = "Contemporary floor lamp with adjustable height and dimmable LED light.",
                    Price = 4500.00m,
                    Unit = "piece",
                    SKU = "DEC-001",
                    Brand = "Kanabco",
                    Weight = 8.5m,
                    Dimensions = "160x30x30 cm",
                    ImageUrl = "https://images.unsplash.com/photo-1507473885765-e6ed057f782c?w=600&h=600&fit=crop",
                    CategoryId = homeDecorsCategory?.Id ?? 5,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Wall Art Set (3 Pieces)",
                    Description = "Set of 3 modern abstract wall art pieces. Ready to hang with included hardware.",
                    Price = 3500.00m,
                    Unit = "set",
                    SKU = "DEC-002",
                    Brand = "Kanabco",
                    Weight = 5.0m,
                    Dimensions = "60x40 cm (each)",
                    ImageUrl = "https://images.unsplash.com/photo-1578301978018-3005759f48f7?w=600&h=600&fit=crop",
                    CategoryId = homeDecorsCategory?.Id ?? 5,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Decorative Mirror",
                    Description = "Large decorative mirror with elegant frame. Perfect for living rooms and bedrooms.",
                    Price = 5500.00m,
                    Unit = "piece",
                    SKU = "DEC-003",
                    Brand = "Kanabco",
                    Weight = 12.0m,
                    Dimensions = "120x80 cm",
                    ImageUrl = "https://images.unsplash.com/photo-1618221195710-dd6b41faaea8?w=600&h=600&fit=crop",
                    CategoryId = homeDecorsCategory?.Id ?? 5,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Throw Pillow Set (4)",
                    Description = "Set of 4 decorative throw pillows with premium covers. Mix of patterns and textures.",
                    Price = 2500.00m,
                    Unit = "set",
                    SKU = "DEC-004",
                    Brand = "Kanabco",
                    Weight = 2.5m,
                    Dimensions = "50x50 cm (each)",
                    ImageUrl = "https://images.unsplash.com/photo-1584100936595-c0655ba5c5a0?w=600&h=600&fit=crop",
                    CategoryId = homeDecorsCategory?.Id ?? 5,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Area Rug (2x3m)",
                    Description = "Modern area rug with geometric pattern. Soft and durable, perfect for living rooms.",
                    Price = 8500.00m,
                    Unit = "piece",
                    SKU = "DEC-005",
                    Brand = "Kanabco",
                    Weight = 15.0m,
                    Dimensions = "200x300 cm",
                    ImageUrl = "https://images.unsplash.com/photo-1600210492486-724fe5c67fb0?w=600&h=600&fit=crop",
                    CategoryId = homeDecorsCategory?.Id ?? 5,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                // ROOMS Category
                new Product
                {
                    Name = "Complete Bedroom Set",
                    Description = "Full bedroom set including bed frame, nightstands, and dresser. Modern and elegant design.",
                    Price = 65000.00m,
                    Unit = "set",
                    SKU = "ROOM-001",
                    Brand = "Kanabco",
                    Weight = 200.0m,
                    Dimensions = "Various",
                    ImageUrl = "https://images.unsplash.com/photo-1631889993951-f6f1358dfc58?w=600&h=600&fit=crop",
                    CategoryId = roomsCategory?.Id ?? 3,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Living Room Package",
                    Description = "Complete living room package with sofa, coffee table, side tables, and TV stand.",
                    Price = 95000.00m,
                    Unit = "set",
                    SKU = "ROOM-002",
                    Brand = "Kanabco",
                    Weight = 280.0m,
                    Dimensions = "Various",
                    ImageUrl = "https://images.unsplash.com/photo-1555041469-a586c61ea9bc?w=600&h=600&fit=crop",
                    CategoryId = roomsCategory?.Id ?? 3,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Dining Room Set",
                    Description = "Complete dining room set with table and 6 chairs. Perfect for family gatherings.",
                    Price = 55000.00m,
                    Unit = "set",
                    SKU = "ROOM-003",
                    Brand = "Kanabco",
                    Weight = 120.0m,
                    Dimensions = "Various",
                    ImageUrl = "https://images.unsplash.com/photo-1586023492125-27b2c045efd7?w=600&h=600&fit=crop",
                    CategoryId = roomsCategory?.Id ?? 3,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                // NEW ARRIVALS Category
                new Product
                {
                    Name = "Minimalist Bookshelf",
                    Description = "New minimalist bookshelf with open shelves and hidden storage. Modern design.",
                    Price = 18000.00m,
                    Unit = "piece",
                    SKU = "NEW-001",
                    Brand = "Kanabco",
                    Weight = 45.0m,
                    Dimensions = "180x40x200 cm",
                    ImageUrl = "https://images.unsplash.com/photo-1586023492125-27b2c045efd7?w=600&h=600&fit=crop",
                    CategoryId = newArrivalsCategory?.Id ?? 6,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Smart TV Stand",
                    Description = "Latest smart TV stand with cable management and integrated lighting.",
                    Price = 22000.00m,
                    Unit = "piece",
                    SKU = "NEW-002",
                    Brand = "Kanabco",
                    Weight = 55.0m,
                    Dimensions = "200x50x60 cm",
                    ImageUrl = "https://images.unsplash.com/photo-1586023492125-27b2c045efd7?w=600&h=600&fit=crop",
                    CategoryId = newArrivalsCategory?.Id ?? 6,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                // BEST SELLING Category
                new Product
                {
                    Name = "Classic Leather Sofa",
                    Description = "Our best-selling classic leather sofa. Comfortable, durable, and timeless design.",
                    Price = 45000.00m,
                    Unit = "piece",
                    SKU = "BEST-001",
                    Brand = "Kanabco",
                    Weight = 95.0m,
                    Dimensions = "220x95x85 cm",
                    ImageUrl = "https://images.unsplash.com/photo-1555041469-a586c61ea9bc?w=600&h=600&fit=crop",
                    CategoryId = bestSellingCategory?.Id ?? 7,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Premium Dining Set",
                    Description = "Customer favorite dining set with extendable table and 8 chairs. High quality materials.",
                    Price = 68000.00m,
                    Unit = "set",
                    SKU = "BEST-002",
                    Brand = "Kanabco",
                    Weight = 150.0m,
                    Dimensions = "220x100x75 cm (table)",
                    ImageUrl = "https://images.unsplash.com/photo-1586023492125-27b2c045efd7?w=600&h=600&fit=crop",
                    CategoryId = bestSellingCategory?.Id ?? 7,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Ergonomic Office Chair",
                    Description = "Top-rated ergonomic office chair with advanced lumbar support. Best seller for home offices.",
                    Price = 12000.00m,
                    Unit = "piece",
                    SKU = "BEST-003",
                    Brand = "Kanabco",
                    Weight = 18.0m,
                    Dimensions = "65x65x120 cm",
                    ImageUrl = "https://images.unsplash.com/photo-1586953208448-b95a79798f07?w=600&h=600&fit=crop",
                    CategoryId = bestSellingCategory?.Id ?? 7,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            return products;
        }

        private static async Task CreateProductInventoriesAsync(ApplicationDbContext context)
        {
            // Get all products and warehouses
            var products = await context.Products.ToListAsync();
            var stores = await context.Warehouses.Where(w => w.IsActive).ToListAsync();
            
            // Check if inventories already exist for these products
            var existingInventories = await context.ProductInventories
                .Where(pi => products.Select(p => p.Id).Contains(pi.ProductId))
                .ToListAsync();
            
            // Remove existing inventories if any
            if (existingInventories.Any())
            {
                context.ProductInventories.RemoveRange(existingInventories);
                await context.SaveChangesAsync();
            }

            var inventories = new List<ProductInventory>();

            foreach (var store in stores)
            {
                foreach (var product in products)
                {
                    // Create inventory for each furniture item in each store
                    var random = new Random();
                    // Furniture items typically have lower stock quantities
                    var storeQuantity = random.Next(3, 12); // Store inventory: 3-12 pieces
                    
                    // Adjust minimum and maximum stock levels for furniture
                    var minStock = product.Unit == "set" ? 1 : 2;  // Minimum 1 set or 2 pieces
                    var maxStock = product.Unit == "set" ? 15 : 25; // Maximum 15 sets or 25 pieces
                    
                    var inventory = new ProductInventory
                    {
                        ProductId = product.Id,
                        WarehouseId = store.Id,
                        Quantity = storeQuantity,
                        POSQuantity = 0, // Not using POS, set to 0
                        Unit = product.Unit,
                        MinimumStockLevel = minStock,
                        MaximumStockLevel = maxStock,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    inventories.Add(inventory);
                }
            }

            context.ProductInventories.AddRange(inventories);
            await context.SaveChangesAsync();
        }
    }
}