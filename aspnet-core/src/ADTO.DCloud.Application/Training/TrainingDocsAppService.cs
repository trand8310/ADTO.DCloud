using ADTO.DCloud.Dto;
using ADTO.DCloud.EmployeeManager;
using ADTO.DCloud.Storage.Dto;
using ADTO.DCloud.Training.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADTO.DCloud.Training
{
    /// <summary>
    /// 培训库管理
    /// </summary>
    //[ADTOSharpAuthorize(PermissionNames.Pages_TrainingLibrary)]
    public class TrainingDocsAppService : DCloudAppServiceBase, ITrainingDocsAppService
    {
        private readonly IRepository<TrainingDoc, Guid> _trainingDocRepository;
        private readonly IRepository<EmployeeInfo, Guid> _employeeInfoRepository;
        public TrainingDocsAppService(
           IRepository<TrainingDoc, Guid> trainingDocRepository,
           IRepository<EmployeeInfo, Guid> employeeInfoRepository
         )
        {
            _trainingDocRepository = trainingDocRepository;
            _employeeInfoRepository = employeeInfoRepository;

        }


        /// <summary>
        /// 获取培训库列表
        /// </summary>
        /// <param name="input">查询实体</param>
        /// <returns></returns>
        public async Task<PagedResultDto<TrainingDocDto>> GetTrainingDocPagedList(PagedTrainingDocRequestDto input)
        {
            //当前登录用户
            if (ADTOSharpSession.UserId == null)
            {
                return new PagedResultDto<TrainingDocDto>();
            }

            var userId = ADTOSharpSession.UserId.Value;
            var query = _trainingDocRepository.GetAllIncluding(p => p.Category)
                .WhereIf(input.CategoryId.HasValue, p => p.Category.Id == input.CategoryId)
                .WhereIf(!string.IsNullOrEmpty(input.Keyword), p => p.Title.Contains(input.Keyword))
                .Select(p => new TrainingDocDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Title,
                    Content = p.Content,
                    CreationTime = p.CreationTime,
                    CreatorUserId = p.CreatorUserId,
                });

            //获取总数
            var resultCount = await query.CountAsync();
            var taskList = query.OrderByDescending(o => o.CreationTime).PageBy(input).ToList();
            var ResultList = ObjectMapper.Map<List<TrainingDocDto>>(taskList);
            foreach (var item in ResultList)
            {
                item.CreateUserName = (await this._employeeInfoRepository.FirstOrDefaultAsync(p => p.Id == item.CreatorUserId))?.Name;
            }
            return new PagedResultDto<TrainingDocDto>(resultCount, ResultList);
        }

        /// <summary>
        /// 新增培训库
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[ADTOSharpAuthorize("Pages.Training.Library.Create")]
        public async Task<JsonResultModel> CreateInfo(TrainingDocDto input)
        {
            TrainingDoc model = ObjectMapper.Map<TrainingDoc>(input);

            var id = await _trainingDocRepository.InsertAndGetIdAsync(model);
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                return new JsonResultModel() { Message = $"操作失败", Success = false };
            }
            return new JsonResultModel() { Message = $"操作成功", Success = true };
        }

        /// <summary>
        /// 修改培训库
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        //[ADTOSharpAuthorize("Pages.Training.Library.Create")]
        public async Task<JsonResultModel> UpdateInfo(TrainingDocDto input)
        {
            var entity = _trainingDocRepository.Get(input.Id);

            if (string.IsNullOrEmpty(entity.Id.ToString()))
            {
                return new JsonResultModel() { Message = $"记录不存在", Success = false };
            }
            entity = ObjectMapper.Map<TrainingDoc>(input);

            await _trainingDocRepository.UpdateAsync(entity);

            return new JsonResultModel() { Message = $"操作成功", Success = true };
        }

        /// <summary>
        /// 删除培训库
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        //[ADTOSharpAuthorize("Pages.Training.Library.Create")]
        public async Task<JsonResultModel> DeleteInfo(EntityDto<Guid> input)
        {
            var entity = await _trainingDocRepository.GetAsync(input.Id);

            if (entity == null)
            {
                return new JsonResultModel() { Message = $"记录不存在", Success = false };
            }
            await _trainingDocRepository.DeleteAsync(entity);
            return new JsonResultModel() { Message = $"操作成功", Success = true };
        }
    }
}
