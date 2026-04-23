using ADTOSharp.Domain.Uow;

namespace ADTOSharp.EntityFramework
{
    public class ADTOSharpEfDbContextInitializationContext
    {
        public IUnitOfWork UnitOfWork { get; }

        public ADTOSharpEfDbContextInitializationContext(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
    }
}
