# Low Stock Items Process Documentation

## Overview
The `GetLowStockItems` endpoint in `DashboardController.cs` identifies and returns products and variants that are running low on stock, excluding products marked as "Always Available".

## Process Flow

### 1. Data Loading
- **Product Inventories**: Loads all `ProductInventories` with related `Product` and `Warehouse` data
- **Variant Inventories**: Loads all `VariantInventories` with related `ProductVariant`, `Product`, and `Warehouse` data

### 2. Filtering Logic

#### For Each Product Inventory:
```csharp
// STEP 1: Skip products that are always available
if (product.AlwaysAvailable)
{
    continue; // Exclude from low stock items entirely
}

// STEP 2: Check if it's a low stock item
bool isLowStock = false;

// Criteria for low stock:
// - Out of stock (quantity = 0)
// - Quantity at or below minimum stock level
// - Quantity within 2 units above minimum (warning zone)
```

#### For Each Variant Inventory:
```csharp
// STEP 1: Skip variants of products that are always available
if (product.AlwaysAvailable)
{
    continue; // Exclude from low stock items entirely
}

// STEP 2: Same low stock criteria as products
```

### 3. Severity Classification

Items are classified by severity:
- **Critical**: Quantity is 0 (out of stock)
- **High**: Quantity is below 50% of minimum stock level
- **Medium**: Quantity is at or below minimum stock level
- **Low**: Quantity is within 2 units above minimum (warning zone)

### 4. Response Structure

Each low stock item includes:
- Type (Product or Variant)
- Product information (ID, Name, SKU)
- Variant information (if applicable)
- Warehouse information
- Current quantity and stock levels
- Severity and availability status

## Key Features

### ✅ Always Available Exclusion
Products with `AlwaysAvailable = true` are **completely excluded** from the low stock items list, regardless of their inventory levels. This is because:
- These products don't require inventory tracking
- They're always available for sale regardless of stock
- Including them would create false alerts

### ✅ Comprehensive Coverage
- Handles both product-level and variant-level inventories
- Checks multiple warehouses
- Considers minimum stock levels and warning zones

### ✅ Severity-Based Prioritization
Items are sorted by quantity and severity to help prioritize restocking efforts.

## Testing Results

**Backend Status**: ✅ Running on port 8080
**Always Available Products in DB**: 
- Bar Stool (Set of 2)
- Modern Floor Lamp
- Complete Bedroom Set
- Minimalist Bookshelf

**Verification**: ✅ These products do NOT appear in low stock items (0 found)

## Endpoint

**URL**: `GET /api/Dashboard/low-stock-items`
**Authorization**: Required (Bearer token)
**Response**: Array of `LowStockItemResponse` objects

## Code Location

File: `Api/Controllers/DashboardController.cs`
Method: `GetLowStockItems()` (lines 151-331)




