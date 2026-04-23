using ADTOSharp.Domain.Uow;
using ADTOSharp.EntityFrameworkCore;
using ADTO.DCloud.EntityFrameworkCore;
using ADTO.OpenIddict.EntityFrameworkCore.Scopes;


namespace ADTO.DCloud.OpenIddict.Scopes
{
    public class OpenIddictScopeRepository : EfCoreOpenIddictScopeRepository<DCloudDbContext>
    {
        public OpenIddictScopeRepository(
            IDbContextProvider<DCloudDbContext> dbContextProvider,
            IUnitOfWorkManager unitOfWorkManager) : base(dbContextProvider, unitOfWorkManager)
        {
        }
    }
}
