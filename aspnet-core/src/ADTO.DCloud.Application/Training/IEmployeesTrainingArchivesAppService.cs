using ADTO.DCloud.Dto;
using ADTO.DCloud.Training.Dto;
using ADTOSharp.Application.Services;
using ADTOSharp.Application.Services.Dto;
using System.Threading.Tasks;

namespace ADTO.DCloud.Training
{
    /// <summary>
    /// 员工培训记录
    /// </summary>
    public interface IEmployeesTrainingArchivesAppService : IApplicationService
    {
        /// <summary>
        /// 批量导入员工培训记录
        /// </summary>
        /// <returns></returns>
        Task<JsonResultModel> BulkImport();

        /// <summary>
        /// 获取员工培训记录
        /// </summary>
        /// <param name="input">查询实体</param>
        /// <returns></returns>
        Task<PagedResultDto<EmployeesTrainingArchivesDto>> GetTrainingArchivesPagedList(PagedEmployeesTrainingArchivesRequestDto input);

        /// <summary>
        /// 新增员工培训记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<JsonResultModel> CreateInfo(CreateEmployeesTrainingArchivesDto input);

        /// <summary>
        /// 修改员工培训记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<JsonResultModel> UpdateInfo(EmployeesTrainingArchivesDto input);

    }
}
