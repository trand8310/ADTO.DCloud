using ADTO.DCloud;
using ADTO.DCloud.Surveys;
using ADTO.DCloud.Surveys.QuestionCategorys;
using ADTO.DCloud.Surveys.QuestionCategorys.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADTO.OA.Surveys.QuestionCategorys
{
    /// <summary>
    /// 题库类型
    /// </summary>
    public class QuestionCategoryAppService : DCloudAppServiceBase, IQuestionCategoryAppService
    {
        IRepository<SurveyQuestionCategory, Guid> _categoryRepository;
        public QuestionCategoryAppService(IRepository<SurveyQuestionCategory, Guid> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        #region 查询所有题库类型
        /// <summary>
        /// 查询所有题库类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[DataPermission("题库类型")]
        public async Task<List<QuestionCategoryDto>> GetAllList(QuestionCategoryRequestDto input)
        {
            var query = await this._categoryRepository.GetAll().WhereIf(!string.IsNullOrEmpty(input.keyword), q => q.Name.Contains(input.keyword)).ToListAsync();

            var ResultList = ObjectMapper.Map<List<QuestionCategoryDto>>(query);
            return ResultList;
        }
        #endregion

        /// <summary>
        /// 添加题库类型信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateQuestionCategoryAsync(CreateQuestionCategoryDto input)
        {
            var info = ObjectMapper.Map<SurveyQuestionCategory>(input);

            await _categoryRepository.InsertAsync(info);
        }

        /// <summary>
        /// 修改题库类型资料
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateQuestionCategoryAsync(UpdateQuestionCategoryDto input)
        {
            var info = this._categoryRepository.Get(input.Id);
            ObjectMapper.Map(input, info);

            await _categoryRepository.UpdateAsync(info);
        }

        /// <summary>
        /// 删除指定的题库类型资料
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteQuestionCategoryAsync(EntityDto<Guid> input)
        {
            var info = await this._categoryRepository.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            if (info == null)
            {
                throw new UserFriendlyException("操作失败，记录不存在！");
            }

            await _categoryRepository.DeleteAsync(input.Id);
        }

        /// <summary>
        /// 获取题库类型分页列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns></returns>

        public async Task<PagedResultDto<QuestionCategoryDto>> GetQuestionCategoryPageList(PagedQuestionCategoryRequestDto input)
        {
            var query = _categoryRepository.GetAll()
                 .WhereIf(!string.IsNullOrWhiteSpace(input.keyWord), p => p.Name.Contains(input.keyWord));

            var totalCount = await query.CountAsync();
            var items = await query.PageBy(input).ToListAsync(); ;

            return new PagedResultDto<QuestionCategoryDto>(totalCount, ObjectMapper.Map<List<QuestionCategoryDto>>(items));
        }

        /// <summary>
        /// 获取指定题库类型详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<QuestionCategoryDto> GetQuestionCategoryByIdAsync(EntityDto<Guid> input)
        {
            var info = await _categoryRepository.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            var infoDto = ObjectMapper.Map<QuestionCategoryDto>(info);

            return infoDto;
        }
    }
}
