using ADTO.DCloud.DataItem;
using ADTO.DCloud.EmployeeManager;
using ADTO.DCloud.Infrastructur;
using ADTO.DCloud.Training.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Organizations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace ADTO.DCloud.Training
{
    /// <summary>
    /// 培训记录统计
    /// </summary>
    //[ADTOSharpAuthorize(PermissionNames.Pages_TrainingStatistics)]
    public class TrainingArchivesCountAppService : DCloudAppServiceBase, ITrainingArchivesCountAppService
    {
        private readonly IRepository<EmployeesTrainingArchives, Guid> _employeesTrainingArchivesRepository;
        private readonly IRepository<EmployeeInfo, Guid> _employeeInfoRepository;
        private readonly IRepository<OrganizationUnit, Guid> _orgRepository;
        private readonly IRepository<ADTO.DCloud.DataItem.DataItem, Guid> _dataItemRepository;
        private readonly IRepository<DataItemDetail, Guid> _dataItemDetailRepository;
        public TrainingArchivesCountAppService(
            IRepository<EmployeesTrainingArchives, Guid> employeesTrainingArchivesRepository,
            IRepository<EmployeeInfo, Guid> employeeInfoRepository,
            IRepository<OrganizationUnit, Guid> orgRepository,
            IRepository<ADTO.DCloud.DataItem.DataItem, Guid> dataItemRepository,
            IRepository<DataItemDetail, Guid> dataItemDetailRepository
          )
        {
            _employeesTrainingArchivesRepository = employeesTrainingArchivesRepository;
            _employeeInfoRepository = employeeInfoRepository;
            _orgRepository = orgRepository;
            _dataItemRepository = dataItemRepository;
            _dataItemDetailRepository = dataItemDetailRepository;
        }



        private async Task<PagedResultDto<TrainingArchivesCountDto>> GetStatisticsPagedListAsync(PagedTrainingArchivesCountRequestDto input)
        {
            var archives = _employeesTrainingArchivesRepository.GetAll().Where(x => !x.IsDeleted);
            var users = _employeeInfoRepository.GetAll();
            var orgs = _orgRepository.GetAll();
            var companies = _orgRepository.GetAll();
            var dataItems = _dataItemRepository.GetAll();
            var dataItemDetails = _dataItemDetailRepository.GetAll();

            var postLevelQuery =
                from i in dataItems
                join d in dataItemDetails on i.Id equals d.ItemId
                where i.ItemCode == "PostLevel"
                select new
                {
                    d.ItemValue,
                    d.ItemName
                };

            var groupedQuery =
                from a in archives
                group a by new
                {
                    a.TrainingMonth,
                    a.UserId,
                    a.PostLevelId
                }
                into g
                select new
                {
                    g.Key.UserId,
                    g.Key.PostLevelId,
                    g.Key.TrainingMonth,
                    StandardLearningTime = 3,
                    LearningTimeCount = g.Sum(x => x.TrainingHour),
                    ScoreCount = g.Sum(x => x.TrainingHour) * 2,
                    ComplianceRate = g.Sum(x => x.TrainingHour) / 3m * 100m
                };

            var query =
                from t1 in groupedQuery
                join t2 in users on t1.UserId equals t2.Id into userGroup
                from t2 in userGroup.DefaultIfEmpty()

                join t3 in orgs on t2.DepartmentId equals t3.Id into deptGroup
                from t3 in deptGroup.DefaultIfEmpty()

                join t5 in companies on t2.CompanyId equals t5.Id into companyGroup
                from t5 in companyGroup.DefaultIfEmpty()

                join t4 in postLevelQuery on t1.PostLevelId.ToString() equals t4.ItemValue into postLevelGroup
                from t4 in postLevelGroup.DefaultIfEmpty()

                select new TrainingArchivesCountDto
                {
                    UserId = t1.UserId,
                    PostLevelId = t1.PostLevelId,
                    TrainingMonth = t1.TrainingMonth,

                    StandardLearningTime = t1.StandardLearningTime,
                    LearningTimeCount = t1.LearningTimeCount,
                    ScoreCount = t1.ScoreCount,
                    ComplianceRate = t1.ComplianceRate,
                    Name = t2 != null ? t2.Name : "",
                    UserName = t2 != null ? t2.UserName : "",
                    DeptName = t3 != null ? t3.DisplayName : "",
                    PostLevelName = t4 != null ? t4.ItemName : "",
                    CompanyName = t5 != null ? t5.DisplayName : "",
                    CompanyId = t5 != null ? t5.Id : (Guid?)null
                };

            query = query.WhereIf(
                !string.IsNullOrWhiteSpace(input.Keyword),
                x => x.UserName == input.Keyword || x.Name == input.Keyword
            );

            if (!string.IsNullOrWhiteSpace(input.TrainingMonth) && int.TryParse(input.TrainingMonth, out var trainingMonth))
            {
                query = query.Where(x => x.TrainingMonth == trainingMonth);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.TrainingMonth)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<TrainingArchivesCountDto>(totalCount, items);
        }





        /// <summary>
        /// 员工培训记录统计分页列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<TrainingArchivesCountDto>> GetTrainingArchivesCountPagedList(PagedTrainingArchivesCountRequestDto input)
        {
            var list = await GetStatisticsPagedListAsync(input);
            return list;
        }

        /// <summary>
        /// 导出培训记录统计
        /// </summary>
        /// <param name="input">后端分页查询条件</param>
        /// <returns></returns>
        public async Task<Byte[]> ArchivesCountExportExcel(PagedTrainingArchivesCountRequestDto input)
        {
            input.PageSize = int.MaxValue;
            input.PageNumber = 1;
            var list = await GetStatisticsPagedListAsync(input);

            Dictionary<string, string> columnNames = new Dictionary<string, string>
                {
                    { "UserName","工号"},
                    { "Name","用户"},
                    { "CompanyName","公司"},
                    { "DeptName","部门"},
                    { "PostLevelName","职级"},
                    { "StandardLearningTime","标准学时"},
                    { "LearningTimeCount","学时汇总"},
                    { "ScoreCount","学分汇总"},
                    { "ComplianceRate","达标率"},
                    { "TrainingMonth","培训月份"},
                };
            string excelTitle = "培训记录统计导出";
            var file = ExcelHelper.GetByteToExportExcel(list.Items.ToList(), columnNames, excelTitle, "Sheet1");
            return file;
        }
    }
}
