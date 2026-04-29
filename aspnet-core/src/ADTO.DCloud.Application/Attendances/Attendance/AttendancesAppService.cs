using ADTO.DCloud.ApplicationForm.Abs;
using ADTO.DCloud.ApplicationForm.Abs.Dto;
using ADTO.DCloud.ApplicationForm.Abs.enums;
using ADTO.DCloud.ApplicationForm.Att;
using ADTO.DCloud.ApplicationForm.Att.Dto;
using ADTO.DCloud.ApplicationForm.Onb;
using ADTO.DCloud.ApplicationForm.Onb.Dto;
using ADTO.DCloud.ApplicationForm.Out;
using ADTO.DCloud.ApplicationForm.Out.Dto;
using ADTO.DCloud.Attendances;
using ADTO.DCloud.Attendances.Attendance.Dto;
using ADTO.DCloud.Attendances.AttendanceLocations;
using ADTO.DCloud.Attendances.AttendanceTimeRules;
using ADTO.DCloud.Attendances.AttendanceTimes;
using ADTO.DCloud.Attendances.AttendanceTimes.Dto;
using ADTO.DCloud.Attendances.DingDingAttLogs;
using ADTO.DCloud.Attendances.DingDingAttLogs.Dto;
using ADTO.DCloud.Authorization;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.DataAuthorizes;
using ADTO.DCloud.DataItem;
using ADTO.DCloud.DataItem.Dto;
using ADTO.DCloud.Dto;
using ADTO.DCloud.EmployeeManager;
using ADTO.DCloud.EmployeeManager.Dto;
using ADTO.DCloud.Infrastructur;
using ADTO.DCloud.Infrastructure;
using ADTOSharp;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Logging;
using ADTOSharp.Organizations;
using ADTOSharp.Runtime.Caching;
using ADTOSharp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.HPSF;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Twilio.TwiML.Messaging;
using static ADTO.DCloud.ApplicationForm.Abs.enums.EnumAttStatusType;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ADTO.DCloud.Attendances.Attendance
{
    /// <summary>
    /// 考勤状态
    /// </summary>
    //[ADTOSharpAuthorize(PermissionNames.Pages_Attendances)]
    public class AttendancesAppService : DCloudAppServiceBase, IAttendancesAppService
    {
        #region ctor
        private readonly IRepository<AttendanceLog, Guid> _repository;
        private readonly IRepository<AttendanceTime, Guid> _timeRepository;
        private readonly IRepository<AttendanceLocation, Guid> _locationRepository;
        private readonly IRepository<AttendanceTimeRule, Guid> _ruleRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IRepository<OrganizationUnit, Guid> _oganizationUnitRepository;
        private readonly IRepository<PublicHoliDay, Guid> _holidayRepository;
        private readonly IRepository<CHECKINOUT, int> _chekinoutRepository;
        private readonly IRepository<USERINFO, int> _userinfoRepository;
        private readonly IRepository<EmployeeInfo, Guid> _employeeRepository;
        private readonly IRepository<AttendanceMachines, Guid> _machinesRepository;
        private readonly IRepository<DingdingUserAttLog, Guid> _dingddingRepository;
        private readonly IRepository<AttendancerMealStatistic, Guid> _mealgRepository;
        private readonly DataFilterService _dataAuthorizesApp;
        private readonly EmployeeInfoAppService _employeeInfoAppService;
        private readonly DataItemDetailAppService _itemDetailAppService;
        private readonly AttendanceTimeRuleAppService _timeRuleAppService;
        private readonly AttendanceTimeAppService _timeAppService;
        private readonly DingDingAttLogsAppService _dingdingLogAppService;
        private readonly AttendanceLocationsAppService _locationAppService;
        private readonly AdtoAbsAppService _absAppService;
        private readonly AdtoAttAppService _attAppService;
        private readonly AdtoOnbAppService _onbAppService;
        private readonly AdtoOutAppService _outAppService;
        private readonly ICacheManager _cacheManager;
        public AttendancesAppService(IRepository<AttendanceLog, Guid> repository,
            IRepository<AttendanceTime, Guid> timerepository,
            IRepository<AttendanceLocation, Guid> locationRepository,
            IRepository<AttendanceTimeRule, Guid> ruleRepository,
            IRepository<User, Guid> userRepository,
            IRepository<OrganizationUnit, Guid> oganizationUnitRepository,
            DataFilterService dataAuthorizesApp,
            EmployeeInfoAppService employeeInfoAppService,
            ICacheManager cacheManager,
            IRepository<PublicHoliDay, Guid> holidayRepository,
            DataItemDetailAppService itemDetailAppService,
            AttendanceTimeRuleAppService timeRuleAppService,
            AttendanceTimeAppService timeAppService,
            IRepository<CHECKINOUT, int> chekinoutRepository,
            IRepository<USERINFO, int> userinfoRepository,
            IRepository<AttendanceMachines, Guid> machinesRepository,
            DingDingAttLogsAppService dingdingLogAppService,
            AdtoAbsAppService absAppService,
             AdtoAttAppService attAppService,
             AdtoOnbAppService onbAppService,
             AdtoOutAppService outAppService,
             AttendanceLocationsAppService locationAppService,
              IRepository<DingdingUserAttLog, Guid> dingddingRepository,
              IRepository<EmployeeInfo, Guid> employeeRepository,
              IRepository<AttendancerMealStatistic, Guid> mealgRepository)
        {
            _repository = repository;
            _timeRepository = timerepository;
            _locationRepository = locationRepository;
            _ruleRepository = ruleRepository;
            _userRepository = userRepository;
            _oganizationUnitRepository = oganizationUnitRepository;
            _dataAuthorizesApp = dataAuthorizesApp;
            _employeeInfoAppService = employeeInfoAppService;
            _cacheManager = cacheManager;
            _holidayRepository = holidayRepository;
            _itemDetailAppService = itemDetailAppService;
            _timeRuleAppService = timeRuleAppService;
            _timeAppService = timeAppService;
            _chekinoutRepository = chekinoutRepository;
            _userinfoRepository = userinfoRepository;
            _dingdingLogAppService = dingdingLogAppService;
            _absAppService = absAppService;
            _onbAppService = onbAppService;
            _attAppService = attAppService;
            _outAppService = outAppService;
            _machinesRepository = machinesRepository;
            _locationAppService = locationAppService;
            _dingddingRepository = dingddingRepository;
            _employeeRepository = employeeRepository;
            _mealgRepository = mealgRepository;
        }
        #endregion

        #region 查询用户考勤记录信息
        /// <summary>
        /// 分页查询考勤状态
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DataAuthPermission("GetAttendancePagedList")]
        public async Task<PagedResultDto<AttendanceLogJoinDto>> GetAttendancePagedList(GetAttendancePagedInput input)
        {
            input.StartDate = input.StartDate.ToString("yyyy-MM-dd").ToDate();
            input.EndDate = input.EndDate.ToString("yyyy-MM-dd").ToDate().AddSeconds(86399);

            var query = from attendanceLog in this._repository.GetAll()
                        join u1 in _userRepository.GetAll() on attendanceLog.UserId equals u1.Id into u
                        from user in u.DefaultIfEmpty()
                        join b in _oganizationUnitRepository.GetAll() on user.CompanyId equals b.Id into c
                        from company in c.DefaultIfEmpty()
                        join userdepartment in _oganizationUnitRepository.GetAll() on user.DepartmentId equals userdepartment.Id into departmentdetail
                        from department in departmentdetail.DefaultIfEmpty()
                        select new AttendanceLogJoinDto()
                        {
                            Id = attendanceLog.Id,
                            UserId = user.Id,
                            UserName = attendanceLog.UserName,
                            Name = attendanceLog.Name,
                            AttDate = attendanceLog.AttDate,
                            CompanyId = company.Id,
                            DepartmentId = department.Id,
                            AMInTime = attendanceLog.AMInTime,
                            AMInAttTime = attendanceLog.AMInAttTime,
                            AMInType = attendanceLog.AMInType,
                            AMInLocationId = attendanceLog.AMInLocationId,
                            AMOutLocationId = attendanceLog.AMOutLocationId,
                            AMOutAttTime = attendanceLog.AMOutAttTime,
                            AMOutTime = attendanceLog.AMOutTime,
                            PMInAttTime = attendanceLog.PMInAttTime,
                            AMOutType = attendanceLog.AMOutType,
                            PMInType = attendanceLog.PMInType,
                            PMInLocationId = attendanceLog.PMInLocationId,
                            PMOutLocationId = attendanceLog.PMOutLocationId,
                            PMOutTime = attendanceLog.PMOutTime,
                            PMOutAttTime = attendanceLog.PMOutAttTime,
                            PMOutType = attendanceLog.PMOutType,
                            LocationId = attendanceLog.LocationId,
                            CompanyName = company.DisplayName,
                            DepartName = department.DisplayName
                        };

            query = query.Where(q => q.AttDate >= input.StartDate && q.AttDate <= input.EndDate)
            .WhereIf(!string.IsNullOrEmpty(input.Keyword), x => x.Name.Equals(input.Keyword) || x.UserName.Equals(input.Keyword))
            .WhereIf(input.DepartmentId.HasValue && input.DepartmentId != Guid.Empty, x => x.DepartmentId.Equals(input.DepartmentId))
            .WhereIf(input.CompanyId.HasValue, x => x.CompanyId.Equals(input.CompanyId));

            if (input.Status != null && input.Status.Count() > 0)
            {
                var stutus = input.Status;// input.Status.TrimEnd(',').Split(',');
                if (stutus.Contains("审核中"))
                {
                    string[] shz = new string[] { "外出审核中", "出差审核中", "丧假审核中", "事假审核中", "年假审核中", "产假审核中", "婚假审核中", "病假审核中", "调休审核中", "调休审核中", "上午上班忘记打卡审核中", "上午上班迟到忘记打卡审核中", "下午上班忘记打卡审核中", "下午下班忘记打卡审核中", "上午下班忘记打卡审核中", "审核中" };
                    stutus = stutus.Concat(shz).ToArray();
                }
                if (stutus.Contains("忘记打卡"))
                {
                    string[] wdk = new string[] { "上午上班忘记打卡", "上午下班忘记打卡", "下午上班忘记打卡", "下午下班忘记打卡", "上午上班忘记打卡审核中", "上午下班忘记打卡审核中", "下午上班忘记打卡审核中", "下午下班忘记打卡审核中" };
                    stutus = stutus.Concat(wdk).ToArray();
                }

                if (stutus.Contains("审核中") && stutus.Contains("假"))
                {
                    query = query.Where(x => x.AMInType.Contains("审核中") || x.PMOutType.Contains("审核中") || x.AMInType.Contains("假") || x.PMOutType.Contains("假") || stutus.Contains(x.AMInType) || stutus.Contains(x.PMOutType));
                }
                else if (stutus.Contains("审核中") && stutus.Contains("忘记打卡"))
                {
                    query = query.Where(x => x.AMInType.Contains("审核中") || x.PMOutType.Contains("审核中") || x.AMInType.Contains("忘记打卡") || x.PMOutType.Contains("忘记打卡") || stutus.Contains(x.AMInType) || stutus.Contains(x.PMOutType));
                }
                else if (stutus.Contains("新冠确诊"))
                {
                    query = query.Where(x => x.AMInType.Contains("新冠确诊") || x.PMOutType.Contains("新冠确诊") || stutus.Contains(x.AMInType) || stutus.Contains(x.PMOutType));
                }
                else
                {
                    query = query.Where(x => stutus.Contains(x.AMInType) || stutus.Contains(x.PMOutType));
                }
            }
            string permissionCode = GetCurrentPermissionCode();
            //数据权限
            var filteredQuery = await _dataAuthorizesApp.CreateDataAuthorizesFilteredQuery(query, permissionCode);
            query = filteredQuery.Query;

            if (!string.IsNullOrEmpty(input.Sorting))
                query = query.OrderBy(input.Sorting);
            var totalCount = query.Count();
            var list = await query.PageBy(input).ToListAsync();
            //办公地点
            var locationList = await _locationAppService.GetAllAsync(new AttendanceLocations.Dto.GetAttendanceLocationInput() { });
            var listDtos = list.Select(item =>
            {
                item.AMInLocation = locationList.FirstOrDefault(x => x.Id.Equals(item.AMInLocationId))?.LocationName;
                item.AMOutLocation = locationList.FirstOrDefault(x => x.Id.Equals(item.AMOutLocationId))?.LocationName;
                item.PMInLocation = locationList.FirstOrDefault(x => x.Id.Equals(item.PMInLocationId))?.LocationName;
                item.PMOutLocation = locationList.FirstOrDefault(x => x.Id.Equals(item.PMOutLocationId))?.LocationName;
                return item;
            }).ToList();

            return new PagedResultDto<AttendanceLogJoinDto>(totalCount, list);
        }
        #endregion

        #region 删除

        /// <summary>
        /// 删除考勤状态
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteAsync(EntityDto<Guid> input)
        {
            await _repository.DeleteAsync(input.Id);
        }
        #endregion

        #region 更新考勤状态  
        /// <summary>
        /// 更新考勤状态
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[DataAuthPermission(PermissionNames.Pages_Attendances_List_UpdateAttendanceLogs)]
        [HttpPost]
        public async Task UpdateAttendanceLogs(UpdateAttendanceLogsRequestDto input)
        {
            try
            {
                input.StartDate = input.StartDate.ToString("yyyy-MM-dd").ToDate();
                input.EndDate = input.EndDate.ToString("yyyy-MM-dd").ToDate().AddSeconds(86399);
                var employeeList = await _employeeInfoAppService.GetUserJobDate(new EmployeeManager.Dto.GetJobDateRequestInput() { PageSize = int.MaxValue, UserId = input.UserId, CompanyId = input.CompanyId, DepartmentId = input.DepartmentId, Keyword = input.Keyword, StartDate = input.StartDate, EndDate = input.EndDate });
                if (employeeList.Count() <= 0)
                    throw new UserFriendlyException(L("操作成功，没有找到对应用户信息"));
                //获取用户考勤状态总数
                var attendanceLogList = await GetAttendanceLogs(input);
                //判断是否选择更新，如果是只更新当前选中的员工考勤状态
                if (input.IdList != null && input.IdList.Count > 0)
                    employeeList = employeeList.Where(x => attendanceLogList.Select(d => d.UserId).Contains(x.UserId));

                //获取考勤机
                var machinesList = await GetMachinesList();
                var holidayList = await GetPublicHoliDayList(new GetPublicHoliDayRequestInput() { EndDate = input.EndDate, StartDate = input.StartDate });
                //是否考勤（0：双休打卡/1：固定打卡/2：单休打卡/3：排班打卡，4：大小周，5：不打卡）---数据字典
                //var dataitems = await _itemDetailAppService.GetItemDetailList(new DataItem.Dto.DataItemQueryDto() { ItemCode = "IsAttType" });
                //这里获取员工工号，是申请流程时根据用户Id查询对应的记录
                var employeeEneity = employeeList.FirstOrDefault(x => x.UserId.Equals(input.UserId)) ?? new GetUserIsActiveDto();

                //获取考勤时间数据列表
                var ruleTimes = await _timeRuleAppService.GetAllAsync(new AttendanceTimeRules.Dto.GetAttendanceTimeRuleInput() { });
                //获取考勤时间数据列表
                var times = await _timeAppService.GetAttendanceListAsync();
                //获取员工考-勤机打卡记录
                var attLogList = await GetAttendanceCheckTime(new GetAttendanceCheckTimeInput() { StartDate = input.StartDate, EndDate = input.EndDate, Name = employeeEneity.Name, UserName = employeeEneity.UserName });
                //获取钉钉记录
                var dingdingLogs = await _dingdingLogAppService.GetDingdingUserAttLog(new DingDingAttLogs.Dto.GetDingdingUserAttLogInput() { StartDate = input.StartDate, EndDate = input.EndDate, UserName = employeeEneity.UserName });
                //获取请假记录
                var absList = await _absAppService.GetAbsListAsync(new ApplicationForm.Abs.Dto.GetAdtoAbsInput() { EndDate = input.EndDate, StartDate = input.StartDate, Keyword = input.Keyword });
                //获取考勤异常记录
                var attList = await _attAppService.GetAttListAsync(new ApplicationForm.Att.Dto.GetAdtoAttInput() { EndDate = input.EndDate, StartDate = input.StartDate, Keyword = input.Keyword });
                //获取外出异常记录
                var outList = await _outAppService.GetOutListAsync(new ApplicationForm.Abs.Dto.GetAdtoAbsInput() { EndDate = input.EndDate, StartDate = input.StartDate, Keyword = input.Keyword });
                //获取出差异常记录
                var onbList = await _onbAppService.GetOnbListAsync(new ApplicationForm.Abs.Dto.GetAdtoAbsInput { EndDate = input.EndDate, StartDate = input.StartDate, Keyword = input.Keyword });

                List<AttendanceLog> attendanceLogsEntities = new List<AttendanceLog>();
                foreach (var holiday in holidayList)
                {
                    #region 判断是否可打卡
                    if (holiday.HoliDay > DateTime.Now)
                    {
                        Logger.Log(LogSeverity.Info, $"------------------考勤更新：考勤生成时间大于当前时间，无法生成考勤 -------------------------");
                        continue;
                    }
                    //判断如果holiday小于当前日期2个月则不生成考勤
                    if (((DateTime.Now.Year - holiday.HoliDay.Year) * 12 + DateTime.Now.Month - holiday.HoliDay.Month) >= 2)
                    {
                        Logger.Log(LogSeverity.Info, $"------------------考勤更新：判断如果holiday小于当前日期2个月则不生成考勤 -------------------------");
                        continue;
                    }
                    #endregion

                    var attendances = GetActiveTimeConfigs(times, holiday.HoliDay);
                    foreach (var user in employeeList)
                    {
                        #region 打卡条件判断

                        //如果用户不打卡
                        if (user.IsAttType.ToInt() == (int)EnumUserCheckInType.NoCheckin)
                        {
                            continue;
                        }
                        //var dataitem = dataitems.FirstOrDefault(x => x.ItemValue == user.IsAttType);
                        //是否考勤（0：双休打卡/1：固定打卡/2：单休打卡/3：排班打卡，4：大小周，5：不打卡）
                        //正常打卡，如果用户打卡类型是正常打卡,并且公休日的状态不为0（0表示正常上班，1表示休息）则跳出循环，不生成相关考勤
                        if (user.IsAttType == EnumUserCheckInType.NormalPunch.ToString() && holiday.State != 0)
                        {
                            if (holiday.HoliDay.Date != ("2024-02-07").ToDate() || holiday.HoliDay.Date != ("2024-02-15").ToDate() && holiday.HoliDay.Date != ("2024-02-16").ToDate())
                                Logger.Log(LogSeverity.Info, $"------------------考勤更新：{user.UserName}-{user.Name}正常休息，不生成考勤-------------------------");
                            continue;
                        }
                        //单休打卡（IsSixDay：为0表示正常上班）
                        if (user.IsAttType.ToInt() == (int)EnumUserCheckInType.SingleBreakPunch && holiday.IsSixDay != 0)
                        {
                            Logger.Log(LogSeverity.Info, $"------------------考勤更新：{user.UserName}-{user.Name}单休打卡，不生成考勤-------------------------");
                            continue;
                        }
                        //大小周打卡（IsSixDay：为0表示正常上班）
                        if (user.IsAttType.ToInt() == (int)EnumUserCheckInType.SizeWeek && holiday.SizeWeek != 0)
                        {
                            Logger.Log(LogSeverity.Info, $"------------------考勤更新：{user.UserName}-{user.Name}大小周打卡，不生成考勤-------------------------");
                            continue;
                        }
                        //排班打卡,如果当前日期用户排班SchedulingTypeId排班类别为1表示休息，不生成打卡
                        //var scheds = attSchedulingList.Where(q => q.UserId == user.UserId && q.SchedulingTypeId != 1 && q.AttDate.ToString("yyyy-MM-dd") == toDay.ToString("yyyy-MM-dd"));
                        if (user.IsAttType.ToInt() == (int)EnumUserCheckInType.SchedulePunch)
                        {
                            Logger.Log(LogSeverity.Info, $"------------------考勤更新：{user.UserName}-{user.Name}排班打卡，不生成考勤-------------------------");
                            continue;
                        }

                        //根据入职时间判断，如果入职时间小于当前选择的时间，不生成考勤数据
                        if (user.InJobDate.HasValue && DateTime.Compare(holiday.HoliDay, user.InJobDate.Value.Date) < 0)
                        {
                            Logger.Log(LogSeverity.Info, $"------------------考勤更新：{user.UserName}-{user.Name}入职时间小于当前选择的时间 -------------------------");
                            continue;
                        }
                        //判断离职日期是否有值
                        if (user.OutJobDate.HasValue)
                        {
                            //判断当前用户状态是否禁用，并且判断离职日期是否小于当前日期
                            if (user.IsActive == false && user.OutJobDate < holiday.HoliDay)
                            {
                                Logger.Log(LogSeverity.Info, $"------------------考勤更新：{user.UserName}-{user.Name}前用户状态已禁用，并且离职日期小于当前日期，不生成考勤 -------------------------");
                                continue;
                            }
                            else if (holiday.HoliDay > user.OutJobDate)
                            {
                                Logger.Log(LogSeverity.Info, $"------------------考勤更新：{user.UserName}-{user.Name}当前时间大于离职时间，不生成考勤 -------------------------");
                                continue;
                            }
                        }
                        #endregion

                        var attendanceLogsTempEntity = attendanceLogList.FirstOrDefault(x => x.AttDate.Date == holiday.HoliDay.Date && x.UserName.Contains(user.UserName)) ?? new AttendanceLog();
                        //考勤时间规则
                        var rules = ruleTimes.FirstOrDefault(x => x.Id.Equals(user.AttTimeRuleId));
                        var timeIds = rules.AttendanceTimeIds.Split(',').Select(part => Guid.Parse(part.Trim())).ToList();
                        attendanceLogsTempEntity.UserId = user.UserId;
                        attendanceLogsTempEntity.UserName = user.UserName;
                        attendanceLogsTempEntity.Name = user.Name;
                        attendanceLogsTempEntity.CompanyId = user.CompanyId ?? Guid.Empty;
                        attendanceLogsTempEntity.DepartmentId = user.DepartmentId;
                        attendanceLogsTempEntity.AttDate = holiday.HoliDay.Date;
                        //默认-办公地点
                        attendanceLogsTempEntity.LocationId = user.OfficeLocation;
                        attendanceLogsTempEntity.AMInLocationId = attendanceLogsTempEntity.AMInLocationId.HasValue ? attendanceLogsTempEntity.AMInLocationId : user.OfficeLocation;
                        attendanceLogsTempEntity.AMOutLocationId = attendanceLogsTempEntity.AMOutLocationId.HasValue ? attendanceLogsTempEntity.AMOutLocationId.Value : user.OfficeLocation;
                        attendanceLogsTempEntity.PMInLocationId = attendanceLogsTempEntity.PMInLocationId.HasValue ? attendanceLogsTempEntity.PMInLocationId : user.OfficeLocation;
                        attendanceLogsTempEntity.PMOutLocationId = attendanceLogsTempEntity.PMOutLocationId.HasValue ? attendanceLogsTempEntity.PMOutLocationId.Value : user.OfficeLocation;
                        attendanceLogsTempEntity.AMInType = !string.IsNullOrEmpty(attendanceLogsTempEntity.AMInType) ? attendanceLogsTempEntity.AMInType : "未打卡";
                        attendanceLogsTempEntity.AMOutType = !string.IsNullOrEmpty(attendanceLogsTempEntity.AMOutType) ? attendanceLogsTempEntity.AMOutType : "未打卡";
                        attendanceLogsTempEntity.PMInType = !string.IsNullOrEmpty(attendanceLogsTempEntity.PMInType) ? attendanceLogsTempEntity.PMInType : "未打卡";
                        attendanceLogsTempEntity.PMOutType = !string.IsNullOrEmpty(attendanceLogsTempEntity.PMOutType) ? attendanceLogsTempEntity.PMOutType : "未打卡";

                        //获取用户默认--考勤时间
                        var deafultSeanTime = attendances.FirstOrDefault(x => timeIds.Contains(x.Id)) ?? times.FirstOrDefault();
                        //先获取用户已生成的考勤记录，为了防止后期用户更改考勤规则，考勤发送变化 
                        attendanceLogsTempEntity.AMInTime = attendanceLogsTempEntity.AMInTime == TimeSpan.Zero ? deafultSeanTime.AMInTime : attendanceLogsTempEntity.AMInTime;
                        attendanceLogsTempEntity.AMOutTime = attendanceLogsTempEntity.AMOutTime == TimeSpan.Zero ? deafultSeanTime.AMOutTime : attendanceLogsTempEntity.AMOutTime;
                        attendanceLogsTempEntity.PMInTime = attendanceLogsTempEntity.PMInTime == TimeSpan.Zero ? deafultSeanTime.PMInTime : attendanceLogsTempEntity.PMInTime;
                        attendanceLogsTempEntity.PMOutTime = attendanceLogsTempEntity.PMOutTime == TimeSpan.Zero ? deafultSeanTime.PMOutTime : attendanceLogsTempEntity.PMOutTime;
                        #region  根据考勤机-打卡记录生成员工打卡时间
                        //根据考勤机-打卡记录生成员工打卡时间
                        var todayAttlogs = attLogList.Where(x => x.UserName.Equals(user.UserName) || x.Name.Equals(user.Name));
                        if (todayAttlogs.Count() > 0)
                        {
                            foreach (var toDayAttlog in todayAttlogs)
                            {
                                //获取考勤机
                                var machine = machinesList.FirstOrDefault(x => x.MachineAddress.Equals(toDayAttlog.SENSORID));
                                var ruleTime = ruleTimes.FirstOrDefault(x => x.LocationId.Equals(machine.LocationId));
                                var timeId = ruleTime?.AttendanceTimeIds?.Split(',').Select(part => Guid.Parse(part.Trim())).ToList();
                                //根据打卡位置获取-考勤时间
                                var deafultSeanTime_checkinout = attendances.FirstOrDefault(x => timeId.Contains(x.Id));

                                var item = deafultSeanTime_checkinout != null && deafultSeanTime_checkinout.Id != Guid.Empty ? deafultSeanTime_checkinout : deafultSeanTime;
                                //上午上班
                                if (DateTime.Compare(toDayAttlog.CheckTime, holiday.HoliDay.AddHours(4)) > 0 && (DateTime.Compare(toDayAttlog.CheckTime, holiday.HoliDay.AddHours(item.AMOutTime.Hours)) < 0 || DateTime.Compare(toDayAttlog.CheckTime, holiday.HoliDay.AddHours(item.AMOutTime.Hours)) < 0))
                                {
                                    if (attendanceLogsTempEntity.AMInAttTime == TimeSpan.Zero || toDayAttlog.CheckTime.TimeOfDay < attendanceLogsTempEntity.AMInAttTime)
                                    {
                                        attendanceLogsTempEntity.AMInTime = attendanceLogsTempEntity.AMInTime == TimeSpan.Zero ? item.AMInTime : attendanceLogsTempEntity.AMInTime;
                                        attendanceLogsTempEntity.AMInAttTime = toDayAttlog.CheckTime.TimeOfDay;
                                        attendanceLogsTempEntity.AMInLocationId = machine.LocationId;
                                    }
                                }
                                else if (DateTime.Compare(toDayAttlog.CheckTime, holiday.HoliDay.AddHours(item.AMOutTime.Hours - 1)) >= 0 && (DateTime.Compare(toDayAttlog.CheckTime, holiday.HoliDay.AddHours(item.PMInTime.Hours)) < 0 || DateTime.Compare(toDayAttlog.CheckTime, holiday.HoliDay.AddHours(item.PMInTime.Hours)) < 0))
                                {
                                    //上午下班
                                    if (attendanceLogsTempEntity.AMOutAttTime == TimeSpan.Zero || toDayAttlog.CheckTime.TimeOfDay > attendanceLogsTempEntity.AMOutAttTime)
                                    {
                                        attendanceLogsTempEntity.AMOutTime = attendanceLogsTempEntity.AMOutTime == TimeSpan.Zero ? item.AMOutTime : attendanceLogsTempEntity.AMOutTime;
                                        attendanceLogsTempEntity.AMOutAttTime = toDayAttlog.CheckTime.TimeOfDay;
                                        attendanceLogsTempEntity.AMOutLocationId = machine.LocationId;
                                    }
                                }
                                else if (DateTime.Compare(toDayAttlog.CheckTime, holiday.HoliDay.AddHours(attendanceLogsTempEntity.AMOutTime.Hours)) > 0 && (DateTime.Compare(toDayAttlog.CheckTime, holiday.HoliDay.AddHours(item.PMInTime.Hours + 2)) <= 0 || DateTime.Compare(toDayAttlog.CheckTime, holiday.HoliDay.AddHours(item.PMInTime.Hours + 2)) <= 0))//下午上班考勤
                                {
                                    //下午上班
                                    if (attendanceLogsTempEntity.PMInAttTime == TimeSpan.Zero || toDayAttlog.CheckTime.TimeOfDay < attendanceLogsTempEntity.PMInAttTime)
                                    {
                                        attendanceLogsTempEntity.PMInTime = attendanceLogsTempEntity.PMInTime == TimeSpan.Zero ? item.PMInTime : attendanceLogsTempEntity.PMInTime;
                                        attendanceLogsTempEntity.PMInAttTime = toDayAttlog.CheckTime.TimeOfDay;
                                        attendanceLogsTempEntity.PMInLocationId = machine.LocationId;
                                    }
                                }
                                else if (DateTime.Compare(toDayAttlog.CheckTime, holiday.HoliDay.AddHours(item.PMInTime.Hours + 2)) > 0 || DateTime.Compare(toDayAttlog.CheckTime, holiday.HoliDay.AddHours(item.PMInTime.Hours + 2)) > 0)//下午下班考勤
                                {
                                    //下午下班时间如果为空或者是当前时间大于等于下午下班时间则读取对应时间
                                    if (attendanceLogsTempEntity.PMOutAttTime == TimeSpan.Zero || toDayAttlog.CheckTime.TimeOfDay > attendanceLogsTempEntity.PMOutAttTime)
                                    {
                                        attendanceLogsTempEntity.PMOutTime = attendanceLogsTempEntity.PMOutTime == TimeSpan.Zero ? item.PMOutTime : attendanceLogsTempEntity.PMOutTime;
                                        attendanceLogsTempEntity.PMOutAttTime = toDayAttlog.CheckTime.TimeOfDay;
                                        attendanceLogsTempEntity.PMOutLocationId = machine.LocationId;
                                    }
                                }
                            }
                        }
                        #endregion

                        //请假申请单
                        var abs = absList.Where(a => a.UserId == attendanceLogsTempEntity.UserId && (DateTime.Compare(attendanceLogsTempEntity.AttDate.Date, a.StartDate.Date) >= 0 || DateTime.Compare(attendanceLogsTempEntity.AttDate.Date, a.EndDate.Date) <= 0));
                        //出差申请单
                        var onb = onbList.Where(a => a.UserId == attendanceLogsTempEntity.UserId && (DateTime.Compare(attendanceLogsTempEntity.AttDate.Date, a.StartDate.Date) >= 0 || DateTime.Compare(attendanceLogsTempEntity.AttDate.Date, a.EndDate.Date) <= 0));
                        //考勤异常单
                        var att = attList.Where(a => a.UserId == attendanceLogsTempEntity.UserId && a.AttDate >= attendanceLogsTempEntity.AttDate.Date.ToString("yyyy-MM-dd").ToDate() && a.AttDate <= attendanceLogsTempEntity.AttDate.Date.ToString("yyyy-MM-dd").ToDate().AddSeconds(86399));
                        //外出申请单
                        var attout = outList.Where(a => a.UserID == attendanceLogsTempEntity.UserId && (DateTime.Compare(attendanceLogsTempEntity.AttDate.Date, a.StartDate.Date) >= 0 || DateTime.Compare(attendanceLogsTempEntity.AttDate.Date, a.EndDate.Date) <= 0));
                        //钉钉外出数据
                        var dingdingByDateInfos = dingdingLogs.Where(q => q.Type == "DingDing" && q.Timestamp >= attendanceLogsTempEntity.AttDate.Date && q.Timestamp <= attendanceLogsTempEntity.AttDate.AddSeconds(86399) && q.UserName == attendanceLogsTempEntity.UserName);
                        //修改上午上班考勤状态
                        await UpdateAttendanceAMInStatus(attendanceLogsTempEntity, user, abs, onb, attout, att, dingdingByDateInfos);
                        //修改上午下班考勤状态
                        await UpdateAttendanceAMOutStatus(attendanceLogsTempEntity, user, abs, onb, attout, att, dingdingByDateInfos);
                        //修改下午上班时间考勤状态
                        await UpdateAttendancePMInStatus(attendanceLogsTempEntity, user, abs, onb, attout, att, dingdingByDateInfos);
                        //修改上午下班考勤状态
                        await UpdateAttendancePMOutStatus(attendanceLogsTempEntity, user, abs, onb, attout, att, dingdingByDateInfos);
                        //固定打卡，如果有其中一个不是未打卡，则正常生成考勤
                        if ((user.IsAttType.ToInt() == (int)EnumUserCheckInType.FixedPunch) && ((attendanceLogsTempEntity.AMInType != "未打卡" || attendanceLogsTempEntity.PMOutType != "未打卡")))
                        {
                            //不按结束日期开始日期，默认按开始时间月的第一天~开始时间月的最后一天
                            attendanceLogsEntities.Add(attendanceLogsTempEntity);
                        }
                        if (attendanceLogsEntities.Where(d => d.AttDate.Date == attendanceLogsTempEntity.AttDate.Date && d.UserName.Equals(attendanceLogsTempEntity.UserName)).Count() <= 0)
                        {
                            //修改2022-03-08，妇女节，公司全部女生放半天，考勤记录调整
                            if (attendanceLogsTempEntity.AttDate.ToString("MM-dd") == "03-08" && user.Gender == "0")
                            {
                                var _time = TimeSpan.Compare(attendanceLogsTempEntity.AMOutTime, attendanceLogsTempEntity.AMOutAttTime) < 0 ? attendanceLogsTempEntity.AMOutAttTime : TimeSpan.Compare(attendanceLogsTempEntity.AMOutTime, attendanceLogsTempEntity.PMInAttTime) < 0 ? attendanceLogsTempEntity.PMInAttTime : TimeSpan.Compare(attendanceLogsTempEntity.AMOutTime, attendanceLogsTempEntity.PMOutAttTime) < 0 ? attendanceLogsTempEntity.PMOutAttTime : attendanceLogsTempEntity.AMOutAttTime;
                                attendanceLogsTempEntity.PMOutType = TimeSpan.Compare(attendanceLogsTempEntity.AMOutTime, _time) < 0 ? "正常" : attendanceLogsTempEntity.PMOutType != "未打卡" ? attendanceLogsTempEntity.PMOutType : attendanceLogsTempEntity.AMOutType;
                                attendanceLogsTempEntity.PMOutAttTime = _time;
                            }
                            attendanceLogsEntities.Add(attendanceLogsTempEntity);
                            ////新增考勤异常消息通知
                            //var message = await GetAttendanceMessagePush(attendance);
                            //if (message != null)
                            //    messagePushDtos.Add(message);
                        }
                    }
                }
                //排班考勤
                if (attendanceLogsEntities.Count() <= 0)
                    throw new UserFriendlyException(L("操作成功，没有任何考勤需要更新"));
                foreach (var entity in attendanceLogsEntities)
                {
                    if (entity.Id == Guid.Empty)
                        await _repository.InsertAsync(entity);
                    else
                        await _repository.UpdateAsync(entity);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException($"操作失败，请看错误信息" + ex.Message);
            }
        }
        #endregion

        #region 修改上下班考勤状态

        #region 修改上午上班打卡状态
        /// <summary>
        /// 修改上午上班打卡状态
        /// </summary>
        /// <param name="attendanceLogsTempEntity"></param>
        /// <param name="absEntities"></param>
        /// <param name="onbEntities"></param>
        /// <param name="outEntities"></param>
        /// <param name="attEntities"></param>
        /// <param name="dingdingattlogs"></param>
        /// <param name="dataItems"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        protected async Task<AttendanceLog> UpdateAttendanceAMInStatus(AttendanceLog attendanceLogsTempEntity,
            GetUserIsActiveDto user,
            IEnumerable<AdtoAbsAttendanceDto> absEntities,
          IEnumerable<AdtoOnbAttendanceDto> onbEntities,
           IEnumerable<AdtoOutAttendanceDto> outEntities,
            IEnumerable<AdtoAttAttendanceDto> attEntities,
            IEnumerable<DingdingUserAttLogDto> dingdingattlogs
            )
        {
            var morningAbs = absEntities.FirstOrDefault(a => DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.AMInTime), a.StartDate) >= 0 && DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.AMInTime), a.EndDate) <= 0);
            var morningOnb = onbEntities.FirstOrDefault(o => DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.AMInTime), o.StartDate) >= 0 && DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.AMInTime), o.EndDate) < 0);
            var morningOut = outEntities.FirstOrDefault(o => DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.AMInTime), o.StartDate) >= 0 && DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.AMInTime), o.EndDate) < 0);
            //上午上班忘记打卡1，上午上班迟到忘记打卡2，上午下班忘记打卡3，下午上班忘记打卡4，下午下班忘记打卡5
            var morningAtt = attEntities.FirstOrDefault(a => a.AttType == ((int)AttType.上午上班忘记打卡).ToString());
            attendanceLogsTempEntity.AMInType = attendanceLogsTempEntity.AMInType == "钉钉外出" ? attendanceLogsTempEntity.AMInType : "未打卡";
            attendanceLogsTempEntity.AMOutType = attendanceLogsTempEntity.AMOutType == "钉钉外出" ? attendanceLogsTempEntity.AMOutType : "未打卡";
            //流程单据处理
            if (morningAbs != null || morningAtt != null || morningOnb != null || morningOut != null)
            {
                long AmTime = ((long)((TimeSpan)((attendanceLogsTempEntity.AttDate) - new System.DateTime(1970, 1, 1))).TotalMilliseconds);
                if (morningAbs != null && DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.AMInTime), morningAbs.StartDate) >= 0)
                {
                    if (morningAbs.FlowStatus == 1)
                    {
                        attendanceLogsTempEntity.AMInType = ((AbsType)System.Enum.Parse(typeof(AbsType), morningAbs.AbsType.ToString())).ToString();
                    }
                    else
                    {
                        attendanceLogsTempEntity.AMInType = ((AbsType)System.Enum.Parse(typeof(AbsType), morningAbs.AbsType.ToString())).ToString() + "审核中";
                        attendanceLogsTempEntity.AMInType = morningAbs.AbsType.ToInt() == (int)EnumAttStatusType.AbsType.调休 && morningAbs.FlowStatus != 1 ? "未打卡" : attendanceLogsTempEntity.AMInType;
                    }
                }
                else if (morningOnb != null && DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.AMInTime), morningOnb.StartDate) >= 0)
                {
                    if (morningOnb.FlowStatus == 1)
                    {
                        attendanceLogsTempEntity.AMInType = AttStatusType.出差.ToString();
                    }
                    else
                    {
                        attendanceLogsTempEntity.AMInType = AttStatusType.出差审核中.ToString();
                    }
                }
                else if (morningOut != null && DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.AMInTime), morningOut.StartDate) >= 0)
                {
                    if (morningOut.FlowStatus == 1)
                    {
                        attendanceLogsTempEntity.AMInType = AttStatusType.外出.ToString();
                    }
                    else
                    {
                        attendanceLogsTempEntity.AMInType = AttStatusType.外出审核中.ToString();
                    }
                }
                else if (morningAtt != null && morningAtt.AttType == ((int)AttType.上午上班忘记打卡).ToString())
                {
                    attendanceLogsTempEntity.AMInType = morningAtt.FlowStatus == 1 ? AttType.上午上班忘记打卡.ToString() : AttType.上午上班忘记打卡.ToString() + "审核中";
                }
                else
                {
                    attendanceLogsTempEntity.AMInType = AttStatusType.未打卡.ToString();
                }

            }
            else if (attendanceLogsTempEntity.AMInAttTime != TimeSpan.Zero)
            {
                if (TimeSpan.Compare(attendanceLogsTempEntity.AMInAttTime, attendanceLogsTempEntity.AMInTime) <= 0)
                {
                    attendanceLogsTempEntity.AMInType = AttStatusType.正常.ToString();
                }
                else if (TimeSpan.Compare(attendanceLogsTempEntity.AMInAttTime, attendanceLogsTempEntity.AMInTime) > 0 && TimeSpan.Compare(attendanceLogsTempEntity.AMInAttTime, Extensions.TimeSpanAdd(attendanceLogsTempEntity.AMInTime, 0, 30, 0)) <= 0)
                {
                    attendanceLogsTempEntity.AMInType = AttStatusType.迟到.ToString();
                }
                else if (TimeSpan.Compare(attendanceLogsTempEntity.AMInAttTime, attendanceLogsTempEntity.AMOutTime) < 0 && TimeSpan.Compare(attendanceLogsTempEntity.AMInAttTime, Extensions.TimeSpanAdd(attendanceLogsTempEntity.AMInTime, 0, 30, 0)) > 0)
                {
                    attendanceLogsTempEntity.AMInType = AttStatusType.旷工半天.ToString();
                }
            }
            //更新钉钉考勤状态
            else if (attendanceLogsTempEntity.AMInType == AttStatusType.未打卡.ToString() || attendanceLogsTempEntity.AMInType == AttStatusType.迟到.ToString() || attendanceLogsTempEntity.AMInType == AttStatusType.旷工半天.ToString() || attendanceLogsTempEntity.AMInType.Contains("忘记打卡"))
            {
                var _dingdinguserattlogs = dingdingattlogs.Where(d => d.Timestamp >= attendanceLogsTempEntity.AttDate && d.Timestamp <= attendanceLogsTempEntity.AttDate.AddHours(12) && (d.UserName == attendanceLogsTempEntity.UserName)).OrderBy(d => d.Timestamp);
                // 钉钉打卡类型  -默认0是钉钉签到1是考勤打卡
                var _dingdinguserattlogs1 = _dingdinguserattlogs.OrderBy(d => d.Timestamp).FirstOrDefault();
                //钉钉打卡
                if ((user.CheckInRules.ToInt() == ((int)EnumUserCheckInRules.CheckIn) && (user.CheckInRulesEffectiveDate == null || attendanceLogsTempEntity.AttDate >= user.CheckInRulesEffectiveDate)))
                {
                    if (_dingdinguserattlogs1 == null || string.IsNullOrEmpty(_dingdinguserattlogs1.UserName))
                    {
                        attendanceLogsTempEntity.PMOutType = "未打卡";
                    }
                    else if (TimeSpan.Compare(_dingdinguserattlogs1.Timestamp.TimeOfDay, attendanceLogsTempEntity.AMInTime) <= 0)
                    {
                        attendanceLogsTempEntity.AMInAttTime = _dingdinguserattlogs1.Timestamp.TimeOfDay;
                        attendanceLogsTempEntity.AMInType = AttStatusType.正常.ToString();
                    }
                    else if (TimeSpan.Compare(_dingdinguserattlogs1.Timestamp.TimeOfDay, attendanceLogsTempEntity.AMInTime) > 0 && TimeSpan.Compare(_dingdinguserattlogs1.Timestamp.TimeOfDay, Extensions.TimeSpanAdd(attendanceLogsTempEntity.AMInTime, 0, 30, 0)) <= 0)
                    {
                        attendanceLogsTempEntity.AMInAttTime = _dingdinguserattlogs1.Timestamp.TimeOfDay;
                        attendanceLogsTempEntity.AMInType = AttStatusType.迟到.ToString();
                    }
                    else if (TimeSpan.Compare(_dingdinguserattlogs1.Timestamp.TimeOfDay, attendanceLogsTempEntity.AMOutTime) < 0 && TimeSpan.Compare(_dingdinguserattlogs1.Timestamp.TimeOfDay, Extensions.TimeSpanAdd(attendanceLogsTempEntity.AMInTime, 0, 30, 0)) > 0)
                    {
                        attendanceLogsTempEntity.AMInAttTime = _dingdinguserattlogs1.Timestamp.TimeOfDay;
                        attendanceLogsTempEntity.AMInType = AttStatusType.旷工半天.ToString();
                    }
                }
                //钉钉签到
                else if (_dingdinguserattlogs.Count() > 0 && (user.CheckInRules.ToInt() == (int)EnumUserCheckInRules.SignIn || user.CheckInRulesEffectiveDate == null || attendanceLogsTempEntity.AttDate < user.CheckInRulesEffectiveDate))
                {
                    bool isnormal = attendanceLogsTempEntity.AMInType == AttStatusType.正常.ToString() ? true : false;
                    //中午12点以前是上午打卡//attendanceLogsTempEntity.AttDate.AddHours(12)
                    var _morningdingattlog = _dingdinguserattlogs.Where(d => d.Timestamp <= (attendanceLogsTempEntity.AttDate.ToString("yyyy-MM-dd " + (attendanceLogsTempEntity.AMOutTime == TimeSpan.Zero ? new TimeSpan(12, 00, 00) : attendanceLogsTempEntity.AMOutTime)).ToDate()));
                    //中午12点以后算下午
                    var _afterdingattlog = _dingdinguserattlogs.Where(d => d.Timestamp > attendanceLogsTempEntity.AttDate.ToString("yyyy-MM-dd " + (attendanceLogsTempEntity.AMOutTime == TimeSpan.Zero ? new TimeSpan(12, 00, 00) : attendanceLogsTempEntity.AMOutTime)).ToDate());
                    //编辑钉钉考勤优先级，是否高于公司打卡位置及公司考勤机打卡
                    if (_morningdingattlog.Where(q => q.IsNormal == 1).Count() > 0)
                    {
                        attendanceLogsTempEntity.AMInTime = _morningdingattlog.OrderBy(d => d.Timestamp).FirstOrDefault().Timestamp.TimeOfDay;
                        attendanceLogsTempEntity.AMInType = "钉钉签到";
                        attendanceLogsTempEntity.AMOutType = "钉钉签到";
                    }
                    //如果上午有考勤记录，默认上午钉钉外出
                    else if (_morningdingattlog.Count() > 0 && (attendanceLogsTempEntity.AMInType == AttStatusType.未打卡.ToString() || attendanceLogsTempEntity.AMInType == AttStatusType.迟到.ToString() || attendanceLogsTempEntity.AMInType == AttStatusType.旷工半天.ToString() || attendanceLogsTempEntity.AMInType.Contains("忘记打卡")))
                    {
                        var amIntype = IsAttType(attendanceLogsTempEntity.AMInType);
                        var amOutType = IsAttType(attendanceLogsTempEntity.AMOutType);
                        //判断钉钉签到地址是否跟设定考勤地址距离500米以内
                        var dingdingattlog = _morningdingattlog.FirstOrDefault();
                        //钉钉签到地址不属于公司，则判断是否指定人员考勤,敬天广场或者是东塘soho||attendanceLogsTempEntity.WorkArea == 10
                        foreach (var item in _morningdingattlog)
                        {
                            bool iscurrentarea = await this.IsCurrentArea(item.Longitude, item.Latitude, attendanceLogsTempEntity.AttDate, user.OfficeLocation);
                            //不在区域范围里面，钉钉考勤算正常
                            if (iscurrentarea == false)
                            {
                                attendanceLogsTempEntity.AMInType = IsAttType(attendanceLogsTempEntity.AMInType);
                                attendanceLogsTempEntity.AMOutType = IsAttType(attendanceLogsTempEntity.AMOutType);
                                if (attendanceLogsTempEntity.AMInType.Contains("钉钉"))
                                    attendanceLogsTempEntity.AMInAttTime = item.Timestamp.TimeOfDay;
                            }
                        }
                    }

                }

            }
            return attendanceLogsTempEntity;
        }
        #endregion

        #region 修改上午下班打卡状态
        /// <summary>
        /// 修改上午下班打卡状态
        /// </summary>
        /// <param name="attendanceLogsTempEntity"></param>
        /// <param name="absEntities"></param>
        /// <param name="onbEntities"></param>
        /// <param name="outEntities"></param>
        /// <param name="attEntities"></param>
        /// <param name="dingdingattlogs"></param>
        /// <param name="dataItems"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        protected async Task<AttendanceLog> UpdateAttendanceAMOutStatus(AttendanceLog attendanceLogsTempEntity,
            GetUserIsActiveDto user,
            IEnumerable<AdtoAbsAttendanceDto> absEntities,
          IEnumerable<AdtoOnbAttendanceDto> onbEntities,
           IEnumerable<AdtoOutAttendanceDto> outEntities,
            IEnumerable<AdtoAttAttendanceDto> attEntities,
            IEnumerable<DingdingUserAttLogDto> dingdingattlogs
            )
        {
            var morningAbs = absEntities.FirstOrDefault(a => DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.AMInTime), a.StartDate) >= 0 && DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.AMInTime), a.EndDate) <= 0);
            var morningOnb = onbEntities.FirstOrDefault(o => DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.AMInTime), o.StartDate) >= 0 && DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.AMInTime), o.EndDate) < 0);
            var morningOut = outEntities.FirstOrDefault(o => DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.AMInTime), o.StartDate) >= 0 && DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.AMInTime), o.EndDate) < 0);
            //1上午上班忘记打卡，2上午下班忘记打卡，3下午上班忘记打卡，4，下午下班忘记打卡
            var morningAtt = attEntities.Where(a => a.AttType == ((int)AttType.上午下班忘记打卡).ToString()).FirstOrDefault();

            //流程单据处理
            if (morningAbs != null || morningAtt != null || morningOnb != null || morningOut != null)
            {
                if (morningAbs != null && DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.AMOutTime), morningAbs.StartDate) >= 0 && DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.AMOutTime), morningAbs.EndDate) <= 0)
                {
                    if (morningAbs.FlowStatus == 1)
                    {
                        attendanceLogsTempEntity.AMOutType = ((AbsType)System.Enum.Parse(typeof(AbsType), morningAbs.AbsType.ToString())).ToString();
                    }
                    else
                    {
                        attendanceLogsTempEntity.AMOutType = ((AbsType)System.Enum.Parse(typeof(AbsType), morningAbs.AbsType.ToString())).ToString() + "审核中";
                        attendanceLogsTempEntity.AMOutType = morningAbs.AbsType.ToInt() == (int)EnumAttStatusType.AbsType.调休 && morningAbs.FlowStatus != 1 ? "未打卡" : attendanceLogsTempEntity.AMOutType;
                    }
                }
                else if (morningOnb != null && DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.AMOutTime), morningOnb.StartDate) > 0 && DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.AMOutTime), morningOnb.EndDate) < 0)
                {
                    if (morningOnb.FlowStatus == 1)
                    {
                        attendanceLogsTempEntity.AMOutType = AttStatusType.出差.ToString();
                    }
                    else
                    {
                        attendanceLogsTempEntity.AMOutType = AttStatusType.出差审核中.ToString();
                    }
                }
                else if (morningOut != null && DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.AMOutTime), morningOut.StartDate) > 0 && DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.AMOutTime), morningOut.EndDate) < 0)
                {
                    if (morningOut.FlowStatus == 1)
                    {
                        attendanceLogsTempEntity.AMOutType = AttStatusType.外出.ToString();
                    }
                    else
                    {
                        attendanceLogsTempEntity.AMOutType = AttStatusType.外出审核中.ToString();
                    }
                }
                else if (morningAtt != null && morningAtt.AttType.ToInt() == (int)AttType.上午下班忘记打卡)
                {
                    attendanceLogsTempEntity.AMOutType = morningAtt.FlowStatus == 1 ? AttType.上午下班忘记打卡.ToString() : AttType.上午下班忘记打卡.ToString() + "审核中";
                }
                else
                {
                    attendanceLogsTempEntity.AMOutType = AttStatusType.未打卡.ToString();
                }
            }
            else if (attendanceLogsTempEntity.AMOutAttTime != TimeSpan.Zero)
            {
                if (TimeSpan.Compare(attendanceLogsTempEntity.AMOutAttTime, attendanceLogsTempEntity.AMOutTime) >= 0)
                {
                    attendanceLogsTempEntity.AMOutType = AttStatusType.正常.ToString();
                }
                else
                {
                    attendanceLogsTempEntity.AMOutType = AttStatusType.早退.ToString();
                }
            }
            else if (dingdingattlogs != null && (user.CheckInRules.ToInt() == (int)EnumUserCheckInRules.CheckIn && user.CheckInRulesEffectiveDate.HasValue && attendanceLogsTempEntity.AttDate >= user.CheckInRulesEffectiveDate.Value))
            {
                var _dingdinguserattlogs1 = dingdingattlogs.FirstOrDefault(x => x.Timestamp.TimeOfDay.Hours > (attendanceLogsTempEntity.AMOutTime.Hours - 1.5) && x.Timestamp.TimeOfDay.Hours <= (attendanceLogsTempEntity.PMInAttTime.Hours + 1));
                //获取订单签到数据
                //var _dingdinguserattlogs1 = dingdingattlogs.OrderByDescending(d => d.AttDate).FirstOrDefault();
                if (_dingdinguserattlogs1 == null)
                {
                    attendanceLogsTempEntity.AMOutType = AttStatusType.未打卡.ToString();
                }
                //2026-02-13为26年春节放假前一天，下午放假
                else if (attendanceLogsTempEntity.AttDate == DateTime.Parse("2026-02-13") && _dingdinguserattlogs1.Timestamp.TimeOfDay >= attendanceLogsTempEntity.AMOutTime)
                {
                    attendanceLogsTempEntity.AMOutAttTime = _dingdinguserattlogs1.Timestamp.TimeOfDay;
                    attendanceLogsTempEntity.PMOutType = AttStatusType.正常.ToString();
                }
                else if (TimeSpan.Compare(_dingdinguserattlogs1.Timestamp.TimeOfDay, attendanceLogsTempEntity.AMOutTime) >= 0)
                {
                    attendanceLogsTempEntity.AMOutAttTime = _dingdinguserattlogs1.Timestamp.TimeOfDay;
                    attendanceLogsTempEntity.PMOutType = AttStatusType.正常.ToString();
                }
                else if (TimeSpan.Compare(_dingdinguserattlogs1.Timestamp.TimeOfDay, attendanceLogsTempEntity.AMOutTime) < 0 && TimeSpan.Compare(_dingdinguserattlogs1.Timestamp.TimeOfDay, Extensions.TimeSpanAdd(attendanceLogsTempEntity.AMOutTime, 0, -30, 0)) >= 0)
                {
                    attendanceLogsTempEntity.AMOutAttTime = _dingdinguserattlogs1.Timestamp.TimeOfDay;
                    attendanceLogsTempEntity.PMOutType = AttStatusType.早退.ToString();
                }
                else if (TimeSpan.Compare(_dingdinguserattlogs1.Timestamp.TimeOfDay, attendanceLogsTempEntity.AMOutTime) > 0 && TimeSpan.Compare(_dingdinguserattlogs1.Timestamp.TimeOfDay, Extensions.TimeSpanAdd(attendanceLogsTempEntity.AMOutTime, 0, -30, 0)) < 0)
                {
                    attendanceLogsTempEntity.AMOutAttTime = _dingdinguserattlogs1.Timestamp.TimeOfDay;
                    attendanceLogsTempEntity.PMOutType = AttStatusType.旷工半天.ToString();
                }
            }
            else
            {
                attendanceLogsTempEntity.AMOutType = AttStatusType.未打卡.ToString();
            }

            return attendanceLogsTempEntity;
        }
        #endregion

        #region 修改下午上班时间
        /// <summary>
        /// 修改下午上班时间
        /// </summary>
        /// <param name="attendanceLogsTempEntity"></param>
        /// <param name="user"></param>
        /// <param name="absEntities"></param>
        /// <param name="onbEntities"></param>
        /// <param name="outEntities"></param>
        /// <param name="attEntities"></param>
        /// <param name="dingdingattlogs"></param>
        /// <returns></returns>
        protected async Task<AttendanceLog> UpdateAttendancePMInStatus(AttendanceLog attendanceLogsTempEntity,
     GetUserIsActiveDto user,
     IEnumerable<AdtoAbsAttendanceDto> absEntities,
   IEnumerable<AdtoOnbAttendanceDto> onbEntities,
    IEnumerable<AdtoOutAttendanceDto> outEntities,
     IEnumerable<AdtoAttAttendanceDto> attEntities,
     IEnumerable<DingdingUserAttLogDto> dingdingattlogs
     )
        {
            var afternoonsAbs = absEntities.FirstOrDefault(a => DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.PMInTime), a.StartDate) >= 0 && DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.PMInTime), a.EndDate) <= 0);
            var afternoonsOnb = onbEntities.FirstOrDefault(o => DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.PMInTime), o.StartDate) >= 0 && DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.PMInTime), o.EndDate) <= 0);
            var afternoonsOut = outEntities.FirstOrDefault(o => DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.PMInTime), o.StartDate) >= 0 && DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.PMInTime), o.EndDate) <= 0);
            //上午上班忘记打卡1，上午上班迟到忘记打卡2，上午下班忘记打卡3，下午上班忘记打卡4，下午下班忘记打卡5
            var afternoonsAtt = attEntities.FirstOrDefault(a => a.AttType == ((int)AttType.下午上班忘记打卡).ToString());

            if (afternoonsAbs != null || afternoonsAtt != null || afternoonsOnb != null || afternoonsOut != null)
            {
                if (afternoonsAbs != null && DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.PMInTime), afternoonsAbs.StartDate) > 0 && DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.PMInTime), afternoonsAbs.EndDate) < 0)
                {
                    if (afternoonsAbs.FlowStatus == 1)
                    {
                        attendanceLogsTempEntity.PMInType = ((AbsType)System.Enum.Parse(typeof(AbsType), afternoonsAbs.AbsType.ToString())).ToString();
                    }
                    else
                    {
                        attendanceLogsTempEntity.PMInType = ((AbsType)System.Enum.Parse(typeof(AbsType), afternoonsAbs.AbsType.ToString())).ToString() + "审核中";
                        attendanceLogsTempEntity.PMInType = afternoonsAbs.AbsType.ToInt() == (int)EnumAttStatusType.AbsType.调休 && afternoonsAbs.FlowStatus != 1 ? "未打卡" : attendanceLogsTempEntity.PMInType;
                    }
                }
                else if (afternoonsOnb != null && DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.PMInTime), afternoonsOnb.StartDate) > 0 && DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.PMInTime), afternoonsOnb.EndDate) < 0)
                {
                    if (afternoonsOnb.FlowStatus == 1)
                    {
                        attendanceLogsTempEntity.PMInType = AttStatusType.出差.ToString();
                    }
                    else
                    {
                        attendanceLogsTempEntity.PMInType = AttStatusType.出差审核中.ToString();
                    }
                }
                else if (afternoonsOut != null && DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.PMInTime), afternoonsOut.StartDate) > 0 && DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.PMInTime), afternoonsOut.EndDate) < 0)
                {
                    if (afternoonsOut.FlowStatus == 1)
                    {
                        attendanceLogsTempEntity.PMInType = AttStatusType.外出.ToString();
                    }
                    else
                    {
                        attendanceLogsTempEntity.PMInType = AttStatusType.外出审核中.ToString();
                    }
                }
                else if (afternoonsAtt != null && afternoonsAtt.AttType == ((int)AttType.下午上班忘记打卡).ToString())
                {
                    attendanceLogsTempEntity.PMInType = afternoonsAtt.FlowStatus == 1 ? AttType.下午上班忘记打卡.ToString() : AttType.下午上班忘记打卡.ToString() + "审核中";
                }
                else
                {
                    attendanceLogsTempEntity.PMInType = AttStatusType.未打卡.ToString();
                }

            }
            else if (attendanceLogsTempEntity.PMInAttTime != TimeSpan.Zero)
            {
                if (TimeSpan.Compare(attendanceLogsTempEntity.PMInAttTime, attendanceLogsTempEntity.PMInTime) <= 0)
                {
                    attendanceLogsTempEntity.PMInType = AttStatusType.正常.ToString();
                }
                else if (TimeSpan.Compare(attendanceLogsTempEntity.PMInAttTime, attendanceLogsTempEntity.PMInTime) > 0 && TimeSpan.Compare(attendanceLogsTempEntity.PMOutAttTime, Extensions.TimeSpanAdd(attendanceLogsTempEntity.PMInTime, 0, 30, 0)) <= 0)
                {
                    attendanceLogsTempEntity.PMInType = AttStatusType.迟到.ToString();
                }
                else if (TimeSpan.Compare(attendanceLogsTempEntity.PMInAttTime, attendanceLogsTempEntity.PMOutTime) < 0 && TimeSpan.Compare(attendanceLogsTempEntity.PMInAttTime, Extensions.TimeSpanAdd(attendanceLogsTempEntity.PMInTime, 0, 30, 0)) > 0)
                {
                    attendanceLogsTempEntity.PMInType = AttStatusType.旷工半天.ToString();
                }
            }
            else if (dingdingattlogs != null && (user.CheckInRules.ToInt() == ((int)EnumUserCheckInRules.CheckIn) && user.CheckInRulesEffectiveDate.HasValue && attendanceLogsTempEntity.AttDate >= user.CheckInRulesEffectiveDate.Value))
            {
                var _dingdinguserattlogs1 = dingdingattlogs.OrderBy(d => d.Timestamp).FirstOrDefault() ?? new DingdingUserAttLogDto();
                if (_dingdinguserattlogs1 == null)
                {
                    attendanceLogsTempEntity.PMInType = AttStatusType.未打卡.ToString();
                }
                else if (TimeSpan.Compare(_dingdinguserattlogs1.Timestamp.TimeOfDay, attendanceLogsTempEntity.PMInTime) <= 0)
                {
                    attendanceLogsTempEntity.PMInAttTime = _dingdinguserattlogs1.Timestamp.TimeOfDay;
                    attendanceLogsTempEntity.PMInType = AttStatusType.正常.ToString();
                }
                else if (TimeSpan.Compare(_dingdinguserattlogs1.Timestamp.TimeOfDay, attendanceLogsTempEntity.PMInTime) > 0 && TimeSpan.Compare(_dingdinguserattlogs1.Timestamp.TimeOfDay, Extensions.TimeSpanAdd(attendanceLogsTempEntity.PMInTime, 0, 30, 0)) <= 0)
                {
                    attendanceLogsTempEntity.PMInAttTime = _dingdinguserattlogs1.Timestamp.TimeOfDay;
                    attendanceLogsTempEntity.PMInType = AttStatusType.迟到.ToString();
                }
                else if (TimeSpan.Compare(_dingdinguserattlogs1.Timestamp.TimeOfDay, attendanceLogsTempEntity.PMInTime) < 0 && TimeSpan.Compare(_dingdinguserattlogs1.Timestamp.TimeOfDay, Extensions.TimeSpanAdd(attendanceLogsTempEntity.PMInTime, 0, 30, 0)) > 0)
                {
                    attendanceLogsTempEntity.PMInAttTime = _dingdinguserattlogs1.Timestamp.TimeOfDay;
                    attendanceLogsTempEntity.PMInType = AttStatusType.旷工半天.ToString();
                }
            }
            else
            {
                attendanceLogsTempEntity.PMInType = AttStatusType.未打卡.ToString();
            }
            return attendanceLogsTempEntity;
        }
        #endregion

        #region 修改下午下班考勤状态
        /// <summary>
        /// 修改下午上班时间
        /// </summary>
        /// <param name="attendanceLogsTempEntity"></param>
        /// <param name="user"></param>
        /// <param name="absEntities"></param>
        /// <param name="onbEntities"></param>
        /// <param name="outEntities"></param>
        /// <param name="attEntities"></param>
        /// <param name="dingdingattlogs"></param>
        /// <returns></returns>
        protected async Task<AttendanceLog> UpdateAttendancePMOutStatus(AttendanceLog attendanceLogsTempEntity,
     GetUserIsActiveDto user,
     IEnumerable<AdtoAbsAttendanceDto> absEntities,
   IEnumerable<AdtoOnbAttendanceDto> onbEntities,
    IEnumerable<AdtoOutAttendanceDto> outEntities,
     IEnumerable<AdtoAttAttendanceDto> attEntities,
     IEnumerable<DingdingUserAttLogDto> dingdingattlogs
     )
        {
            var afternoonsAbs = absEntities.FirstOrDefault(a => DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.PMOutTime), a.StartDate) >= 0 && DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.PMOutTime), a.EndDate) <= 0);
            var afternoonsOnb = onbEntities.FirstOrDefault(o => DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.PMOutTime), o.StartDate) >= 0 && DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.PMOutTime), o.EndDate) <= 0);
            var afternoonsOut = outEntities.FirstOrDefault(o => DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.PMOutTime), o.StartDate) >= 0 && DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.PMOutTime), o.EndDate) <= 0);
            var afternoonsAtt = attEntities.FirstOrDefault(a => a.AttType.ToInt() == (int)AttType.下午下班忘记打卡);
            //此处判断是为了防止已生成的钉钉状态重置为未打卡，因为钉钉范围计算规则有限制，2025-09-15更改,
            //为什么一开始要重置为空，原因因为单据会被删除
            attendanceLogsTempEntity.PMInType = attendanceLogsTempEntity.PMInType == "钉钉外出" ? attendanceLogsTempEntity.PMInType : "未打卡";
            attendanceLogsTempEntity.PMOutType = attendanceLogsTempEntity.PMOutType == "钉钉外出" ? attendanceLogsTempEntity.PMOutType : "未打卡";
            if (afternoonsAbs != null || afternoonsAtt != null || afternoonsOnb != null || afternoonsOut != null)
            {
                if (afternoonsAbs != null && DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.PMOutTime), afternoonsAbs.EndDate) <= 0)
                {
                    if (afternoonsAbs.FlowStatus == 1)
                    {
                        attendanceLogsTempEntity.PMOutType = ((AbsType)System.Enum.Parse(typeof(AbsType), afternoonsAbs.AbsType.ToString())).ToString();
                    }
                    else
                    {
                        attendanceLogsTempEntity.PMOutType = ((AbsType)System.Enum.Parse(typeof(AbsType), afternoonsAbs.AbsType.ToString())).ToString() + "审核中";
                        attendanceLogsTempEntity.PMOutType = afternoonsAbs.AbsType.ToInt() == (int)EnumAttStatusType.AbsType.调休 && afternoonsAbs.FlowStatus != 1 ? "未打卡" : attendanceLogsTempEntity.PMOutType;
                    }
                }
                else if (afternoonsOnb != null && DateTime.Compare(attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.PMOutTime), afternoonsOnb.EndDate) <= 0)
                {
                    if (afternoonsOnb.FlowStatus == 1)
                    {
                        attendanceLogsTempEntity.PMOutType = AttStatusType.出差.ToString();
                    }
                    else
                    {
                        attendanceLogsTempEntity.PMOutType = AttStatusType.出差审核中.ToString();
                    }
                }
                else if (afternoonsOut != null && DateTime.Compare(afternoonsOut.EndDate, attendanceLogsTempEntity.AttDate.Add(attendanceLogsTempEntity.PMOutTime)) >= 0)
                {
                    if (afternoonsOut.FlowStatus == 1)
                    {
                        attendanceLogsTempEntity.PMOutType = AttStatusType.外出.ToString();
                    }
                    else
                    {
                        attendanceLogsTempEntity.PMOutType = AttStatusType.外出审核中.ToString();
                    }
                }
                else if (afternoonsAtt != null && afternoonsAtt.AttType.ToInt() == (int)AttType.下午下班忘记打卡)
                {
                    attendanceLogsTempEntity.PMOutType = afternoonsAtt.FlowStatus == 1 ? AttType.下午下班忘记打卡.ToString() : AttType.下午下班忘记打卡.ToString() + "审核中";
                }
                else
                {
                    attendanceLogsTempEntity.PMOutType = AttStatusType.未打卡.ToString();
                }

            }
            else if (attendanceLogsTempEntity.PMOutAttTime != TimeSpan.Zero)
            {
                if (TimeSpan.Compare(attendanceLogsTempEntity.PMOutAttTime, attendanceLogsTempEntity.PMOutTime) >= 0)
                {
                    attendanceLogsTempEntity.PMOutType = AttStatusType.正常.ToString();
                }
                else if (TimeSpan.Compare(attendanceLogsTempEntity.PMOutAttTime, attendanceLogsTempEntity.PMOutTime) < 0 && TimeSpan.Compare(attendanceLogsTempEntity.PMOutAttTime, Extensions.TimeSpanAdd(attendanceLogsTempEntity.PMOutTime, 0, -30, 0)) >= 0)
                {
                    attendanceLogsTempEntity.PMOutType = AttStatusType.早退.ToString();
                }
                else if (TimeSpan.Compare(attendanceLogsTempEntity.PMOutAttTime, attendanceLogsTempEntity.PMInTime) > 0 && TimeSpan.Compare(attendanceLogsTempEntity.PMOutAttTime, Extensions.TimeSpanAdd(attendanceLogsTempEntity.PMOutTime, 0, -30, 0)) < 0)
                {
                    attendanceLogsTempEntity.PMOutType = AttStatusType.旷工半天.ToString();
                }
            }
            //更新钉钉考勤状态
            else if (attendanceLogsTempEntity.PMOutType == AttStatusType.未打卡.ToString() || attendanceLogsTempEntity.PMOutType == AttStatusType.早退.ToString() || attendanceLogsTempEntity.PMOutType == AttStatusType.旷工半天.ToString() || attendanceLogsTempEntity.PMOutType.Contains("忘记打卡"))
            {
                //获取钉钉记录
                var _dingdinguserattlogs = dingdingattlogs.Where(d => d.Timestamp >= attendanceLogsTempEntity.AttDate.AddHours(12) && d.Timestamp <= attendanceLogsTempEntity.AttDate.AddSeconds(86399) && (d.UserName == attendanceLogsTempEntity.UserName)).OrderBy(d => d.Timestamp);
                //中午12点以后算下午
                var _afterdingattlog = _dingdinguserattlogs.Where(d => d.Timestamp > attendanceLogsTempEntity.AttDate.ToString("yyyy-MM-dd " + (attendanceLogsTempEntity.AMOutTime == TimeSpan.Zero ? new TimeSpan(12, 00, 00) : attendanceLogsTempEntity.AMOutTime)).ToDate());
                //钉钉打卡（CheckInType=1）、企业微信打卡（CheckInType=11）,需要计算迟到早退
                var _dingdinguserattlogs1 = _afterdingattlog.Where(d => d.Type == "1").OrderByDescending(d => d.Timestamp).FirstOrDefault() ?? new DingdingUserAttLogDto();
                if ((user.CheckInRules.ToInt() == (int)EnumUserCheckInRules.CheckIn && (user.CheckInRulesEffectiveDate == null || attendanceLogsTempEntity.AttDate >= user.CheckInRulesEffectiveDate.Value)))
                {
                    if (_dingdinguserattlogs1 == null)
                    {
                        attendanceLogsTempEntity.PMOutType = "未打卡";
                    }
                    //2026-02-13为26年春节放假前一天，下午放假
                    else if (attendanceLogsTempEntity.AttDate == DateTime.Parse("2026-02-13") && _dingdinguserattlogs1.Timestamp.TimeOfDay >= attendanceLogsTempEntity.AMOutTime)
                    {
                        attendanceLogsTempEntity.PMOutAttTime = _dingdinguserattlogs1.Timestamp.TimeOfDay;
                        attendanceLogsTempEntity.PMOutType = AttStatusType.正常.ToString();
                    }
                    else if (TimeSpan.Compare(_dingdinguserattlogs1.Timestamp.TimeOfDay, attendanceLogsTempEntity.PMOutTime) >= 0)
                    {
                        attendanceLogsTempEntity.PMOutAttTime = _dingdinguserattlogs1.Timestamp.TimeOfDay;
                        attendanceLogsTempEntity.PMOutType = AttStatusType.正常.ToString();
                    }
                    else if (TimeSpan.Compare(_dingdinguserattlogs1.Timestamp.TimeOfDay, attendanceLogsTempEntity.PMOutTime) < 0 && TimeSpan.Compare(_dingdinguserattlogs1.Timestamp.TimeOfDay, Extensions.TimeSpanAdd(attendanceLogsTempEntity.PMOutTime, 0, -30, 0)) >= 0)
                    {
                        attendanceLogsTempEntity.PMOutAttTime = _dingdinguserattlogs1.Timestamp.TimeOfDay;
                        attendanceLogsTempEntity.PMOutType = AttStatusType.早退.ToString();
                    }
                    else if (TimeSpan.Compare(_dingdinguserattlogs1.Timestamp.TimeOfDay, attendanceLogsTempEntity.PMInTime) > 0 && TimeSpan.Compare(_dingdinguserattlogs1.Timestamp.TimeOfDay, Extensions.TimeSpanAdd(attendanceLogsTempEntity.PMOutTime, 0, -30, 0)) < 0)
                    {
                        attendanceLogsTempEntity.PMOutAttTime = _dingdinguserattlogs1.Timestamp.TimeOfDay;
                        attendanceLogsTempEntity.PMOutType = AttStatusType.旷工半天.ToString();
                    }
                }
                else if (_afterdingattlog.Count() > 0 && (user.CheckInRules.ToInt() == ((int)EnumUserCheckInRules.SignIn) || user.CheckInRulesEffectiveDate == null || attendanceLogsTempEntity.AttDate < user.CheckInRulesEffectiveDate))
                {
                    //编辑钉钉考勤优先级，是否高于公司打卡位置及公司考勤机打卡
                    if (_afterdingattlog.Where(q => q.Type == "1").Count() > 0)
                    {
                        attendanceLogsTempEntity.PMInType = IsAttType(attendanceLogsTempEntity.PMInType, "钉钉签到");
                        attendanceLogsTempEntity.PMOutType = IsAttType(attendanceLogsTempEntity.PMOutType, "钉钉签到");
                    }
                    //如果下午有考勤记录，默认上午钉钉外出
                    else if (_afterdingattlog.Count() > 0 && (attendanceLogsTempEntity.PMOutType == AttStatusType.未打卡.ToString() || attendanceLogsTempEntity.PMOutType == AttStatusType.早退.ToString() || attendanceLogsTempEntity.PMOutType == AttStatusType.旷工半天.ToString() || attendanceLogsTempEntity.PMOutType.Contains("忘记打卡")))
                    {
                        //如果下午签到地址在公司则不算正常考勤
                        foreach (var item in _afterdingattlog)
                        {
                            bool iscurrentarea = await this.IsCurrentArea(item.Longitude, item.Latitude, attendanceLogsTempEntity.AttDate, user.OfficeLocation);
                            //不在区域范围里面，钉钉考勤算正常
                            if (iscurrentarea == false)
                            {
                                attendanceLogsTempEntity.PMInType = IsAttType(attendanceLogsTempEntity.PMInType);
                                attendanceLogsTempEntity.PMOutType = IsAttType(attendanceLogsTempEntity.PMOutType);
                                if (attendanceLogsTempEntity.PMOutType.Contains("钉钉"))
                                    attendanceLogsTempEntity.PMOutAttTime = item.Timestamp.TimeOfDay;
                                break;
                            }
                        }
                    }
                }
            }
            return attendanceLogsTempEntity;
        }
        #endregion

        #region 返回钉钉考勤状态
        /// <summary>
        /// 返回钉钉考勤状态
        /// </summary>
        /// <param name="type"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private string IsAttType(string type, string text = "钉钉外出")
        {
            return (type == AttStatusType.未打卡.ToString() || type == AttStatusType.外出.ToString() || type == AttStatusType.外出审核中.ToString() || type == AttStatusType.出差.ToString() || type == AttStatusType.出差审核中.ToString() || type.Contains("忘记打卡")) ? text : type;
        }
        #endregion

        #region 根据传入的经纬度判断是否在签到范围内
        /// <summary>
        /// 根据传入的经纬度判断是否在签到范围内
        /// </summary>
        /// <param name="input"></param>
        /// <returns>返回 true 表示在区域范围内，false不在区域范围里面</returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> IsCurrentArea(string Longitude, string Latitude, DateTime AttDAte, Guid? officeLocation)
        {
            //在500米范围之内算正常 Longitude:经度，Latitude：维度
            var location = await _locationAppService.GetAllAsync(new AttendanceLocations.Dto.GetAttendanceLocationInput() { });//AttendanceArea
            var item = location.Where(d => d.Id == officeLocation).FirstOrDefault();
            if (item != null && item.Id != Guid.Empty)
            {
                int defaultDistance = item.AttendanceRadius.HasValue ? item.AttendanceRadius.Value.ToInt() : 2000;
                var distance = GaodeLocation.getDistance((item.LocationLatOrLon.Split(',').Length >= 2 ? $"{item.LocationLatOrLon.Split(',')[0]},{item.LocationLatOrLon.Split(',')[1]}" : item.LocationLatOrLon), Longitude + "," + Latitude);
                if (AttDAte >= ("2024-01-01").ToDate() && distance <= defaultDistance)
                    return true;
                else if (distance <= 500)
                    return true;
            }
            return false;
        }
        #endregion

        #endregion

        #region 获取员工考勤机打卡记录和钉钉记录，一个接口返回数据
        /// <summary>
        /// 获取员工考勤机-打卡记录 和  钉钉记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DataAuthPermission("GetPagedAttendanceRecordList")]
        public async Task<PagedResultWithAuthDto<AttendanceRecordOrDingdingDto>> GetPagedAttendanceRecordOrDingdingList(GetAttendanceLogListInput input)
        {
            input.StartDate = input.StartDate.Date;
            input.EndDate = input.EndDate.Date.AddSeconds(86399);
            var attendanceQuery = from checkin in _chekinoutRepository.GetAll()
                                  join userInfo in _userinfoRepository.GetAll() on checkin.USERID equals userInfo.Id
                                  join employee in _userRepository.GetAll() on userInfo.BADGENUMBER equals employee.UserName
                                  join department in _oganizationUnitRepository.GetAll() on employee.DepartmentId equals department.Id
                                  join company in _oganizationUnitRepository.GetAll() on employee.CompanyId equals company.Id
                                  where checkin.CheckTime >= input.StartDate && checkin.CheckTime <= input.EndDate
                                  select new AttendanceRecordOrDingdingDto
                                  {
                                      Name = employee.Name,
                                      UserName = employee.UserName,
                                      AttDate = checkin.CheckTime,
                                      SENSORID = checkin.SENSORID,
                                      UserId = employee.Id,
                                      Type = "考勤机",// checkin.SENSORID,//$"考勤机-{checkin.SENSORID}",
                                      MachineAddress = "",
                                      DeptpartmentId = department.Id,
                                      DeptpartmentName = department.DisplayName,
                                      CompanyId = company.Id,
                                      CompanyName = company.DisplayName
                                  };
            attendanceQuery = attendanceQuery.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.UserName.Equals(input.Keyword) || x.Name.Contains(input.Keyword));
            var dingdingQuery = from dingding in _dingddingRepository.GetAll()
                                join user in _userRepository.GetAll() on dingding.UserName equals user.UserName
                                join department in _oganizationUnitRepository.GetAll() on user.DepartmentId equals department.Id
                                join company in _oganizationUnitRepository.GetAll() on user.CompanyId equals company.Id
                                where dingding.Timestamp >= input.StartDate && dingding.Timestamp <= input.EndDate
                                select new AttendanceRecordOrDingdingDto
                                {
                                    Name = dingding.Name,
                                    UserName = dingding.UserName,
                                    AttDate = dingding.Timestamp,
                                    SENSORID = "DingTalk",           // 钉钉来源固定标识
                                    UserId = user.Id,               // 假设有 UserId
                                    Type = "钉钉打卡",
                                    MachineAddress = dingding.DetailPlace,
                                    DeptpartmentId = department.Id,
                                    DeptpartmentName = department.DisplayName,
                                    CompanyId = company.Id,
                                    CompanyName = company.DisplayName

                                };
            dingdingQuery = dingdingQuery.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.UserName.Equals(input.Keyword) || x.Name.Contains(input.Keyword));
            // 5. 最后统一投影到 DTO（此时所有客户端表达式都在 Concat 之后）
            var resultQuery = attendanceQuery.Concat(dingdingQuery);
            //    .Select(x => new AttendanceRecordOrDingdingDto
            //{
            //    Name = x.Name,
            //    UserName = x.UserName,
            //    AttDate = x.AttDate,
            //    SENSORID = x.SENSORID,
            //    UserId = x.UserId,
            //    // 根据 SourceType 动态生成 Type 字符串
            //    Type = x.Type,
            //    MachineAddress = x.MachineAddress,
            //    DeptpartmentId = x.DeptpartmentId,
            //    DeptpartmentName = x.DeptpartmentName,
            //    CompanyId = x.CompanyId,
            //    CompanyName = x.CompanyName
            //});

            string permissionCode = GetCurrentPermissionCode();
            //数据权限
            var filteredQuery = await _dataAuthorizesApp.CreateDataAuthorizesFilteredQuery(resultQuery, permissionCode);
            var query = filteredQuery.Query;



            //if (!string.IsNullOrEmpty(input.Sorting))
            //    resultQuery = resultQuery.OrderBy(input.Sorting);
            var totalCount = query.Count();
            var list = await query.PageBy(input).ToListAsync();
            //办公地点
            var machinesDtos = await GetMachinesList();
            var listDtos = list.Select(item =>
            {
                if (item.SENSORID != "DingTalk")
                {
                    var machine = machinesDtos.FirstOrDefault(x => x.MachineAddress.Equals(item.SENSORID));
                    item.MachineAddress = machine != null ? machine.Description : "";
                }
                return item;
            }).ToList();

            return new PagedResultWithAuthDto<AttendanceRecordOrDingdingDto>(totalCount, list);

        }

        #endregion

        #region 考勤月报表

        /// <summary>
        /// 考勤月报表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DataAuthPermission("GetMonthReport")]
        public async Task<PagedResultWithAuthDto<MonthReportDto>> GetMonthReport(GetMonthReportInput input)
        {
            input.StartDate = input.StartDate.Date;
            input.EndDate = input.EndDate.Date.AddSeconds(86399);

            var query = _employeeRepository.GetAll().Where(x => x.IsAttType != EnumUserCheckInType.NoCheckin.ToString());
            query = query.WhereIf(input.CompanyId.HasValue, x => x.CompanyId.Equals(input.CompanyId))
                .WhereIf(input.DepartmentId.HasValue, x => x.DepartmentId.Equals(input.DepartmentId))
                .WhereIf(!input.Keyword.IsNullOrEmpty(), x => x.Name.Equals(input.Keyword) || x.UserName.Equals(input.Keyword))
                .Where(x => (x.IsActive && x.InJobDate <= input.EndDate && (x.OutJobDate >= input.EndDate || x.OutJobDate == null || x.InJobDate == null))
                || (x.IsActive.Equals(false) && x.InJobDate <= input.EndDate && x.OutJobDate >= input.EndDate)
                || (x.IsActive.Equals(false) && x.InJobDate <= input.EndDate && x.OutJobDate >= input.StartDate))
                ;
            //获取总数
            var resultCount = await query.CountAsync();
            var list = query.PageBy(input).ToList();
            var listDtos = list.Select(item =>
            {
                var dto = ObjectMapper.Map<MonthReportDto>(item);
                return dto;
            }).ToList();
            return new PagedResultWithAuthDto<MonthReportDto>(resultCount, listDtos);
        }

        #endregion

        #region 分组查询餐补统计数据
        /// <summary>
        /// 餐补分组统计接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DataAuthPermission("GetAttendancerMealStatistics")]
        public async Task<PagedResultDto<AttendancerMealStatisticsDto>> GetAttendancerMealStatistics(GetAttendancerMealStatisticsInput input)
        {
            input.StartDate = input.StartDate.Date;
            input.EndDate = input.EndDate.Date.AddSeconds(86399);
            var query = _mealgRepository.GetAll();
            query = query.Where(x => x.AttDate >= input.StartDate && x.AttDate <= input.EndDate)
                .WhereIf(!string.IsNullOrEmpty(input.Keyword), x => x.Name.Contains(input.Keyword) || x.UserName.Equals(input.Keyword))
                .WhereIf(input.DepartmentId.HasValue, x => x.DepartmentId.Equals(input.DepartmentId))
                .WhereIf(input.CompanyId.HasValue, x => x.CompanyId.Equals(input.CompanyId))
                ;
            string permissionCode = GetCurrentPermissionCode();
            //数据权限
            var filteredQuery = await _dataAuthorizesApp.CreateDataAuthorizesFilteredQuery(query, permissionCode);
            query = filteredQuery.Query;
            // 2. 分组聚合
            var result = query.GroupBy(x => new { x.UserId, x.UserName })
                              .Select(g => new
                              {
                                  g.Key.UserId,
                                  g.Key.UserName,
                                  Name = g.Max(x => x.Name),
                                  DepartmentId = g.Max(x => x.DepartmentId),
                                  CompanyId = g.Max(x => x.CompanyId),
                                  LunchCount = g.Sum(x => x.LunchCount),
                                  LunchPrice = g.Sum(x => x.LunchPrice),
                                  DinnerCount = g.Sum(x => x.DinnerCount),
                                  DinnerPrice = g.Sum(x => x.DinnerPrice),
                                  TotalPrice = g.Sum(x => x.TotalPrice)
                              });
            //获取总数
            var resultCount = await result.CountAsync();
            var list = result.PageBy(input).ToList();
            var listDtos = list.GroupBy(g => g.UserId).Select(item =>
            {
                var dto = ObjectMapper.Map<AttendancerMealStatisticsDto>(item);
                return dto;
            }).ToList();
            return new PagedResultWithAuthDto<AttendancerMealStatisticsDto>(resultCount, listDtos);
        }
        #endregion


        #region 获取员工考勤机-打卡记录
        /// <summary>
        /// 获取员工考勤机-打卡记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DataAuthPermission("GetPagedAttendanceRecordList")]
        public async Task<PagedResultWithAuthDto<CHECKINOUTDto>> GetPagedAttendanceRecordList(GetAttendanceLogListInput input)
        {
            input.StartDate = input.StartDate.Date;
            input.EndDate = input.EndDate.Date.AddSeconds(86399);
            var query = from checkin in _chekinoutRepository.GetAll()
                        join userInfo in _userinfoRepository.GetAll() on checkin.USERID equals userInfo.Id
                        join employee in _userRepository.GetAll() on userInfo.BADGENUMBER equals employee.UserName
                        select new CHECKINOUTDto
                        {
                            Name = employee.Name,
                            UserName = employee.UserName,
                            CHECKTYPE = checkin.CHECKTYPE,
                            CheckTime = checkin.CheckTime,
                            SENSORID = checkin.SENSORID,
                            SN = checkin.SN,
                            UserId = employee.Id,
                            UserInfoId = checkin.USERID,
                            VERIFYCODE = checkin.VERIFYCODE,
                        }
            ;
            string permissionCode = GetCurrentPermissionCode();
            //数据权限
            var filteredQuery = await _dataAuthorizesApp.CreateDataAuthorizesFilteredQuery(query, permissionCode);
            query = filteredQuery.Query;

            query = query.Where(x => x.CheckTime >= input.StartDate && x.CheckTime <= input.EndDate)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.UserName.Equals(input.Keyword) || x.Name.Contains(input.Keyword));

            if (!string.IsNullOrEmpty(input.Sorting))
                query = query.OrderBy(input.Sorting);
            var totalCount = query.Count();
            var list = await query.PageBy(input).ToListAsync();
            //办公地点
            var machinesDtos = await GetMachinesList();
            var listDtos = list.Select(item =>
            {
                var machine = machinesDtos.FirstOrDefault(x => x.MachineAddress.Equals(item.SENSORID));
                item.OfficeLocation = machine != null ? machine.Description : "";
                return item;
            }).ToList();

            return new PagedResultWithAuthDto<CHECKINOUTDto>(totalCount, list);
        }
        #endregion

        #region 根据时间关键词获取员工考勤机考勤打卡记录
        /// <summary>
        /// 根据时间关键词获取员工考勤机-考勤打卡记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HiddenApi]
        public async Task<IEnumerable<AttendanceCheckTimeDto>> GetAttendanceCheckTime(GetAttendanceCheckTimeInput input)
        {
            //var cacheManager = _cacheManager.GetCache($"ADTO.DCloud-ATTENDANCES");
            //cacheManager.DefaultSlidingExpireTime = TimeSpan.FromDays(1);
            //var cacheKey = $"GetAttendanceCheckTime{input.StartDate.ToString("yyyyMMdd")}-{input.EndDate.ToString("yyyyMMdd")}";
            //var list = await cacheManager.GetAsync(cacheKey, async (t) =>
            //{
            var query = from r in this._chekinoutRepository.GetAll()
                        join user in this._userinfoRepository.GetAll() on r.USERID equals user.Id
                        select new AttendanceCheckTimeDto { Name = user.Name, UserName = user.BADGENUMBER, CheckTime = r.CheckTime, SN = r.SN, SENSORID = r.SENSORID };
            var list = await query
                .Where(x => x.CheckTime >= input.StartDate && x.CheckTime <= input.EndDate)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Name.Equals(input.Keyword) || x.UserName.Equals(input.Keyword))
                .WhereIf(!string.IsNullOrWhiteSpace(input.Name) || !string.IsNullOrWhiteSpace(input.UserName), x => x.Name.Equals(input.Name) || x.UserName.Equals(input.UserName))
                .ToListAsync();
            //cacheManager.Set(cacheKey, list);
            return list;
            //}) as IEnumerable<AttendanceCheckTimeDto>;
            //return list;
        }
        #endregion

        #region 获取用户生成考勤记录
        /// <summary>
        /// 获取用户考勤机-打卡记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<IEnumerable<AttendanceLog>> GetAttendanceLogs(UpdateAttendanceLogsRequestDto input)
        {
            input.StartDate = input.StartDate.ToString("yyyy-MM-dd").ToDate();
            input.EndDate = input.EndDate.ToString("yyyy-MM-dd").ToDate().AddSeconds(86399);
            var query = _repository.GetAll()
                .Where(x => x.AttDate >= input.StartDate && x.AttDate <= input.EndDate)
                .WhereIf(input.IdList != null && input.IdList.Count > 0, x => input.IdList.Contains(x.Id))
                .WhereIf(input.UserId != Guid.Empty, x => x.UserId.Equals(input.UserId))
                .WhereIf(input.CompanyId.HasValue, x => x.CompanyId.Equals(input.CompanyId))
                .WhereIf(input.DepartmentId.HasValue, x => x.DepartmentId.Equals(input.DepartmentId))
                .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Name.Contains(input.Keyword) || x.UserName.Contains(input.Keyword));

            if (input.Status != null && input.Status.Count() > 0)
            {
                var stutus = input.Status;
                if (stutus.Contains("审核中"))
                {
                    string[] shz = new string[] { "外出审核中", "出差审核中", "丧假审核中", "事假审核中", "年假审核中", "产假审核中", "婚假审核中", "病假审核中", "调休审核中", "调休审核中", "上午上班忘记打卡审核中", "上午上班迟到忘记打卡审核中", "下午上班忘记打卡审核中", "下午下班忘记打卡审核中", "上午下班忘记打卡审核中", "审核中" };
                    stutus = stutus.Concat(shz).ToArray();
                }
                if (stutus.Contains("忘记打卡"))
                {
                    string[] wdk = new string[] { "上午上班忘记打卡", "上午上班迟到忘记打卡", "下午上班忘记打卡", "下午下班忘记打卡", "上午下班忘记打卡" };
                    stutus = stutus.Concat(wdk).ToArray();
                }

                if (stutus.Contains("审核中") && stutus.Contains("假"))
                {
                    query = query.Where(x => x.AMInType.Contains("审核中") || x.PMOutType.Contains("审核中") || x.AMInType.Contains("假") || x.PMOutType.Contains("假") || stutus.Contains(x.AMInType) || stutus.Contains(x.PMOutType));
                }
                else if (stutus.Contains("审核中") && stutus.Contains("忘记打卡"))
                {
                    query = query.Where(x => x.AMInType.Contains("审核中") || x.PMOutType.Contains("审核中") || x.AMInType.Contains("忘记打卡") || x.PMOutType.Contains("忘记打卡") || stutus.Contains(x.AMInType) || stutus.Contains(x.PMOutType));
                }
                else if (stutus.Contains("新冠确诊"))
                {
                    query = query.Where(x => x.AMInType.Contains("新冠确诊") || x.PMOutType.Contains("新冠确诊") || stutus.Contains(x.AMInType) || stutus.Contains(x.PMOutType));
                }
                else
                {
                    query = query.Where(x => stutus.Contains(x.AMInType) || stutus.Contains(x.PMOutType));
                }
            }
            var list = await query.ToListAsync();
            //var listDtos = ObjectMapper.Map<List<AttendanceLogDto>>(list);
            return list;
        }
        #endregion

        #region 默认考勤机
        /// <summary>
        /// 默认考勤机
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<AttendanceMachinesDto>> GetMachinesList()
        {
            var cacheManager = _cacheManager.GetCache($"ADTO.DCloud-ATTENDANCES");
            cacheManager.DefaultSlidingExpireTime = TimeSpan.FromDays(1);
            var cacheKey = $"GetMachinesList";
            var list = await cacheManager.GetAsync(cacheKey, async (t) =>
            {
                var query = await _machinesRepository.GetAllListAsync();
                var listDto = ObjectMapper.Map<IEnumerable<AttendanceMachinesDto>>(query);
                cacheManager.Set(cacheKey, listDto);
                return listDto;
            }) as IEnumerable<AttendanceMachinesDto>;
            return list;
        }
        #endregion

        #region 考勤公休日
        /// <summary>
        /// 根据时间范围获取公休日
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PublicHoliDayDto>> GetPublicHoliDayList(GetPublicHoliDayRequestInput input)
        {
            var cacheManager = _cacheManager.GetCache($"ADTO.DCloud-ATTENDANCES");
            cacheManager.DefaultSlidingExpireTime = TimeSpan.FromDays(1);
            var cacheKey = $"GetPublicHoliDayList{input.StartDate.ToString("yyyyMMdd")}-{input.EndDate.ToString("yyyyMMdd")}";
            var list = await cacheManager.GetAsync(cacheKey, async (t) =>
            {
                var query = await _holidayRepository.GetAllListAsync();
                var list = query.Where(q => q.HoliDay >= input.StartDate && q.HoliDay <= input.EndDate);
                var listDto = ObjectMapper.Map<IEnumerable<PublicHoliDayDto>>(list);
                //cacheManager.Set(cacheKey, listDto);
                return listDto.OrderBy(o => o.HoliDay);

            }) as IEnumerable<PublicHoliDayDto>;
            return list;
        }
        #endregion

        #region 修改考勤状态
        /// <summary>
        /// 修改考勤状态--考勤异常  Guid userId, DateTime attDate
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="attDate"></param>
        /// <returns></returns>
        public async Task UpdateAttendanceLog(Guid processId, string schemeCode)
        {
            switch (schemeCode)
            {
                case "KQ004"://考勤异常
                    var att = await _attAppService.GetAsync(new EntityDto<Guid> { Id = processId });
                    if (att != null && att.Id != Guid.Empty)
                        await UpdateAttendanceLogs(new UpdateAttendanceLogsRequestDto() { StartDate = att.AttDate, EndDate = att.AttDate, UserId = att.UserId });
                    break;
                case "ADTO_ABS_2026"://请假
                    var abs = await _absAppService.GetAsync(new EntityDto<Guid> { Id = processId });
                    if (abs != null && abs.Id != Guid.Empty)
                        await UpdateAttendanceLogs(new UpdateAttendanceLogsRequestDto() { StartDate = abs.StartDate, EndDate = abs.EndDate, UserId = abs.UserId });
                    break;
                case "ADTO_OUT_2026"://外出
                    var outatt = await _outAppService.GetAsync(new EntityDto<Guid> { Id = processId });
                    if (outatt != null && outatt.Id != Guid.Empty)
                        await UpdateAttendanceLogs(new UpdateAttendanceLogsRequestDto() { StartDate = outatt.StartDate, EndDate = outatt.EndDate, UserId = outatt.UserId });
                    break;
                case "KQ002"://出差
                    var onb = await _onbAppService.GetAsync(new EntityDto<Guid> { Id = processId });
                    if (onb != null && onb.Id != Guid.Empty)
                        await UpdateAttendanceLogs(new UpdateAttendanceLogsRequestDto() { StartDate = onb.StartDate, EndDate = onb.EndDate, UserId = onb.UserId });
                    break;
            }
            //await UpdateAttendanceLogs(new UpdateAttendanceLogsRequestDto() { StartDate = attDate, EndDate = attDate, UserId = userId });
        }
        #endregion


        #region 扩展方法
        /// <summary>
        /// 获取考勤时间
        /// </summary>
        /// <param name="times"></param>
        /// <param name="currentDate"></param>
        /// <returns></returns>
        public static List<AttendanceTimeDto> GetActiveTimeConfigs(List<AttendanceTimeDto> times, DateTime currentDate)
        {
            int curMonth = currentDate.Month;
            int curDay = currentDate.Day;
            int curVal = curMonth * 100 + curDay; // 例如 0420 -> 420

            return times.Where(x =>
            {
                int sMonth = x.SDate.Month;
                int sDay = x.SDate.Day;
                int eMonth = x.EDate.Month;
                int eDay = x.EDate.Day;
                int sVal = sMonth * 100 + sDay;
                int eVal = eMonth * 100 + eDay;

                if (sVal <= eVal) // 不跨年，如 5/1 - 9/30
                {
                    return curVal >= sVal && curVal <= eVal;
                }
                else // 跨年，如 10/1 - 4/30
                {
                    return curVal >= sVal || curVal <= eVal;
                }
            }).ToList();
        }
        #endregion
    }
}
