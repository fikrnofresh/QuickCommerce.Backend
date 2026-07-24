using Microsoft.EntityFrameworkCore;
using QuickCommerce.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickCommerce.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            Console.WriteLine("🔥 DB SEEDER EXECUTING...");

            await context.Database.MigrateAsync();

            var now = DateTime.UtcNow;

            // =====================================================
            // 1️⃣ PERMISSIONS
            // =====================================================

            if (!context.Permissions.Any())
            {
                var permissionNames = new List<string>
                {
                    "DASHBOARD.VIEW",
                    "STORE.CREATE",
                    "STORE.UPDATE",
                    "STORE.DELETE",
                    "STORE.VIEW",
                    "USER.CREATE",
                    "USER.UPDATE",
                    "USER.DELETE",
                    "USER.VIEW",
                    "ROLE.CREATE",
                    "ROLE.UPDATE",
                    "ROLE.DELETE",
                    "ROLE.VIEW",
                    "PRODUCT.CREATE",
                    "PRODUCT.UPDATE",
                    "PRODUCT.DELETE",
                    "PRODUCT.VIEW",
                    "INVENTORY.VIEW",
                    "INVENTORY.UPDATE",
                    "ORDER.VIEW",
                    "ORDER.UPDATE",
                    "ORDER.CANCEL",
                    "DELIVERY.ASSIGN",
                    "DELIVERY.UPDATE",
                    "DELIVERY.VIEW",
                    "ANALYTICS.VIEW",
                    "PLATFORM_FEE.UPDATE",
                    "SETTLEMENT.VIEW"
                };

                var permissions = permissionNames.Select(name => new Permission
                {
                    Name = name,
                    Description = name.Replace(".", " "),
                    Category = name.Split('.')[0],
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now
                }).ToList();

                await context.Permissions.AddRangeAsync(permissions);
                await context.SaveChangesAsync();
            }

            // =====================================================
            // 2️⃣ ROLES
            // =====================================================

            if (!context.Roles.Any())
            {
                var roles = new List<Role>
                {
                    new Role
                    {
                        Name = "SUPER_ADMIN",
                        Scope = "PLATFORM",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Role
                    {
                        Name = "STORE_ADMIN",
                        Scope = "STORE",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Role
                    {
                        Name = "STAFF",
                        Scope = "STORE",
                        CreatedAt = now,
                        UpdatedAt = now
                    }
                };

                await context.Roles.AddRangeAsync(roles);
                await context.SaveChangesAsync();
            }

            // =====================================================
            // 3️⃣ MAP ROLE → PERMISSIONS
            // =====================================================

            var superAdminRole = context.Roles.First(r => r.Name == "SUPER_ADMIN");
            var storeAdminRole = context.Roles.First(r => r.Name == "STORE_ADMIN");
            var staffRole = context.Roles.First(r => r.Name == "STAFF");

            var allPermissions = context.Permissions.ToList();

            if (!context.RolePermissions.Any())
            {
                // SUPER ADMIN → All Permissions
                foreach (var permission in allPermissions)
                {
                    context.RolePermissions.Add(new RolePermission
                    {
                        RoleId = superAdminRole.Id,
                        PermissionId = permission.Id
                    });
                }

                // STORE ADMIN → All except 2 platform controls
                var storeAdminPermissions = allPermissions
                    .Where(p => p.Name != "PLATFORM_FEE.UPDATE"
                             && p.Name != "SETTLEMENT.VIEW")
                    .ToList();

                foreach (var permission in storeAdminPermissions)
                {
                    context.RolePermissions.Add(new RolePermission
                    {
                        RoleId = storeAdminRole.Id,
                        PermissionId = permission.Id
                    });
                }

                // STAFF → Limited
                var staffPermissionNames = new List<string>
                {
                    "DASHBOARD.VIEW",
                    "ORDER.VIEW",
                    "ORDER.UPDATE",
                    "DELIVERY.VIEW",
                    "INVENTORY.VIEW",
                    "PRODUCT.VIEW"
                };

                var staffPermissions = allPermissions
                    .Where(p => staffPermissionNames.Contains(p.Name))
                    .ToList();

                foreach (var permission in staffPermissions)
                {
                    context.RolePermissions.Add(new RolePermission
                    {
                        RoleId = staffRole.Id,
                        PermissionId = permission.Id
                    });
                }

                await context.SaveChangesAsync();
            }

            // =====================================================
            // 4️⃣ DEFAULT SUPER ADMIN USER
            // =====================================================

            if (!context.Users.Any(u => u.PhoneNumber == "9999999999"))
            {
                var user = new User
                {
                    PhoneNumber = "9999999999",
                    IsPhoneVerified = true,
                    CreatedAt = now,
                    UpdatedAt = now
                };

                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                context.UserRoles.Add(new UserRole
                {
                    UserId = user.Id,
                    RoleId = superAdminRole.Id
                });

                await context.SaveChangesAsync();
            }

            Console.WriteLine("✅ DB SEEDING COMPLETED.");
        }
    }
}