using System;
using System.Data;
using System.Linq;
using ADTOSharp.UI;
using System.Threading.Tasks;
using System.Collections.Generic;
using ADTO.DCloud.Surveys.Surveys.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using ADTOSharp.EntityFrameworkCore.Repositories;

namespace ADTO.DCloud.Surveys.Surveys
{
    /// <summary>
    /// 考卷管理
    /// </summary>
    public class SurveyAppService : DCloudAppServiceBase, ISurveyAnswerAppService
    {
        #region Fields
        private readonly IRepository<Survey, Guid> _repository;
        private readonly IRepository<SurveyAnswer, Guid> _answerrepository;

        #endregion

        #region   ctor
        public SurveyAppService(IRepository<Survey, Guid> repository,
             IRepository<SurveyAnswer, Guid> answerrepository)
        {

            _repository = repository;
            _answerrepository = answerrepository;

        }
        #endregion

        /// <summary>
        /// 查询考卷分页列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<PagedResultDto<SurveyDto>> GetSurveyPageListAsync(PagedSurveyRequestDto input)
        {
            var query = _repository.GetAll()
                .WhereIf(!string.IsNullOrEmpty(input.keyword), q => q.Name.Contains(input.keyword));
            //获取总数
            var resultCount = await query.CountAsync();
            var list = query.OrderByDescending(o => o.CreationTime).PageBy(input).ToList();
            var ResultList = list.Select(item =>
            {
                var dto = ObjectMapper.Map<SurveyDto>(item);
                return dto;
            }).ToList();

            return new PagedResultDto<SurveyDto>(resultCount, ResultList);
        }

        /// <summary>
        /// 查询有效考卷(新增题库  所属考卷关联)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<SurveyDto>> GetEffectiveSurveyList()
        {
            var query = _repository.GetAll().Where(q => DateTime.Now >= q.StarDate && DateTime.Now <= q.EndDate);

            var ResultList = query.ToList().Select(item =>
            {
                var dto = ObjectMapper.Map<SurveyDto>(item);
                return dto;
            }).ToList();
            return ResultList;
        }

        /// <summary>
        /// 根据Id获取当前考卷详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [ADTOSharpAllowAnonymous]
        public async Task<SurveyDto> GetSurveyInfoAsync(EntityDto<Guid> input)
        {
            var entity = await _repository.GetAsync(input.Id);
            var dto = ObjectMapper.Map<SurveyDto>(entity);
            return dto;
        }

        /// <summary>
        /// 新增考卷
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<SurveyDto> CreateSurveyAsync(CreateSurveyDto input)
        {
            Survey entity = ObjectMapper.Map<Survey>(input);


            var id = await _repository.InsertAndGetIdAsync(entity);

            foreach (var userId in input.UserIdList)
            {
                SurveyAnswer p_entity = new SurveyAnswer() { SurveyId = id, UserId = userId };
                await _answerrepository.InsertAsync(p_entity);
            }

            return ObjectMapper.Map<SurveyDto>(entity);
        }

        /// <summary>
        /// 修改考卷
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<SurveyDto> UpdateSurveyAsync(UpdateSurveyDto input)
        {
            var entity = await _repository.GetAsync(input.Id);
            if (entity == null)
            {
                throw new UserFriendlyException($"考卷不存在！");
            }
            //修改考卷主记录
            ObjectMapper.Map(input, entity);
            entity.QuestionSource = input.QuestionSource.ToString();
            await _repository.UpdateAsync(entity);

            //删除不存在的用户
            await this._answerrepository.DeleteAsync(p => p.SurveyId == input.Id && !input.UserIdList.Contains(p.UserId));

            //新增
            foreach (var userId in input.UserIdList)
            {
                var exists = await _answerrepository.GetAll().Where(p => p.SurveyId == input.Id && p.UserId == userId).AnyAsync();
                if (!exists)
                {
                    await _answerrepository.InsertAsync(new SurveyAnswer { SurveyId = input.Id, UserId = userId });
                }
            }

            return ObjectMapper.Map<SurveyDto>(entity);
        }

        /// <summary>
        /// 删除考卷
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async Task DeleteSurveyAsync(EntityDto<Guid> input)
        {
            var entity = _repository.Get(input.Id);
            if (entity == null)
            {
                throw new UserFriendlyException($"数据不存在");
            }

            await _repository.DeleteAsync(entity);
        }
    }
}
