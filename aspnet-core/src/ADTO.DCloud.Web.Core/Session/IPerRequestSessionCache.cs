using System.Threading.Tasks;
using ADTO.DCloud.Sessions.Dto;
 

namespace ADTO.DCloud.Web.Session;

public interface IPerRequestSessionCache
{
    Task<GetCurrentSessionOutput> GetCurrentSessionAsync();
}
