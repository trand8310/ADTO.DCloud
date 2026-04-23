using ADTO.DCloud.Dto;
using ADTO.DCloud.Training.Dto;
using ADTOSharp.Application.Services;
using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADTO.DCloud.Training
{
    /// <summary>
    ///培训库分类
    /// </summary>
    public interface ITrainingDocCategoryAppService : IApplicationService
    {
        /// <summary>
        /// 获取培训库类别
        /// </summary>
        /// <param name="input">查询实体</param>
        /// <returns></returns>
        Task<PagedResultDto<TrainingDocCategoryDto>> GetDocCategoryPagedList(PagedTrainingDocCategoryRequestDto input);

        /// <summary>
        /// 新增培训库类别
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<JsonResultModel> CreateInfo(TrainingDocCategoryDto input);

        /// <summary>
        /// 修改培训库类别
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        Task<JsonResultModel> UpdateInfo(TrainingDocCategoryDto input);

        /// <summary>
        /// 删除培训库类别
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<JsonResultModel> DeleteInfo(Guid Id);

        /// <summary>
        /// 得到所有培训库类别
        /// </summary>
        /// <returns></returns>
        Task<List<TrainingDocCategoryDto>> GetList();

    }
}
