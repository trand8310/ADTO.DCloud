using ADTO.DCloud.DatabaseManager.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Runtime.Security;
using ADTOSharp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ADTO.DCloud.DatabaseManager
{
    /// <summary>
    /// 数据库链接管理服务
    /// </summary>
    public class DataConnectionAppService : DCloudAppServiceBase, IDataConnectionAppService
    {
        private readonly IRepository<DataConnections, Guid> _connectionsRepository;
        public DataConnectionAppService(IRepository<DataConnections, Guid> connectionsRepository)
        {
            _connectionsRepository = connectionsRepository;
        }

        /// <summary>
        /// 添加数据库链接信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreatDataConnectionAsync(CreateDataConnectionDto input)
        {
            var info = ObjectMapper.Map<DataConnections>(input);
            if (!string.IsNullOrWhiteSpace(info.Password))
            {
                //加密保存
                info.Password = SimpleStringCipher.Instance.Encrypt(info.Password);
            }
            await _connectionsRepository.InsertAsync(info);
        }

        /// <summary>
        /// 修改数据库链接资料
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// 
        public async Task UpdateDataConnectionAsync(CreateDataConnectionDto input)
        {
            var info = this._connectionsRepository.Get(input.Id.Value);
            ObjectMapper.Map(input, info);

            await _connectionsRepository.UpdateAsync(info);

        }

        /// <summary>
        /// 删除指定的数据库链接资料
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async Task DeleteDataConnectionAsync(EntityDto<Guid> input)
        {
            var info = await this._connectionsRepository.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            if (info == null)
            {
                throw new UserFriendlyException("操作失败，记录不存在！");
            }

            await _connectionsRepository.DeleteAsync(input.Id);
        }

        /// <summary>
        /// 获取数据库链接分页列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns></returns>

        public async Task<PagedResultDto<DataConnectionDto>> GetDataConnectionPageList(PagedDataConnectionResultRequestDto input)
        {
            var query = _connectionsRepository.GetAll()
                 .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), p => p.Name.Contains(input.Keyword));

            var totalCount = await query.CountAsync();
            var items = await query.PageBy(input).ToListAsync(); ;
            //var list = items.Select(item =>
            //{
            //    var dto = ObjectMapper.Map<DataConnectionDto>(item);
            //    return dto;
            //}).ToList();
            return new PagedResultDto<DataConnectionDto>(totalCount, ObjectMapper.Map<List<DataConnectionDto>>(items));
        }

        /// <summary>
        /// 获取指定数据库链接详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<DataConnectionDto> GetDataConnectionByIdAsync(EntityDto<Guid> input)
        {
            var info = await _connectionsRepository.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            var infoDto = ObjectMapper.Map<DataConnectionDto>(info);

            return infoDto;
        }
    }
}
