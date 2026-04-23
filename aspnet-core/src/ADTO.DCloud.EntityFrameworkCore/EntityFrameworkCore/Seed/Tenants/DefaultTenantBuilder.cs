using ADTOSharp.MultiTenancy;
using ADTO.DCloud.Editions;
using ADTO.DCloud.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace ADTO.DCloud.EntityFrameworkCore.Seed.Tenants;

public class DefaultTenantBuilder
{
    private readonly DCloudDbContext _context;

    public DefaultTenantBuilder(DCloudDbContext context)
    {
        _context = context;
    }

    public void Create()
    {
        CreateDefaultTenant();
    }

    private void CreateDefaultTenant()
    {
        // Default tenant

        var defaultTenant = _context.Tenants.IgnoreQueryFilters().FirstOrDefault(t => t.TenancyName == ADTOSharpTenantBase.DefaultTenantName);
        if (defaultTenant == null)
        {
            defaultTenant = new Tenant(ADTOSharpTenantBase.DefaultTenantName, ADTOSharpTenantBase.DefaultTenantName) { Id = Guid.Parse("00000000-0000-0000-0000-000000000001") };

            var defaultEdition = _context.Editions.IgnoreQueryFilters().FirstOrDefault(e => e.Name == EditionManager.DefaultEditionName);
            if (defaultEdition != null)
            {
                defaultTenant.EditionId = defaultEdition.Id;
            }

            _context.Tenants.Add(defaultTenant);
            _context.SaveChanges();
        }
    }
}

