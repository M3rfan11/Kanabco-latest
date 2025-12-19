# Permission-Based Role Management System

## Overview
A comprehensive permission-based role management system has been implemented that allows SuperAdmin to customize roles and assign specific permissions (Create, Update, Delete, Read) to each role through the admin dashboard.

## What Was Implemented

### 1. Database Models
- **Permission**: Stores permissions with Resource (e.g., "Products", "Categories") and Action (e.g., "Create", "Update", "Delete", "Read")
- **RolePermission**: Junction table linking roles to permissions (many-to-many relationship)
- **SuperAdmin Role**: Special role that automatically bypasses all permission checks

### 2. Authorization System
- **PermissionAuthorizationHandler**: Custom authorization handler that checks user permissions
- **RequirePermissionAttribute**: Custom attribute for protecting endpoints
  - Usage: `[RequirePermission("Products", "Create")]`
  - SuperAdmin automatically bypasses all checks

### 3. API Endpoints

#### Permissions Controller (SuperAdmin Only)
- `GET /api/permissions` - Get all permissions
- `GET /api/permissions/by-resource` - Get permissions grouped by resource
- `GET /api/permissions/{id}` - Get specific permission

#### Roles Controller (SuperAdmin Only)
- `GET /api/roles` - Get all roles
- `GET /api/roles/{id}` - Get specific role
- `GET /api/roles/{id}/with-permissions` - Get role with all assigned permissions
- `POST /api/roles` - Create new role
- `PATCH /api/roles/{id}` - Update role
- `DELETE /api/roles/{id}` - Delete role
- `POST /api/roles/{roleId}/permissions` - Assign permission to role
  - Body: `{ "permissionId": 1 }`
- `DELETE /api/roles/{roleId}/permissions/{permissionId}` - Remove permission from role

#### Users Controller (Updated)
- `POST /api/users/{userId}/roles` - Assign role to user (SuperAdmin only)
- `DELETE /api/users/{userId}/roles/{roleId}` - Remove role from user (SuperAdmin only)

### 4. Updated Controllers
The following controllers now use permission-based authorization:
- **ProductController**: Uses `Products.Create`, `Products.Update`, `Products.Delete`, `Products.Read`, `Inventory.Update`
- **CategoryController**: Uses `Categories.Create`, `Categories.Update`, `Categories.Delete`, `Categories.Read`
- **VariantInventoryController**: Uses `Inventory.Create`, `Inventory.Update`, `Inventory.Delete`

## Available Permissions

The system seeds 32 permissions across 8 resources:
- **Products**: Create, Update, Delete, Read
- **Categories**: Create, Update, Delete, Read
- **Users**: Create, Update, Delete, Read
- **Roles**: Create, Update, Delete, Read
- **Inventory**: Create, Update, Delete, Read
- **Orders**: Create, Update, Delete, Read
- **Warehouses**: Create, Update, Delete, Read
- **Reports**: Create, Update, Delete, Read

## How SuperAdmin Can Use It

### Step 1: View All Available Permissions
```http
GET /api/permissions
Authorization: Bearer {superadmin_token}
```

### Step 2: Create a Custom Role
```http
POST /api/roles
Authorization: Bearer {superadmin_token}
Content-Type: application/json

{
  "name": "ProductManager",
  "description": "Can manage products and categories"
}
```

### Step 3: Assign Permissions to the Role
```http
POST /api/roles/{roleId}/permissions
Authorization: Bearer {superadmin_token}
Content-Type: application/json

{
  "permissionId": 1  // Products.Create
}
```

Repeat for each permission you want to assign (e.g., Products.Update, Products.Delete, Products.Read, Categories.Create, etc.)

### Step 4: Assign Role to User
```http
POST /api/users/{userId}/roles
Authorization: Bearer {superadmin_token}
Content-Type: application/json

{
  "roleId": 2  // ProductManager role ID
}
```

### Step 5: View Role with Permissions
```http
GET /api/roles/{roleId}/with-permissions
Authorization: Bearer {superadmin_token}
```

## Example Workflow

1. **Create a "Sales Staff" role** that can only read products and create orders:
   - Create role: `POST /api/roles` with name "SalesStaff"
   - Assign `Products.Read` permission
   - Assign `Orders.Create` permission

2. **Create a "Inventory Manager" role** that can manage inventory:
   - Create role: `POST /api/roles` with name "InventoryManager"
   - Assign `Inventory.Create`, `Inventory.Update`, `Inventory.Read` permissions
   - Do NOT assign `Inventory.Delete` (they can't delete inventory)

3. **Assign roles to users**:
   - Assign "SalesStaff" role to user 5: `POST /api/users/5/roles` with roleId
   - Assign "InventoryManager" role to user 6: `POST /api/users/6/roles` with roleId

## Important Notes

1. **SuperAdmin Bypass**: Users with "SuperAdmin" role automatically have access to all endpoints, regardless of permissions
2. **Multiple Roles**: Users can have multiple roles, and permissions are combined (OR logic)
3. **Role Deletion**: Cannot delete a role that is assigned to any users
4. **Permission Seeding**: Permissions are automatically seeded on first run
5. **SuperAdmin Permissions**: SuperAdmin role automatically gets all permissions assigned during seeding

## Migration Applied

The database migration `AddPermissionSystem` has been applied, creating:
- `Permissions` table
- `RolePermissions` junction table
- All necessary indexes and foreign keys

## Testing

The system has been tested and compiles successfully. All controllers have been updated to use the new permission system where appropriate.

## Next Steps for Frontend Integration

1. Create a role management page in the admin dashboard
2. Display all permissions grouped by resource
3. Allow SuperAdmin to:
   - Create/edit roles
   - Check/uncheck permissions for each role
   - Assign roles to users
   - View which users have which roles







