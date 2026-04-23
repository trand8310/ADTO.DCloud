using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Training.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Runtime.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using ADTOSharp.Linq.Extensions;
using ADTO.DCloud.Dto;


namespace ADTO.DCloud.Training
{
    /// <summary>
    /// 培训库类别
    /// </summary>
    //[ADTOSharpAuthorize(PermissionNames.Pages_TrainingDocuments)]
    public class TrainingDocCategoryAppService : DCloudAppServiceBase, ITrainingDocCategoryAppService
    {
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IRepository<TrainingDocCategory, Guid> _trainingDocCategoryRepository;
        private readonly IRepository<TrainingDoc, Guid> _trainingDocRepository;
        private readonly IADTOSharpSession _session;
        public TrainingDocCategoryAppService(
           IRepository<TrainingDocCategory, Guid> repository,
           IRepository<User, Guid> userRepository,
           IRepository<TrainingDoc, Guid> trainingDocRepository,
           IADTOSharpSession session
         )
        {
            _userRepository = userRepository;
            _session = session;
            _trainingDocRepository = trainingDocRepository;
        }

        /// <summary>
        /// 得到所有培训库类别
        /// </summary>
        /// <returns></returns>
        [ADTOSharpAllowAnonymous]
        public async Task<List<TrainingDocCategoryDto>> GetList()
        {
            var list = await _trainingDocCategoryRepository.GetAllListAsync();
            return ObjectMapper.Map<List<TrainingDocCategoryDto>>(list);
        }

        /// <summary>
        /// 获取培训库类别
        /// </summary>
        /// <param name="input">查询实体</param>
        /// <returns></returns>
        public async Task<PagedResultDto<TrainingDocCategoryDto>> GetDocCategoryPagedList(PagedTrainingDocCategoryRequestDto input)
        {
            //当前登录用户
            if (_session.UserId == null)
            {
                return new PagedResultDto<TrainingDocCategoryDto>();
            }
            var userId = ADTOSharpSession.GetUserId();
            var query = _trainingDocCategoryRepository.GetAll().WhereIf(!string.IsNullOrEmpty(input.Keyword), p => p.Title.Contains(input.Keyword));

            //获取总数
            var resultCount = await query.CountAsync();
            var taskList = query.OrderBy(o => o.Sord).PageBy(input).ToList();
            var ResultList = ObjectMapper.Map<List<TrainingDocCategoryDto>>(taskList);
            foreach (var item in ResultList)
            {
                item.CreateUserName = this._userRepository.Get(item.CreatorUserId.Value)?.Name;
            }
            return new PagedResultDto<TrainingDocCategoryDto>(resultCount, ResultList);
        }

        /// <summary>
        /// 新增培训库类别
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<JsonResultModel> CreateInfo(TrainingDocCategoryDto input)
        {
            TrainingDocCategory model = ObjectMapper.Map<TrainingDocCategory>(input);

            model.Id = await _trainingDocCategoryRepository.InsertAndGetIdAsync(model);
            if (string.IsNullOrWhiteSpace(model.Id.ToString()))
            {
                return new JsonResultModel() { Message = $"操作失败", Success = false };
            }
            return new JsonResultModel() { Message = $"操作成功", Success = true };
        }

        /// <summary>
        /// 修改培训库类别
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResultModel> UpdateInfo(TrainingDocCategoryDto input)
        {
            var entity = _trainingDocCategoryRepository.Get(input.Id);

            if (string.IsNullOrEmpty(entity.Id.ToString()))
            {
                return new JsonResultModel() { Message = $"记录不存在", Success = false };
            }

            entity = ObjectMapper.Map<TrainingDocCategory>(input);
            await _trainingDocCategoryRepository.UpdateAsync(entity);
            return new JsonResultModel() { Message = $"操作成功", Success = true };
        }

        /// <summary>
        /// 删除培训库类别
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResultModel> DeleteInfo(Guid Id)
        {
            var vInfo = _trainingDocCategoryRepository.Get(Id);

            if (string.IsNullOrEmpty(vInfo.Id.ToString()))
            {
                return new JsonResultModel() { Message = $"记录不存在", Success = false };
            }

            var docList = this._trainingDocRepository.GetAllIncluding(p => p.Category).Where(p => p.Category.Id == Id).ToList();
            if (docList.Count > 0)
            {
                return new JsonResultModel() { Message = $"不允许删除，存在类别关联的培训库数据", Success = false };
            }
            await _trainingDocCategoryRepository.DeleteAsync(vInfo.Id);
            return new JsonResultModel() { Message = $"操作成功", Success = true };
        }
    }
}
