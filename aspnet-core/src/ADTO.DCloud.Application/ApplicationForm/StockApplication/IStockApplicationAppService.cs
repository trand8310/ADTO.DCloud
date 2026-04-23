using ADTO.DCloud.ApplicationForm.Stocks.Dto;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.WorkFlow.NodeMethod;
using ADTOSharp.Application.Services;
using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm.Stocks
{
    /// <summary>
    /// 库存管理
    /// </summary>
    public interface IStockApplicationAppService : IApplicationService
    {
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task Execute(Guid id);
        /// <summary>
        /// 新增流程表单
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>

        public  Task ExecuteInsert(string data);

        /// <summary>
        /// 修改流程表单
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public  Task ExecuteUpdate(string data);
        /// <summary>
        /// 删除表单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public  Task ExecuteDelete(Guid id);
    }
}
