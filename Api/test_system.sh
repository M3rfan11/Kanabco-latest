#!/bin/bash

# Comprehensive System Testing Script
# Tests all major functionality

API_URL="http://localhost:8080"
TOKEN=""
TEST_RESULTS="/tmp/test_results.txt"

echo "=== COMPREHENSIVE SYSTEM TESTING ===" > $TEST_RESULTS
echo "Date: $(date)" >> $TEST_RESULTS
echo "" >> $TEST_RESULTS

# Get authentication token
echo "1. Authenticating..."
TOKEN=$(curl -s -X POST "$API_URL/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"Admin123!"}' \
  | jq -r '.accessToken // empty')

if [ -z "$TOKEN" ]; then
  echo "❌ Authentication failed" >> $TEST_RESULTS
  exit 1
fi

echo "✅ Authentication successful" >> $TEST_RESULTS
echo "" >> $TEST_RESULTS

# Test 1: Categories
echo "=== TEST 1: CATEGORIES ===" >> $TEST_RESULTS

# Get categories
echo "1.1 Getting categories..."
CATEGORIES=$(curl -s "$API_URL/api/Category" -H "Authorization: Bearer $TOKEN")
CAT_COUNT=$(echo "$CATEGORIES" | jq 'length')
echo "   Found $CAT_COUNT categories" >> $TEST_RESULTS

# Test 2: Products
echo "=== TEST 2: PRODUCTS ===" >> $TEST_RESULTS

# Get products
echo "2.1 Getting products..."
PRODUCTS=$(curl -s "$API_URL/api/Product/all" -H "Authorization: Bearer $TOKEN")
PROD_COUNT=$(echo "$PRODUCTS" | jq 'length')
echo "   Found $PROD_COUNT products" >> $TEST_RESULTS

# Test 3: Inventory
echo "=== TEST 3: INVENTORY ===" >> $TEST_RESULTS

# Get low stock items
echo "3.1 Getting low stock items..."
LOW_STOCK=$(curl -s "$API_URL/api/Dashboard/low-stock-items" -H "Authorization: Bearer $TOKEN")
LOW_STOCK_COUNT=$(echo "$LOW_STOCK" | jq 'length')
echo "   Found $LOW_STOCK_COUNT low stock items" >> $TEST_RESULTS

# Test 4: Sales
echo "=== TEST 4: SALES ===" >> $TEST_RESULTS

# Get sales orders
echo "4.1 Getting sales orders..."
SALES=$(curl -s "$API_URL/api/Sales" -H "Authorization: Bearer $TOKEN")
SALES_COUNT=$(echo "$SALES" | jq 'length')
echo "   Found $SALES_COUNT sales orders" >> $TEST_RESULTS

# Test 5: Customers
echo "=== TEST 5: CUSTOMERS ===" >> $TEST_RESULTS

# Get customers
echo "5.1 Getting customers..."
CUSTOMERS=$(curl -s "$API_URL/api/Sales/customers" -H "Authorization: Bearer $TOKEN")
CUST_COUNT=$(echo "$CUSTOMERS" | jq 'length')
echo "   Found $CUST_COUNT customers" >> $TEST_RESULTS

# Test 6: Permissions
echo "=== TEST 6: PERMISSIONS ===" >> $TEST_RESULTS

# Get user permissions
echo "6.1 Getting user permissions..."
PERMS=$(curl -s "$API_URL/api/permissions/my-permissions" -H "Authorization: Bearer $TOKEN")
PERM_COUNT=$(echo "$PERMS" | jq 'length')
echo "   Found $PERM_COUNT permission groups" >> $TEST_RESULTS

# Test 7: Dashboard Stats
echo "=== TEST 7: DASHBOARD ===" >> $TEST_RESULTS

# Get dashboard stats
echo "7.1 Getting dashboard stats..."
STATS=$(curl -s "$API_URL/api/Dashboard/stats" -H "Authorization: Bearer $TOKEN")
echo "   Stats retrieved successfully" >> $TEST_RESULTS

echo "" >> $TEST_RESULTS
echo "=== TEST SUMMARY ===" >> $TEST_RESULTS
echo "Categories: $CAT_COUNT" >> $TEST_RESULTS
echo "Products: $PROD_COUNT" >> $TEST_RESULTS
echo "Low Stock Items: $LOW_STOCK_COUNT" >> $TEST_RESULTS
echo "Sales Orders: $SALES_COUNT" >> $TEST_RESULTS
echo "Customers: $CUST_COUNT" >> $TEST_RESULTS
echo "Permission Groups: $PERM_COUNT" >> $TEST_RESULTS

cat $TEST_RESULTS




