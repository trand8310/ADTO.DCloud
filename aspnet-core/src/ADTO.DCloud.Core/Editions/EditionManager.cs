using ADTOSharp.Application.Editions;
using ADTOSharp.Application.Features;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADTO.DCloud.Editions;

public class EditionManager : ADTOSharpEditionManager
{
    public const string DefaultEditionName = "Standard";

    public EditionManager(
        IRepository<Edition, Guid> editionRepository,
        IADTOSharpZeroFeatureValueStore featureValueStore,
        IUnitOfWorkManager unitOfWorkManager) 
        : base(editionRepository, featureValueStore, unitOfWorkManager)
    {
    }

    public async Task<List<Edition>> GetAllAsync()
    {
        return await EditionRepository.GetAllListAsync();
    }
}
