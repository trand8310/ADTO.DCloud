using ADTO.DCloud.ProjectManage.Dto;
using System.Threading.Tasks;
using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.ProjectManage
{
    /// <summary>
    /// 项目日志相关操作
    /// </summary>
    public interface IProjectLogAppService
    {
        /// <summary>
        /// 添加项目日志信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task CreateProjectLogAsync(CreateProjectLogDto input);

        /// <summary>
        /// 项目日志分页列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<ProjectLogDto>> GetProjectLogPageList(PagedProjectLogResultRequestDto input);
    }
}
