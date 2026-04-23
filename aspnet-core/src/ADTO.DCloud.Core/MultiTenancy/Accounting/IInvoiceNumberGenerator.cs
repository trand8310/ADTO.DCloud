using System.Threading.Tasks;
using ADTOSharp.Dependency;

namespace ADTO.DCloud.MultiTenancy.Accounting
{
    public interface IInvoiceNumberGenerator : ITransientDependency
    {
        Task<string> GetNewInvoiceNumber();
    }
}