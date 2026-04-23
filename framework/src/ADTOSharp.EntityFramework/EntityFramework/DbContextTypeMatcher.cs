using ADTOSharp.Domain.Uow;

namespace ADTOSharp.EntityFramework
{
    public class DbContextTypeMatcher : DbContextTypeMatcher<ADTOSharpDbContext>
    {
        public DbContextTypeMatcher(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider) 
            : base(currentUnitOfWorkProvider)
        {
        }
    }
}