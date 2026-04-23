using ADTO.DCloud.ApplicationForm.TrainingRequireForm.Dto;
using ADTO.DCloud.ApplicationForm.TrainingRoomRequireForm.Dto;
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

namespace ADTO.DCloud.ApplicationForm.TrainingRequireForm
{
    /// <summary>
    /// 培训请假申请单
    /// </summary>

    [ADTOSharpAuthorize]
    public class TrainingRequireFormAppService : DCloudAppServiceBase, ITrainingRequireFormAppService
    {
        private readonly IRepository<Adto_TrainingRequireForm, Guid> _repository;
        private readonly IRepository<OrganizationUnit, Guid> _orgRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IRepository<WorkFlowProcess, Guid> _processRepository;
        private readonly DataFilterService _dataAuthorizesApp;
        public TrainingRequireFormAppService(IRepository<Adto_TrainingRequireForm, Guid> repository,
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
        /// 获取分页列表用户培训请假申请数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DataAuthPermission("GetTrainingPageList")]
        public async Task<PagedResultDto<TrainingRequireFormDto>> GetTrainingPageList(GetTrainingPagedInput input)
        {
            if (input.StartDate == null)
            {
                //如果日期为空，默认显示近一个月
                input.StartDate = DateTime.Now.AddMonths(-1);
            }
            //因为添加日期为年与日，否则不会包含今天数据
            if (input.EndDate != null)
            {
                input.EndDate = input.EndDate.Value.AddDays(1);
            }
            else
            {
                input.EndDate = DateTime.Now.AddDays(1);
            }
            var query = from r in this._repository.GetAll()
                        join p in this._processRepository.GetAll() on r.Id equals p.Id into p_d
                        from process in p_d.DefaultIfEmpty()
                        join uu in this._userRepository.GetAll() on r.UserId equals uu.Id
                        join d in this._orgRepository.GetAll() on r.DepartmentId equals d.Id
                        join com in this._orgRepository.GetAll() on r.CompanyId equals com.Id
                        where r.IsDeleted.Equals(false)
                        select new TrainingRequireFormDto
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
                            CreationTime = r.CreationTime,
                            CreatorUserId = r.CreatorUserId,
                            Remark=r.Remark,
                            StartDate=r.StartDate,
                            EndDate=r.EndDate,
                            IsFinished = process == null ? 0 : process.IsFinished ?? 0,//== 1 ? "结束" : "审核中",
                            SchemeCode = process == null ? "" : process.SchemeCode
                        };
            query = query.Where(a => (DateTime.Compare(Convert.ToDateTime(input.StartDate), a.StartDate) >= 0 && DateTime.Compare(Convert.ToDateTime(input.StartDate), a.EndDate) <= 0)
                     || (DateTime.Compare(Convert.ToDateTime(input.EndDate), a.StartDate) >= 0 && DateTime.Compare(Convert.ToDateTime(input.EndDate), a.EndDate) <= 0)
                     || (DateTime.Compare(a.StartDate, Convert.ToDateTime(input.StartDate)) >= 0 && DateTime.Compare(a.StartDate, Convert.ToDateTime(input.EndDate)) <= 0)
                     || (DateTime.Compare(a.EndDate, Convert.ToDateTime(input.StartDate)) >= 0 && DateTime.Compare(a.EndDate, Convert.ToDateTime(input.EndDate)) <= 0))
                     .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), p => p.UserName == input.Keyword || p.Name.Contains(input.Keyword))
                     .WhereIf(!string.IsNullOrWhiteSpace(input.Title), p => p.Title.Contains(input.Title))
                     .WhereIf(!string.IsNullOrWhiteSpace(input.CompanyId.ToString()), p => p.CompanyId == input.CompanyId)
                     .WhereIf(input.DepartmentId != null, p => p.DepartmentId == input.DepartmentId);

            string permissionCode = PermissionHelper.GetPermissionCode(this.GetType().GetMethod(nameof(GetTrainingPageList)));
            //数据权限
            query = await _dataAuthorizesApp.CreateDataFilteredQuery(query, permissionCode);

            //自定义排序
            if (!string.IsNullOrWhiteSpace(input.Sorting))
                query = query.OrderBy(input.Sorting);

            //获取总数
            var resultCount = await query.CountAsync();
            var taskList = query.PageBy(input).ToList();
            return new PagedResultDto<TrainingRequireFormDto>(resultCount, taskList);
        }

        /// <summary>
        /// 获取培训请假申请单数据列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TrainingRequireFormDto>> GetAllAsync(GetTrainingRequireFormInput input)
        {
            var query = _repository.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.Filter), q => q.Title.Contains(input.Filter));
            var list = await query.ToListAsync();
            var listDtos = ObjectMapper.Map<List<TrainingRequireFormDto>>(list);
            return listDtos;
        }


        /// <summary>
        /// 获取分页列表培训请假申请单数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<TrainingRequireFormDto>> GetAllPageListAsync(GetTrainingRequireFormPagedInput input)
        {
            var query = _repository.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.Filter), q => q.Title.Contains(input.Filter));
            if (input.Sorting != null)
                query = query.OrderBy(input.Sorting);
            var totalCount = query.Count();
            var list = await query.PageBy(input).ToListAsync();
            var listDtos = ObjectMapper.Map<List<TrainingRequireFormDto>>(list);
            return new PagedResultDto<TrainingRequireFormDto>(totalCount, listDtos);
        }

        /// <summary>
        /// 获取流程设计(依流程Id)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<TrainingRequireFormDto> GetAsync(EntityDto<Guid> input)
        {
            var scheme = await _repository.GetAsync(input.Id);
            return ObjectMapper.Map<TrainingRequireFormDto>(scheme);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 新增培训请假申请单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<TrainingRequireFormDto> CreateAsync(CreateTrainingRequireFormDto input)
        {
            try
            {

                var dto = ObjectMapper.Map<Adto_TrainingRequireForm>(input);
                await _repository.InsertAsync(dto);
                CurrentUnitOfWork.SaveChanges();
                return ObjectMapper.Map<TrainingRequireFormDto>(dto);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        /// <summary>
        /// 修改培训请假申请单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<TrainingRequireFormDto> UpdateAsync(UpdateTrainingRequireFormDto input)
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
        /// 删除培训请假申请单
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
            var dto = input.Data.ToObject<CreateTrainingRequireFormDto>();
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
            var dto = input.Data.ToObject<UpdateTrainingRequireFormDto>();
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
