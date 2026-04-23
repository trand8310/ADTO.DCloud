using ADTOSharp.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.NodeMethod
{
    /// <summary>
    /// 流程绑定方法需要继承的接口
    /// </summary>
    public interface IWorkFlowMethod : IApplicationService
    {
        /// <summary>
        /// 流程绑定方法需要继承的接口
        /// </summary>
        /// <param name="parameter"></param>
        Task Execute(WfMethodParameter parameter);


        /// <summary>
        /// 表单-新增
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task ExecuteInsert(string data);
        /// <summary>
        /// 表单-修改
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task ExecuteUpdate(string data);

        /// <summary>
        /// 表单-删除
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task ExecuteDelete(Guid id);
    }
}
