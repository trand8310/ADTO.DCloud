using ADTOSharp.Localization;
using ADTOSharp.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ADTO.DCloud.EntityFrameworkCore.Seed.Host;

public class DefaultLanguagesCreator
{
    public static List<ApplicationLanguage> InitialLanguages => GetInitialLanguages();

    private readonly DCloudDbContext _context;

    private static List<ApplicationLanguage> GetInitialLanguages()
    {
        var tenantId = DCloudConsts.MultiTenancyEnabled ? null : (Guid?)MultiTenancyConsts.DefaultTenantId;
        return new List<ApplicationLanguage>
        {
            new ApplicationLanguage(tenantId, "en", "English", "famfamfam-flags us"){ Id=Guid.Parse("00000000-0000-0000-0000-000000000001")},
            new ApplicationLanguage(tenantId, "zh-Hans", "简体中文", "famfamfam-flags cn"){ Id=Guid.Parse("00000000-0000-0000-0000-000000000002")},
        };
    }

    public DefaultLanguagesCreator(DCloudDbContext context)
    {
        _context = context;
    }

    public void Create()
    {
        CreateLanguages();
    }

    private void CreateLanguages()
    {
        foreach (var language in InitialLanguages)
        {
            AddLanguageIfNotExists(language);
        }
    }

    private void AddLanguageIfNotExists(ApplicationLanguage language)
    {
        if (_context.Languages.IgnoreQueryFilters().Any(l => l.TenantId == language.TenantId && l.Name == language.Name))
        {
            return;
        }

        _context.Languages.Add(language);
        _context.SaveChanges();
    }
}

