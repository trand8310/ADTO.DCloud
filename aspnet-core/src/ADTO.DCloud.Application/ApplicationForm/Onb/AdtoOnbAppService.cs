using ADTO.DCloud.ApplicationForm.Abs.Dto;
using ADTO.DCloud.ApplicationForm.Att.Dto;
using ADTO.DCloud.ApplicationForm.Onb.Dto;
using ADTO.DCloud.ApplicationForm.Out.Dto;
using ADTO.DCloud.Attendances.AttendanceTimeRules;
using ADTO.DCloud.Attendances;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.DataAuthorizes;
using ADTO.DCloud.DataItem;
using ADTO.DCloud.DeptRoles;
using ADTO.DCloud.DeptRoles.Dto;
using ADTO.DCloud.EmployeeManager;
using ADTO.DCloud.Infrastructur;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.WorkFlow.NodeMethod;
using ADTO.DCloud.WorkFlow.Processes;
using ADTOSharp;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Organizations;
using ADTOSharp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm.Onb
{
    /// <summary>
    /// 出差申请
    /// </summary>
    [ADTOSharpAuthorize]
    public class AdtoOnbAppService : DCloudAppServiceBase, IAdtoOnbAppService
    {
        private readonly IRepository<Adto_Onb, Guid> _repository;
        private readonly IRepository<EmployeeInfo, Guid> _employeeRepository;
        private readonly IRepository<PublicHoliDay, Guid> _holidayRepository;
        private readonly IRepository<AttendanceTime, Guid> _attendanceTimeRepository;
        private readonly AttendanceTimeRuleAppService _timeRuleAppService;
        private readonly IApplicationFormCheckDateAppService _applicationFormAppService;
        private readonly IRepository<OrganizationUnit, Guid> _orgRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IRepository<WorkFlowProcess, Guid> _processRepository;
        private readonly DataFilterService _dataAuthorizesApp;
        public AdtoOnbAppService(IRepository<Adto_Onb, Guid> repository,
            IRepository<EmployeeInfo, Guid> employeeRepository,
             IRepository<PublicHoliDay, Guid> holidayRepository,
             IRepository<AttendanceTime, Guid> attendanceTimeRepository,
              AttendanceTimeRuleAppService timeRuleAppService,
              IApplicationFormCheckDateAppService applicationFormAppService,
              IRepository<OrganizationUnit, Guid> orgRepository,
              IRepository<User, Guid> userRepository,
              DataFilterService dataAuthorizesApp,
              IRepository<WorkFlowProcess, Guid> processRepository)
        {
            _repository = repository;
            _repository = repository;
            _employeeRepository = employeeRepository;
            _holidayRepository = holidayRepository;
            _attendanceTimeRepository = attendanceTimeRepository;
            _timeRuleAppService = timeRuleAppService;
            _applicationFormAppService = applicationFormAppService;
            _orgRepository = orgRepository;
            _userRepository = userRepository;
            _processRepository = processRepository;
            _dataAuthorizesApp = dataAuthorizesApp;
        }


        #region 获取数据
        /// <summary>
        /// 单据管理-出差申请单据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DataAuthPermission("GetOnbPagedList")]
        public async Task<PagedResultDto<AdtoOnbDto>> GetOnbPagedList(GetAdtoOnbPageInput input)
        {
            if (input.StartDate == null)
            {
                //如果日期为空，默认显示近一个月
                input.StartDate = DateTime.Now.AddMonths(-1);
            }
            //因为添加日期为年与日，否则不会包含今天数据
            if (input.EndDate == null)
            {
                input.EndDate = DateTime.Now.AddDays(1);
            }
            //var query = from r in this._repository.GetAll()
            //            join p in this._processRepository.GetAll() on r.Id equals p.Id
            //            join u in this._userRepository.GetAll() on r.UserID equals u.Id into r_u
            //            from uu in r_u.DefaultIfEmpty()
            //            join d in this._orgRepository.GetAll() on uu.DepartmentId equals d.Id into r_d
            //            from dd in r_d.DefaultIfEmpty()
            //            join c in this._orgRepository.GetAll() on uu.CompanyId equals c.Id into c_d
            //            from com in c_d.DefaultIfEmpty()

            var query = from r in this._repository.GetAll()
                        join p in this._processRepository.GetAll() on r.Id equals p.Id into p_d
                        from process in p_d.DefaultIfEmpty()
                        join uu in this._userRepository.GetAll() on r.UserId equals uu.Id
                        join d in this._orgRepository.GetAll() on uu.DepartmentId equals d.Id
                        join com in this._orgRepository.GetAll() on uu.CompanyId equals com.Id
                        where r.IsDeleted.Equals(false)
                        select new AdtoOnbDto
                        {
                            Id = r.Id,
                            Title = r.Title,
                            UserId = r.UserId,
                            UserName = uu.UserName,
                            Name = uu.Name,
                            CompanyId = com.Id,
                            CompanyName = com.DisplayName,
                            DepartmentId = r.DepartmentId,
                            DepartmentName = d.DisplayName,
                            StartDate = r.StartDate,
                            EndDate = r.EndDate,
                            Remark = r.Remark,
                            Days = r.Days,
                            CreationTime = r.CreationTime,
                            CreatorUserId = r.CreatorUserId,
                            IsFinished = process == null ? 0 : process.IsFinished ?? 0,//== 1 ? "结束" : "审核中",
                            SchemeCode = process == null ? "" : process.SchemeCode
                        };
            query = query.Where(a => (DateTime.Compare(Convert.ToDateTime(input.StartDate), a.StartDate) >= 0 && DateTime.Compare(Convert.ToDateTime(input.StartDate), a.EndDate) <= 0)
                         || (DateTime.Compare(Convert.ToDateTime(input.EndDate), a.StartDate) >= 0 && DateTime.Compare(Convert.ToDateTime(input.EndDate), a.EndDate) <= 0)
                         || (DateTime.Compare(a.StartDate, Convert.ToDateTime(input.StartDate)) >= 0 && DateTime.Compare(a.StartDate, Convert.ToDateTime(input.EndDate)) <= 0)
                         || (DateTime.Compare(a.EndDate, Convert.ToDateTime(input.StartDate)) >= 0 && DateTime.Compare(a.EndDate, Convert.ToDateTime(input.EndDate)) <= 0)
                         ).WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), p => p.UserName == (input.Keyword) || p.Name.Contains(input.Keyword))
                         .WhereIf(!string.IsNullOrWhiteSpace(input.Title), p => p.Title.Contains(input.Title))
                         .WhereIf(!string.IsNullOrWhiteSpace(input.CompanyId.ToString()), p => p.CompanyId == input.CompanyId)
                         .WhereIf(input.DepartmentId != null, p => p.DepartmentId == input.DepartmentId)
                         ;
            string permissionCode = PermissionHelper.GetPermissionCode(this.GetType().GetMethod(nameof(GetOnbPagedList)));
            //数据权限
            query = await _dataAuthorizesApp.CreateDataFilteredQuery(query, permissionCode);

            //自定义排序
            if (!string.IsNullOrWhiteSpace(input.Sorting))
                query = query.OrderBy(input.Sorting);

            //获取总数
            var resultCount = await query.CountAsync();
            var taskList = query.PageBy(input).ToList();
            return new PagedResultDto<AdtoOnbDto>(resultCount, taskList);
        }

        /// <summary>
        /// 获取出差数据列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AdtoOnbDto>> GetAllAsync(GetAdtoOnbInput input)
        {
            var query = _repository.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.Filter), q => q.Title.Contains(input.Filter));
            var list = await query.ToListAsync();
            var listDtos = ObjectMapper.Map<List<AdtoOnbDto>>(list);
            return listDtos;
        }


        /// <summary>
        /// 获取分页列表出差数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<AdtoOnbDto>> GetAllPageListAsync(GetAdtoOnbPageInput input)
        {
            var query = _repository.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), q => q.Title.Contains(input.Keyword));
            if (input.Sorting != null)
                query = query.OrderBy(input.Sorting);
            var totalCount = query.Count();
            var list = await query.PageBy(input).ToListAsync();
            var listDtos = ObjectMapper.Map<List<AdtoOnbDto>>(list);
            return new PagedResultDto<AdtoOnbDto>(totalCount, listDtos);
        }

        /// <summary>
        /// 获取出差数据列表-计算考勤机异常
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AdtoOnbAttendanceDto>> GetOnbListAsync(GetAdtoAbsInput input)
        {
            var query = from r in this._repository.GetAll()
                        join pro in this._processRepository.GetAll() on r.Id equals pro.Id
                        join user in this._userRepository.GetAll() on r.UserId equals user.Id
                        select new AdtoOnbAttendanceDto
                        {
                            Id = r.Id,
                            UserId = r.UserId,
                            Name = user.Name,
                            UserName = user.UserName,
                            CompanyId = r.CompanyId,
                            DepartmentId = r.DepartmentId,
                            StartDate = r.StartDate,
                            EndDate = r.EndDate,
                            FlowStatus = pro.IsFinished ?? 0
                        };
            query = query.Where(a => (a.StartDate.Date >= input.StartDate && a.StartDate.Date <= input.EndDate) ||
        (a.EndDate.Date >= input.StartDate && a.EndDate.Date <= input.EndDate) ||
        (input.StartDate >= a.StartDate.Date && input.StartDate <= a.EndDate.Date) ||
        (input.EndDate >= a.StartDate.Date && input.EndDate <= a.EndDate.Date))
                .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Name.Contains(input.Keyword) || x.UserName.Equals(input.Keyword));

            var list = await query.ToListAsync();
            var listDtos = ObjectMapper.Map<List<AdtoOnbAttendanceDto>>(list);
            return listDtos;
        }
        /// <summary>
        /// 获取流程设计(依流程Id)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<AdtoOnbDto> GetAsync(EntityDto<Guid> input)
        {
            var scheme = await _repository.GetAsync(input.Id);
            return ObjectMapper.Map<AdtoOnbDto>(scheme);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 新增出差
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HiddenApi]
        public async Task<AdtoOnbDto> CreateAsync(CreateAdtoOnbDto input)
        {
            try
            {
                if (ADTOSharpSession.UserId == null)
                    throw new ADTOSharpException(L("ApplicationFailedUserLoginStatusIsAbnormal"));
                var dto = ObjectMapper.Map<Adto_Onb>(input);
                await _repository.InsertAsync(dto);
                CurrentUnitOfWork.SaveChanges();
                return ObjectMapper.Map<AdtoOnbDto>(dto);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        /// <summary>
        /// 修改出差
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HiddenApi]
        public async Task<AdtoOnbDto> UpdateAsync(UpdateAdtoOnbDto input)
        {
            try
            {
                if (ADTOSharpSession.UserId == null)
                    throw new ADTOSharpException(L("ApplicationFailedUserLoginStatusIsAbnormal"));
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
        /// 删除出差
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task DeleteAsync(EntityDto<Guid> input)
        {
            if (ADTOSharpSession.UserId == null)
                throw new ADTOSharpException(L("ApplicationFailedUserLoginStatusIsAbnormal"));
            await _repository.DeleteAsync(input.Id);

        }
        /// <summary>
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
            var dto = input.Data.ToObject<CreateAdtoOnbDto>();
            await OnbCheckValidity(new OnbCheckValidityDto() { UserId = dto.UserId, StartDate = dto.StartDate, EndDate = dto.EndDate });
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
            var dto = input.Data.ToObject<UpdateAdtoOnbDto>();
            await this.UpdateAsync(dto);
        }
        /// <summary>
        /// 删除表单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task ExecuteDelete(Guid id)
        {
            if (ADTOSharpSession.UserId == null)
            {
                throw new ADTOSharpException(L("ApplicationFailedUserLoginStatusIsAbnormal"));
            }
            await this.DeleteAsync(new EntityDto<Guid>() { Id = id });
        }
        #endregion

        #region 表单验证
        /// <summary>
        /// 校验出差申请单合法性
        /// 判断是否存在相同时间段的出差申请单
        /// </summary>
        /// <returns></returns>
        public async Task OnbCheckValidity(OnbCheckValidityDto input)
        {
            if (ADTOSharpSession.UserId == null)
            {
                throw new ADTOSharpException(L("ApplicationFailedUserLoginStatusIsAbnormal"));
            }
            Guid userId = input.UserId.HasValue ? input.UserId.Value : ADTOSharpSession.UserId.Value;
            if (input.StartDate == null || input.EndDate == null)
            {
                throw new ADTOSharpException(L("ApplicationFailed.DateCannotBeEmpty"));
            }
            DateTime startDate = input.StartDate.Value;
            DateTime endDate = input.EndDate.Value;
            if (DateTime.Compare(endDate, startDate) <= 0)
            {
                throw new ADTOSharpException(L("Applicationfailed:Theenddatecannotbeearlierthanthestartdate.Pleasecheckandresubmit"));
            }

            if (input.Id == Guid.Empty || input.Id == null)
            {
                bool isdateresult = await _applicationFormAppService.IsDateValid(startDate, "Onb");
                //判断申请日期是否为上个月的考勤数据
                if (isdateresult)
                {
                    throw new ADTOSharpException(L("ApplicationFailedTheDeadlineForSubmissionHaPassed"));
                }
            }
            //验证申请时间是否有申请请假、出差、出差申请单
            await _applicationFormAppService.IsExistApplication(userId, startDate, endDate, input.Id);
            //获取用户是否有被禁止申请出差流程
            var disableUserWrokFlows = await _applicationFormAppService.GetDisableUserListByUserId(ADTOSharpSession.UserId.Value, "Adto_Onb");
            if (disableUserWrokFlows.Count() > 0)
            {
                throw new ADTOSharpException(L("Applicationfailed.Sorryyoucannotapplyforthisprocess.PleasesigninwithinyourcompanyDingTalkteam.Thankyouforyourcooperation."));
            }
        }
        #endregion

        #region 扩展
        /// <summary>
        /// 获取前端出差，前端调用
        /// </summary>
        /// <param name="StartDateTime"></param>
        /// <param name="EndDateTime"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetOnbDaysNum(GeFromObjectInput input)
        {
            Dictionary<string, object> result = new Dictionary<string, object>() { };
            var dict = input.Data.ToDictionary(p => p.Name, p => p.Value);
            var season = await _timeRuleAppService.GetSeason(DateTime.Now);
            int hours = 8, minutes = season == "Winter" ? 30 : 0;
            DateTime startDate = string.IsNullOrWhiteSpace(dict.GetValueOrDefault("StartDate")) || IsWrappedWithDollarSign(dict.GetValueOrDefault("StartDate")) || dict.GetValueOrDefault("StartDate") == "undefined" ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hours, minutes, 0) : dict.GetValueOrDefault("StartDate").ToDate();
            DateTime endDate = string.IsNullOrWhiteSpace(dict.GetValueOrDefault("EndDate")) || IsWrappedWithDollarSign(dict.GetValueOrDefault("StartDate")) || dict.GetValueOrDefault("StartDate") == "undefined" ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 30, 0) : dict.GetValueOrDefault("EndDate").ToDate();

            TimeSpan aMInTime = new TimeSpan(hours, minutes, 0);
            TimeSpan aMOutTime = new TimeSpan(12, 0, 0);
            TimeSpan pMInTime = new TimeSpan(13, 0, 0);
            TimeSpan pMOutTime = new TimeSpan(17, 30, 0);
            double days = 0;
            result.Add("StartDate", startDate.ToString("yyyy-MM-dd HH:mm"));
            result.Add("EndDate", endDate.ToString("yyyy-MM-dd HH:mm"));
            result.Add("DayCount", CalculateDays(startDate, endDate, aMInTime, aMOutTime, pMInTime, pMOutTime));
            if (!ADTOSharpSession.UserId.HasValue)
                return new JsonResult(new { Data = result });
            var employee = await this._employeeRepository.FirstOrDefaultAsync(p => p.UserId == ADTOSharpSession.UserId);
            if (employee == null || employee.Id == Guid.Empty)
                return new JsonResult(new { Data = result });
            //考勤时间表
            //var areaSeasonTime = await _attendanceTimeRepository.GetAllAsync();
            var timeRule = await _timeRuleAppService.GetAsync(new EntityDto<Guid> { Id = employee.AttTimeRuleId.Value });
            //获取天数失败，用户考勤规则不存在，请联系人资设置考勤规则！
            if (timeRule == null || string.IsNullOrWhiteSpace(timeRule.AttendanceTimeIds))
                throw new ADTOSharpException(L("FAILEDTOOBTAINTHENUMBEROFDAYS.THEUSERATTENDANCERULEDOESNOTEXIST.PLEASECONTACTMETOSETTHEATTENDANCERULE"));

            var attendanceTimeIds = timeRule.AttendanceTimeIds.ToGuidList();
            //考勤时间表
            var areaSeasonTime = await _attendanceTimeRepository.GetAll().Where(x => attendanceTimeIds.Contains(x.Id)).ToListAsync();
            foreach (var item in areaSeasonTime)
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
            for (DateTime dt = startDate.Date; dt <= endDate.Date; dt = dt.AddDays(1))
            {
                double amtime = 0, pmtime = 0;
                //season = await _timeRuleAppService.GetSeason(startDate);
                //var time = areaSeasonTime.FirstOrDefault(t => t.AttendanceTimeRule.Id == employee.AttTimeRuleId && t.Season == season);
                var time = areaSeasonTime.FirstOrDefault(t => dt >= t.SDate && dt <= t.EDate);
                if (time != null && time.Id != Guid.Empty)
                {
                    aMInTime = time.AMInTime;
                    aMOutTime = time.AMOutTime;
                    pMInTime = time.PMInTime;
                    pMOutTime = time.PMOutTime;
                }
                #region MyRegion
                if (DateTime.Compare(startDate, dt.Add(aMOutTime)) >= 0 && DateTime.Compare(startDate, dt.Add(pMOutTime)) < 0)
                {
                    amtime = 0;
                    pmtime = 4;
                }
                else if (DateTime.Compare(endDate, dt.Add(pMInTime)) <= 0 && DateTime.Compare(endDate, dt.Add(aMInTime)) > 0)
                {
                    amtime = 4;
                    pmtime = 0;
                }
                else if (DateTime.Compare(startDate, dt.Add(aMOutTime)) < 0 && DateTime.Compare(endDate, dt.Add(pMInTime)) > 0)
                {
                    amtime = 4;
                    pmtime = 4;
                }
                days += amtime + pmtime > 5 ? 1 : amtime + pmtime > 1 && amtime + pmtime < 5 ? 0.5 : 0;
                #endregion
            }
            result["DayCount"] = days;
            return new JsonResult(new { Data = result });
        }
        /// <summary>
        /// 根据时间计算天数
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static double CalculateDays(DateTime start, DateTime end, TimeSpan amInTime, TimeSpan amOutTime, TimeSpan pmInTime, TimeSpan pmOutTime)
        {
            double days = 0.0;
            for (DateTime dt = start.Date; dt <= end.Date; dt = dt.AddDays(1))
            {
                double amtime = 0, pmtime = 0;
                if (DateTime.Compare(start, dt.Add(amOutTime)) >= 0 && DateTime.Compare(start, dt.Add(pmOutTime)) < 0)
                {
                    amtime = 0;
                    pmtime = 4;
                }
                else if (DateTime.Compare(end, dt.Add(pmInTime)) <= 0 && DateTime.Compare(end, dt.Add(amInTime)) > 0)
                {
                    amtime = 4;
                    pmtime = 0;
                }
                else if (DateTime.Compare(start, dt.Add(amOutTime)) < 0 && DateTime.Compare(end, dt.Add(pmInTime)) > 0)
                {
                    amtime = 4;
                    pmtime = 4;
                }
                days += amtime + pmtime > 5 ? 1 : amtime + pmtime > 1 && amtime + pmtime < 5 ? 0.5 : 0;
            }
            return days;
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
    }
}
