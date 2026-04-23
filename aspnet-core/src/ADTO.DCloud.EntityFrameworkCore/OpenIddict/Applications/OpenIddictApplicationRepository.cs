using ADTOSharp.Domain.Uow;
using ADTOSharp.EntityFrameworkCore;
using ADTO.DCloud.EntityFrameworkCore;
using ADTO.OpenIddict.EntityFrameworkCore.Applications;

namespace ADTO.DCloud.OpenIddict.Applications
{
    public class OpenIddictApplicationRepository : EfCoreOpenIddictApplicationRepository<DCloudDbContext>
    {
        public OpenIddictApplicationRepository(
            IDbContextProvider<DCloudDbContext> dbContextProvider,
            IUnitOfWorkManager unitOfWorkManager) : base(dbContextProvider, unitOfWorkManager)
        {
        }
    }
}
