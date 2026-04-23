using ADTO.DCloud.Customers.CustomerLogManage.Dto;
using System.Threading.Tasks;

namespace ADTO.DCloud.Customers.CustomerLogManage
{
    public interface ICustomerLogAppService
    {
        /// <summary>
        /// 添加客户日志信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task CreateCustomerLogAsync(CreateCustomerLogDto input);
    }
}
