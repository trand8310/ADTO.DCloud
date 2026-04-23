
using ADTO.DCloud.Dto;
using ADTO.DCloud.Dto.Excel;
using ADTO.DCloud.EmployeeManager.Dto;
using ADTO.DCloud.Infrastructur;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.OA4PTest;
using ADTOSharp;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.EntityFrameworkCore.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ADTO.DCloud.EmployeeManager
{
    public class OA4PTestingAppService : DCloudAppServiceBase, IOA4PTestingAppService
    {

        #region Fields

        private readonly IRepository<OA4PQuestion, Guid> _repository;
        private readonly IRepository<OA4PStatistic, Guid> _statisticrepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Ctor
        public OA4PTestingAppService(
            IRepository<OA4PQuestion, Guid> repository,
            IRepository<OA4PStatistic, Guid> statisticrepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _statisticrepository = statisticrepository;
            _httpContextAccessor = httpContextAccessor;

        }
        #endregion

        #region Methods



        #region 4P测试
        /// <summary>
        /// 获取分页-4P测试题库信息
        /// </summary>
        /// <param name="input">查询实体</param>
        /// <returns></returns>
        /// 
        public async Task<PagedResultDto<OA4PQuestionDto>> GetPage4PQuestionList(PagedOA4PQuestionResultRequestDto input)
        {
            var query = _repository.GetAll()
                .WhereIf(!string.IsNullOrEmpty(input.Type), q => q.Type.Equals(input.Type))
                .WhereIf(input.StartDate.HasValue && input.EndDate.HasValue, q => q.CreationTime >= input.StartDate && q.CreationTime <= input.EndDate)
                .WhereIf(!string.IsNullOrWhiteSpace(input.keyword), q => q.QuestionsTitle.Contains(input.keyword) || 
                q.Option_1.Contains(input.keyword) || 
                q.Option_2.Contains(input.keyword) || 
                q.Option_3.Contains(input.keyword) || 
                q.Option_4.Contains(input.keyword));

            var resultCount = await query.CountAsync();
            var list = query.OrderByDescending(x => x.CreationTime).PageBy(input).ToList();
            var ResultList = ObjectMapper.Map<List<OA4PQuestionDto>>(list);

            return new PagedResultDto<OA4PQuestionDto>(resultCount, ResultList);
        }

        /// <summary>
        /// 获取分页-4P测试提交统计
        /// </summary>
        /// <param name="input">查询实体</param>
        /// <returns></returns>
        public async Task<PagedResultDto<OA4PStatisticDto>> GetPage4PStatisticList(Paged4PStatisticResultRequestDto input)
        {
            var query = _statisticrepository.GetAll()
                .Where(q => q.Type == "4PTest")
                .WhereIf(input.StartDate.HasValue && input.EndDate.HasValue, q => q.CreationTime >= input.StartDate && q.CreationTime <= input.EndDate.Value.ToString("yyyy-MM-dd").ToDate().AddSeconds(86399))
               .WhereIf(!string.IsNullOrWhiteSpace(input.keyword), q => q.PostName.Contains(input.keyword.Trim()) || q.TrueName.Contains(input.keyword.Trim()) || q.Mobile.Contains(input.keyword.Trim()))
               .OrderByDescending(x => x.CreationTime);
            var resultCount = await query.CountAsync();
            var list = query.PageBy(input).ToList();
            var ResultList = ObjectMapper.Map<List<OA4PStatisticDto>>(list);
            return new PagedResultDto<OA4PStatisticDto>(resultCount, ResultList);
        }

        #endregion

        private string GetRequestPath()
        {
            return IocManager.Instance.Resolve<IHttpContextAccessor>().HttpContext.Request.Path;
        }
        /// <summary>
        /// 根据编号查询问卷信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<OA4PQuestionDto> Get4PQuestionAsync(EntityDto<Guid> input)
        {
            var entity = _repository.Get(input.Id);
            var dto = ObjectMapper.Map<OA4PQuestionDto>(entity);
            return dto;
        }

        /// <summary>
        /// 根据编号查询4p测试统计详细信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<OA4PStatisticDto> Get4PStatisticAsync(EntityDto<Guid> input)
        {
            var entity = _statisticrepository.Get(input.Id);
            var dto = ObjectMapper.Map<OA4PStatisticDto>(entity);
            return dto;
        }

        #region 题库操作

        /// <summary>
        /// 新增-题库信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<JsonResultModel> Insert4PQuestion(CreateOA4PQuestionDto dto)
        {
            var entity = ObjectMapper.Map<OA4PQuestion>(dto);
            var Id = await _repository.InsertAndGetIdAsync(entity);
            if (Id != Guid.Empty)
            {
                return new JsonResultModel().SuccessInfo("操作成功");
            }
            return new JsonResultModel().ErrorInfo("操作失败");
        }
        /// <summary>
        /// 编辑-题库信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<OA4PQuestionDto> Update4PQuestion(OA4PQuestionDto dto)
        {
            var entity = _repository.Get(dto.Id);
            ObjectMapper.Map(dto, entity);
            await _repository.UpdateAsync(entity);
            return ObjectMapper.Map<OA4PQuestionDto>(entity);
        }

        /// <summary>
        /// 删除-题库信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<JsonResultModel> Delete4PQuestion(EntityDto<Guid> input)
        {
            var entity = _repository.Get(input.Id);
            if (entity == null)
            {
                return new JsonResultModel().ErrorInfo("操作失败");
            }
            await _repository.DeleteAsync(entity);
            return new JsonResultModel().SuccessInfo("操作成功");
        }
        #endregion


        /// <summary>
        /// 前端-返回用户4P测试题库信息
        /// </summary>
        /// <returns></returns>
        [ADTOSharpAllowAnonymous]
        public async Task<JsonResultModel> GetList()
        {
            var query = await _repository.GetAllListAsync();
            var list = query.Where(q => q.Type == "4PTest").ToList();
            return new JsonResultModel() { Message = $"操作成功", Success = true, Data = "{\"starTime\":\"" + DateTime.Now + "\",\"result\":" + list.ToJson() + "}" };
        }

        /// <summary>
        /// 前端-返回用户DISC测试题库信息
        /// </summary>
        /// <returns></returns>
        [ADTOSharpAllowAnonymous]
        public async Task<JsonResultModel> GetDiscList()
        {
            var query = await _repository.GetAllListAsync();
            query = query.Where(q => q.Type == "DISC").OrderBy(d => d.SortCode).ToList();
            var list = ObjectMapper.Map<List<OA4PQuestionDto>>(query);
            return new JsonResultModel() { Message = $"操作成功", Success = true, Data = "{\"starTime\":\"" + DateTime.Now + "\",\"list\":" + list.ToJson() + "}" };
        }
        #region 前端用户4P测试提交
        /// <summary>
        /// 前端-用户4P测试提交
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [ADTOSharpAllowAnonymous]
        [HttpPost]
        public async Task<JsonResultModel> Submit4PTest(CreateOA4PStatisticDto input)
        {
            OA4PStatistic entity = ObjectMapper.Map<OA4PStatistic>(input);
            List<OA4PDataDto> list = input.Data;//.ToObject<List<OA4PDataDto>>();
            if (string.IsNullOrEmpty(input.Type) || input.Type == "4PTest")
            {
                entity.Type = "4PTest";
                entity.Option_1_Total = list.Where(t => t.value == "1").Count();
                entity.Option_2_Total = list.Where(t => t.value == "2").Count();
                entity.Option_3_Total = list.Where(t => t.value == "3").Count();
                entity.Option_4_Total = list.Where(t => t.value == "4").Count();
            }
            else
            {
                entity.Type = string.IsNullOrEmpty(input.Type) ? "DISC" : input.Type;
                entity.Option_1_Total = list.Where(t => t.value == "D").Count();
                entity.Option_2_Total = list.Where(t => t.value == "I").Count();
                entity.Option_3_Total = list.Where(t => t.value == "S").Count();
                entity.Option_4_Total = list.Where(t => t.value == "C").Count();
            }
            TimeSpan timeDifference = DateTime.Now - input.StartTime;
            int minutesDifference = (int)timeDifference.TotalMinutes;
            int secondsDifference = (int)timeDifference.TotalSeconds % 60;
            string formattedTime = $"{minutesDifference}分{secondsDifference}秒";

            entity.Inviter = input.Inviter;
            entity.UesTime = formattedTime;// Extensions.DiffTimes(input.StartTime, DateTime.Now);
            entity.Option_Sum_Total = list.Count;
            entity.Data = list.ToJson();
            var Id = await _statisticrepository.InsertAndGetIdAsync(entity);
            if (Id == Guid.Empty)
            {
                return new JsonResultModel() { Message = $"操作失败", Success = false };
            }
            return new JsonResultModel() { Message = $"操作成功", Success = true };
        }
        #endregion

        #region DISC性格测试

        /// <summary>
        /// 获取分页-4P测试题库信息
        /// </summary>
        /// <param name="input">查询实体</param>
        /// <returns></returns>
       // [DataPermission("DISC测试题库信息")]
        public async Task<PagedResultDto<OA4PQuestionDto>> GetPageDISCQuestionList(PagedOA4PQuestionResultRequestDto input)
        {
            var query = _repository.GetAll()
               .WhereIf(!string.IsNullOrEmpty(input.Type), q => q.Type.Equals(input.Type))
               .WhereIf(input.StartDate.HasValue && input.EndDate.HasValue, q => q.CreationTime >= input.StartDate && q.CreationTime <= input.EndDate)
               .WhereIf(!string.IsNullOrWhiteSpace(input.keyword), q => q.QuestionsTitle.Contains(input.keyword) || q.Option_1.Contains(input.keyword) || q.Option_2.Contains(input.keyword) || q.Option_3.Contains(input.keyword) || q.Option_4.Contains(input.keyword));

            //query = await _permissionAppServiceService.CreateDataFilteredQuery<OA4PQuestion>(query, this.GetRequestPath());

            var resultCount = await query.CountAsync();
            var list = query.OrderBy(x => x.SortCode).PageBy(input).ToList();
            var ResultList = ObjectMapper.Map<List<OA4PQuestionDto>>(list);

            return new PagedResultDto<OA4PQuestionDto>(resultCount, ResultList);
        }
        /// <summary>
        /// 获取分页-4P测试提交统计
        /// </summary>
        /// <param name="input">查询实体</param>
        /// <returns></returns>
        //[DataPermission("DISC测试提交统计")]
        public async Task<PagedResultDto<OA4PStatisticDto>> GetPageDISCStatisticList(Paged4PStatisticResultRequestDto input)
        {
            var query = _statisticrepository.GetAll()
                .Where(q => q.Type == "DISC")
                .WhereIf(input.StartDate.HasValue && input.EndDate.HasValue, q => q.CreationTime >= input.StartDate && q.CreationTime <= input.EndDate.Value.ToString("yyyy-MM-dd").ToDate().AddSeconds(86399))
                .WhereIf(!string.IsNullOrWhiteSpace(input.keyword), q => q.PostName.Contains(input.keyword.Trim()) || q.TrueName.Contains(input.keyword.Trim()) || q.Mobile.Contains(input.keyword.Trim()))
                .OrderByDescending(x => x.CreationTime);
            var resultCount = await query.CountAsync();
            var list = query.PageBy(input).ToList();
            var ResultList = ObjectMapper.Map<List<OA4PStatisticDto>>(list);
            return new PagedResultDto<OA4PStatisticDto>(resultCount, ResultList);
        }
        #region 批量导入DISC测试题库
        /// <summary>
        /// 批量导入DISC测试题库
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        //[IgnoreAntiforgeryToken]
        //[AbpAuthorize("Pages.DISCTest.List.ImportDISC")]
        public async Task<JsonResultModel> ImportDISC()
        {
            try
            {
                //当前登录用户
                if (!ADTOSharpSession.UserId.HasValue)
                {
                    throw new UserFriendlyException("请登录.");
                }
                var httpContext = _httpContextAccessor.HttpContext;
                IFormFile formFile = httpContext.Request.Form.Files.FirstOrDefault();
                if (formFile == null)
                {
                    return new JsonResultModel() { Message = $"操作失败，没有文件上传", Success = false };
                }
                string strExtension = Path.GetExtension(formFile.FileName);
                List<OA4PQuestionDto> list = new List<OA4PQuestionDto>();
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
                    int rows = sheet.PhysicalNumberOfRows;

                    List<string> numberList = new List<string>();
                    for (int i = 0; i < rows; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        numberList.Add(row.Cells[0].ToString().Trim());
                    }
                    List<List<string>> groups = new List<List<string>>();
                    for (int i = 0; i < numberList.Count; i += 5)
                    {
                        int remainingItems = Math.Min(5, numberList.Count - i);
                        List<string> group = numberList.GetRange(i, remainingItems);
                        groups.Add(group);
                    }
                    // 打印每个组的内容
                    foreach (var group in groups)
                    {
                        OA4PQuestionDto questionDto = new OA4PQuestionDto() { SortCode = groups.IndexOf(group), EnabledMark = 1, Type = "DISC" };
                        #region 循环
                        //foreach (var item in group)
                        //{
                        //    var index = group.IndexOf(item);
                        //    switch (index)
                        //    {
                        //        case 0:
                        //            questionDto.QuestionsTitle = group[index];
                        //            break;
                        //        case 1:
                        //            questionDto.Option_1 = group[index];
                        //            break;
                        //        case 2:
                        //            questionDto.Option_2 = group[index];
                        //            break;
                        //        case 3:
                        //            questionDto.Option_3 = group[index];
                        //            break;
                        //        case 4:
                        //            questionDto.Option_4 = group[index];
                        //            break;
                        //    };
                        //}
                        #endregion
                        questionDto.QuestionsTitle = group[0];
                        questionDto.Option_1 = group[1];
                        questionDto.Option_2 = group[2];
                        questionDto.Option_3 = group[3];
                        questionDto.Option_4 = group[4];
                        list.Add(questionDto);
                    }
                }

                else
                {
                    return new JsonResultModel() { Message = $"操作失败，请检查Excel文件格式", Success = false };
                }

                if (list != null && list.Count > 0)
                {
                    var modelList = ObjectMapper.Map<List<OA4PQuestion>>(list);

                    await _repository.InsertRangeAsync(modelList);
                    //await _repository.GetDbContext().BulkInsertAsync(modelList);
                }
                return new JsonResultModel() { Message = $"操作成功", Success = true };
            }
            catch (Exception ex)
            {
                return new JsonResultModel() { Message = $"操作失败,服务器异常，请联系开发人员：" + ex.Message, Success = false };
            }
        }

        #endregion
        #endregion


        #region 导出4P测试统计记录
        /// <summary>
        /// 导出4P测试统计记录
        /// </summary>
        /// <param name="input">后端分页查询条件</param>
        /// <returns></returns>
        //[ADTOSharpAuthorize("Pages.4PTest.TestStatistical.Export")]
        [HttpPost]
        public async Task<FileResult> ExportExcel(Paged4PStatisticResultRequestDto input)
        {
           
            //查询语句
            var query = await this.GetPage4PStatisticList(input);
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.Title = "4P测试统计结果";
            excelconfig.TitleFont = "微软雅黑";
            excelconfig.TitlePoint = 25;
            excelconfig.FileName = "4P测试统计结果";
            excelconfig.IsAllSizeColumn = true;
            //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
            excelconfig.ColumnEntity = new List<ColumnModel>();
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "TrueName", ExcelColumn = "真实姓名" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "PostName", ExcelColumn = "应聘职位" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "Mobile", ExcelColumn = "手机号码" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "Inviter", ExcelColumn = "邀请人" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "Option_1_Total", ExcelColumn = "选项一个数" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "Option_2_Total", ExcelColumn = "选项二个数" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "Option_3_Total", ExcelColumn = "选项三个数" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "Option_4_Total", ExcelColumn = "选项四个数" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "Option_Sum_Total", ExcelColumn = "答题个数" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "CreationTime", ExcelColumn = "提交时间" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "UesTime", ExcelColumn = "用时" });
            var dtSource = Extensions.ListToDataTable(query.Items.ToList());
            var fileBytes = ExcelHelper.ExportDataTableMemoryStream(dtSource, excelconfig);
            var content = new System.IO.MemoryStream(fileBytes);
            content.Position = 0;

            return new FileStreamResult(content, "application/vnd.ms-excel;charset=utf-8")
            {
                FileDownloadName = "4P测试统计结果.xls"
            };
        }

        /// <summary>
        /// 导出4P测试统计记录
        /// </summary>
        /// <param name="input">后端分页查询条件</param>
        /// <returns></returns>
        //[ADTOSharpAuthorize("Pages.4PTest.TestTalentPool.Export")]
        [HttpPost]
        public async Task<FileResult> ExportTalentPoolExcel(Paged4PStatisticResultRequestDto input)
        {
            //input.MaxResultCount = int.MaxValue;
            //input.SkipCount = 0;
            //查询语句
            var query = await this.GetPage4PStatisticList(input);
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.Title = "人才库统计结果";
            excelconfig.TitleFont = "微软雅黑";
            excelconfig.TitlePoint = 25;
            excelconfig.FileName = "人才库统计结果";
            excelconfig.IsAllSizeColumn = true;
            //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
            excelconfig.ColumnEntity = new List<ColumnModel>();
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "TrueName", ExcelColumn = "真实姓名" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "Gender", ExcelColumn = "性别" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "Mobile", ExcelColumn = "联系方式" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "Education", ExcelColumn = "学历" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "PostName", ExcelColumn = "应聘岗位" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "Inviter", ExcelColumn = "邀请人" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "Option_1_Total", ExcelColumn = "选项一个数" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "Option_2_Total", ExcelColumn = "选项二个数" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "Option_3_Total", ExcelColumn = "选项三个数" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "Option_4_Total", ExcelColumn = "选项四个数" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "Option_Sum_Total", ExcelColumn = "答题个数" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "CreationTime", ExcelColumn = "提交时间" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "UesTime", ExcelColumn = "用时" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "Source", ExcelColumn = "渠道" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "IsInterviewed", ExcelColumn = "是否参与面试" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "IsPassed", ExcelColumn = "是否通过" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "InterviewRating", ExcelColumn = "面试评价" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "IsOffer", ExcelColumn = "是否Offer" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "ExpectedJoiningDate", ExcelColumn = "拟入职日期" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "ResumeFile", ExcelColumn = "电子简历" });
            var dtSource = Extensions.ListToDataTable(query.Items.ToList());
            var fileBytes = ExcelHelper.ExportDataTableMemoryStream(dtSource, excelconfig);
            var content = new System.IO.MemoryStream(fileBytes);
            content.Position = 0;

            return new FileStreamResult(content, "application/vnd.ms-excel;charset=utf-8")
            {
                FileDownloadName = "人才库统计结果.xls"
            };
        }
        #endregion



        #endregion
    }
}
