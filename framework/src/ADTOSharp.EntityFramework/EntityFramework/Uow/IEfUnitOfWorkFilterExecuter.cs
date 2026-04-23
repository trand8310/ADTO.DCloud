using System.Data.Entity;
using ADTOSharp.Domain.Uow;

namespace ADTOSharp.EntityFramework.Uow
{
    public interface IEfUnitOfWorkFilterExecuter : IUnitOfWorkFilterExecuter
    {
        void ApplyCurrentFilters(IUnitOfWork unitOfWork, DbContext dbContext);
    }
}