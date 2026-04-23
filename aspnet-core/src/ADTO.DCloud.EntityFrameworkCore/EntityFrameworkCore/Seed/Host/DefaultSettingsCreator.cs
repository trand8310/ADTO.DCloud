using ADTOSharp.Configuration;
using ADTOSharp.Localization;
using ADTOSharp.MultiTenancy;
using ADTOSharp.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace ADTO.DCloud.EntityFrameworkCore.Seed.Host;

public class DefaultSettingsCreator
{
    private readonly DCloudDbContext _context;

    public DefaultSettingsCreator(DCloudDbContext context)
    {
        _context = context;
    }

    public void Create()
    {
        Guid? tenantId = null;


        if (DCloudConsts.MultiTenancyEnabled == false)
        {
#pragma warning disable CS0162
            tenantId = MultiTenancyConsts.DefaultTenantId;
#pragma warning restore CS0162
        }

        // Emailing
        AddSettingIfNotExists(EmailSettingNames.DefaultFromAddress, "admin@adtogroup.com", tenantId);
        AddSettingIfNotExists(EmailSettingNames.DefaultFromDisplayName, "webmaster", tenantId);

        // Languages
        //AddSettingIfNotExists(LocalizationSettingNames.DefaultLanguage, "zh-Hans", tenantId);
    }

    private void AddSettingIfNotExists(string name, string value, Guid? tenantId = null)
    {
        if (_context.Settings.IgnoreQueryFilters().Any(s => s.Name == name && s.TenantId == tenantId && s.UserId == null))
        {
            return;
        }

        _context.Settings.Add(new Setting(tenantId, null, name, value));
        _context.SaveChanges();
    }
}

