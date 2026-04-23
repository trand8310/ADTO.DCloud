using ADTO.DCloud.ApplicationForm.Abs.Dto;
using ADTO.DCloud.ApplicationForm.Abs.enums;
using ADTO.DCloud.Attendances.AttendanceTimeRules;
using ADTO.DCloud.Attendances;
using ADTO.DCloud.Authorization;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.DataAuthorizes;
using ADTO.DCloud.DataItem;
using ADTO.DCloud.DeptRoles;
using ADTO.DCloud.DeptRoles.Dto;
using ADTO.DCloud.Dto;
using ADTO.DCloud.EmployeeManager;
using ADTO.DCloud.Infrastructur;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.Messages;
using ADTO.DCloud.Migrations;
using ADTO.DCloud.WorkFlow.NodeMethod;
using ADTO.DCloud.WorkFlow.Processes;
using ADTOSharp;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Organizations;
using ADTOSharp.Reflection.Extensions;
using ADTOSharp.UI;
using Dapper;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm.Abs
{
    /// <summary>
    /// 请假申请
    /// </summary>
    [ADTOSharpAuthorize]
    public class AdtoAbsAppService : DCloudAppServiceBase, IAdtoAbsAppService
    {
        private readonly IRepository<Adto_Abs, Guid> _repository;
        private readonly IRepository<Adto_Onb, Guid> _onbrepository;
        private readonly IRepository<Adto_Out, Guid> _outrepository;
        private readonly IRepository<EmployeeInfo, Guid> _employeeRepository;
        private readonly IRepository<PublicHoliDay, Guid> _holidayRepository;
        private readonly IRepository<AttendanceTime, Guid> _attendanceTimeRepository;
        private readonly AttendanceTimeRuleAppService _timeRuleAppService;
        private readonly IApplicationFormCheckDateAppService _applicationFormAppService;
        private readonly IRepository<OrganizationUnit, Guid> _orgRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IRepository<WorkFlowProcess, Guid> _processRepository;
        private readonly IRepository<DataItemDetail, Guid> _dataitemRepository;
        private readonly DataFilterService _dataAuthorizesApp;



        public AdtoAbsAppService(IRepository<Adto_Abs, Guid> repository,
            IRepository<EmployeeInfo, Guid> employeeRepository,
             IRepository<PublicHoliDay, Guid> holidayRepository,
             IRepository<AttendanceTime, Guid> attendanceTimeRepository,
              AttendanceTimeRuleAppService timeRuleAppService,
              IRepository<Adto_Onb, Guid> onbrepository,
              IRepository<Adto_Out, Guid> outrepository,
              IApplicationFormCheckDateAppService applicationFormAppService,
              IRepository<OrganizationUnit, Guid> orgRepository,
              IRepository<User, Guid> userRepository,
              DataFilterService dataAuthorizesApp,
              IRepository<WorkFlowProcess, Guid> processRepository,
               IRepository<DataItemDetail, Guid> dataitemRepository)
        {
            _repository = repository;
            _employeeRepository = employeeRepository;
            _holidayRepository = holidayRepository;
            _attendanceTimeRepository = attendanceTimeRepository;
            _timeRuleAppService = timeRuleAppService;
            _applicationFormAppService = applicationFormAppService;
            _onbrepository = onbrepository;
            _outrepository = outrepository;
            _orgRepository = orgRepository;
            _userRepository = userRepository;
            _processRepository = processRepository;
            _dataitemRepository = dataitemRepository;
            _dataAuthorizesApp = dataAuthorizesApp;
        }

        #region 获取数据

        /// <summary>
        /// 获取请假数据列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AdtoAbsDto>> GetAllAsync(GetAdtoAbsInput input)
        {
            var query = _repository.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), q => q.Title.Contains(input.Keyword));
            var list = await query.ToListAsync();
            var listDtos = ObjectMapper.Map<List<AdtoAbsDto>>(list);
            return listDtos;
        }


        /// <summary>
        /// 获取分页列表请假数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DataAuthPermission("GetAbsPageList")]
        public async Task<PagedResultWithAuthDto<AdtoAbsDto>> GetAbsPageList(GetAdtoAbsPageInput input)
        {
            if (input.StartDate == null)
            {
                //如果日期为空，默认显示近一个月
                input.StartDate = DateTime.Now.AddMonths(-1);
            }
            //因为添加日期为年与日，否则不会包含今天数据
            if (input.EndDate != null)
            {
                input.EndDate = input.EndDate.Value.Add(new TimeSpan(23, 59, 59));
            }
            else
            {
                input.EndDate = DateTime.Now.AddDays(1);
            }
            var query = from r in this._repository.GetAll()
                        join p in this._processRepository.GetAll() on r.Id equals p.Id into p_d
                        from process in p_d.DefaultIfEmpty()// 左连接 p 允许流程不存在，表单存在的情况,人事可作废申请表
                        join user in this._userRepository.GetAll() on r.UserId equals user.Id
                        join depart in this._orgRepository.GetAll() on user.DepartmentId equals depart.Id
                        join com in this._orgRepository.GetAll() on user.CompanyId equals com.Id
                        join i in this._dataitemRepository.GetAllIncluding(p => p.Item) on r.AbsType equals i.ItemValue
                        where i.Item.ItemCode == "qjlx" && r.IsDeleted.Equals(false)
                        select new AdtoAbsDto
                        {
                            Id = r.Id,
                            Title = r.Title,
                            Remark = r.Remark,
                            UserId = r.UserId,
                            UserName = user.UserName,
                            Name = user.Name,
                            CompanyId = com.Id,
                            CompanyName = com.DisplayName,
                            DepartmentId = r.DepartmentId,
                            DepartmentName = depart.DisplayName,
                            StartDate = r.StartDate,
                            EndDate = r.EndDate,
                            AbsType = r.AbsType,
                            AbsTypeName = i.ItemName,
                            CreationTime = r.CreationTime,
                            CreatorUserId = r.CreatorUserId,
                            Days = r.Days,
                            IsFinished = process == null ? 0 : process.IsFinished ?? 0,//== 1 ? "结束" : "审核中",
                            SchemeCode = process == null ? "" : process.SchemeCode
                        };
            query = query.Where(a => (DateTime.Compare(Convert.ToDateTime(input.StartDate), a.StartDate) >= 0 && DateTime.Compare(Convert.ToDateTime(input.StartDate), a.EndDate) <= 0)
                     || (DateTime.Compare(Convert.ToDateTime(input.EndDate), a.StartDate) >= 0 && DateTime.Compare(Convert.ToDateTime(input.EndDate), a.EndDate) <= 0)
                     || (DateTime.Compare(a.StartDate, Convert.ToDateTime(input.StartDate)) >= 0 && DateTime.Compare(a.StartDate, Convert.ToDateTime(input.EndDate)) <= 0)
                     || (DateTime.Compare(a.EndDate, Convert.ToDateTime(input.StartDate)) >= 0 && DateTime.Compare(a.EndDate, Convert.ToDateTime(input.EndDate)) <= 0)
                     ).WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), p => p.UserName == input.Keyword || p.Name.Contains(input.Keyword))
                     .WhereIf(!string.IsNullOrWhiteSpace(input.Title), p => p.Title.Contains(input.Title))
                     .WhereIf(!string.IsNullOrWhiteSpace(input.CompanyId.ToString()), p => p.CompanyId == input.CompanyId)
                     .WhereIf(input.DepartmentId != null, p => p.DepartmentId == input.DepartmentId)
                     .WhereIf(!string.IsNullOrWhiteSpace(input.AbsType), p => p.AbsType == input.AbsType)
                    ;

            string permissionCode = PermissionHelper.GetPermissionCode(this.GetType().GetMethod(nameof(GetAbsPageList)));
            //数据权限
            var filteredQuery = await _dataAuthorizesApp.CreateDataAuthorizesFilteredQuery(query, permissionCode);
            query = filteredQuery.Query;
            //自定义排序
            if (!string.IsNullOrWhiteSpace(input.Sorting))
                query = query.OrderBy(input.Sorting);
            //获取总数
            var resultCount = await query.CountAsync();
            var taskList = query.PageBy(input).ToList();
            return new PagedResultWithAuthDto<AdtoAbsDto>(resultCount, taskList, filteredQuery.DataAuthFields);
        }
        /// <summary>
        /// 获取流程设计(依流程Id)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<AdtoAbsDto> GetAsync(EntityDto<Guid> input)
        {
            var scheme = await _repository.GetAsync(input.Id);
            return ObjectMapper.Map<AdtoAbsDto>(scheme);
        }
        /// <summary>
        /// 获取请假数据列表-计算考勤机异常
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AdtoAbsAttendanceDto>> GetAbsListAsync(GetAdtoAbsInput input)
        {
            var query = from r in this._repository.GetAll()
                        join pro in this._processRepository.GetAll() on r.Id equals pro.Id
                        join user in this._userRepository.GetAll() on r.UserId equals user.Id
                        select new AdtoAbsAttendanceDto
                        {
                            Id = r.Id,
                            UserId=r.UserId,
                            AbsType = r.AbsType,
                            Name = user.Name,
                            UserName = user.UserName,
                            CompanyId = r.CompanyId,
                            DepartmentId = r.DepartmentId,
                            StartDate = r.StartDate,
                            EndDate = r.EndDate,
                            Days = r.Days,
                            FlowStatus = pro.IsFinished ?? 0
                        };
            query = query.Where(a => (a.StartDate.Date >= input.StartDate && a.StartDate.Date <= input.EndDate) ||
        (a.EndDate.Date >= input.StartDate && a.EndDate.Date <= input.EndDate) ||
        (input.StartDate >= a.StartDate.Date && input.StartDate <= a.EndDate.Date) ||
        (input.EndDate >= a.StartDate.Date && input.EndDate <= a.EndDate.Date))
                .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Name.Contains(input.Keyword) || x.UserName.Equals(input.Keyword));

            var list = await query.ToListAsync();
            var listDtos = ObjectMapper.Map<List<AdtoAbsAttendanceDto>>(list);
            return listDtos;
        }

        #endregion

        #region 提交数据
        /// <summary>
        /// 新增请假
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HiddenApi]
        public async Task<AdtoAbsDto> CreateAsync(CreateAdtoAbsDto input)
        {
            try
            {
                var dto = ObjectMapper.Map<Adto_Abs>(input);
                await _repository.InsertAsync(dto);
                CurrentUnitOfWork.SaveChanges();
                return ObjectMapper.Map<AdtoAbsDto>(dto);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        /// <summary>
        /// 修改请假
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HiddenApi]
        public async Task<AdtoAbsDto> UpdateAsync(UpdateAdtoAbsDto input)
        {
            try
            {
                var entity = await _repository.GetAsync(input.Id);
                ObjectMapper.Map(input, entity);
                await _repository.UpdateAsync(entity);
                return await GetAsync(new EntityDto<Guid>() { Id = entity.Id });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        /// <summary>
        /// 删除请假
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteAsync(EntityDto<Guid> input)
        {
            await _repository.DeleteAsync(input.Id);

        }/// <summary>
         /// 执行查询
         /// </summary>
         /// <param name="id"></param>
         /// <returns></returns>
        public async Task Execute(Guid id)
        {
            await this.GetAsync(new EntityDto<Guid>() { Id = id });
        }
        /// <summary>
        /// 新增流程表单
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>

        public async Task ExecuteInsert(string data)
        {
            var input = data.ToObject<WfMethodDto>();
            var dto = input.Data.ToObject<CreateAdtoAbsDto>();
            //表单验证
            await AbsCheckValidity(new AbsCheckValidityDto() { EndDate = dto.EndDate, StartDate = dto.StartDate, AbsType = dto.AbsType.ToInt(), Day = dto.Days.ToDouble(), Id = dto.Id, UserId = dto.UserId });
            //表单提交
            await this.CreateAsync(dto);
        }

        /// <summary>
        /// 修改流程表单
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task ExecuteUpdate(string data)
        {
            var input = data.ToObject<WfMethodDto>();
            var dto = input.Data.ToObject<UpdateAdtoAbsDto>();
            //表单验证
            //await AbsCheckValidity(new AbsCheckValidityDto() { EndDate = dto.EndDate, StartDate = dto.StartDate, AbsType = dto.AbsType.ToInt(), Day = dto.Days.ToDouble(), Id = dto.Id, UserId = dto.UserId });
            //表单提交
            await this.UpdateAsync(dto);
        }
        /// <summary>
        /// 删除表单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task ExecuteDelete(Guid id)
        {
            await this.DeleteAsync(new EntityDto<Guid>() { Id = id });
        }

        /// <summary>
        /// 修改请假
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        [ADTOSharpAuthorize(PermissionNames.Pages_Workflow_ApplicationForm_Abs_Edit)]
        public async Task<AdtoAbsDto> UpdateAbsAsync(UpdateAdtoAbsDto input)
        {
            //表单验证
            await AbsCheckValidity(new AbsCheckValidityDto() { EndDate = input.EndDate, StartDate = input.StartDate, AbsType = input.AbsType.ToInt(), Day = input.Days.ToDouble(), Id = input.Id, UserId = input.UserId });
            return await this.UpdateAsync(input);
        }
        #endregion

        #region  请假单-请假类型年假处理
        /// <summary>
        /// 获取请假类型的，年假的对应信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetFromObjectAsync(GeFromObjectInput data)
        {
            Dictionary<string, object> result = new Dictionary<string, object>() { };

            string executionName = "other";
            if (data.Data.Count <= 0)
            {
                result.Add("absTypeText", L("RequestParameterIsEmpty"));//请求参数为空
                return new JsonResult(new { executionName = executionName, Data = result });
            }
            var dict = data.Data.ToDictionary(p => p.Name, p => p.Value);
            string absType = dict.GetValueOrDefault("absType");
            DateTime startDate = string.IsNullOrWhiteSpace(dict.GetValueOrDefault("StartDate")) || dict.GetValueOrDefault("StartDate") == "undefined" ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0) : dict.GetValueOrDefault("StartDate").ToDate();
            DateTime endDate = string.IsNullOrWhiteSpace(dict.GetValueOrDefault("EndDate")) || dict.GetValueOrDefault("StartDate") == "undefined" ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 30, 0) : dict.GetValueOrDefault("EndDate").ToDate();

            switch (absType)
            {
                case "3":
                    executionName = "nianjia";
                    var employee = await _employeeRepository.FirstOrDefaultAsync(q => q.UserId.Equals(ADTOSharpSession.UserId));
                    if (employee == null || !employee.InJobDate.HasValue)
                    {
                        //您的入职时间为空！
                        result.Add("absTypeText", L("YourStartDateIsBlank"));
                        return new JsonResult(new { executionName = executionName, Data = result });
                    }
                    StringBuilder text = new StringBuilder();
                    //入职时间
                    var _jobindate = employee.InJobDate.Value;
                    double days = 0;
                    var isyear = false;
                    var year = startDate.Year - _jobindate.Year;

                    double otherday = 0;
                    //工作年限 
                    int _workingYears = startDate.Year - _jobindate.Year;

                    //入职月份的次月开始计算，+1是指包括自己
                    int _monthCount = (12 - _jobindate.Month);
                    //如果当前年份小于等于
                    if ((_jobindate.Year == (startDate.Year - 1) || _jobindate.Year == startDate.Year) && startDate.Month <= _jobindate.Month)
                    {
                        //本人于{0}入职,年假区间为入职满一年的次月1日开始，当前没有可休年假！
                        result.Add("absTypeText", L("ThereIsCurrentlyNoAnnualLeaveAvailable", _jobindate.ToString("yyyy年MM月dd日")));
                        return new JsonResult(new { executionName = executionName, Data = result });
                    }
                    isyear = (startDate.Month - _jobindate.Month) <= 0 && _jobindate.Year != (startDate.Year - 1) ? true : false;
                    //开始时间
                    var _startime = isyear && _jobindate.Month < 12 ? startDate.AddYears(-1) : startDate;
                    //结束时间
                    var _endtime = isyear ? _jobindate.AddYears(year) : _jobindate.AddYears(year + 1);
                    //年假开始月份
                    int _starmonth = _jobindate.AddMonths(1).Month;
                    //年假结束月份
                    int _endmonth = _jobindate.Month;
                    //2014年1月1日前入职的员工，年假区间是每年的1月1日-12月31日
                    if (_jobindate.Year < 2014)
                    {
                        _startime = new DateTime(startDate.Year, 1, 1);
                        _endtime = new DateTime(startDate.Year, 12, 31);
                        _starmonth = 1;
                        _endmonth = 12;
                    }
                    var annualleave = await this.GetAnnualLeaveDay(_jobindate, startDate);
                    //入职年限满10年或者是20年,只有2014年1月1号之前入职的用户需要进行折算
                    if (_jobindate < ("2014-01-01").ToDate() && _workingYears == 10)
                    {
                        var count = Convert.ToDouble(5) / Convert.ToDouble(12) * _monthCount;
                        otherday = (count - Math.Floor(count)) >= 0.5 ? (Math.Floor(count) + 0.5) : Math.Floor(count);
                    }
                    //入职年限满20年,只有2014年1月1号之前入职的用户需要进行折算
                    else if (_jobindate < ("2014-01-01").ToDate() && _workingYears == 20)
                    {
                        var count = Convert.ToDouble(5) / Convert.ToDouble(12) * _monthCount;
                        otherday = (count - Math.Floor(count)) >= 0.5 ? (Math.Floor(count) + 0.5) : Math.Floor(count);
                    }
                    var defaultDay = 0;
                    if (startDate >= new DateTime(2026, _starmonth, 1))
                        defaultDay = 1;
                    text.Append(L("IStartedWorkingOnAndMyAnnualLeaveFallsOnFebruary1stYear", _jobindate.ToString("yyyy年MM月dd日"), _startime.Year, _starmonth));//本人于{0}入职,本次年假区间为{1}年{2}月1日-
                    text.Append(L("YearMonthDayCanEnjoyDaysOfAnnualLeaveExcludingDaysAlreadyTakenDuringTheSpringFestival", _endtime.Year, _endmonth, DateTime.DaysInMonth(_endtime.Year, _endmonth), annualleave, (2 - defaultDay)));//{0}年{1}月{2}日，可享受年假{3}天（除去春节已休的{4}天），
                    //其他年假大于0.5天表示10年或者是20年计算出的年假
                    if (otherday >= 0.5)
                        text.Append(L("AmongTheseDaysOfAnnualLeaveCanOnlyBeRequestedAndTakenDuringThePeriodFrom", otherday, _startime.Year, _jobindate.AddMonths(1).Month, _startime.Year));//其中{0}天年假需在{1}年{2}月1日~{3}年12月31日方可申请并休完。

                    //获取已请假数据
                    var edtime = new DateTime(_endtime.Year, _endmonth, DateTime.DaysInMonth(_endtime.Year, _endmonth)).AddSeconds(86399);
                    var sbsInfos = _repository.GetAll()
               .Where(p => p.UserId == ADTOSharpSession.UserId)
               .Where(p => p.AbsType == "3")
               .Where(p => p.StartDate >= Convert.ToDateTime(_startime.Year + "-" + _starmonth + "-1"))
               .Where(p => p.EndDate <= edtime).ToList();
                    foreach (var item in sbsInfos)
                    {
                        //isyear为true表示年份从去年开始，月份要大于入职时间的月份
                        //days += item.Days;
                        double day = await this.GetDaysNumMethod(employee, item.StartDate, item.EndDate, Convert.ToInt32(item.AbsType));
                        days += day;
                        //判断开始时间是否等于结束时间
                        if (item.StartDate.ToDate == item.EndDate.ToDate)
                        {
                            //{0}已休{1}天，
                            text.Append(L("HasTakenDayOff", item.StartDate.ToString("yyyy年MM月dd日"), day));
                        }
                        else
                        {
                            //{0}~{1}已休{2}，
                            text.Append(L("LeaveTakenFromMonthToMonthNumberDays", item.StartDate.ToString("yyyy年MM月dd日"), item.EndDate.ToString("yyyy年MM月dd日"), day));
                        }
                    }
                    //请假天>=3表示今年年假已用完
                    if (Convert.ToDouble(days) >= annualleave)
                    {
                        //年假已用完
                        text.Append(L("ThisYearAnnualLeaveHasBeenUsedUp"));
                    }
                    else
                    {
                        var day = (annualleave - Convert.ToDouble(days));
                        //剩余年假{0}天。此外年假不能与法定节假日和春节假连休！
                        text.Append(L("RemainingAnnualLeaveDaysInAdditionAnnualLeaveCannotBeTakenConsecutivelyWithStatutoryHolidaysAndTheSpringFestivalHoliday", day));
                    }
                    double dayCount = await this.GetDaysNumMethod(employee, startDate, endDate, absType.ToInt());
                    result.Add("DayCount", dayCount);
                    result.Add("absTypeText", $"{text.ToString()}");
                    break;
                default:
                    executionName = absType;
                    double day1 = await this.GetDaysNum(data);
                    result.Add("DayCount", day1);
                    break;
            }

            return new JsonResult(new
            {
                executionName = executionName,
                Data = result,
            });
        }

        #endregion

        #region 获取考勤时间
        /// <summary>
        /// 获取请假时间
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetAdtoAbsAttendanceDate(GeFromObjectInput input)
        {
            Dictionary<string, object> result = new Dictionary<string, object>() { };
            var dict = input.Data.ToDictionary(p => p.Name, p => p.Value);
            var season = await _timeRuleAppService.GetSeason(DateTime.Now);
            int hours = 8, minutes = season == "Winter" ? 30 : 0;

            DateTime startDate = string.IsNullOrWhiteSpace(dict.GetValueOrDefault("StartDate")) || IsWrappedWithDollarSign(dict.GetValueOrDefault("StartDate")) || dict.GetValueOrDefault("StartDate") == "undefined" ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hours, minutes, 0) : dict.GetValueOrDefault("StartDate").ToDate();
            DateTime endDate = string.IsNullOrWhiteSpace(dict.GetValueOrDefault("EndDate")) || IsWrappedWithDollarSign(dict.GetValueOrDefault("StartDate")) || dict.GetValueOrDefault("EndDate") == "undefined" ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 30, 0) : dict.GetValueOrDefault("EndDate").ToDate();
            result.Add("DayCount", 1);
            int? AbsType = dict.GetValueOrDefault("AbsType").ToInt();
            result.Add("StartDate", (endDate.Date + new TimeSpan(08, 30, 0)).ToString("yyyy-MM-dd HH:mm"));
            result.Add("EndDate", (endDate.Date + new TimeSpan(17, 30, 0)).ToString("yyyy-MM-dd HH:mm"));

            if (!ADTOSharpSession.UserId.HasValue)
                return new JsonResult(new { Data = result });
            var employee = await _employeeRepository.FirstOrDefaultAsync(d => d.UserId.Equals(ADTOSharpSession.UserId));
            if (employee == null || employee.Id == Guid.Empty || employee.AttTimeRuleId == null || employee.AttTimeRuleId == Guid.Empty)
                return new JsonResult(new { Data = result });
            //season = await _timeRuleAppService.GetSeason(startDate);

            var timeRule = await _timeRuleAppService.GetAsync(new EntityDto<Guid> { Id = employee.AttTimeRuleId.Value });
            if (timeRule == null || timeRule.Id == Guid.Empty)
                return new JsonResult(new { Data = result });

            var attendanceTimeIds = timeRule.AttendanceTimeIds.ToGuidList();
            var times = await _attendanceTimeRepository.GetAll().Where(q => attendanceTimeIds.Contains(q.Id)).ToListAsync();
            foreach (var item in times)
            {
                int year = startDate.Year;
                DateTime start, end;

                if (startDate.Month >= 10) // 10、11、12月
                {
                    item.SDate = new DateTime(year, item.SDate.Month, item.SDate.Day);
                    item.EDate = new DateTime(year + 1, item.EDate.Month, item.EDate.Day);// new DateTime(year + 1, 4, 30);
                }
                else if (startDate.Month <= 4) // 1、2、3、4月
                {
                    item.SDate = new DateTime(year - 1, item.SDate.Month, item.SDate.Day);
                    item.EDate = new DateTime(year, item.EDate.Month, item.EDate.Day);
                }
            }
            var stime = times.FirstOrDefault(x => startDate >= x.SDate && startDate <= x.EDate);
            if (stime == null || stime.Id == Guid.Empty)
                return new JsonResult(new { Data = result });
            //result["StartDate"] = ((startDate.Date + time.AMInTime).ToString("yyyy-MM-dd HH:mm"));
            //result["EndDate"] = ((startDate.Date + time.PMOutTime).ToString("yyyy-MM-dd HH:mm"));

            result["DayCount"] = await GetDaysNumMethod(employee, startDate, endDate, AbsType);
            result["StartDate"] = startDate.TimeOfDay <= new TimeSpan((stime.AMOutTime.Hours - 2), stime.AMInTime.Minutes, stime.AMInTime.Seconds) ?
                    startDate.Date.Add(new TimeSpan(stime.AMInTime.Hours, stime.AMInTime.Minutes, 0)) :
                   startDate.TimeOfDay <= stime.PMInTime ?
                   startDate.Date.Add(new TimeSpan(stime.PMInTime.Hours, stime.PMInTime.Minutes, 0)) :
                    startDate.Date.Add(new TimeSpan(stime.PMInTime.Hours, stime.PMInTime.Minutes, 0));

            var etime = times.FirstOrDefault(x => endDate >= x.SDate && endDate <= x.EDate);
            if (etime == null || etime.Id == Guid.Empty)
                return new JsonResult(new { Data = result });

            result["EndDate"] = endDate.TimeOfDay >= new TimeSpan(etime.AMOutTime.Hours, etime.AMOutTime.Minutes, etime.AMOutTime.Seconds) && endDate.TimeOfDay <= etime.PMInTime ?
               endDate.Date.Add(etime.AMOutTime) : endDate.Date.Add(etime.PMOutTime);
            return new JsonResult(new { Data = result });
        }
        /// <summary>
        /// 验证字符是否包含$
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsWrappedWithDollarSign(string input)
        {
            // 如果输入为空或长度不足2，则不可能以 $ 开头和结尾
            return !string.IsNullOrEmpty(input) &&
                   input.Length >= 2 &&
                   input.StartsWith("$") &&
                   input.EndsWith("$");
        }
        #endregion

        #region 表单验证
        /// <summary>
        /// 请假申请单校验接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task AbsCheckValidity(AbsCheckValidityDto input)
        {
            if (!ADTOSharpSession.UserId.HasValue)
                throw new ADTOSharpException(L("AbnormalUserLoginStatus"));

            Guid userId = input.UserId.HasValue ? input.UserId.Value : ADTOSharpSession.UserId.Value;
            DateTime startDate = input.StartDate.Value;
            DateTime endDate = input.EndDate.Value;
            if (DateTime.Compare(endDate, startDate) <= 0)
                throw new ADTOSharpException(L("申请失败：结束日期不能小于开始日期,请检查再提交"));//申请失败：结束日期不能小于开始日期,请检查再提交
            if (input.Id == Guid.Empty || input.Id == null)
            {
                bool isdateresult = await _applicationFormAppService.IsDateValid(startDate, "Abs");
                //判断申请日期是否为上个月的考勤数据
                if (isdateresult)
                {
                    throw new ADTOSharpException(L("ApplicationFailedTheDeadlineForSubmissionHaPassed"));
                }
            }

            //验证申请时间是否有申请请假、出差、外出申请单
            await _applicationFormAppService.IsExistApplication(userId, startDate, endDate, input.Id);
            //年假天数判断
            if (input.AbsType == (int)(EnumAttStatusType.AbsType.年假))
            {
                //根据请假日期前后各加减一天来看是否国家法定节假日，如果是，则提示年不能与法定节假日同休,节前减12小时，节后加12小时
                var entime = endDate >= new DateTime(startDate.Year, startDate.Month, startDate.Day, 17, 00, 00) ? endDate.AddHours(12) : startDate.AddHours(12);
                var holidayList = await _holidayRepository.GetAll().Where(x => x.IsLegalHoliday == 1 && x.HoliDay >= startDate && x.HoliDay <= endDate).ToListAsync();
                if (holidayList.Count() > 0)
                {
                    throw new ADTOSharpException(L("申请失败：年假申请不能与国家法定节假日连休"));
                }
                var userInfo = await this._employeeRepository.FirstOrDefaultAsync(p => p.UserId.Equals(userId));
                if (userInfo == null || userInfo.InJobDate == null)
                {
                    throw new ADTOSharpException(L("申请失败：入职日期为空"));
                }
                DateTime indt = userInfo.InJobDate.ToDate();
                //DateTime tsd, esd;
                TimeSpan inTimeSpan = new TimeSpan(indt.Ticks);
                TimeSpan nowTimeSpan = new TimeSpan(input.StartDate.Value.Ticks);
                TimeSpan ts = nowTimeSpan.Subtract(inTimeSpan);
                //判断入职日期是否满1年
                if ((userInfo.InJobDate.Value.Year == (input.StartDate.Value.Year - 1) || userInfo.InJobDate.Value.Year == input.StartDate.Value.Year) && input.StartDate.Value.Month <= userInfo.InJobDate.Value.Month)
                {
                    throw new ADTOSharpException(L("年假区间为入职满一年的次月1日开始，当前没有可休年假！"));
                }
                var holiDayNum = await this.GetAnnualLeaveDay(indt, input.StartDate.Value);
                int _workingYears = input.StartDate.Value.Year - userInfo.InJobDate.Value.Year;//工作年限 
                int _monthCount = (12 - userInfo.InJobDate.Value.Month);
                double otherday = 0;
                if (userInfo.InJobDate.Value < ("2014-01-01").ToDate() && _workingYears == 10)
                {
                    var count = Convert.ToDouble(5) / Convert.ToDouble(12) * _monthCount;
                    otherday = (count - Math.Floor(count)) >= 0.5 ? (Math.Floor(count) + 0.5) : Math.Floor(count);
                }
                //入职年限满20年
                else if (userInfo.InJobDate.Value < ("2014-01-01").ToDate() && _workingYears == 20)
                {
                    var count = Convert.ToDouble(5) / Convert.ToDouble(12) * _monthCount;
                    otherday = (count - Math.Floor(count)) >= 0.5 ? (count + 0.5) : Math.Floor(count);
                }

                double num = 0.0;
                var isyear = (input.StartDate.Value.Month - indt.Month) <= 0 && indt.Year != (input.StartDate.Value.Year - 1) ? true : false;
                var year = input.StartDate.Value.Year - indt.Year;
                var _startime = isyear && indt.Month < 12 ? input.StartDate.Value.AddYears(-1) : input.StartDate.Value;//开始时间
                var _endtime = isyear ? indt.AddYears(year) : indt.AddYears(year + 1);//结束时间
                int _starmonth = indt.AddMonths(1).Month;//年假开始月份
                int _endmonth = indt.Month;//年假结束月份
                //2014年1月1日前入职的员工，年假区间是每年的1月1日-12月31日
                if (indt.Year < 2014)
                {
                    _startime = new DateTime(input.StartDate.Value.Year, 1, 1);
                    _endtime = new DateTime(input.StartDate.Value.Year, 12, 31).AddSeconds(86399);
                    _starmonth = 1;
                    _endmonth = 12;
                }
                //获取已请年假数据
                var absList = await _repository.GetAll().Where(x => x.UserId.Equals(userId) && x.AbsType == input.AbsType.ToString()
                && x.StartDate >= (_startime.Year + "-" + _starmonth + "-1").ToDate()
                && x.EndDate <= (_endtime.Year + "-" + _endmonth + "-" + DateTime.DaysInMonth(_endtime.Year, _endmonth)).ToDate()).ToListAsync();
                if (absList.Count() > 0)
                {
                    foreach (var abs in absList)
                    {
                        num += await GetDaysNumMethod(userInfo, abs.StartDate, abs.EndDate, null, true);
                    }
                    holiDayNum -= num;
                }
                if (holiDayNum <= 0)
                {
                    throw new ADTOSharpException(L("年假区间为入职满一年的次月1日开始，当前没有可休年假！"));
                }
                else
                {
                    num = 0;
                    //获取当前数据申请开始时间及结束时间总共申请
                    num = await GetDaysNumMethod(userInfo, startDate, endDate, null, true);

                    //申请天数是否大于剩余年假天数，holiDayNum为当年所有年假天数
                    if (num > holiDayNum)
                    {
                        throw new ADTOSharpException(L("您的年假天数不够本次调休!剩余年假天数:{0}天", holiDayNum));
                    }
                    else
                    {
                        //年假天数减去计算天数，得到实际年假天数
                        //计算出的年假天数只能在规定时间内使用，不能跨年，也不能提前
                        if (otherday >= 0.5)
                        {
                            var thisstartime = new DateTime(_startime.Year, indt.AddMonths(1).Month, 1);
                            var thisendtime = new DateTime(_startime.Year, 12, 31, 23, 59, 59);
                            //当前申请时间必须要在，年假可休范围之类,如果申请时间小于年假开始时间或者是申请结束时间大于年假结束时间，则不能申请此时间段的年假
                            if (startDate >= thisstartime && endDate <= thisendtime)
                            {
                                if (num > holiDayNum)
                                {
                                    throw new ADTOSharpException(L("您的年假天数不够本次休息!剩余年假天数{0}天", holiDayNum));
                                }
                                else
                                {
                                    throw new ADTOSharpException(L("申请成功,剩余年假天数为:{0}天!", (holiDayNum - num)));
                                }
                            }
                            else if (num > (holiDayNum - otherday))
                            {
                                throw new ADTOSharpException(L("您的年假天数不够本次休息，剩余年假天数{0}天，有{1}天假需在{2}年{3}月1日~{4}年12月31日方可申请并休完。",
                                    holiDayNum, otherday, _startime.Year, indt.AddMonths(1).Month, _startime.Year));
                            }
                            else
                            {
                                throw new ADTOSharpException(L("申请成功,剩余年假天数为:{0}天!", (holiDayNum - num)));
                            }
                        }
                    }
                }
            }
            //调休限制，需求提出：2025-02-05
            else if (input.AbsType == (int)(EnumAttStatusType.AbsType.调休))
            {
                //1.调休不能连续请超过2天
                //2.调休1个月内不能超过4天
                //获取请假天数
                var day = input.Day;
                if (input.Day <= 0)
                {
                    //day = this.GetDaysNum(startDate, endDate, input.AbsType).Result;
                    day = await this.GetDaysNumMethod(null, startDate, endDate, input.AbsType.ToInt());
                }
                if (day > 2)
                {
                    throw new ADTOSharpException(L("申请失败：调休申请只能连续申请2天，每月不超过4天！"));
                }
                var absList = _repository.GetAll()
                    .Where(x => x.UserId.Equals(userId) && x.AbsType == input.AbsType.ToString())
                    .Where(x => (x.StartDate >= startDate && x.StartDate <= endDate) || (x.EndDate >= startDate && x.EndDate <= endDate));
                var days = absList.Select(f => f.Days).Sum();
                if ((days + day.ToDecimal()) > 4)
                    throw new ADTOSharpException(L("申请失败：调休申请每次只能申请2天，每月不得超过4天！"));
            }
        }
        #endregion

        #region 扩展
        /// <summary>
        /// 根据用户Id和传入的时间获取年假天数
        /// </summary>
        /// <param name="inJobDate">入职时间</param>
        /// <param name="startDate">当前时间</param>
        /// <returns></returns>
        public async Task<double> GetAnnualLeaveDay(DateTime inJobDate, DateTime thisDate)
        {
            //入职时间
            var _jobindate = inJobDate;// userInfo.detail.InJobDate.Value;
            double days = 0;
            var isyear = false;
            var year = thisDate.Year - _jobindate.Year;
            //默认年假为3天，满10年或者20年，按此计算方式5/12*入职满年限的次月到年底的月份数，超过10年+5天，超过20年+10天
            double annualleave = 3;
            double otherday = 0;
            //工作年限 
            int _workingYears = thisDate.Year - _jobindate.Year;
            //入职月份的次月开始计算，+1是指包括自己
            //int _monthCount = (12 - (userInfo.detail.InJobDate.Value.Month == 12 ? _jobindate.AddMonths(1).Month : _jobindate.Month));
            int _monthCount = (12 - _jobindate.Month);
            //如果当前年份小于等于
            if ((_jobindate.Year == (thisDate.Year - 1) || _jobindate.Year == thisDate.Year) && thisDate.Month <= _jobindate.Month)
                return 0;
            isyear = (thisDate.Month - _jobindate.Month) <= 0 && _jobindate.Year != (thisDate.Year - 1) ? true : false;
            //开始时间
            var _startime = isyear && _jobindate.Month < 12 ? thisDate.AddYears(-1) : thisDate;
            //结束时间
            var _endtime = isyear ? _jobindate.AddYears(year) : _jobindate.AddYears(year + 1);
            //年假开始月份
            int _starmonth = _jobindate.AddMonths(1).Month;
            //年假结束月份
            int _endmonth = _jobindate.Month;
            //2014年1月1日前入职的员工，年假区间是每年的1月1日-12月31日
            if (_jobindate.Year < 2014)
            {
                _startime = new DateTime(thisDate.Year, 1, 1);
                _endtime = new DateTime(thisDate.Year, 12, 31);
                _starmonth = 1;
                _endmonth = 12;
            }

            //入职年限满10年
            if (_workingYears > 10 && _workingYears < 20)
            {
                //如果入职年限大于10小于20年假为10天
                annualleave = 8;
            }
            //入职年限满10年或者是20年,只有2014年1月1号之前入职的用户需要进行折算
            else if (_jobindate < ("2014-01-01").ToDate() && _workingYears == 10)
            {
                var count = Convert.ToDouble(5) / Convert.ToDouble(12) * _monthCount;
                otherday = (count - Math.Floor(count)) >= 0.5 ? (Math.Floor(count) + 0.5) : Math.Floor(count);
                annualleave = otherday + 3;
            }
            //入职年限满20年,只有2014年1月1号之前入职的用户需要进行折算
            else if (_jobindate < ("2014-01-01").ToDate() && _workingYears == 20)
            {
                var count = Convert.ToDouble(5) / Convert.ToDouble(12) * _monthCount;
                otherday = (count - Math.Floor(count)) >= 0.5 ? (Math.Floor(count) + 0.5) : Math.Floor(count);
                annualleave = otherday + 8;
            }
            else if (year == 10 && thisDate.Month >= _starmonth && _jobindate.Month != 12)
                annualleave = 8;
            else if (year == 20 && thisDate.Month >= _starmonth && _jobindate.Month != 12)
                annualleave = 13;
            //默认13天年假
            else if (_workingYears > 20)
                annualleave = 13;
            //如果当前时间大于等于入职时间，则给他默认加一天年假
            if (thisDate >= new DateTime(2026, _starmonth, 1))
            {
                annualleave += 1;
            }
            //未满1年没有年假
            if (_workingYears < 1)
                annualleave = 0;
            return annualleave;
        }
        /// <summary>
        /// 请假天数计算，前端调用
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="StartDateTime"></param>
        /// <param name="EndDateTime"></param>
        /// <param name="AbsType"></param>
        /// <param name="Flag"></param>
        /// <returns></returns>
        public async Task<double> GetDaysNum(GeFromObjectInput input)
        {
            if (!ADTOSharpSession.UserId.HasValue)
                return 1;
            Dictionary<string, object> result = new Dictionary<string, object>() { };
            var season = await _timeRuleAppService.GetSeason(DateTime.Now);
            int hours = 8, minutes = season == "Winter" ? 30 : 0;
            var dict = input.Data.ToDictionary(p => p.Name, p => p.Value);
            DateTime startDate = string.IsNullOrWhiteSpace(dict.GetValueOrDefault("StartDate")) ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hours, minutes, 0) : dict.GetValueOrDefault("StartDate").ToDate();
            DateTime endDate = string.IsNullOrWhiteSpace(dict.GetValueOrDefault("EndDate")) ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 30, 0) : dict.GetValueOrDefault("EndDate").ToDate();
            Guid? UserId = string.IsNullOrWhiteSpace(dict.GetValueOrDefault("UserId")) ? null : Guid.Parse(dict.GetValueOrDefault("UserId"));
            int? AbsType = dict.GetValueOrDefault("absType").ToInt();
            EmployeeInfo employeeInfo = null;
            if (UserId.HasValue)
            {
                employeeInfo = await this._employeeRepository.FirstOrDefaultAsync(p => p.UserId == UserId);
            }
            return await this.GetDaysNumMethod(employeeInfo, startDate, endDate, AbsType, true);
        }
        /// <summary>
        /// 根据用户，开始时间结束时间获取天数（前端不调用）
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="StartDateTime"></param>
        /// <param name="EndDateTime"></param>
        /// <param name="AbsType"></param>
        /// <param name="Flag"></param>
        /// <returns></returns>
        [HiddenApi]
        public async Task<double> GetDaysNumMethod(EmployeeInfo employee, DateTime StartDateTime, DateTime EndDateTime, int? AbsType, bool Flag = true)
        {
            if (!ADTOSharpSession.UserId.HasValue)
            {
                return 0;
            }
            Guid compnayId = Guid.Empty;
            if (employee == null || employee.Id == Guid.Empty)
                employee = await this._employeeRepository.FirstOrDefaultAsync(p => p.UserId == ADTOSharpSession.UserId);

            if (employee == null || employee.Id == Guid.Empty)
            {
                return 1;
            }
            double days = 0.0;
            //公休表
            var dayList = await _holidayRepository.GetAll().Where(q => q.HoliDay >= StartDateTime.Date && q.HoliDay <= EndDateTime.Date).ToListAsync();

            var timeRule = await _timeRuleAppService.GetAsync(new EntityDto<Guid> { Id = employee.AttTimeRuleId.Value });
            if (timeRule == null || string.IsNullOrWhiteSpace(timeRule.AttendanceTimeIds))
                throw new ADTOSharpException(L("获取天数失败，用户考勤规则不存在，请联系人资设置考勤规则！"));

            var attendanceTimeIds = timeRule.AttendanceTimeIds.ToGuidList();
            //考勤时间表
            var areaSeasonTime = await _attendanceTimeRepository.GetAll().Where(x => attendanceTimeIds.Contains(x.Id)).ToListAsync();
            //foreach (var item in areaSeasonTime)
            //{
            //    int year = StartDateTime.Year;
            //    DateTime start, end;

            //    if (StartDateTime.Month >= 10) // 10、11、12月
            //    {
            //        item.SDate = new DateTime(year, item.SDate.Month, item.SDate.Day);
            //        item.EDate = new DateTime(year+1, item.EDate.Month, item.EDate.Day);// new DateTime(year + 1, 4, 30);
            //    }
            //    else if (StartDateTime.Month <= 4) // 1、2、3、4月
            //    {
            //        item.SDate = new DateTime(year - 1, item.SDate.Month, item.SDate.Day);
            //        item.EDate = new DateTime(year, item.EDate.Month, item.EDate.Day);
            //    }
            //}
            for (DateTime dt = StartDateTime.Date; dt <= EndDateTime.Date; dt = dt.AddDays(1))
            {
                double amtime = 0, pmtime = 0;
                //AbsType:5产假
                var day = dayList.Where(d => d.HoliDay == dt.Date).FirstOrDefault();
                //判断用户打卡类型
                switch (employee.IsAttType)
                {
                    case "0"://正常打卡(双休打卡)，先判断是否产假，产假是算所有天数
                        day = (AbsType.HasValue && AbsType.Value == 5) ? dayList.FirstOrDefault(d => d.HoliDay == dt.Date) : dayList.FirstOrDefault(d => d.State == 0 && d.HoliDay == dt.Date);
                        break;
                    case "1"://固定天数打卡
                        day = dayList.FirstOrDefault(d => d.HoliDay == dt.Date);
                        break;
                    case "2"://单休打卡
                        day = dayList.FirstOrDefault(d => d.HoliDay == dt.Date);
                        break;
                    case "3"://单休打卡
                        day = (AbsType.HasValue && AbsType.Value == 5) ? dayList.FirstOrDefault(d => d.HoliDay == dt.Date) : dayList.FirstOrDefault(d => d.IsSixDay == 0 && d.HoliDay == dt.Date);
                        break;
                    case "4"://大小周
                        day = (AbsType.HasValue && AbsType.Value == 5) ? dayList.FirstOrDefault(d => d.HoliDay == dt.Date) : dayList.FirstOrDefault(d => d.SizeWeek == 0 && d.HoliDay == dt.Date);
                        break;
                }
                if (Flag)
                {
                    if (day == null)
                        continue;
                }
                //var season = await _timeRuleAppService.GetSeason(dt);

                var time = areaSeasonTime.FirstOrDefault(t => dt >= t.SDate && dt <= t.EDate);
                if (DateTime.Compare(StartDateTime, dt.Add(time.AMOutTime)) >= 0 && DateTime.Compare(StartDateTime, dt.Add(time.PMOutTime)) < 0)
                {
                    amtime = 0;
                    pmtime = 4;
                }
                else if (DateTime.Compare(EndDateTime, dt.Add(time.PMInTime)) <= 0 && DateTime.Compare(EndDateTime, dt.Add(time.AMInTime)) > 0)
                {
                    amtime = 4;
                    pmtime = 0;
                }
                else if (DateTime.Compare(StartDateTime, dt.Add(time.AMOutTime)) < 0 && DateTime.Compare(EndDateTime, dt.Add(time.PMInTime)) > 0)
                {
                    amtime = 4;
                    pmtime = 4;
                }
                days += amtime + pmtime > 5 ? 1 : amtime + pmtime > 1 && amtime + pmtime < 5 ? 0.5 : 0;
            }
            return days;
        }

        #endregion
    }
}
