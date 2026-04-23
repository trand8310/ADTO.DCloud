using ADTO.DCloud.ApplicationForm.Abs.Dto;
using ADTO.DCloud.ApplicationForm.Att.Dto;
using ADTO.DCloud.ApplicationForm.Onb.Dto;
using ADTO.DCloud.ApplicationForm.Out.Dto;
using ADTO.DCloud.Authorization;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.DataAuthorizes;
using ADTO.DCloud.DataItem;
using ADTO.DCloud.DeptRoles;
using ADTO.DCloud.DeptRoles.Dto;
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
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using static ADTO.DCloud.ApplicationForm.Abs.enums.EnumAttStatusType;

namespace ADTO.DCloud.ApplicationForm.Att
{
    /// <summary>
    /// 考勤异常
    /// </summary>
    [ADTOSharpAuthorize]
    public class AdtoAttAppService : DCloudAppServiceBase, IAdtoAttAppService
    {
        private readonly IRepository<Adto_Att, Guid> _repository;
        private readonly IApplicationFormCheckDateAppService _applicationFormAppService;
        private readonly IRepository<OrganizationUnit, Guid> _orgRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IRepository<WorkFlowProcess, Guid> _processRepository;
        private readonly DataFilterService _dataAuthorizesApp;
        private readonly IRepository<DataItemDetail, Guid> _dataitemRepository;
        private readonly DataItemDetailAppService _dataItemAppService;
        public AdtoAttAppService(IRepository<Adto_Att, Guid> repository,
            IApplicationFormCheckDateAppService applicationFormAppService,
              IRepository<OrganizationUnit, Guid> orgRepository,
              IRepository<User, Guid> userRepository,
              DataFilterService dataAuthorizesApp,
              IRepository<WorkFlowProcess, Guid> processRepository,
              DataItemDetailAppService dataItemAppService)
        {
            _repository = repository;
            _applicationFormAppService = applicationFormAppService;
            _orgRepository = orgRepository;
            _userRepository = userRepository;
            _processRepository = processRepository;
            _dataAuthorizesApp = dataAuthorizesApp;
            //_dataitemRepository = dataitemRepository;
            _dataItemAppService = dataItemAppService;
        }

        #region 获取数据

        /// <summary>
        /// 单据管理-考勤异常申请单据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DataAuthPermission("GetAttPagedList")]
        public async Task<PagedResultDto<AdtoAttDto>> GetAttPagedList(GetAttPagedListInput input)
        {
            #region 默认一个月的数据
            if (input.StartDate == null)
            {
                //如果日期为空，默认显示近一个月
                input.StartDate = DateTime.Now.AddMonths(-1);
            }
            //因为添加日期为年与日，否则不会包含今天数据
            if (input.EndDate == null)
            {
                input.EndDate = DateTime.Now;
            }
            else
            {
                input.EndDate = input.EndDate.Value.Add(new TimeSpan(23, 59, 59));
            }
            #endregion

            #region 左连接
            //var query = from r in this._repository.GetAll()
            //            join p in this._processRepository.GetAll() on r.Id equals p.Id into p_d
            //            from propcess in p_d.DefaultIfEmpty()
            //            join u in this._userRepository.GetAll() on r.UserId equals u.Id into r_u
            //            from user in r_u.DefaultIfEmpty()
            //            join d in this._orgRepository.GetAll() on user.DepartmentId equals d.Id into r_d
            //            from depart in r_d.DefaultIfEmpty()
            //            join c in this._orgRepository.GetAll() on user.CompanyId equals c.Id into c_d
            //            from company in c_d.DefaultIfEmpty()
            //            join i in this._dataitemRepository.GetAllIncluding(p => p.Item) on r.AttType equals i.ItemValue
            //            where i.Item.ItemCode == "AttType"
            #endregion
            var query = from r in this._repository.GetAll()
                        join p in this._processRepository.GetAll() on r.Id equals p.Id into p_j
                        from process in p_j.DefaultIfEmpty()  // 左连接 p 允许流程不存在，表单存在的情况,人事可作废申请表
                        join u in this._userRepository.GetAll() on r.UserId equals u.Id
                        join d in this._orgRepository.GetAll() on u.DepartmentId equals d.Id
                        join c in this._orgRepository.GetAll() on u.CompanyId equals c.Id
                        select new AdtoAttDto
                        {
                            Id = r.Id,
                            Title = r.Title,
                            UserId = r.UserId,
                            UserName = u.UserName,
                            Name = u.Name,
                            CompanyId = c.Id,
                            CompanyName = c.DisplayName,
                            DepartmentId = r.DepartmentId,
                            DepartmentName = d.DisplayName,
                            AttDate = r.AttDate,
                            AttType = r.AttType,
                            CreationTime = r.CreationTime,
                            CreatorUserId = r.CreatorUserId,
                            Remark = r.Remark,
                            IsFinished = process == null ? 0 : process.IsFinished ?? 0,//== 1 ? "结束" : "审核中",
                            SchemeCode = process == null ? "" : process.SchemeCode
                        };
            query = query.WhereIf(input.StartDate.HasValue && input.EndDate.HasValue, x => x.AttDate >= input.StartDate && x.AttDate <= input.EndDate)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), p => p.UserName == input.Keyword || p.Name.Contains(input.Keyword))
                       .WhereIf(!string.IsNullOrWhiteSpace(input.Title), p => p.Title.Contains(input.Title))
                       .WhereIf(!string.IsNullOrWhiteSpace(input.CompanyId.ToString()), p => p.CompanyId == input.CompanyId)
                       .WhereIf(input.DepartmentId != null, p => p.DepartmentId == input.DepartmentId)
                       .WhereIf(!string.IsNullOrWhiteSpace(input.AttType), p => p.AttType == input.AttType);

