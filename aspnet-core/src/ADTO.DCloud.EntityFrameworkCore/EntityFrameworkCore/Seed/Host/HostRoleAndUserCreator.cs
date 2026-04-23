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

namespace ADTO.DCloud.EntityFrameworkCore.Seed.Host;

public class HostRoleAndUserCreator
{
    private readonly DCloudDbContext _context;

    public HostRoleAndUserCreator(DCloudDbContext context)
    {
        _context = context;
    }

    public void Create()
    {
        CreateHostRoleAndUsers();
    }

    private void CreateHostRoleAndUsers()
    {
        // Admin role for host

        var adminRoleForHost = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == null && r.Name == StaticRoleNames.Host.Admin);
        if (adminRoleForHost == null)
        {
            adminRoleForHost = _context.Roles.Add(new Role(null, StaticRoleNames.Host.Admin, StaticRoleNames.Host.Admin) { IsStatic = true, IsDefault = true, Id = Guid.Parse("00000000-0000-0000-0000-000000000003") }).Entity;
            _context.SaveChanges();
        }

        // Grant all permissions to admin role for host

        var grantedPermissions = _context.Permissions.IgnoreQueryFilters()
            .OfType<RolePermissionSetting>()
            .Where(p => p.TenantId == null && p.RoleId == adminRoleForHost.Id)
            .Select(p => p.Name)
            .ToList();

        var permissions = PermissionFinder
            .GetAllPermissions(new DCloudAuthorizationProvider(DCloudConsts.MultiTenancyEnabled))
            .Where(p => p.MultiTenancySides.HasFlag(MultiTenancySides.Host) &&
                        !grantedPermissions.Contains(p.Name))
            .ToList();

        if (permissions.Any())
        {
            _context.Permissions.AddRange(
                permissions.Select(permission => new RolePermissionSetting
                {
                    TenantId = null,
                    Name = permission.Name,
                    IsGranted = true,
                    RoleId = adminRoleForHost.Id
                })
            );
            _context.SaveChanges();
        }


        //User role  for host

        var userRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == null && r.Name == StaticRoleNames.Tenants.User);
        if (userRole == null)
        {
            _context.Roles.Add(new Role(null, StaticRoleNames.Tenants.User, StaticRoleNames.Tenants.User) { IsStatic = true, IsDefault = true, Id = Guid.Parse("00000000-0000-0000-0000-000000000004") });
            _context.SaveChanges();
        }

        //grantedPermissions = _context.Permissions.IgnoreQueryFilters()
        // .OfType<RolePermissionSetting>()
        // .Where(p => p.TenantId == null && p.RoleId == userRole.Id)
        // .Select(p => p.Name)
        // .ToList();

        //permissions = PermissionFinder
        //   .GetAllPermissions(new DCloudAuthorizationProvider(DCloudConsts.MultiTenancyEnabled))
        //   .Where(p => p.MultiTenancySides.HasFlag(MultiTenancySides.Host) &&
        //               !grantedPermissions.Contains(p.Name))
        //   .ToList();

        //if (permissions.Any())
        //{
        //    _context.Permissions.AddRange(
        //        permissions.Select(permission => new RolePermissionSetting
        //        {
        //            TenantId = null,
        //            Name = permission.Name,
        //            IsGranted = true,
        //            RoleId = userRole.Id
        //        })
        //    );
        //    _context.SaveChanges();
        //}






        // Admin user for host

        var adminUserForHost = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == null && u.UserName == ADTOSharpUserBase.AdminUserName);
        if (adminUserForHost == null)
        {
            var user = new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                TenantId = null,
                UserName = ADTOSharpUserBase.AdminUserName,
                Name = "admin",
                EmailAddress = "admin@adtogroup.com",
                IsEmailConfirmed = true,
                IsActive = true
            };

            user.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(user, "123qwe");
            user.SetNormalizedNames();

            adminUserForHost = _context.Users.Add(user).Entity;
            _context.SaveChanges();

            // Assign Admin role to admin user
            _context.UserRoles.Add(new UserRole(null, adminUserForHost.Id, adminRoleForHost.Id) { Id = Guid.Parse("00000000-0000-0000-0000-000000000002") });
            _context.SaveChanges();

            _context.SaveChanges();
        }


        var testUserForHost = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == null && u.UserName == "test");
        if (testUserForHost == null)
        {
            var user = new User
            {
                TenantId = null,
                UserName = "test",
                Name = "测试用户",
                EmailAddress = "test@adtogroup.com",
                IsEmailConfirmed = true,
                IsActive = true
            };

            user.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(user, "123456");
            user.SetNormalizedNames();

            testUserForHost = _context.Users.Add(user).Entity;
            _context.SaveChanges();

            // Assign Admin role to admin user
            _context.UserRoles.Add(new UserRole(null, testUserForHost.Id, adminRoleForHost.Id));
            _context.SaveChanges();

            _context.SaveChanges();
        }
    }
}

