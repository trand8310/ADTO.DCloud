using ADTOSharp.Domain.Uow;
using ADTOSharp.EntityFrameworkCore;
using ADTO.DCloud.EntityFrameworkCore;
using ADTO.OpenIddict.EntityFrameworkCore.Authorizations;

namespace ADTO.DCloud.OpenIddict.Authorizations
{
    public class OpenIddictAuthorizationRepository : EfCoreOpenIddictAuthorizationRepository<DCloudDbContext>
    {
        public OpenIddictAuthorizationRepository(
            IDbContextProvider<DCloudDbContext> dbContextProvider,
            IUnitOfWorkManager unitOfWorkManager) : base(dbContextProvider, unitOfWorkManager)
        {
        }
    }
}
