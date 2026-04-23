using ADTOSharp.Domain.Uow;
using ADTOSharp.EntityFramework;

namespace ADTOSharp.EntityFrameworkCore;

public class DbContextTypeMatcher : DbContextTypeMatcher<ADTOSharpDbContext>
{
    public DbContextTypeMatcher(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        : base(currentUnitOfWorkProvider)
    {
    }
}