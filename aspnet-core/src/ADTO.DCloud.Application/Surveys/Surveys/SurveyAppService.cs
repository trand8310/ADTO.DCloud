using ADTO.DCloud.Surveys.SurveyAnswers.Dto;
using ADTO.DCloud.Surveys.Surveys.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ADTO.DCloud.Surveys.Surveys
{
    /// <summary>
    /// 考卷
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
        /// 查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<PagedResultDto<SurveyDto>> GetAllAsync(PagedSurveyRequestDto input)
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
        /// 查询有效考卷
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<SurveyDto>> GetEffectiveSurveyList(SurveyEffectiveRequestDto input)
        {
            var query = _repository.GetAll()
                .WhereIf(!string.IsNullOrEmpty(input.keyword), q => q.Name.Contains(input.keyword));
            query = query.Where(q => DateTime.Now >= q.StarDate && DateTime.Now <= q.EndDate);
            var ResultList = query.ToList().Select(item =>
            {
                var dto = ObjectMapper.Map<SurveyDto>(item);
                return dto;
            }).ToList();
            return ResultList;
        }

        /// <summary>
        /// 根据Id获取当前数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [ADTOSharpAllowAnonymous]
        public async Task<SurveyDto> GetAsync(EntityDto<Guid> input)
        {
            var entity = await _repository.GetAsync(input.Id);
            var dto = ObjectMapper.Map<SurveyDto>(entity);
            return dto;
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<SurveyDto> CreateAsync(CreateSurveyDto input)
        {
            Survey entity = ObjectMapper.Map<Survey>(input);

            var id = await _repository.InsertAndGetIdAsync(entity);

            foreach (var item in input.UserList)
            {
                SurveyAnswer p_entity = new SurveyAnswer() { SurveyId = id, UserId = item.Id };
                await _answerrepository.InsertAsync(p_entity);
            }

            return ObjectMapper.Map<SurveyDto>(entity);
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<SurveyDto> UpdateAsync(UpdateSurveyDto input)
        {
            var entity = _repository.Get(input.Id);

            ObjectMapper.Map(input, entity);
            entity.QuestionSource = input.QuestionSource.ToString();
            await _repository.UpdateAsync(entity);

            var userlist = _answerrepository.GetAll().Where(q => q.SurveyId == entity.Id);
            foreach (var item in input.UserList)
            {
                var pentity = userlist.Where(q => q.UserId == item.Id).FirstOrDefault() ?? new SurveyAnswer() { SurveyId = entity.Id, UserId = item.Id };
                await _answerrepository.InsertOrUpdateAsync(pentity);
            }

            return ObjectMapper.Map<SurveyDto>(entity);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async Task DeleteAsync(EntityDto<Guid> input)
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
