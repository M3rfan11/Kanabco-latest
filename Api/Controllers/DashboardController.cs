using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.DTOs;
using Api.Services;
using Api.Attributes;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize] // Removed class-level authorize - add to individual methods as needed
public class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IAuditService _auditService;
    private readonly IRevenueTrackingService _revenueTrackingService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(ApplicationDbContext context, IAuditService auditService, IRevenueTrackingService revenueTrackingService, ILogger<DashboardController> logger)
    {
        _context = context;
        _auditService = auditService;
        _revenueTrackingService = revenueTrackingService;
        _logger = logger;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
        return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
    }

    /// <summary>
    /// Get dashboard statistics with real-time revenue tracking
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<DashboardStatsResponse>> GetStats()
    {
        try
        {
            // Use revenue tracking service for real-time totals
            var totalRevenue = await _revenueTrackingService.GetTotalRevenueAsync();
            var totalCosts = await _revenueTrackingService.GetTotalCostsAsync();

            var stats = new DashboardStatsResponse
            {
                TotalProducts = await _context.Products.CountAsync(p => p.IsActive),
                TotalWarehouses = await _context.Warehouses.CountAsync(),
                TotalUsers = await _context.Users.CountAsync(u => u.IsActive),
                TotalInventory = await _context.ProductInventories.SumAsync(pi => pi.Quantity),
                PendingPurchases = await _context.PurchaseOrders.CountAsync(po => po.Status == "Pending"),
                PendingSales = await _context.SalesOrders.CountAsync(so => so.Status == "Pending"),
                LowStockItems = await _context.ProductInventories
                    .CountAsync(pi => pi.Quantity <= pi.MinimumStockLevel),
                TotalRevenue = totalRevenue, // Real-time revenue from tracking service
                TotalCosts = totalCosts, // Real-time costs from tracking service
                PendingRequests = await _context.ProductRequests.CountAsync(pr => pr.Status == "Pending"),
                CompletedAssemblies = await _context.ProductAssemblies.CountAsync(pa => pa.Status == "Completed"),
            };

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dashboard statistics");
            return StatusCode(500, new { message = "An error occurred while retrieving dashboard statistics" });
        }
    }

    /// <summary>
    /// Get recent activity
    /// </summary>
    [HttpGet("recent-activity")]
    public async Task<ActionResult<IEnumerable<RecentActivityResponse>>> GetRecentActivity()
    {
        try
        {
            var activities = new List<RecentActivityResponse>();

            // Recent purchases
            var recentPurchases = await _context.PurchaseOrders
                .Include(po => po.CreatedByUser)
                .OrderByDescending(po => po.CreatedAt)
                .Take(5)
                .Select(po => new RecentActivityResponse
                {
                    Id = po.Id,
                    Type = "Purchase",
                    Title = $"Purchase Order #{po.OrderNumber}",
                    Description = $"Order from {po.SupplierName} - ${po.TotalAmount:F2}",
                    Status = po.Status,
                    CreatedAt = po.CreatedAt,
                    UserName = po.CreatedByUser.FullName
                })
                .ToListAsync();

            // Recent sales
            var recentSales = await _context.SalesOrders
                .Include(so => so.CreatedByUser)
                .OrderByDescending(so => so.CreatedAt)
                .Take(5)
                .Select(so => new RecentActivityResponse
                {
                    Id = so.Id,
                    Type = "Sale",
                    Title = $"Sales Order #{so.OrderNumber}",
                    Description = $"Order for {so.CustomerName} - ${so.TotalAmount:F2}",
                    Status = so.Status,
                    CreatedAt = so.CreatedAt,
                    UserName = so.CreatedByUser.FullName
                })
                .ToListAsync();

            // Recent product requests
            var recentRequests = await _context.ProductRequests
                .Include(pr => pr.RequestedByUser)
                .Include(pr => pr.Warehouse)
                .OrderByDescending(pr => pr.RequestDate)
                .Take(5)
                .Select(pr => new RecentActivityResponse
                {
                    Id = pr.Id,
                    Type = "Product Request",
                    Title = $"Product Request #{pr.Id}",
                    Description = $"Request for {pr.Warehouse.Name}",
                    Status = pr.Status,
                    CreatedAt = pr.RequestDate,
                    UserName = pr.RequestedByUser.FullName
                })
                .ToListAsync();

            activities.AddRange(recentPurchases);
            activities.AddRange(recentSales);
            activities.AddRange(recentRequests);

            return Ok(activities.OrderByDescending(a => a.CreatedAt).Take(10));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recent activity");
            return StatusCode(500, new { message = "An error occurred while retrieving recent activity" });
        }
    }

    /// <summary>
    /// Get low stock items for both products and variants
    /// </summary>
    [HttpGet("low-stock-items")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<LowStockItemResponse>>> GetLowStockItems()
    {
        try
        {
            _logger.LogInformation("=== GetLowStockItems endpoint called - NEW CODE ===");
            Console.WriteLine("=== GetLowStockItems endpoint called - NEW CODE ===");
            var lowStockItems = new List<LowStockItemResponse>();

            // Load all product inventories with related data - only active products
            var productInventories = await _context.ProductInventories
                .Include(pi => pi.Product)
                .Include(pi => pi.Warehouse)
                .Where(pi => pi.Product != null && 
                             pi.Warehouse != null && 
                             pi.Product.IsActive)
                .ToListAsync();

            // Filter for low stock product inventories
            foreach (var pi in productInventories)
            {
                var product = pi.Product!;
                var warehouse = pi.Warehouse!;
                
                // Skip products that are always available - they don't need inventory tracking
                if (product.AlwaysAvailable)
                {
                    continue;
                }
                
                // Check if this is a low stock item
                bool isLowStock = false;
                
                // Out of stock
                if (pi.Quantity == 0)
                {
                    isLowStock = true;
                }
                // Or quantity is at or below minimum stock level
                else if (pi.MinimumStockLevel.HasValue && pi.Quantity <= pi.MinimumStockLevel.Value)
                {
                    isLowStock = true;
                }
                // Or quantity is within 2 units above minimum (warning zone)
                else if (pi.MinimumStockLevel.HasValue && pi.Quantity <= (pi.MinimumStockLevel.Value + 2))
                {
                    isLowStock = true;
                }

                if (isLowStock)
                {
                    var isOutOfStock = pi.Quantity == 0;
                    var isNotAvailable = isOutOfStock && !product.AlwaysAvailable && !product.SellWhenOutOfStock;
                    
                    // Determine severity
                    string severity;
                    if (isOutOfStock)
                    {
                        severity = "Critical";
                    }
                    else if (pi.MinimumStockLevel.HasValue && pi.Quantity < pi.MinimumStockLevel.Value * 0.5m)
                    {
                        severity = "High";
                    }
                    else if (pi.MinimumStockLevel.HasValue && pi.Quantity <= pi.MinimumStockLevel.Value)
                    {
                        severity = "Medium";
                    }
                    else
                    {
                        severity = "Low";
                    }

                    // Use product unit if available, otherwise use inventory unit, default to "piece"
                    var displayUnit = product.Unit ?? pi.Unit ?? "piece";
                    // Normalize "bottle" to "piece" for furniture items
                    if (displayUnit == "bottle")
                    {
                        displayUnit = "piece";
                    }

                    lowStockItems.Add(new LowStockItemResponse
                    {
                        Type = "Product",
                        Id = pi.Id,
                        ProductId = product.Id,
                        ProductName = product.Name ?? "Unknown Product",
                        ProductSKU = product.SKU,
                        VariantId = null,
                        VariantAttributes = null,
                        WarehouseId = warehouse.Id,
                        WarehouseName = warehouse.Name ?? "Unknown Warehouse",
                        Quantity = pi.Quantity,
                        Unit = displayUnit,
                        MinimumStockLevel = pi.MinimumStockLevel,
                        MaximumStockLevel = pi.MaximumStockLevel,
                        IsOutOfStock = isOutOfStock,
                        IsNotAvailable = isNotAvailable,
                        Severity = severity,
                        UpdatedAt = pi.UpdatedAt
                    });
                }
            }

            // Load all variant inventories with related data - only active products
            var variantInventories = await _context.VariantInventories
                .Include(vi => vi.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                .Include(vi => vi.Warehouse)
                .Where(vi => vi.ProductVariant != null && 
                             vi.ProductVariant.Product != null && 
                             vi.ProductVariant.Product.IsActive &&
                             vi.Warehouse != null)
                .ToListAsync();

            // Filter for low stock variant inventories
            foreach (var vi in variantInventories)
            {
                var variant = vi.ProductVariant!;
                var product = variant.Product!;
                var warehouse = vi.Warehouse!;
                
                // Skip products that are always available - they don't need inventory tracking
                if (product.AlwaysAvailable)
                {
                    continue;
                }
                
                // Check if this is a low stock item
                bool isLowStock = false;
                
                // Out of stock
                if (vi.Quantity == 0)
                {
                    isLowStock = true;
                }
                // Or quantity is at or below minimum stock level
                else if (vi.MinimumStockLevel.HasValue && vi.Quantity <= vi.MinimumStockLevel.Value)
                {
                    isLowStock = true;
                }
                // Or quantity is within 2 units above minimum (warning zone)
                else if (vi.MinimumStockLevel.HasValue && vi.Quantity <= (vi.MinimumStockLevel.Value + 2))
                {
                    isLowStock = true;
                }

                if (isLowStock)
                {
                    var isOutOfStock = vi.Quantity == 0;
                    var isNotAvailable = isOutOfStock && !product.AlwaysAvailable && !product.SellWhenOutOfStock;
                    
                    // Determine severity
                    string severity;
                    if (isOutOfStock)
                    {
                        severity = "Critical";
                    }
                    else if (vi.MinimumStockLevel.HasValue && vi.Quantity < vi.MinimumStockLevel.Value * 0.5m)
                    {
                        severity = "High";
                    }
                    else if (vi.MinimumStockLevel.HasValue && vi.Quantity <= vi.MinimumStockLevel.Value)
                    {
                        severity = "Medium";
                    }
                    else
                    {
                        severity = "Low";
                    }

                    // Use product unit if available, otherwise use inventory unit, default to "piece"
                    var variantDisplayUnit = product.Unit ?? vi.Unit ?? "piece";
                    // Normalize "bottle" to "piece" for furniture items
                    if (variantDisplayUnit == "bottle")
                    {
                        variantDisplayUnit = "piece";
                    }

                    lowStockItems.Add(new LowStockItemResponse
                    {
                        Type = "Variant",
                        Id = vi.Id,
                        ProductId = product.Id,
                        ProductName = product.Name ?? "Unknown Product",
                        ProductSKU = product.SKU,
                        VariantId = variant.Id,
                        VariantAttributes = variant.Attributes,
                        WarehouseId = warehouse.Id,
                        WarehouseName = warehouse.Name ?? "Unknown Warehouse",
                        Quantity = vi.Quantity,
                        Unit = variantDisplayUnit,
                        MinimumStockLevel = vi.MinimumStockLevel,
                        MaximumStockLevel = vi.MaximumStockLevel,
                        IsOutOfStock = isOutOfStock,
                        IsNotAvailable = isNotAvailable,
                        Severity = severity,
                        UpdatedAt = vi.UpdatedAt
                    });
                }
            }

            _logger.LogInformation($"Returning {lowStockItems.Count} low stock items");
            return Ok(lowStockItems.OrderBy(item => item.Quantity).ThenBy(item => item.Severity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving low stock items: {Message}", ex.Message);
            return StatusCode(500, new { message = "An error occurred while retrieving low stock items", error = ex.Message });
        }
    }

    /// <summary>
    /// Refresh revenue and cost totals (Admin only)
    /// </summary>
    [HttpPost("refresh-totals")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RefreshTotals()
    {
        try
        {
            await _revenueTrackingService.RefreshAllTotalsAsync();
            return Ok(new { message = "Revenue and cost totals refreshed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing totals");
            return StatusCode(500, new { message = "An error occurred while refreshing totals" });
        }
    }
}