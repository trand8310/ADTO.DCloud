using ADTO.DCloud.Editions;
using ADTO.DCloud.Features;
using ADTOSharp.Application.Editions;
using ADTOSharp.Application.Features;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using System;
using System.Linq;

namespace ADTO.DCloud.EntityFrameworkCore.Seed.Host;

public class DefaultEditionCreator
{
    private readonly DCloudDbContext _context;

    public DefaultEditionCreator(DCloudDbContext context)
    {
        _context = context;
    }

    public void Create()
    {
        CreateEditions();
    }

    private void CreateEditions()
    {
        //var defaultEdition = _context.Editions.IgnoreQueryFilters().FirstOrDefault(e => e.Name == EditionManager.DefaultEditionName);
        //if (defaultEdition == null)
        //{
        //    defaultEdition = new Edition { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = EditionManager.DefaultEditionName, DisplayName = EditionManager.DefaultEditionName };
        //    _context.Editions.Add(defaultEdition);
        //    _context.SaveChanges();
        //    /* Add desired features to the standard edition, if wanted... */
        //}

        var defaultEdition = _context.Editions.IgnoreQueryFilters().FirstOrDefault(e => e.Name == EditionManager.DefaultEditionName);
        if (defaultEdition == null)
        {
            defaultEdition = new SubscribableEdition { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = EditionManager.DefaultEditionName, DisplayName = EditionManager.DefaultEditionName };
            _context.Editions.Add(defaultEdition);
            _context.SaveChanges();
            CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.ChatFeature, true);
            CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.TenantToTenantChatFeature, true);
            CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.TenantToHostChatFeature, true);
        }
    }


    private void CreateFeatureIfNotExists(Guid editionId, string featureName, bool isEnabled)
    {
        var defaultEditionChatFeature = _context.EditionFeatureSettings.IgnoreQueryFilters()
                                                    .FirstOrDefault(ef => ef.EditionId == editionId && ef.Name == featureName);

        if (defaultEditionChatFeature == null)
        {
            _context.EditionFeatureSettings.Add(new EditionFeatureSetting
            {
                Name = featureName,
                Value = isEnabled.ToString().ToLower(),
                EditionId = editionId
            });
        }
    }
}

