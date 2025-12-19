#!/bin/bash

# Test script to verify variant inventory connection with products
# This script tests the API endpoints to ensure products with variants correctly link to variant inventory

API_BASE_URL="http://localhost:8080"
TOKEN=""

echo "=== Testing Variant Inventory Connection ==="
echo ""

# Step 1: Login to get token
echo "1. Logging in..."
LOGIN_RESPONSE=$(curl -s -X POST "$API_BASE_URL/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@company.com",
    "password": "Admin123!"
  }')

TOKEN=$(echo $LOGIN_RESPONSE | grep -o '"token":"[^"]*' | cut -d'"' -f4)

if [ -z "$TOKEN" ]; then
  echo "ERROR: Failed to login"
  echo "Response: $LOGIN_RESPONSE"
  exit 1
fi

echo "✓ Login successful"
echo ""

# Step 2: Get all products
echo "2. Fetching all products..."
PRODUCTS_RESPONSE=$(curl -s -X GET "$API_BASE_URL/api/Product/all" \
  -H "Authorization: Bearer $TOKEN")

# Check if we got products
PRODUCT_COUNT=$(echo $PRODUCTS_RESPONSE | grep -o '"id"' | wc -l | tr -d ' ')
echo "✓ Found $PRODUCT_COUNT products"
echo ""

# Step 3: Find a product with variants
echo "3. Looking for products with variants..."
PRODUCTS_WITH_VARIANTS=$(echo $PRODUCTS_RESPONSE | grep -o '"variants":\[[^]]*\]' | grep -v '\[\]' | wc -l | tr -d ' ')
echo "✓ Found products with variants: $PRODUCTS_WITH_VARIANTS"
echo ""

# Step 4: Get first product with variants and check its TotalQuantity
echo "4. Checking TotalQuantity calculation for products with variants..."
FIRST_PRODUCT_WITH_VARIANTS=$(echo $PRODUCTS_RESPONSE | grep -o '{"id":[0-9]*,"name":"[^"]*"[^}]*"variants":\[[^]]*\][^}]*}' | head -1)

if [ ! -z "$FIRST_PRODUCT_WITH_VARIANTS" ]; then
  PRODUCT_ID=$(echo $FIRST_PRODUCT_WITH_VARIANTS | grep -o '"id":[0-9]*' | cut -d':' -f2)
  PRODUCT_NAME=$(echo $FIRST_PRODUCT_WITH_VARIANTS | grep -o '"name":"[^"]*' | cut -d'"' -f4)
  TOTAL_QUANTITY=$(echo $FIRST_PRODUCT_WITH_VARIANTS | grep -o '"totalQuantity":[0-9.]*' | cut -d':' -f2)
  
  echo "  Product: $PRODUCT_NAME (ID: $PRODUCT_ID)"
  echo "  TotalQuantity from API: $TOTAL_QUANTITY"
  
  # Get variant IDs for this product
  VARIANTS=$(echo $FIRST_PRODUCT_WITH_VARIANTS | grep -o '"variants":\[[^]]*\]')
  VARIANT_IDS=$(echo $VARIANTS | grep -o '"id":[0-9]*' | cut -d':' -f2)
  
  echo "  Variant IDs: $VARIANT_IDS"
  
  # Calculate expected total from variant inventories
  EXPECTED_TOTAL=0
  for VARIANT_ID in $VARIANT_IDS; do
    VARIANT_INVENTORY=$(curl -s -X GET "$API_BASE_URL/api/VariantInventory/variant/$VARIANT_ID" \
      -H "Authorization: Bearer $TOKEN")
    
    VARIANT_QUANTITY=$(echo $VARIANT_INVENTORY | grep -o '"quantity":[0-9.]*' | cut -d':' -f2 | head -1)
    if [ ! -z "$VARIANT_QUANTITY" ]; then
      EXPECTED_TOTAL=$(echo "$EXPECTED_TOTAL + $VARIANT_QUANTITY" | bc)
      echo "    Variant $VARIANT_ID inventory: $VARIANT_QUANTITY"
    else
      echo "    Variant $VARIANT_ID: No inventory (0)"
    fi
  done
  
  echo "  Expected TotalQuantity (sum of variant inventories): $EXPECTED_TOTAL"
  
  if [ "$TOTAL_QUANTITY" = "$EXPECTED_TOTAL" ]; then
    echo "  ✓ TotalQuantity matches expected value!"
  else
    echo "  ✗ MISMATCH: TotalQuantity ($TOTAL_QUANTITY) != Expected ($EXPECTED_TOTAL)"
  fi
else
  echo "  No products with variants found"
fi

echo ""
echo "=== Test Complete ==="








