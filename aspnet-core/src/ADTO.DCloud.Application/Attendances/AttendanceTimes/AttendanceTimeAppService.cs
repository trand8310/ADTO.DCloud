using ADTO.DCloud.Attendances.AttendanceLocations.Dto;
using ADTO.DCloud.Attendances.AttendanceTimeRules.Dto;
using ADTO.DCloud.Attendances.AttendanceTimes.Dto;
using ADTO.DCloud.Attendances;
using ADTO.DCloud.DataItem.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Runtime.Caching;
using AutoMapper.Internal.Mappers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Attendances.AttendanceTimes
{
    /// <summary>
    /// 考勤时间管理
    /// </summary>
    public class AttendanceTimeAppService : DCloudAppServiceBase, IAttendanceTimeAppService
    {
        private readonly IRepository<AttendanceTime, Guid> _repository;
        private readonly IRepository<AttendanceTimeRule, Guid> _ruleRepository;
        private readonly IRepository<AttendanceLocation, Guid> _locationRepository;
        private readonly ICacheManager _cacheManager;
        public AttendanceTimeAppService(IRepository<AttendanceTime, Guid> repository,
            IRepository<AttendanceTimeRule, Guid> ruleRepository,
            IRepository<AttendanceLocation, Guid> locationRepository,
            ICacheManager cacheManager)
        {
            _repository = repository;
            _ruleRepository = ruleRepository;
            _locationRepository = locationRepository;
            _cacheManager = cacheManager;
        }


        #region 获取数据

        /// <summary>
        /// 获取分页列表考勤时间数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<AttendanceTimeDto>> GetAllPageListAsync(GetAttendanceTimePagedInput input)
        {
            var query = _repository.GetAll().Where(d => d.IsActive == true)
            //.WhereIf(!string.IsNullOrWhiteSpace(input.KeyWord), q => q.Season.Contains(input.KeyWord) 
            //|| q.Location.LocationName.Contains(input.KeyWord) || q.AttendanceTimeRule.RuleName.Contains(input.KeyWord))
            //.WhereIf(input.LocationId.HasValue,q=>q.Location.Id.Equals(input.LocationId))
            .WhereIf(!string.IsNullOrWhiteSpace(input.KeyWord), q => q.Name.Contains(input.KeyWord) || q.Remark.Contains(input.KeyWord));
            if (input.Sorting != null)
                query = query.OrderBy(input.Sorting);
            var totalCount = query.Count();
            var list = await query.PageBy(input).ToListAsync();
            var listDtos = ObjectMapper.Map<List<AttendanceTimeDto>>(list);
            return new PagedResultDto<AttendanceTimeDto>(totalCount, listDtos);
        }
        /// <summary>
        /// 获取所有启用的列表考勤时间数据
        /// </summary>
        /// <returns></returns>
        public async Task<List<AttendanceTimeDto>> GetAttendanceListAsync()
        {
            //var cacheManager = _cacheManager.GetCache($"ADTO.DCloud");
            //var cacheKey = $".Attendance.GetAttendanceList";
            //var cacheVal = await cacheManager.GetOrDefaultAsync(cacheKey) as List<AttendanceTimeDto>;
            //if (cacheVal == null || cacheVal.Count() <= 0)
            //{
                var query = await _repository.GetAll().Where(d => d.IsActive == true).ToListAsync();
                var listDtos = ObjectMapper.Map<List<AttendanceTimeDto>>(query);
                //await cacheManager.SetAsync(cacheKey, listDtos);
                return listDtos;
            //}
            //else
            //{
            //    return cacheVal;
            //}
        }
        /// <summary>
        /// 获取考勤时间(依考勤时间Id)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<AttendanceTimeDto> GetAsync(EntityDto<Guid> input)
        {
            var scheme = await _repository.GetAsync(input.Id);
            return ObjectMapper.Map<AttendanceTimeDto>(scheme);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 新增考勤时间
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<AttendanceTimeDto> CreateAsync(CreateAttendanceTimeDto input)
        {
            var entity = ObjectMapper.Map<AttendanceTime>(input);
            //var loaction =await _locationRepository.GetAsync(input.LocationId);
            //entity.Location = loaction;
            //var rule =await _ruleRepository.GetAsync(input.AttendanceTimeRuleId);
            //entity.AttendanceTimeRule = rule;
            await _repository.InsertAsync(entity);
            CurrentUnitOfWork.SaveChanges();
            return ObjectMapper.Map<AttendanceTimeDto>(entity);
        }
        /// <summary>
        /// 修改考勤时间
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<AttendanceTimeDto> UpdateAsync(UpdateAttendanceTimeDto input)
        {
            var entity = await _repository.GetAsync(input.Id);
            ObjectMapper.Map(input, entity);
            //var loaction = await _locationRepository.GetAsync(input.LocationId);
            //entity.Location = loaction;

            //var rule = await _ruleRepository.GetAsync(input.AttendanceTimeRuleId);
            //entity.AttendanceTimeRule = rule;
            await _repository.UpdateAsync(entity);
            return await GetAsync(new EntityDto<Guid>() { Id = entity.Id });
        }
        /// <summary>
        /// 删除考勤时间
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteAsync(EntityDto<Guid> input)
        {
            await _repository.DeleteAsync(input.Id);
        }
        /// <summary>
        /// 修改考勤时间禁用启用状态
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<AttendanceTimeDto> UpdateStateAsync(UpdateStateDto input)
        {
            var entity = await _repository.GetAsync(input.Id);
            entity.IsActive = input.IsActive;
            await _repository.UpdateAsync(entity);
            return await GetAsync(new EntityDto<Guid>() { Id = entity.Id });
        }
        #endregion
    }
}
