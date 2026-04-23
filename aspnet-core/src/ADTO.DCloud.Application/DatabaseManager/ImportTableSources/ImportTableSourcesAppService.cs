using System;
using System.Linq;
using ADTOSharp.UI;
using System.Threading.Tasks;
using ADTOSharp.Linq.Extensions;
using System.Collections.Generic;
using ADTOSharp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using ADTO.DCloud.DatabaseManager.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTO.DCloud.DatabaseManager.ImportTableSources.Dto;

namespace ADTO.DCloud.DatabaseManager.ImportTableSources
{
    /// <summary>
    /// 数据表管理（导入表源表）
    /// </summary>
    public class ImportTableSourcesAppService : DCloudAppServiceBase, IImportTableSourcesAppService
    {
        private readonly IRepository<ImportTableSource, Guid> _connectionsRepository;
        public ImportTableSourcesAppService(IRepository<ImportTableSource, Guid> connectionsRepository)
        {
            _connectionsRepository = connectionsRepository;
        }

        /// <summary>
        /// 添加数据表管理信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreatImportTableSourcesAsync(CreateImportTableSourcesDto input)
        {
            var info = ObjectMapper.Map<ImportTableSource>(input);

            await _connectionsRepository.InsertAsync(info);
        }

        /// <summary>
        /// 修改数据表管理资料
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateImportTableSourcesAsync(CreateImportTableSourcesDto input)
        {
            var info = this._connectionsRepository.Get(input.Id.Value);
            ObjectMapper.Map(input, info);

            await _connectionsRepository.UpdateAsync(info);
        }

        /// <summary>
        /// 删除指定的数据表管理资料
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteImportTableSourcesAsync(EntityDto<Guid> input)
        {
            var info = await this._connectionsRepository.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            if (info == null)
            {
                throw new UserFriendlyException("操作失败，记录不存在！");
            }
            await _connectionsRepository.DeleteAsync(input.Id);
        }

        /// <summary>
        /// 获取数据表管理分页列表
        /// 导入表的列表数据，来源这个接口（以前是获取数据库所有表）
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns></returns>
        public async Task<PagedResultDto<ImportTableSourcesDto>> GetImportTableSourcesPageList(PagedDataConnectionResultRequestDto input)
        {
            var query = _connectionsRepository.GetAll()
                 .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), p => p.Name.Contains(input.Keyword) || p.Desc.Contains(input.Keyword));

            var totalCount = await query.CountAsync();
            var items = await query.PageBy(input).ToListAsync(); ;
         
            return new PagedResultDto<ImportTableSourcesDto>(totalCount, ObjectMapper.Map<List<ImportTableSourcesDto>>(items));
        }

        /// <summary>
        /// 获取指定数据表管理详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ImportTableSourcesDto> GetImportTableSourcesByIdAsync(EntityDto<Guid> input)
        {
            var info = await _connectionsRepository.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            var infoDto = ObjectMapper.Map<ImportTableSourcesDto>(info);

            return infoDto;
        }
    }
}

