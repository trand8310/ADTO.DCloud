using System;
using System.Threading.Tasks;
using ADTO.DCloud.FormScheme.Dto;
using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.FormScheme
{
    /// <summary>
    /// 表单模板相关方法
    /// </summary>
    public interface IFormSchemeAppService
    {
        /// <summary>
        /// 获取动态表单详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<FormSchemeOutputDto> GetFormSchemInfo(EntityDto<Guid> input);

        /// <summary>
        /// 修改表单设计
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task UpdateFormScheme(CreateFormSchemeInputDto input);

        /// <summary>
        /// 新增表单设计(自动建表类型去除)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<FormSchemeInfoDto> CreateFormScheme(CreateFormSchemeInputDto input);

        /// <summary>
        /// 获取动态表单模板分页列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns></returns>

        Task<PagedResultDto<FormSchemeListShowDto>> GetFormSchemePageList(PagedFormSchemeResultRequestDto input);
    }
}
