using ADTO.DCloud.DataItem;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.Surveys.QuestionCategorys.Dto;
using ADTO.DCloud.Surveys.SurveyQuestions.Dto;
using ADTO.OA.Surveys.SurveyQuestions;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.EntityFrameworkCore.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Runtime.Caching;
using ADTOSharp.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace ADTO.DCloud.Surveys.SurveyQuestions
{
    /// <summary>
    /// 题库管理
    /// </summary>
    public class SurveyQuestionAppService : DCloudAppServiceBase, ISurveyQuestionAppService
    {
        #region Fields
        private readonly IRepository<SurveyQuestionCategory, Guid> _categoryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly IAbpSession _abpSession;
        private readonly DataItemDetailAppService _dataItemDetailAppService;
        private readonly IRepository<Survey, Guid> _surveyrepository;
        private readonly ICacheManager _cacheManager;
        IRepository<SurveyQuestion, Guid> _repository;
        #endregion

        #region ctor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        public SurveyQuestionAppService(
             IRepository<SurveyQuestion, Guid> repository,
             IRepository<SurveyQuestionCategory, Guid> categoryRepository,
             IHttpContextAccessor httpContextAccessor,
             //IAbpSession abpSession,
             DataItemDetailAppService dataItemDetailAppService,
             IRepository<Survey, Guid> surveyrepository,
             ICacheManager cacheManager)
        {
            _categoryRepository = categoryRepository;
            _httpContextAccessor = httpContextAccessor;
            _dataItemDetailAppService = dataItemDetailAppService;
            _surveyrepository = surveyrepository;
            _cacheManager = cacheManager;
            _repository = repository;
        }

        #endregion

        /// <summary>
        /// 添加题库信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateQuestionAsync(CreateSurveyQuestionDto input)
        {
            var info = ObjectMapper.Map<SurveyQuestion>(input);

            await _repository.InsertAsync(info);
        }

        /// <summary>
        /// 修改题库资料
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateQuestionAsync(UpdateSurveyQuestionDto input)
        {
            var info = this._repository.Get(input.Id);
            ObjectMapper.Map(input, info);

            await _repository.UpdateAsync(info);
        }

        /// <summary>
        /// 删除指定的题库资料
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteQuestionAsync(EntityDto<Guid> input)
        {
            var info = await this._repository.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            if (info == null)
            {
                throw new UserFriendlyException("操作失败，记录不存在！");
            }

            await _repository.DeleteAsync(input.Id);
        }

        /// <summary>
        /// 获取指定题库详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<SurveyQuestionDto> GetQuestionByIdAsync(EntityDto<Guid> input)
        {
            var info = await _repository.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            var infoDto = ObjectMapper.Map<SurveyQuestionDto>(info);

            return infoDto;
        }

        #region 获取分页题库信息
        /// <summary>
        /// -获取分页题库信息
        /// </summary>
        /// <param name="input">查询实体</param>
        /// <returns></returns>
        public async Task<PagedResultDto<SurveyQuestionDto>> GetSurveyQuestionPagedAllList(PagedSurveyQuestionReqeustDto input)
        {
            var query = from question in this._repository.GetAll()
                        join categorys in _categoryRepository.GetAll() on question.QuestionCategoryId equals categorys.Id into a
                        from category in a.DefaultIfEmpty()
                        select new { question, category };
            query = query.WhereIf(!string.IsNullOrEmpty(input.Keyword), d => d.question.QuestionName.Contains(input.Keyword) || d.category.Name.Contains(input.Keyword))
                .WhereIf(!string.IsNullOrEmpty(input.QuestionType), d => d.question.QuestionType == input.QuestionType)
                .WhereIf(input.CategoryId.HasValue && input.CategoryId != Guid.Empty, q => q.category.Id == input.CategoryId);
            //获取总数
            var resultCount = await query.CountAsync();
            var list = query.PageBy(input).ToList().Select(item =>
            {
                var dto = ObjectMapper.Map<SurveyQuestionDto>(item.question);
                dto.QuestionCategory = item.category != null ? item.category.Name : "";
                return dto;
            }).ToList();
            return new PagedResultDto<SurveyQuestionDto>(resultCount, list);
        }
        #endregion

        #region 前端题库查询
        /// <summary>
        /// -获取考卷题库信息
        /// </summary>
        /// <param name="input">查询实体</param>
        /// <returns></returns>
        public async Task<SurveyQuestionListDto> GetSurveyQuestionList(SurveyQuestionReqeustDto input)
        {
            if (!input.SurveyId.HasValue)
            {
                throw new UserFriendlyException($"考试已结束");
            }

            var survey = _surveyrepository.Get(input.SurveyId.Value);

            if (survey == null || survey.Id == Guid.Empty || DateTime.Compare(survey.EndDate, DateTime.Now) < 0 || survey.EndDate < DateTime.Now)
            {
                throw new UserFriendlyException($"考试已结束");
            }


            if (DateTime.Compare(DateTime.Now, survey.StarDate) < 0)
            {
                throw new UserFriendlyException($"考试未开始");
            }

            var source = survey.QuestionSource.Split(',').ToArray();
            SurveyQuestionListDto surveydto = new SurveyQuestionListDto() { Timer = survey.Timer, Name = survey.Name, Remark = survey.Remark };
            var query = this._repository.GetAll().Where(q => q.SurveyID == input.SurveyId || q.SurveyID == null || q.SurveyID == Guid.Empty);
            query = query.Where(q => source.Contains(q.QuestionCategoryId.ToString()));
            var surveyList = query.Where(q => q.SurveyID == input.SurveyId);


            List<SurveyQuestion> surveyQuestions = new List<SurveyQuestion>();
            //选择题
            var choiceList = query.Where(q => q.QuestionType == "1" || q.QuestionType == "2").OrderBy(q => Guid.NewGuid());
            //判断题
            var isList = query.Where(q => q.QuestionType == "3").OrderBy(q => Guid.NewGuid());
            //填空题
            var fillList = query.Where(q => q.QuestionType == "4").OrderBy(q => Guid.NewGuid());
            //简答题
            var answerList = query.Where(q => q.QuestionType == "5").OrderBy(q => Guid.NewGuid());

            //选择题
            decimal chScore = 0;
            foreach (var item in choiceList)
            {
                if ((chScore + item.Score) <= survey.ChoiceQuestionScore && surveyQuestions.Where(d => d.Id == item.Id).Count() <= 0)
                {
                    surveyQuestions.Add(item);
                    chScore = chScore + item.Score;
                }
                else if (chScore == survey.ChoiceQuestionScore)
                    break;
            }
            //判断
            decimal pdScore = 0;
            foreach (var item in isList)
            {
                if ((pdScore + item.Score) <= survey.IsQuestionScore && surveyQuestions.Where(d => d.Id == item.Id).Count() <= 0)
                {
                    surveyQuestions.Add(item);
                    pdScore = pdScore + item.Score;
                }
                else if (pdScore == survey.IsQuestionScore)
                    break;
            }
            //填空
            decimal tkScore = 0;
            foreach (var item in fillList)
            {
                if ((tkScore + item.Score) <= survey.FillQuestionScore && surveyQuestions.Where(d => d.Id == item.Id).Count() <= 0)
                {
                    surveyQuestions.Add(item);
                    tkScore = tkScore + item.Score;
                }
                else if (tkScore == survey.FillQuestionScore)
                    break;
            }
            //简单
            decimal jdScore = 0;
            foreach (var item in answerList)
            {
                if ((jdScore + item.Score) <= survey.AnswerQuestionScore && surveyQuestions.Where(d => d.Id == item.Id).Count() <= 0)
                {
                    surveyQuestions.Add(item);
                    jdScore = jdScore + item.Score;
                }
                else if (jdScore == survey.AnswerQuestionScore)
                    break;
            }
            //获取总数
            var list = surveyQuestions.Select(item =>
             {
                 var dto = ObjectMapper.Map<SurveyQuestionPCDto>(item);
                 return dto;
             }).ToList();
            surveydto.SurveyQuestions = list.OrderBy(o => o.QuestionType).ToList();
            surveydto.StarDate = DateTime.Now;

            return surveydto;
        }

        #endregion

        #region 获取答题时间
        /// <summary>
        ///返回前端答题时间
        /// </summary>
        /// <returns></returns>
        //public JsonResultModel GetAnswerTime()
        //{
        //    return new JsonResultModel() { Message = $"答题时间{DateTime.Now}", Success = true, Data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
        //}
        #endregion

        #region  批量导入
        /// <summary>
        /// 批量导入excel提交信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<string> ImportSurveyQuestion()
        {
            try
            {
                //当前登录用户
                Guid userId = ADTOSharpSession.UserId.Value;
                if (userId == Guid.Empty)
                {
                    throw new UserFriendlyException($"操作失败，当前用户未登录");
                }
                DataTable dt = new DataTable();
                DataTable dataTable = new DataTable();
                var httpContext = _httpContextAccessor.HttpContext;
                IFormFile formFile = httpContext.Request.Form.Files.FirstOrDefault();
                if (formFile == null)
                {
                    throw new UserFriendlyException($"操作失败，没有文件上传");
                }
                string strExtension = Path.GetExtension(formFile.FileName);
                List<SurveyQuestionDto> questionEntities = new List<SurveyQuestionDto>();
                IWorkbook workbook;
                if (".xls".Equals(strExtension, StringComparison.CurrentCultureIgnoreCase))
                {
                    //xls 格式
                    workbook = new HSSFWorkbook(formFile.OpenReadStream());
                }
                else if (".xlsx".Equals(strExtension, StringComparison.CurrentCultureIgnoreCase))
                {
                    //xlsx 格式
                    workbook = new XSSFWorkbook(formFile.OpenReadStream());
                }
                else
                {
                    workbook = null;
                }

                if (workbook != null)
                {
                    ISheet sheet = workbook.GetSheetAt(0);

                    IRow headerRow = sheet.GetRow(1);
                    int cellCount = headerRow.LastCellNum;

                    for (int j = 0; j < cellCount; j++)
                    {
                        NPOI.SS.UserModel.ICell cell = headerRow.GetCell(j);
                        dt.Columns.Add(cell + "");
                    }

                    for (int i = (sheet.FirstRowNum + 2); i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row != null)
                        {
                            DataRow dataRow = dt.NewRow();

                            for (int j = row.FirstCellNum; j < cellCount; j++)
                            {
                                if (row.GetCell(j) != null)
                                    dataRow[j] = row.GetCell(j).ToString();
                            }
                            dt.Rows.Add(dataRow);
                        }
                    }
                    var categoryList = _categoryRepository.GetAll();
                    var dataitems = await _dataItemDetailAppService.GetItemDetailList(new DataItem.Dto.DataItemQueryDto() { ItemCode = "QuestionType" });
                    foreach (DataRow item in dt.Rows)
                    {
                        var dataitem = dataitems.Where(d => d.ItemName.Equals(item["题库类型"] + "")).FirstOrDefault();
                        List<QuestionOptionDto> list = new List<QuestionOptionDto>();
                        char currentLetter = 'A';
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            var value = item[dt.Columns[i].ColumnName] + "";
                            if (dt.Columns[i].ColumnName.Contains("题库选项") && !string.IsNullOrEmpty(value))
                            {
                                QuestionOptionDto dto = new QuestionOptionDto() { Id = currentLetter, Name = item[dt.Columns[i].ColumnName] + "" };
                                list.Add(dto);
                                currentLetter = ((char)(currentLetter + 1));
                            }
                        }
                        string answer = item["正确答案"] + "";
                        if (dataitem.ItemName.Contains("多选"))
                        {
                            answer = string.Join(",", Regex.Matches(item["正确答案"].ToString(), "[a-zA-Z]"));
                        }
                        var category = categoryList.Where(d => d.Name.Equals(item["题库类别"] + "")).FirstOrDefault();
                        SurveyQuestionDto questionDto = new SurveyQuestionDto()
                        {
                            Id = Guid.NewGuid(),
                            QuestionName = item["题库名称"].ToString(),
                            QuestionCategoryId = category == null || category.Id == Guid.Empty ? new Guid("02854D0E-3B26-4136-88F2-B967222DE344") : category.Id,
                            QuestionType = dataitem != null ? dataitem.ItemValue : "",
                            Score = item["题目分值"] + "",
                            CorrectAnswer = answer,
                            Option = list.ToJson(),
                        };
                        questionEntities.Add(questionDto);
                    }
                }
                else
                {
                    throw new UserFriendlyException($"操作失败，请检查Excel文件格式");
                }

                if (questionEntities != null && questionEntities.Count > 0)
                {
                    var modelList = ObjectMapper.Map<List<SurveyQuestion>>(questionEntities);
                    // await this._repository.GetDbContext().BulkInsertOrUpdateAsync(modelList);

                    await this._repository.InsertRangeAsync(modelList);
                }

                //return new JsonResultModel() { Message = $"操作成功，总共导入{questionEntities.Count()}条数据", Success = true };
                return $"操作成功，总共导入{questionEntities.Count()}条数据";
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException($"操作失败,服务器异常，请联系开发人员：" + ex.Message);
            }
        }
        #endregion
    }
}
