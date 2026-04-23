using ADTO.DCloud.Attendances.AttendanceLocations.Dto;
using ADTO.DCloud.Attendances.AttendanceTimeRules.Dto;
using ADTO.DCloud.Attendances;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Runtime.Caching;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Attendances.AttendanceTimeRules
{
    /// <summary>
    /// 考勤时间规则
    /// </summary>
    public class AttendanceTimeRuleAppService : DCloudAppServiceBase, IAttendanceTimeRuleAppService
    {
        private readonly IRepository<AttendanceTimeRule, Guid> _repository;
        private readonly ICacheManager _cacheManager;
        public AttendanceTimeRuleAppService(IRepository<AttendanceTimeRule, Guid> repository,
            ICacheManager cacheManager)
        {
            _repository = repository;
            _cacheManager = cacheManager;
        }


        #region 获取数据
        /// <summary>
        /// 获取考勤时间数据列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AttendanceTimeRuleDto>> GetAllAsync(GetAttendanceTimeRuleInput input)
        {
            var cacheManager = _cacheManager.GetCache($"ADTO.DCloud.Attendance");
            var cacheKey = $".AttendanceTimeRule.GetAll.{input.KeyWord}";
            var cacheVal = await cacheManager.GetOrDefaultAsync(cacheKey) as IEnumerable<AttendanceTimeRuleDto>;
            if (cacheVal == null || cacheVal.Count() <= 0)
            {
                var query = _repository.GetAll().Where(d => d.IsActive == true)
                .WhereIf(!string.IsNullOrWhiteSpace(input.KeyWord), q => q.RuleName.Contains(input.KeyWord));
                if (input.Sorting != null)
                    query = query.OrderBy(input.Sorting);
                var list = await query.ToListAsync();
                var listDtos = ObjectMapper.Map<List<AttendanceTimeRuleDto>>(list);
                await cacheManager.SetAsync(cacheKey, listDtos);
                return listDtos;
            }
            else
            {
                return cacheVal;
            }
        }


        /// <summary>
        /// 获取分页列表考勤时间数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<AttendanceTimeRuleDto>> GetAllPageListAsync(GetAttendanceTimeRulePagedInput input)
        {
            var query = _repository.GetAll().Where(d => d.IsActive == true)
                 .WhereIf(!string.IsNullOrWhiteSpace(input.KeyWord), q => q.RuleName.Contains(input.KeyWord));
            if (input.Sorting != null)
                query = query.OrderBy(input.Sorting);
            var totalCount = query.Count();
            var list = await query.PageBy(input).ToListAsync();
            var listDtos = ObjectMapper.Map<List<AttendanceTimeRuleDto>>(list);
            return new PagedResultDto<AttendanceTimeRuleDto>(totalCount, listDtos);
        }

        /// <summary>
        /// 获取考勤时间(依考勤时间Id)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<AttendanceTimeRuleDto> GetAsync(EntityDto<Guid> input)
        {
            var scheme = await _repository.GetAsync(input.Id);
            return ObjectMapper.Map<AttendanceTimeRuleDto>(scheme);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 新增考勤时间
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<AttendanceTimeRuleDto> CreateAsync(CreateAttendanceTimeRuleDto input)
        {
            var dto = ObjectMapper.Map<AttendanceTimeRule>(input);
            await _repository.InsertAsync(dto);
            CurrentUnitOfWork.SaveChanges();
            return ObjectMapper.Map<AttendanceTimeRuleDto>(dto);
        }
        /// <summary>
        /// 修改考勤时间
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<AttendanceTimeRuleDto> UpdateAsync(UpdateAttendanceTimeRuleDto input)
        {
            var entity = await _repository.GetAsync(input.Id);
            ObjectMapper.Map(input, entity);
            await _repository.UpdateAsync(entity);
            var cacheManager = _cacheManager.GetCache($"ADTO.DCloud.Attendance");
            await cacheManager.RemoveAsync(".AttendanceTimeRule.List");
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
            var cacheManager = _cacheManager.GetCache($"ADTO.DCloud.Attendance");
            await cacheManager.RemoveAsync(".AttendanceTimeRule.List");
        }
        /// <summary>
        /// 修改考勤时间禁用启用状态
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<AttendanceTimeRuleDto> UpdateStateAsync(UpdateStateDto input)
        {
            var entity = await _repository.GetAsync(input.Id);
            entity.IsActive = input.IsActive;
            await _repository.UpdateAsync(entity);

            var cacheManager = _cacheManager.GetCache($"ADTO.DCloud.Attendance");
            await cacheManager.RemoveAsync(".AttendanceTimeRule.List");

            return await GetAsync(new EntityDto<Guid>() { Id = entity.Id });
        }
        #endregion

        #region 扩展
        public async Task<string> GetSeason(DateTime dt)
        {
            string Winter = "10-1";
            string Summer = "05-1";
            DateTime WinterDate = DateTime.Parse(dt.ToString("yyyy-") + Winter);
            DateTime SunmerDate = DateTime.Parse(dt.ToString("yyyy-") + Summer);
            if (DateTime.Compare(dt, SunmerDate) >= 0 && DateTime.Compare(dt, WinterDate) < 0)
            {
                return "Summer";
            }
            else
            {
                return "Winter";
            }
        }
        #endregion
    }
}
