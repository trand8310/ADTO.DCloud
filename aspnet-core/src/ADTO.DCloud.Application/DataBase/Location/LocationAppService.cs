using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTO.DCloud.AreaBase;
using ADTO.DCloud.DataBase.Location.Dto;
using ADTOSharp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;

namespace ADTO.DCloud.DataBase.Location
{
    public class LocationAppService : DCloudAppServiceBase, ILocationAppService
    {
        #region Fields
        private readonly IRepository<Base_Area, Guid> _areaRepository;
        private readonly IRepository<Base_Country, Guid> _countryRepository;
        private readonly IRepository<Base_Province, Guid> _provinceRepository;
        private readonly IRepository<Base_City, Guid> _cityRepository;
        private readonly IRepository<Base_County, Guid> _countyRepository;

        #endregion

        #region Ctor
        public LocationAppService(
              IRepository<Base_Area, Guid> areaRepository
            , IRepository<Base_City, Guid> cityRepository
            , IRepository<Base_Country, Guid> countryRepository
            , IRepository<Base_County, Guid> countyRepository
            , IRepository<Base_Province, Guid> provinceRepository
           )
        {
            _areaRepository = areaRepository;
            _cityRepository = cityRepository;
            _countryRepository = countryRepository;
            _countyRepository = countyRepository;
            _provinceRepository = provinceRepository;
        }
        #endregion

        /// <summary>
        /// 获取区域国家省市区联动数据，动态加载
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
       
        public async Task<List<CascaderDto>> GetRegionChildren(Guid? parentId, int level)
        {
            if (level < 0 || level > 4)
                throw new ArgumentException("Level must be between 0 and 4", nameof(level));
            return level switch
            {
                // 第一级：大区
                0 => await _areaRepository.GetAll()
                    .Select(a => new CascaderDto { Value = a.Id, Label = a.Name, Leaf = !_countryRepository.GetAll().Any(p => p.AreaId == a.Id) })
                    .ToListAsync(),

                // 第二级：国家
                1 => await _countryRepository.GetAll()
                    .Where(c => c.AreaId == parentId)
                    .Select(c => new CascaderDto { Value = c.Id, Label = c.Name, Leaf = !_provinceRepository.GetAll().Any(p => p.CountryId == c.Id) })
                    .ToListAsync(),

                // 第三级：省份
                2 => await _provinceRepository.GetAll()
                    .Where(c => c.CountryId == parentId)
                    .Select(c => new CascaderDto { Value = c.Id, Label = c.Name, Leaf = ! _cityRepository.GetAll().Any(p => p.ProvinceId == c.Id) })
                    .ToListAsync(),

                // 第四级：城市
                3 => await _cityRepository.GetAll()
                    .Where(c => c.ProvinceId == parentId)
                    .Select(c => new CascaderDto { Value = c.Id, Label = c.Name, Leaf = !_countyRepository.GetAll().Any(p => p.CityId == c.Id) })
                    .ToListAsync(),

                // 第五级：区县 // 区县默认是最后一级
                4 => await _countyRepository.GetAll()
                    .Where(c => c.CityId == parentId)
                    .Select(c => new CascaderDto { Value = c.Id, Label = c.Name, Leaf = true })
                    .ToListAsync(),

                _ => throw new ArgumentException("Invalid level")
            };
        }

        /// <summary>
        /// 实现获取区域最后一级中文名称(因为层级不确定)
        /// </summary>
        /// <param name="areaId"></param>
        /// <param name="countryId"></param>
        /// <param name="provinceId"></param>
        /// <param name="cityId"></param>
        /// <param name="countyId"></param>
        /// <returns></returns>
        public async Task<string> GetLastLevelNameAsync(Guid areaId, Guid countryId, Guid? provinceId, Guid? cityId, Guid? countyId)
        {
            // 假设你的 DbContext 中有对应的 DbSet
            var lastLevelName = await _countyRepository.GetAll()
                .Where(c => c.Id == countyId)
                .Select(c => c.Name)
                .FirstOrDefaultAsync();

            if (lastLevelName != null) return lastLevelName;

            lastLevelName = await _cityRepository.GetAll()
                .Where(c => c.Id == cityId)
                .Select(c => c.Name)
                .FirstOrDefaultAsync();

            if (lastLevelName != null) return lastLevelName;

            lastLevelName = await _provinceRepository.GetAll()
                .Where(p => p.Id == provinceId)
                .Select(p => p.Name)
                .FirstOrDefaultAsync();

            if (lastLevelName != null) return lastLevelName;

            lastLevelName = await _countryRepository.GetAll()
                .Where(c => c.Id == countryId)
                .Select(c => c.Name)
                .FirstOrDefaultAsync();

            if (lastLevelName != null) return lastLevelName;

            return await _areaRepository.GetAll()
                .Where(a => a.Id == areaId)
                .Select(a => a.Name)
                .FirstOrDefaultAsync() ?? "未知区域";
        }
    }
}

