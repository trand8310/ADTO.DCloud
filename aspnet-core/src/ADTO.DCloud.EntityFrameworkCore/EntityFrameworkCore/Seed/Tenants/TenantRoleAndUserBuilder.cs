using ADTOSharp.Authorization;
using ADTOSharp.Authorization.Roles;
using ADTOSharp.Authorization.Users;
using ADTOSharp.MultiTenancy;
using ADTO.DCloud.Authorization;
using ADTO.DCloud.Authorization.Roles;
using ADTO.DCloud.Authorization.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq;
using System;

namespace ADTO.DCloud.EntityFrameworkCore.Seed.Tenants;

public class TenantRoleAndUserBuilder
{
    private readonly DCloudDbContext _context;
    private readonly Guid _tenantId;

    public TenantRoleAndUserBuilder(DCloudDbContext context, Guid tenantId)
    {
        _context = context;
        _tenantId = tenantId;
    }

    public void Create()
    {
        CreateRolesAndUsers();
    }

    private void CreateRolesAndUsers()
    {
        // Admin role

        var adminRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.Admin);
        if (adminRole == null)
        {
            adminRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.Admin, StaticRoleNames.Tenants.Admin) { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),IsStatic = true }).Entity;
            _context.SaveChanges();
        }

        //User role

        var userRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.User);
        if (userRole == null)
        {
            _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.User, StaticRoleNames.Tenants.User) { IsStatic = true, IsDefault = true, Id = Guid.Parse("00000000-0000-0000-0000-000000000002") });
            _context.SaveChanges();
        }


        // Grant all permissions to admin role

        var grantedPermissions = _context.Permissions.IgnoreQueryFilters()
            .OfType<RolePermissionSetting>()
            .Where(p => p.TenantId == _tenantId && p.RoleId == adminRole.Id)
            .Select(p => p.Name)
            .ToList();

        var permissions = PermissionFinder
            .GetAllPermissions(new DCloudAuthorizationProvider(DCloudConsts.MultiTenancyEnabled))
            .Where(p => p.MultiTenancySides.HasFlag(MultiTenancySides.Tenant) &&
                        !grantedPermissions.Contains(p.Name))
            .ToList();

        if (permissions.Any())
        {
            _context.Permissions.AddRange(
                permissions.Select(permission => new RolePermissionSetting
                {
                    TenantId = _tenantId,
                    Name = permission.Name,
                    IsGranted = true,
                    RoleId = adminRole.Id
                })
            );
            _context.SaveChanges();
        }

        // Admin user

        var adminUser = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == _tenantId && u.UserName == ADTOSharpUserBase.AdminUserName);
        if (adminUser == null)
        {
            adminUser = User.CreateTenantAdminUser(_tenantId, "admin@defaulttenant.com");
            adminUser.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(adminUser, "123qwe");
            adminUser.IsEmailConfirmed = true;
            adminUser.IsActive = true;
            adminUser.Id = Guid.Parse("00000000-0000-0000-0000-000000000001");

            _context.Users.Add(adminUser);
            _context.SaveChanges();

            // Assign Admin role to admin user
            _context.UserRoles.Add(new UserRole(_tenantId, adminUser.Id, adminRole.Id) { Id = Guid.Parse("00000000-0000-0000-0000-000000000001") });
            _context.SaveChanges();
        }
    }
}

