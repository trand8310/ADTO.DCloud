using ADTO.DCloud.Training.Dto;
using ADTOSharp.Application.Services;
using ADTOSharp.Application.Services.Dto;
using System.Threading.Tasks;

namespace ADTO.DCloud.Training
{
    /// <summary>
    /// 培训记录统计
    /// </summary>
    public interface ITrainingArchivesCountAppService : IApplicationService
    {
        /// 员工培训记录统计分页列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<TrainingArchivesCountDto>> GetTrainingArchivesCountPagedList(PagedTrainingArchivesCountRequestDto input);
    }
}
