using ADTOSharp.Domain.Uow;
using ADTOSharp.EntityFrameworkCore;
using ADTO.DCloud.EntityFrameworkCore;
using ADTO.OpenIddict.EntityFrameworkCore.Tokens;

namespace ADTO.DCloud.OpenIddict.Tokens
{
    public class OpenIddictTokenRepository : EfCoreOpenIddictTokenRepository<DCloudDbContext>
    {
        public OpenIddictTokenRepository(
            IDbContextProvider<DCloudDbContext> dbContextProvider,
            IUnitOfWorkManager unitOfWorkManager) : base(dbContextProvider, unitOfWorkManager)
        {
        }
    }
}
