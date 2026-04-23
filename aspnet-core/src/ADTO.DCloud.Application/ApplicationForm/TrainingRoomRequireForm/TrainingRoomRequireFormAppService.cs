using ADTO.DCloud.ApplicationForm.Out.Dto;
using ADTO.DCloud.ApplicationForm.TrainingRoomRequireForm.Dto;
using ADTO.DCloud.ApplicationForm.UseCar.Dto;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.DataAuthorizes;
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
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm.TrainingRoomRequireForm
{

    /// <summary>
    /// 会议室申请
    /// </summary>

    [ADTOSharpAuthorize]
    public class TrainingRoomRequireFormAppService : DCloudAppServiceBase, ITrainingRoomRequireFormAppService
    {
        private readonly IRepository<Adto_TrainingRoomRequireForm, Guid> _repository;
        private readonly IRepository<OrganizationUnit, Guid> _orgRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IRepository<WorkFlowProcess, Guid> _processRepository;
        private readonly DataFilterService _dataAuthorizesApp;
        public TrainingRoomRequireFormAppService(IRepository<Adto_TrainingRoomRequireForm, Guid> repository,
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
        /// 获取分页列表用户会议室申请数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DataAuthPermission("GetTrainingRoomPageList")]
        public async Task<PagedResultDto<TrainingRoomRequireFormDto>> GetTrainingRoomPageList(GetTrainingRoomPagedInput input)
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
            var query = from r in this._repository.GetAll()
                        join p in this._processRepository.GetAll() on r.Id equals p.Id into p_d
                        from process in p_d.DefaultIfEmpty()
                        join uu in this._userRepository.GetAll() on r.UserId equals uu.Id
                        join d in this._orgRepository.GetAll() on r.DepartmentId equals d.Id
                        join com in this._orgRepository.GetAll() on r.CompanyId equals com.Id
                        where r.IsDeleted.Equals(false)
                        select new TrainingRoomRequireFormDto
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
                            Participants = r.Participants,
                            CreationTime = r.CreationTime,
                            CreatorUserId = r.CreatorUserId,
                            Equipment = r.Equipment,
                            TrainingRoomName = r.TrainingRoomName,
                            Type = r.Type,
                            Area = r.Area,
                            SDate = r.SDate,
                            EDate = r.EDate,
                            IsFinished = process == null ? 0 : process.IsFinished ?? 0,//== 1 ? "结束" : "审核中",
                            SchemeCode = process == null ? "" : process.SchemeCode
                        };
            query = query.Where(a => (DateTime.Compare(Convert.ToDateTime(input.StartDate), a.SDate) >= 0 && DateTime.Compare(Convert.ToDateTime(input.StartDate), a.EDate) <= 0)
                     || (DateTime.Compare(Convert.ToDateTime(input.EndDate), a.SDate) >= 0 && DateTime.Compare(Convert.ToDateTime(input.EndDate), a.EDate) <= 0)
                     || (DateTime.Compare(a.SDate, Convert.ToDateTime(input.StartDate)) >= 0 && DateTime.Compare(a.SDate, Convert.ToDateTime(input.EndDate)) <= 0)
                     || (DateTime.Compare(a.EDate, Convert.ToDateTime(input.StartDate)) >= 0 && DateTime.Compare(a.EDate, Convert.ToDateTime(input.EndDate)) <= 0)
                     ).WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), p => p.UserName == input.Keyword || p.Name.Contains(input.Keyword))
                     .WhereIf(!string.IsNullOrWhiteSpace(input.Title), p => p.Title.Contains(input.Title))
                     .WhereIf(!string.IsNullOrWhiteSpace(input.CompanyId.ToString()), p => p.CompanyId == input.CompanyId)
                     .WhereIf(input.DepartmentId != null, p => p.DepartmentId == input.DepartmentId)
                     .WhereIf(!string.IsNullOrWhiteSpace(input.Participants), x => x.Participants.Contains(input.Participants))//参与对象
                     .WhereIf(!string.IsNullOrWhiteSpace(input.Area1), x => x.Area.Contains(input.Area1))//区域
                     .WhereIf(!string.IsNullOrWhiteSpace(input.TrainingRoomName), x => x.TrainingRoomName.Contains(input.TrainingRoomName))//会议室
                     .WhereIf(!string.IsNullOrWhiteSpace(input.Type), x => x.Type.Contains(input.Type))//会议室类型
                    ;

            string permissionCode = PermissionHelper.GetPermissionCode(this.GetType().GetMethod(nameof(GetTrainingRoomPageList)));
            //数据权限
            query = await _dataAuthorizesApp.CreateDataFilteredQuery(query, permissionCode);

            //自定义排序
            if (!string.IsNullOrWhiteSpace(input.Sorting))
                query = query.OrderBy(input.Sorting);

            //获取总数
            var resultCount = await query.CountAsync();
            var taskList = query.PageBy(input).ToList();
            return new PagedResultDto<TrainingRoomRequireFormDto>(resultCount, taskList);
        }
        /// <summary>
        /// 获取会议室申请申请单数据列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TrainingRoomRequireFormDto>> GetAllAsync(GetTrainingRoomRequireFormInput input)
        {
            var query = _repository.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.Filter), q => q.Title.Contains(input.Filter));
            var list = await query.ToListAsync();
            var listDtos = ObjectMapper.Map<List<TrainingRoomRequireFormDto>>(list);
            return listDtos;
        }


        /// <summary>
        /// 获取分页列表会议室申请申请单数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<TrainingRoomRequireFormDto>> GetAllPageListAsync(GetTrainingRoomRequireFormPagedInput input)
        {
            var query = _repository.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.Filter), q => q.Title.Contains(input.Filter));
            if (input.Sorting != null)
                query = query.OrderBy(input.Sorting);
            var totalCount = query.Count();
            var list = await query.PageBy(input).ToListAsync();
            var listDtos = ObjectMapper.Map<List<TrainingRoomRequireFormDto>>(list);
            return new PagedResultDto<TrainingRoomRequireFormDto>(totalCount, listDtos);
        }

        /// <summary>
        /// 获取流程设计(依流程Id)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<TrainingRoomRequireFormDto> GetAsync(EntityDto<Guid> input)
        {
            var scheme = await _repository.GetAsync(input.Id);
            return ObjectMapper.Map<TrainingRoomRequireFormDto>(scheme);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 新增会议室申请申请单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<TrainingRoomRequireFormDto> CreateAsync(CreateTrainingRoomRequireFormDto input)
        {
            try
            {

                var dto = ObjectMapper.Map<Adto_TrainingRoomRequireForm>(input);
                await _repository.InsertAsync(dto);
                CurrentUnitOfWork.SaveChanges();
                return ObjectMapper.Map<TrainingRoomRequireFormDto>(dto);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        /// <summary>
        /// 修改会议室申请申请单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<TrainingRoomRequireFormDto> UpdateAsync(UpdateTrainingRoomRequireFormDto input)
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
        /// 删除会议室申请申请单
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
            var dto = input.Data.ToObject<CreateTrainingRoomRequireFormDto>();
            //表单验证
            await this.RoomCheckValidity(dto);
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
            var dto = input.Data.ToObject<UpdateTrainingRoomRequireFormDto>();
            //表单验证
            await this.RoomCheckValidity(dto);
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
        /// 校验会议室申请单合法性
        /// 时间判断
        /// </summary>
        /// <returns></returns>
        public async Task<bool> RoomCheckValidity<T>(T dto) where T : ITrainingRoomDto
        {
            if (!ADTOSharpSession.UserId.HasValue)
                throw new ADTOSharpException(L("AbnormalUserLoginStatus"));
            if (string.IsNullOrWhiteSpace(dto.TrainingRoomName))
                throw new ADTOSharpException(L("TrainingRoomRequireForm.ApplicationFailedPleaseSelectAMeetingRoom"));//申请失败：请选择会议室
            if (dto.SDate == DateTime.MinValue || dto.EDate == DateTime.MinValue)
                throw new ADTOSharpException(L("TrainingRoomRequireForm.ApplicationFailedDateCannotBeEmpty"));//申请失败：日期不能为空

            DateTime startDate = dto.SDate;
            DateTime endDate = dto.EDate;
            if (Math.Abs(endDate.Subtract(startDate).Days) != 0)
                throw new ADTOSharpException(L("TrainingRoomRequireForm.ApplicationFailedMeetingRoomsCannotBeAppliedForAcrossDays"));//申请失败：会议室不能跨天申请

            if (dto.TrainingRoomName == "1105" && (startDate.Hour < 10 || endDate.Hour < 10))
                throw new ADTOSharpException(L("TrainingRoomRequireForm.ApplicationFailedAlreadyAppliedbytheInternationalBusinessCenter"));//申请失败：已被国际业务中心申请
            var trainingRoomList = await _repository.GetAll().Where(t => t.TrainingRoomName == dto.TrainingRoomName &&
                   ((DateTime.Compare(Convert.ToDateTime(startDate), t.SDate) >= 0 && DateTime.Compare(Convert.ToDateTime(startDate), t.EDate) <= 0)
                  || (DateTime.Compare(Convert.ToDateTime(endDate), t.SDate) >= 0 && DateTime.Compare(Convert.ToDateTime(endDate), t.EDate) <= 0)
                  || (DateTime.Compare(t.SDate, Convert.ToDateTime(startDate)) >= 0 && DateTime.Compare(t.SDate, Convert.ToDateTime(endDate)) <= 0)
                  || (DateTime.Compare(t.EDate, Convert.ToDateTime(startDate)) >= 0 && DateTime.Compare(t.EDate, Convert.ToDateTime(endDate)) <= 0))
                  ).ToListAsync();
            if (trainingRoomList.Count > 0)
            {
                throw new ADTOSharpException(L("TrainingRoomRequireForm.ApplicationFailedTheConferenceRoomhasbeenappliedforduringthistimeperiod", startDate, endDate));//申请了此会议室
            }
            return true;
        }
        #endregion
    }
}

