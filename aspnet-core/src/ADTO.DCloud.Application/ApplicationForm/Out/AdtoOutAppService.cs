using ADTO.DCloud.ApplicationForm.Abs.Dto;
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
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm.Out
{
    [ADTOSharpAuthorize]
    public class AdtoOutAppService : DCloudAppServiceBase, IAdtoOutAppService
    {
        private readonly IRepository<Adto_Out, Guid> _repository;
        private readonly IApplicationFormCheckDateAppService _applicationFormAppService;

        private readonly IRepository<OrganizationUnit, Guid> _orgRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IRepository<WorkFlowProcess, Guid> _processRepository;
        private readonly DataFilterService _dataAuthorizesApp;
        public AdtoOutAppService(IRepository<Adto_Out, Guid> repository,
            IApplicationFormCheckDateAppService applicationFormAppService,
              IRepository<OrganizationUnit, Guid> orgRepository,
              IRepository<User, Guid> userRepository,
              DataFilterService dataAuthorizesApp,
              IRepository<WorkFlowProcess, Guid> processRepository)
        {
            _repository = repository;
            _applicationFormAppService = applicationFormAppService;
            _orgRepository = orgRepository;
            _userRepository = userRepository;
            _processRepository = processRepository;
            _dataAuthorizesApp = dataAuthorizesApp;
        }

        #region 获取数据

        /// <summary>
        /// 获取外出申请单数据列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AdtoOutDto>> GetAllAsync(GetAdtoOutInput input)
        {
            var query = _repository.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.Filter), q => q.Title.Contains(input.Filter));
            var list = await query.ToListAsync();
            var listDtos = ObjectMapper.Map<List<AdtoOutDto>>(list);
            return listDtos;
        }


        /// <summary>
        /// 获取分页列表外出申请单数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<AdtoOutDto>> GetAllPageListAsync(GetAdtoOutPageInput input)
        {
            var query = _repository.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), q => q.Title.Contains(input.Keyword));
            if (input.Sorting != null)
                query = query.OrderBy(input.Sorting);
            var totalCount = query.Count();
            var list = await query.PageBy(input).ToListAsync();
            var listDtos = ObjectMapper.Map<List<AdtoOutDto>>(list);
            return new PagedResultDto<AdtoOutDto>(totalCount, listDtos);
        }
        /// <summary>
        /// 单据管理-外出申请单据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DataAuthPermission("GetOutPagedList")]
        public async Task<PagedResultDto<AdtoOutDto>> GetOutPagedList(GetAdtoOutPageInput input)
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
                        join uu in this._userRepository.GetAll() on r.UserID equals uu.Id
                        join d in this._orgRepository.GetAll() on uu.DepartmentId equals d.Id
                        join com in this._orgRepository.GetAll() on uu.CompanyId equals com.Id
                        where r.IsDeleted.Equals(false)
                        select new AdtoOutDto
                        {
                            Id = r.Id,
                            Title = r.Title,
                            UserId = r.UserID,
                            UserName = uu.UserName,
                            Name = uu.Name,
                            CompanyId = com.Id,
                            CompanyName = com.DisplayName,
                            DepartmentId = r.DepartmentId,
                            DepartmentName = d.DisplayName,
                            StartDate = r.StartDate,
                            EndDate = r.EndDate,
                            Remark = r.Remark,
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
            string permissionCode = PermissionHelper.GetPermissionCode(this.GetType().GetMethod(nameof(GetOutPagedList)));
            //数据权限
            query = await _dataAuthorizesApp.CreateDataFilteredQuery(query, permissionCode);

            //自定义排序
            if (!string.IsNullOrWhiteSpace(input.Sorting))
                query = query.OrderBy(input.Sorting);

            //获取总数
            var resultCount = await query.CountAsync();
            var taskList = query.PageBy(input).ToList();
            return new PagedResultDto<AdtoOutDto>(resultCount, taskList);
        }
        /// <summary>
        /// 获取外出数据列表-计算考勤机异常
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AdtoOutAttendanceDto>> GetOutListAsync(GetAdtoAbsInput input)
        {
            var query = from r in this._repository.GetAll()
                        join pro in this._processRepository.GetAll() on r.Id equals pro.Id
                        join user in this._userRepository.GetAll() on r.UserID equals user.Id
                        select new AdtoOutAttendanceDto
                        {
                            Id = r.Id,
                            UserID = r.UserID,
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
            var listDtos = ObjectMapper.Map<List<AdtoOutAttendanceDto>>(list);
            return listDtos;
        }
        /// <summary>
        /// 获取流程设计(依流程Id)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<AdtoOutDto> GetAsync(EntityDto<Guid> input)
        {
            var scheme = await _repository.GetAsync(input.Id);
            return ObjectMapper.Map<AdtoOutDto>(scheme);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 新增外出申请单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<AdtoOutDto> CreateAsync(CreateAdtoOutDto input)
        {
            try
            {

                var dto = ObjectMapper.Map<Adto_Out>(input);
                await _repository.InsertAsync(dto);
                CurrentUnitOfWork.SaveChanges();
                return ObjectMapper.Map<AdtoOutDto>(dto);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        /// <summary>
        /// 修改外出申请单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<AdtoOutDto> UpdateAsync(UpdateAdtoOutDto input)
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
        /// 删除外出申请单
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
            var dto = input.Data.ToObject<CreateAdtoOutDto>();
            await TaskOutCheckValidity(new OutCheckValidityDto() { UserId = dto.UserID, StartDate = dto.StartDate, EndDate = dto.EndDate });
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
            var dto = input.Data.ToObject<UpdateAdtoOutDto>();
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
        #endregion

        #region 表单验证
        /// <summary>
        /// 校验外出申请单合法性
        /// 判断是否存在相同时间段的外出申请单\不允许超过一天
        /// </summary>
        /// <returns></returns>
        public async Task TaskOutCheckValidity(OutCheckValidityDto input)
        {
            if (ADTOSharpSession.UserId == null)
            {
                throw new ADTOSharpException(L("申请失败：用户登录状态异常"));
            }
            Guid userId = input.UserId.HasValue ? input.UserId.Value : ADTOSharpSession.UserId.Value;
            if (input.StartDate == null || input.EndDate == null)
            {
                throw new ADTOSharpException(L("申请失败：日期不能为空"));
            }
            DateTime startDate = input.StartDate.Value;
            DateTime endDate = input.EndDate.Value;
            if (DateTime.Compare(endDate, startDate) <= 0)
            {
                throw new ADTOSharpException(L("申请失败：结束日期不能小于开始日期,请检查再提交"));
            }
            var vDays = endDate.ToString("yyyy-MM-dd").ToDate().Subtract(startDate.ToString("yyyy-MM-dd").ToDate()).Days;
            //不允许超过一天
            if (vDays > 0)
            {
                throw new ADTOSharpException(L("申请失败：申请天数不能超过1天"));
            }
            bool isdateresult = await _applicationFormAppService.IsDateValid(startDate, "Out");
            //判断申请日期是否为上个月的考勤数据
            if (isdateresult)
            {
                throw new ADTOSharpException(L("ApplicationFailedTheDeadlineForSubmissionHaPassed"));
            }

            await _applicationFormAppService.IsExistApplication(userId, startDate, endDate, input.Id);

            //获取用户是否有被禁止申请外出流程
            var disableUserWrokFlows = await _applicationFormAppService.GetDisableUserListByUserId(userId, "Adto_Out");
            if (disableUserWrokFlows.Count() > 0)
            {
                throw new ADTOSharpException(L("很抱歉，您不能申请此流程，请在公司钉钉团队里面进行签到，感谢您的配合！"));
            }
        }
        #endregion
    }
}
