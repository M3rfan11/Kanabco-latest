#!/bin/bash

echo "=== API Testing Script ==="
echo ""

# Step 1: Login
echo "1. Logging in as admin@example.com..."
LOGIN_RESPONSE=$(curl -s -X POST http://localhost:8080/api/Auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"Admin123!"}')

TOKEN=$(echo $LOGIN_RESPONSE | grep -o '"accessToken":"[^"]*' | cut -d'"' -f4)
ROLES=$(echo $LOGIN_RESPONSE | grep -o '"roles":\[[^]]*\]')

echo "Login response roles: $ROLES"
echo "Token extracted: ${#TOKEN} characters"
echo ""

if [ -z "$TOKEN" ]; then
  echo "❌ Failed to get token!"
  exit 1
fi

# Step 2: Check user info
echo "2. Checking user info (/api/Auth/me)..."
USER_INFO=$(curl -s -X GET http://localhost:8080/api/Auth/me \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json")
echo "$USER_INFO" | grep -o '"roles":\[[^]]*\]' || echo "No roles found"
echo ""

# Step 3: Test Products
echo "3. Testing Products (/api/Product/all)..."
PRODUCTS_STATUS=$(curl -s -w "%{http_code}" -o /tmp/products.json -X GET "http://localhost:8080/api/Product/all" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json")
if [ "$PRODUCTS_STATUS" = "200" ]; then
  PRODUCT_COUNT=$(cat /tmp/products.json | grep -o '"id"' | wc -l | tr -d ' ')
  echo "✅ Products: HTTP $PRODUCTS_STATUS - Found $PRODUCT_COUNT products"
else
  echo "❌ Products: HTTP $PRODUCTS_STATUS"
  cat /tmp/products.json
fi
echo ""

# Step 4: Test Categories
echo "4. Testing Categories (/api/Category/all)..."
CATEGORIES_STATUS=$(curl -s -w "%{http_code}" -o /tmp/categories.json -X GET "http://localhost:8080/api/Category/all" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json")
if [ "$CATEGORIES_STATUS" = "200" ]; then
  CATEGORY_COUNT=$(cat /tmp/categories.json | grep -o '"id"' | wc -l | tr -d ' ')
  echo "✅ Categories: HTTP $CATEGORIES_STATUS - Found $CATEGORY_COUNT categories"
else
  echo "❌ Categories: HTTP $CATEGORIES_STATUS"
  cat /tmp/categories.json
fi
echo ""

# Step 5: Test Roles
echo "5. Testing Roles (/api/roles)..."
ROLES_STATUS=$(curl -s -w "%{http_code}" -o /tmp/roles.json -X GET "http://localhost:8080/api/roles" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json")
if [ "$ROLES_STATUS" = "200" ]; then
  ROLE_COUNT=$(cat /tmp/roles.json | grep -o '"id"' | wc -l | tr -d ' ')
  echo "✅ Roles: HTTP $ROLES_STATUS - Found $ROLE_COUNT roles"
else
  echo "❌ Roles: HTTP $ROLES_STATUS"
  cat /tmp/roles.json
fi
echo ""

# Step 6: Test Inventory (if endpoint exists)
echo "6. Testing Inventory..."
INVENTORY_STATUS=$(curl -s -w "%{http_code}" -o /tmp/inventory.json -X GET "http://localhost:8080/api/ProductInventory" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json")
if [ "$INVENTORY_STATUS" = "200" ]; then
  echo "✅ Inventory: HTTP $INVENTORY_STATUS"
else
  echo "⚠️  Inventory: HTTP $INVENTORY_STATUS (may not exist or require different endpoint)"
fi
echo ""

echo "=== Test Complete ==="







