using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADTO.DCloud.DataSource.Dto;
using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.DataSource
{
    /// <summary>
    /// 数据源相关方法
    /// </summary>
    public interface IDataSourceAppService
    {
        /// <summary>
        /// 添加数据源\数据视图
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task CreateDataSourceAsync(CreateDataSourceInputDto input);

        /// <summary>
        /// 修改数据源\数据视图
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task UpdateDataSourceAsync(CreateDataSourceInputDto input);

        /// <summary>
        /// 获取指定数据源\数据视图
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<DataSourceDto> GetDataSourceByIdAsync(EntityDto<Guid> input);

        /// <summary>
        /// 删除指定的数据源\数据视图
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task DeleteDataSourceAsync(EntityDto<Guid> input);

        /// <summary>
        /// 获取数据源\数据视图 分页列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns></returns>
        Task<PagedResultDto<DataSourceDto>> GetDataSourcePageList(PagedDataSourceResultRequestDto input);

        /// <summary>
        /// 根据编码获取实体
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns></returns>
        Task<DataSourceDto> GetDataSourceByCodeAsync(DataSourceQueryDto input);

        /// <summary>
        /// 获取数据源列名
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> GetDataColName(string sql);

    }
}