            string permissionCode = PermissionHelper.GetPermissionCode(this.GetType().GetMethod(nameof(GetAttPagedList)));
            //数据权限
            query = await _dataAuthorizesApp.CreateDataFilteredQuery(query, permissionCode);

            //自定义排序
            if (!string.IsNullOrWhiteSpace(input.Sorting))
                query = query.OrderBy(input.Sorting);
            //获取总数
            var resultCount = await query.CountAsync();
            var taskList = query.PageBy(input).ToList();
            var items = await _dataItemAppService.GetItemDetailList(new DataItem.Dto.DataItemQueryDto() { ItemCode = "AttType" });
            var list = taskList.Select(item =>
            {
                var itemdetail = items.FirstOrDefault(x => x.ItemValue == item.AttType);
                item.AttTypeName = itemdetail != null ? itemdetail.ItemName : "";
                return item;
            }).ToList();
            return new PagedResultDto<AdtoAttDto>(resultCount, list);
        }


        /// <summary>
        /// 获取考勤异常数据列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AdtoAttDto>> GetAllAsync(GetAdtoAttInput input)
        {
            var query = _repository.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), q => q.Title.Contains(input.Keyword));
            var list = await query.ToListAsync();
            var listDtos = ObjectMapper.Map<List<AdtoAttDto>>(list);
            return listDtos;
        }


        /// <summary>
        /// 获取分页列表考勤异常数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<AdtoAttDto>> GetAllPageListAsync(GetAdtoAttPageInput input)
        {
            var query = _repository.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.Filter), q => q.Title.Contains(input.Filter));
            if (input.Sorting != null)
                query = query.OrderBy(input.Sorting);
            var totalCount = query.Count();
            var list = await query.PageBy(input).ToListAsync();
            var listDtos = ObjectMapper.Map<List<AdtoAttDto>>(list);
            return new PagedResultDto<AdtoAttDto>(totalCount, listDtos);
        }

        /// <summary>
        /// 获取考勤异常数据列表-计算考勤机异常
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AdtoAttAttendanceDto>> GetAttListAsync(GetAdtoAttInput input)
        {
            var query = from r in this._repository.GetAll()
                        join pro in this._processRepository.GetAll() on r.Id equals pro.Id
                        join user in this._userRepository.GetAll() on r.UserId equals user.Id
                        select new AdtoAttAttendanceDto
                        {
                            Id = r.Id,
                            UserId = r.UserId,
                            Name = user.Name,
                            UserName = user.UserName,
                            CompanyId = r.CompanyId,
                            DepartmentId = r.DepartmentId,
                            AttDate = r.AttDate,
                            AttType = r.AttType,
                            FlowStatus = pro.IsFinished ?? 0
                        };
            query = query.Where(x => x.AttDate >= input.StartDate && x.AttDate <= input.EndDate)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Name.Contains(input.Keyword) || x.UserName.Equals(input.Keyword));

            var list = await query.ToListAsync();
            var listDtos = ObjectMapper.Map<List<AdtoAttAttendanceDto>>(list);
            return listDtos;
        }
        /// <summary>
        /// 获取流程设计(依流程Id)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<AdtoAttDto> GetAsync(EntityDto<Guid> input)
        {
            var scheme = await _repository.GetAsync(input.Id);
            return ObjectMapper.Map<AdtoAttDto>(scheme);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 新增考勤异常
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HiddenApi]
        public async Task<AdtoAttDto> CreateAsync(CreateAdtoAttDto input)
        {
            try
            {
                if (ADTOSharpSession.UserId == null)
                    throw new ADTOSharpException(L("ApplicationFailedUserLoginStatusIsAbnormal"));
                var dto = ObjectMapper.Map<Adto_Att>(input);
                await _repository.InsertAsync(dto);
                CurrentUnitOfWork.SaveChanges();
                return ObjectMapper.Map<AdtoAttDto>(dto);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        /// <summary>
        /// 修改考勤异常
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HiddenApi]
        public async Task<AdtoAttDto> UpdateAsync(UpdateAdtoAttDto input)
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
        /// 删除考勤异常
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteAsync(EntityDto<Guid> input)
        {
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
            var dto = input.Data.ToObject<CreateAdtoAttDto>();
            await AttCheckValidity(new AttCheckValidityDto() { AttDate = dto.AttDate, UserId = dto.UserId });
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
            var dto = input.Data.ToObject<UpdateAdtoAttDto>();
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
                throw new ADTOSharpException(L("ApplicationFailedUserLoginStatusIsAbnormal"));
            await this.DeleteAsync(new EntityDto<Guid>() { Id = id });
        }
        /// <summary>
        /// 修改考勤异常请假单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        [ADTOSharpAuthorize(PermissionNames.Pages_Workflow_ApplicationForm_Att_Edit)]
        public async Task<AdtoAttDto> UpdateAbsAsync(UpdateAdtoAttDto input)
        {
            //表单验证
            await AttCheckValidity(new AttCheckValidityDto() { AttDate = input.AttDate, UserId = input.UserId, Id = input.Id });
            return await this.UpdateAsync(input);
        }
        #endregion

        #region 表单校验

        /// <summary>
        /// 校验考勤异常申请单合法性
        /// 每月不能超过三次申请单，不管状态
        /// </summary>
        /// <returns></returns>
        public async Task AttCheckValidity(AttCheckValidityDto input)
        {
            //每月考勤异常不能超过三次
            if (ADTOSharpSession.UserId == null)
                throw new ADTOSharpException(L("ApplicationFailedUserLoginStatusIsAbnormal"));
            Guid userId = input.UserId.HasValue ? input.UserId.Value : ADTOSharpSession.UserId.Value;
            if (input.AttDate == null)
            {
                throw new ADTOSharpException(L("Application.failedAbnormalAttendanceDatesCannotBeLeftBlank"));
            }
            DateTime AttDate = input.AttDate.Value;

            if (input.Id == Guid.Empty)
            {
                bool isdateresult = await _applicationFormAppService.IsDateValid(AttDate, "Att");
                //判断申请日期是否为上个月的考勤数据
                if (isdateresult)
                {
                    throw new ADTOSharpException(L("ApplicationFailedTheDeadlineForSubmissionHaPassed"));
                }
            }

            //月初
            DateTime begin = Convert.ToDateTime(AttDate.ToString("yyyy-MM") + "-01");
            //下月月初
            DateTime end = Convert.ToDateTime(AttDate.ToString("yyyy-MM") + "-01").AddMonths(+1);
            var list = await _repository.GetAll().Where(p => p.UserId == userId).Where(p => p.AttDate >= begin && p.AttDate < end && p.Id != input.Id).ToListAsync();
            if (list.Count >= 3)
            {
                throw new ADTOSharpException(L("申请失败：您这个月已申请3次考勤忘打卡！"));
            }

            //var ountList = _adtoOutRepository.GetAll().Where(q => q.UserID == userId && q.StartDate >= input.AttDate.ToDate() && input.AttDate.ToDate() <= q.EndDate).ToList();
            //if (ountList.Count() > 0)
            //{
            //    return new JsonResultModel() { Message = $"校验通过", Success = true };
            //}

            ////考勤记录一次都没有的不能申请忘记打卡
            //var user = _userRepository.Get(userId);
            //var logList = await _attendancesAppService.GetViewAttendanceLogRecordAPPList(new ViewAttendanceLogRecordRequestDto() { Keyword = user.UserName, StartDate = input.AttDate.ToDate(), EndDate = input.AttDate.ToDate() });
            //if (logList.Count() <= 0)
            //{
            //    return new JsonResultModel() { Message = $"申请失败！一天之内没有任何打卡记录，不能申请忘记打卡！", Success = false };
            //}

        }
        #endregion
    }
}

