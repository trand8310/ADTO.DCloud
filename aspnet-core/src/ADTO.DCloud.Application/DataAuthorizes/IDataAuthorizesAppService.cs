using ADTO.DCloud.DataAuthorizes.Dto;
using ADTOSharp.Application.Services;
using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataAuthorizes
{
    public interface IDataAuthorizesAppService : IApplicationService
    {
        /// <summary>
        /// 查询数据权限列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<PagedResultDto<DataAuthorizeDto>> GetAllPageListAsync(GeDataAuthorizePagedInput input);
        /// <summary>
        /// 创建数据查询,返回带字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        Task<IQueryable<T>> CreateDataFilteredQuery<T>(IQueryable<T> query, string methodName);
    }
}
