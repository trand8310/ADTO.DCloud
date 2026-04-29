using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Authorization.Users.Dto;
using ADTO.DCloud.DataItem;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.Surveys.SurveyAnswers.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Organizations;
using ADTOSharp.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Surveys.SurveyAnswers
{
    /// <summary>
    /// 答卷
    /// </summary>
    public class SurveyAnswerAppService : DCloudAppServiceBase, ISurveyAnswerAppService
    {
        #region Fields
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IRepository<OrganizationUnit, Guid> _orgRepository;
        private readonly IRepository<SurveyAnswerDetail, Guid> _answerDetailRepository;
        private readonly IRepository<SurveyQuestion, Guid> _questionRepository;
        IRepository<SurveyAnswer, Guid> _answerRepository;
        private IRepository<DataItemDetail, Guid> _dataItemDetailRepository;
        #endregion

        #region ctor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        public SurveyAnswerAppService(
            IRepository<SurveyAnswer, Guid> answerRepository,
            IRepository<User, Guid> userRepository,
            IRepository<OrganizationUnit, Guid> orgRepository,
            IRepository<SurveyAnswerDetail, Guid> answerDetailRepository,
            IRepository<SurveyQuestion, Guid> questionRepository
            , IRepository<DataItemDetail, Guid> dataItemDetailRepository)
        {
            _answerRepository = answerRepository;
            _userRepository = userRepository;
            _answerDetailRepository = answerDetailRepository;
            _orgRepository = orgRepository;
            _questionRepository = questionRepository;
            _dataItemDetailRepository = dataItemDetailRepository;
        }
        #endregion

        /// <summary>
        /// 不分页查询当前考卷下面的所有用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<UserDto>> GetSurveyAnswerList(SurveyAnswerRequestDto input)
        {
            var query = from participant in this._answerRepository.GetAll()
                        join u in _userRepository.GetAll() on participant.UserId equals u.Id into a
                        from user in a.DefaultIfEmpty()
                        join d in _orgRepository.GetAll() on user.DepartmentId equals d.Id into deptment
                        from department in deptment.DefaultIfEmpty()
                        where participant.SurveyId.Equals(input.SurveyId)
                        select new UserDto
                        {
                            Id = user.Id,
                            Name = user.Name ?? "",
                            UserName = user.UserName ?? "",
                            DepartmentName = department.DisplayName ?? "",
                            DepartmentId = user.DepartmentId,
                            CompanyId = user.CompanyId
                        };
            return await query.ToListAsync();
        }

        /// <summary>
        /// 分页查询答卷信息/内部考核统计分页列表 GetAllAsync
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        //[DataPermission("考卷统计")]
        public async Task<PagedResultDto<SurveyAnswerDto>> GetSurveyAnswerPagedListAsync(PagedSurveyAnswerRequestDto input)
        {
            var query = from participant in this._answerRepository.GetAll()
                        join u in _userRepository.GetAll() on participant.UserId equals u.Id into a
                        from user in a.DefaultIfEmpty()
                        join d in _orgRepository.GetAll() on user.DepartmentId equals d.Id into d1
                        from department in d1.DefaultIfEmpty()

                        join t9 in _dataItemDetailRepository.GetAllIncluding(p => p.Item).Where(p => p.Item.ItemCode == "AnswerStatus") on participant.AnswerStatus.ToString() equals t9.ItemValue into itemdetails
                        from itemdetail in itemdetails.DefaultIfEmpty()

                        select new { participant, user, department, itemdetail };
            query = query.WhereIf(input.SurveyId.HasValue, q => q.participant.SurveyId.Equals(input.SurveyId))
                .WhereIf(input.DepartmentId.HasValue && input.DepartmentId != Guid.Empty, q => q.department.Id == input.DepartmentId)
                .WhereIf(input.CompanyId.HasValue && input.CompanyId != Guid.Empty, q => q.user.CompanyId.Equals(input.CompanyId))
                .WhereIf(!string.IsNullOrEmpty(input.keyword), q => q.user.UserName.Equals(input.keyword) || q.user.Name.Contains(input.keyword));

            //获取总数
            var resultCount = await query.CountAsync();
            var list = query.PageBy(input).ToList();
            var ResultList = list.Select(item =>
            {
                var dto = ObjectMapper.Map<SurveyAnswerDto>(item.participant);
                dto.Name = item.user != null ? item.user.Name : "";
                dto.UserName = item.user != null ? item.user.UserName : "";
                dto.Department = item.department != null ? item.department.DisplayName : "";
                dto.AnswerStatusText = item.itemdetail.ItemName;
                return dto;
            }).ToList();

            return new PagedResultDto<SurveyAnswerDto>(resultCount, ResultList);
        }

        ///// <summary>
        ///// 导出考卷统计
        ///// </summary>
        ///// <param name="input">后端分页查询条件</param>
        ///// <returns></returns>
        //[ADTOSharpAllowAnonymous]
        ////[AbpAuthorize("Pages.Survey.AnswerList.Export")]
        //[HttpPost]
        //public async Task<FileResult> GetAllExportExcel(PagedSurveyAnswerRequestDto input)
        //{
        //    input.MaxResultCount = int.MaxValue;
        //    input.SkipCount = 0;
        //    //查询语句
        //    var query = await this.GetAllAsync(input);
        //    ExcelConfig excelconfig = new ExcelConfig();
        //    excelconfig.Title = "答卷统计";
        //    excelconfig.TitleFont = "微软雅黑";
        //    excelconfig.TitlePoint = 25;
        //    excelconfig.FileName = "答卷统计";
        //    excelconfig.IsAllSizeColumn = true;
        //    //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
        //    excelconfig.ColumnEntity = new List<ColumnModel>();
        //    excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "Department", ExcelColumn = "所属部门" });
        //    excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "Name", ExcelColumn = "姓名" });
        //    excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "UserName", ExcelColumn = "工号" });
        //    excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "AnswerStatus", ExcelColumn = "答卷状态" });
        //    excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "AnswerStartTime", ExcelColumn = "答题开始时间" });
        //    excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "AnswerEndTime", ExcelColumn = "答题结束时间" });
        //    excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "SpendMinutes", ExcelColumn = "花费时间" });
        //    excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "AnswerQuestionAmount", ExcelColumn = "已答题数" });
        //    excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "Score", ExcelColumn = "分数" });

        //    var dtSource = Extensions.ListToDataTable(query.Items.ToList());
        //    var fileBytes = ExcelHelper.ExportDataTableMemoryStream(dtSource, excelconfig);
        //    var content = new System.IO.MemoryStream(fileBytes);
        //    content.Position = 0;

        //    return new FileStreamResult(content, "application/vnd.ms-excel;charset=utf-8")
        //    {
        //        FileDownloadName = "答卷统计.xls"
        //    };
        //}
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteAnswer(EntityDto<Guid> input)
        {
            var entity = this._answerRepository.Get(input.Id);
            await this._answerRepository.DeleteAsync(entity);
        }
        /// <summary>
        /// 根据用户和考卷Id删除参与人员
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[AbpAuthorize("Pages.Survey.AnswerList.Delete")]
        [HttpPost]
        public async Task DeleteAnswerByUserIdSurveyId(DeleteByUserIdSurveyIdDto input)
        {
            if (!input.SurveyId.HasValue || input.SurveyId == Guid.Empty)
            {
                throw new UserFriendlyException($"不存在");
            }

            if (!input.UserId.HasValue || input.UserId <= 0)
            {
                throw new UserFriendlyException($"不存在");
            }

            var entity = await this._answerRepository.GetAll().Where(q => q.SurveyId.Equals(input.SurveyId) && q.UserId.Equals(input.UserId)).FirstOrDefaultAsync();

            if (entity == null || entity.Id == Guid.Empty)
            {
                throw new UserFriendlyException($"不存在");
            }

            await this._answerRepository.DeleteAsync(entity);
        }
        /// <summary>
        /// 保存试卷
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        //[AbpAuthorize("Pages.Survey.AnswerList.GradingPapers")]
        public async Task AuditAnswerAsync(AuditSurveyAnswerDto input)
        {
            try
            {
                var entity = this._answerRepository.Get(input.Id);
                if (entity == null || entity.Id == Guid.Empty)
                {
                    throw new UserFriendlyException($"提交失败，试卷不存在");
                }

                var entityDetail = _answerDetailRepository.GetAll().Where(q => q.AnswerID == entity.Id).FirstOrDefault();
                if (entityDetail == null || entityDetail.Id == Guid.Empty)
                {
                    throw new UserFriendlyException($"提交失败，试卷不存在");
                }
                entity.AnswerStatus = 3;
                entity.Status = 1;
                entity.Score = input.Score;

                await this._answerRepository.UpdateAsync(entity);
                entityDetail.AnswerContent = input.AnswerContent;
                entityDetail.AnswerValue = input.AnswerValue;
                await _answerDetailRepository.UpdateAsync(entityDetail);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException($"提交失败!");
            }
        }

        /// <summary>
        /// 保存试卷
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task SubmitAnswerAsync(SubmitSurveyAnswerDto input)
        {
            try
            {
                var user = _userRepository.GetAll().Where(q => q.UserName.Equals(input.UserName) && q.Name.Equals(input.Name)).FirstOrDefault();
                if (user == null || user.Id == Guid.Empty)
                {
                    throw new UserFriendlyException($"提交失败，姓名或工号错误，用户信息不存在!");
                }

                List<AnswerQuestionDto> surveyQuestions = input.AnswerContent.ToObject<List<AnswerQuestionDto>>();
                if (surveyQuestions.Count() <= 0)
                {
                    throw new UserFriendlyException($"提交失败，考卷题库为空，请刷新重试!");
                }

                var entity = this._answerRepository.GetAll().Where(q => q.UserId == user.Id && q.SurveyId == input.SurveyId).FirstOrDefault() ?? new SurveyAnswer();
                if (entity.AnswerStatus != 1 && entity.AnswerStatus != 0)
                {
                    throw new UserFriendlyException($"提交失败，试卷已提交，不能重复提交");
                }

                entity.AnswerQuestionAmount = input.AnswerQuestionAmount;
                entity.UserId = user.Id;
                entity.SurveyId = input.SurveyId;
                TimeSpan timeDifference = DateTime.Now - input.StarDate;
                int minutesDifference = (int)timeDifference.TotalMinutes;
                int secondsDifference = (int)timeDifference.TotalSeconds % 60;
                entity.SpendMinutes = $"{minutesDifference}分{secondsDifference}秒";
                entity.Status = 1;
                entity.AnswerStatus = 2;
                entity.AnswerStartTime = input.StarDate;
                entity.AnswerEndTime = DateTime.Now;

                var ids = surveyQuestions.Select(d => d.Id);
                var questionList = _questionRepository.GetAll().Where(d => ids.Contains(d.Id)).OrderBy(o => o.QuestionType);
                foreach (var item in surveyQuestions)
                {
                    var question = questionList.Where(d => d.Id.Equals(item.Id)).FirstOrDefault() ?? new SurveyQuestion();
                    item.CorrectAnswer = question.CorrectAnswer;
                    var a = item.Answer;
                    switch (item.QuestionType)
                    {
                        //多选+判断
                        case "1":
                        case "2":
                            // 使用正则表达式匹配文本
                            //Match matches = new Regex(@"/[a-zA-Z]/g").Match(item.CorrectAnswer);
                            item.AnswerScore = item.CorrectAnswer == string.Join(",", item.Answer) ? item.Score : Convert.ToDecimal(0);
                            break;
                        case "3"://填空
                            // 使用正则表达式匹配文本
                            //Match matches1 = new Regex(@"/^[a-zA-Z0-9\u4e00-\u9fa5]+$/").Match(item.CorrectAnswer);
                            item.AnswerScore = item.CorrectAnswer.Trim(' ') == string.Join(",", item.Answer) ? item.Score : Convert.ToDecimal(0);
                            break;
                    }
                }
                entity.Score = surveyQuestions.Sum(q => q.AnswerScore);

                if (entity == null || entity.Id == Guid.Empty)
                    entity.Id = await this._answerRepository.InsertAndGetIdAsync(entity);
                else
                    await this._answerRepository.UpdateAsync(entity);

                await _answerDetailRepository.InsertAsync(new SurveyAnswerDetail() { AnswerID = entity.Id, AnswerContent = surveyQuestions.ToJson(), AnswerValue = input.AnswerValue });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException($"提交失败,{ex.Message}");
            }
        }

        /// <summary>
        /// 判断用户姓名和工号,是否正确
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task IsUserNameAndNumber(IsUserNameAndNumberDto input)
        {
            var user = await _userRepository.GetAll().Where(q => q.UserName.Equals(input.UserName) && q.Name.Equals(input.Name)).FirstOrDefaultAsync();
            if (user == null || user.Id == Guid.Empty)
            {
                throw new UserFriendlyException($"姓名或工号错误，用户信息不存在");
            }
        }

        /// <summary>
        /// 根据Id获取当前答卷详细信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [ADTOSharpAllowAnonymous]
        public async Task<SurveyAnswerDetailDto> GetAnswerDetailAsync(EntityDto<Guid> input)
        {
            var entity = await _answerDetailRepository.GetAll().Where(q => q.AnswerID.Equals(input.Id)).FirstOrDefaultAsync();
            return ObjectMapper.Map<SurveyAnswerDetailDto>(entity);
        }
    }
}
