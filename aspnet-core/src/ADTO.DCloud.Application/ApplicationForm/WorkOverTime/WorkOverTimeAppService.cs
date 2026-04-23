using ADTO.DCloud.ApplicationForm.Att;
using ADTO.DCloud.ApplicationForm.Att.Dto;
using ADTO.DCloud.ApplicationForm.WorkOverTime.Dto;
using ADTO.DCloud.ApplicationForm.Out.Dto;
using ADTO.DCloud.Authorization;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.DataAuthorizes;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.WorkFlow.NodeMethod;
using ADTO.DCloud.WorkFlow.Processes;
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
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks; 


namespace ADTO.DCloud.ApplicationForm.WorkOverTime
{
    /// <summary>
    /// 加班申请
    /// </summary>
    [ADTOSharpAuthorize]
    public class WorkOverTimeAppService : DCloudAppServiceBase, IWorkOverTimeAppService
    {
        private readonly IRepository<Adto_WorkOverTime, Guid> _repository;
        private readonly IRepository<OrganizationUnit, Guid> _orgRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IRepository<WorkFlowProcess, Guid> _processRepository;
        private readonly DataFilterService _dataAuthorizesApp;
        public WorkOverTimeAppService(IRepository<Adto_WorkOverTime, Guid> repository,
              IRepository<OrganizationUnit, Guid> orgRepository,
              IRepository<User, Guid> userRepository,
              DataFilterService dataAuthorizesApp,
              IRepository<WorkFlowProcess, Guid> processRepository)
        {
            _repository = repository;
            _orgRepository = orgRepository;
            _userRepository = userRepository;
            _processRepository = processRepository;
            _dataAuthorizesApp = dataAuthorizesApp;
        }

        #region 获取数据
        /// <summary>
        /// 单据管理-加班申请单据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DataAuthPermission("GetWorkOverTimePagedList")]
        public async Task<PagedResultDto<WorkOverTimeDto>> GetWorkOverTimePagedList(GetWorkOverTimePagedListInput input)
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
            //            join u in this._userRepository.GetAll() on r.UserId equals u.Id into r_u
            //            from uu in r_u.DefaultIfEmpty()
            //            join d in this._orgRepository.GetAll() on uu.DepartmentId equals d.Id into r_d
            //            from dd in r_d.DefaultIfEmpty()
            //            join c in this._orgRepository.GetAll() on uu.CompanyId equals c.Id into c_d
            //            from com in c_d.DefaultIfEmpty()
            //            join p in this._processRepository.GetAll() on r.Id equals p.Id
            var query = from r in this._repository.GetAll()
                        join p in this._processRepository.GetAll() on r.Id equals p.Id into pro
                        from process in pro.DefaultIfEmpty()
                        join uu in this._userRepository.GetAll() on r.UserID equals uu.Id
                        join d in this._orgRepository.GetAll() on uu.DepartmentId equals d.Id
                        join com in this._orgRepository.GetAll() on uu.CompanyId equals com.Id
                        where r.IsDeleted.Equals(false)
                        select new WorkOverTimeDto
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
                            CreationTime = r.CreationTime,
                            CreatorUserId = r.CreatorUserId,
                            Remark=r.Remark,
                            StartDateTime=r.StartDateTime,
                            EndDateTime=r.EndDateTime,
                            Times=r.Times,
                            IsFinished = process == null ? 0 : process.IsFinished ?? 0,//== 1 ? "结束" : "审核中",
                            SchemeCode = process == null ? "" : process.SchemeCode
                        };
            query = query.Where(x => x.CreationTime >= input.StartDate && x.CreationTime <= input.EndDate)
                     .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), p => p.UserName == input.Keyword || p.Name.Contains(input.Keyword))
                     .WhereIf(!string.IsNullOrWhiteSpace(input.Title), p => p.Title.Contains(input.Title))
                    ;

            string permissionCode = PermissionHelper.GetPermissionCode(this.GetType().GetMethod(nameof(GetWorkOverTimePagedList)));
            //数据权限
            query = await _dataAuthorizesApp.CreateDataFilteredQuery(query, permissionCode);

            //自定义排序
            if (!string.IsNullOrWhiteSpace(input.Sorting))
                query = query.OrderBy(input.Sorting);
            //获取总数
            var resultCount = await query.CountAsync();
            var taskList = query.PageBy(input).ToList();
            return new PagedResultDto<WorkOverTimeDto>(resultCount, taskList);
        }
        /// <summary>
        /// 获取加班申请数据列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<WorkOverTimeDto>> GetAllAsync(GetWorkOverTimeInput input)
        {
            var query = _repository.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.KeyWord), q => q.Title.Contains(input.KeyWord));
            var list = await query.ToListAsync();
            var listDtos = ObjectMapper.Map<List<WorkOverTimeDto>>(list);
            return listDtos;
        }


        /// <summary>
        /// 获取分页列表加班申请数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<WorkOverTimeDto>> GetAllPageListAsync(GetWorkOverTimePagedInput input)
        {
            var query = _repository.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.Filter), q => q.Title.Contains(input.Filter));
            if (input.Sorting != null)
                query = query.OrderBy(input.Sorting);
            var totalCount = query.Count();
            var list = await query.PageBy(input).ToListAsync();
            var listDtos = ObjectMapper.Map<List<WorkOverTimeDto>>(list);
            return new PagedResultDto<WorkOverTimeDto>(totalCount, listDtos);
        }

        /// <summary>
        /// 获取流程设计(依流程Id)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<WorkOverTimeDto> GetAsync(EntityDto<Guid> input)
        {
            var scheme = await _repository.GetAsync(input.Id);
            return ObjectMapper.Map<WorkOverTimeDto>(scheme);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 新增加班申请
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<WorkOverTimeDto> CreateAsync(CreateWorkOverTimeDto input)
        {
            try
            {
                var dto = ObjectMapper.Map<Adto_WorkOverTime>(input);
                await _repository.InsertAsync(dto);
                CurrentUnitOfWork.SaveChanges();
                return ObjectMapper.Map<WorkOverTimeDto>(dto);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        /// <summary>
        /// 修改加班申请
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<WorkOverTimeDto> UpdateAsync(UpdateWorkOverTimeDto input)
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
        /// 删除加班申请
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
            var dto = input.Data.ToObject<CreateWorkOverTimeDto>();
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
            var dto = input.Data.ToObject<UpdateWorkOverTimeDto>();
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
    }
}
