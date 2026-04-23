using ADTO.DCloud.Attendances.AttendanceLocations.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Runtime.Caching;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;



namespace ADTO.DCloud.Attendances.AttendanceLocations
{
    /// <summary>
    /// 办公地点管理
    /// </summary>
    public class AttendanceLocationsAppService : DCloudAppServiceBase, IAttendanceLocationsAppService
    {
        private readonly IRepository<AttendanceLocation, Guid> _repository;
        private readonly ICacheManager _cacheManager;
        public AttendanceLocationsAppService(IRepository<AttendanceLocation, Guid> repository, ICacheManager cacheManager)
        {
            _repository = repository;
            _cacheManager = cacheManager;
        }


        #region 获取数据
        /// <summary>
        /// 获取办公地点数据列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AttendanceLocationDto>> GetAllAsync(GetAttendanceLocationInput input)
        {
            var cacheManager = _cacheManager.GetCache($"ADTO.DCloud.Attendance");
            var cacheKey = $"AttendanceLocation.GetAll";
            var cacheVal = await cacheManager.GetOrDefaultAsync(cacheKey) as IEnumerable<AttendanceLocationDto>;
            if (cacheVal == null || cacheVal.Count() < 1)
            {
                var query = _repository.GetAll().Where(d => d.IsActive == true)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), q => q.LocationName.Contains(input.Keyword));
                if (input.Sorting != null)
                    query = query.OrderBy(input.Sorting);
                var list = await query.OrderBy(o => o.DisplayOrder).OrderByDescending(d => d.CreationTime).ToListAsync();
                var listDtos = ObjectMapper.Map<List<AttendanceLocationDto>>(list);
                await cacheManager.SetAsync(cacheKey, listDtos);
                return listDtos;
            }
            else
            {
                return cacheVal;
            }
        }


        /// <summary>
        /// 获取分页列表办公地点数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<AttendanceLocationDto>> GetAllPageListAsync(GetAttendanceLocationPagedInput input)
        {
            var query = _repository.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), q => q.LocationName.Contains(input.Keyword))
                .WhereIf(input.IsActive.HasValue, q => q.IsActive == input.IsActive);
            if (input.Sorting != null)
                query = query.OrderBy(input.Sorting);
            var totalCount = query.Count();
            var list = await query.PageBy(input).ToListAsync();
            var listDtos = ObjectMapper.Map<List<AttendanceLocationDto>>(list);
            return new PagedResultDto<AttendanceLocationDto>(totalCount, listDtos);
        }

        /// <summary>
        /// 获取办公地点(依办公地点Id)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<AttendanceLocationDto> GetAsync(EntityDto<Guid> input)
        {
            var scheme = await _repository.GetAsync(input.Id);
            return ObjectMapper.Map<AttendanceLocationDto>(scheme);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 新增办公地点
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<AttendanceLocationDto> CreateAsync(CreateAttendanceLocation input)
        {
            var dto = ObjectMapper.Map<AttendanceLocation>(input);
            await _repository.InsertAsync(dto);
            CurrentUnitOfWork.SaveChanges();
            return ObjectMapper.Map<AttendanceLocationDto>(dto);
        }
        /// <summary>
        /// 修改办公地点
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<AttendanceLocationDto> UpdateAsync(UpdateAttendanceLocationDto input)
        {
            var entity = await _repository.GetAsync(input.Id);
            ObjectMapper.Map(input, entity);
            await _repository.UpdateAsync(entity);

            var cacheManager = _cacheManager.GetCache($"ADTO.DCloud.Attendance");
            await cacheManager.RemoveAsync($"AttendanceLocation.GetAll");
            return await GetAsync(new EntityDto<Guid>() { Id = entity.Id });
        }
        /// <summary>
        /// 删除办公地点
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[HttpDelete("workflow/scheme/{id}")]
        public async Task DeleteAsync(EntityDto<Guid> input)
        {
            await _repository.DeleteAsync(input.Id);
            var cacheManager = _cacheManager.GetCache($"ADTO.DCloud.Attendance");
            await cacheManager.RemoveAsync($"AttendanceLocation.GetAll");
        }
        /// <summary>
        /// 修改办公地点禁用启用状态
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<AttendanceLocationDto> UpdateStateAsync(UpdateStateDto input)
        {
            var entity = await _repository.GetAsync(input.Id);
            entity.IsActive = input.IsActive;
            await _repository.UpdateAsync(entity);
            var cacheManager = _cacheManager.GetCache($"ADTO.DCloud.Attendance");
            await cacheManager.RemoveAsync($"AttendanceLocation.GetAll");
            return await GetAsync(new EntityDto<Guid>() { Id = entity.Id });
        }
        #endregion


    }
}
