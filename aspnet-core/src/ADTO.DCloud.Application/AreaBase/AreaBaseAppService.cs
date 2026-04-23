using ADTO.DCloud.AreaBase.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADTO.DCloud.AreaBase
{
    /// <summary>
    /// 大区、国家、省、市、区 相关接口
    /// </summary>
    [ADTOSharpAuthorize]
    public class AreaBaseAppService : DCloudAppServiceBase, IAreaBaseAppService
    {
        #region
        private readonly IRepository<Base_Area, Guid> _areaRepository;
        private readonly IRepository<Base_Country, Guid> _countryRepository;
        private readonly IRepository<Base_Province, Guid> _provinceRepository;
        private readonly IRepository<Base_City, Guid> _cityRepository;
        private readonly IRepository<Base_County, Guid> _countyRepository;

        public AreaBaseAppService(
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
        /// 获取区域国家省市区联动数据，一级一级动态加载
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<List<AreaResponseDto>> GetAreaTreeList(AreaQueryDto input)
        {
            if (input.Level < 1 || input.Level > 5)
            {
                throw new UserFriendlyException("无效的层级，仅支持1-5（1=大区/2=国家/3=省份/4=城市/5=区县）");
            }
            if (input.Level > 1 && !input.ParentId.HasValue)
            {
                throw new UserFriendlyException($"层级{input.Level}必须提供parentId");
            }
            var result = new List<AreaResponseDto>();

            switch (input.Level)
            {
                // 1. 加载大区（默认一级）
                case 1:
                    result = await _areaRepository.GetAll()
                        //.Where(a => a.ParentId == input.ParentId)
                        .Select(a => new AreaResponseDto
                        {
                            Id = a.Id,
                            Name = a.Name,
                            ParentId = null,
                            Level = 1,
                            Code = a.Code,
                            IsActive=a.IsActive,
                            // 判断是否有子级（国家）
                            HasChildren = _countryRepository.GetAll().Any(c => c.AreaId == a.Id)
                        })
                        .ToListAsync();

                    break;

                // 2. 加载国家（二级）
                case 2:
                    result = await _countryRepository.GetAll()
                       .Where(c => c.AreaId == input.ParentId)
                       .Select(c => new AreaResponseDto
                       {
                           Id = c.Id,
                           Name = c.Name,
                           ParentId = c.AreaId,
                           Level = 2,
                           Code = c.Code,
                           IsActive = c.IsActive,
                           // 判断是否有子级（省份）
                           HasChildren = _provinceRepository.GetAll().Any(p => p.CountryId == c.Id)
                       })
                       .ToListAsync();

                    break;

                // 3. 加载省份（三级）
                case 3:
                    result = await _provinceRepository.GetAll()
                        .Where(p => p.CountryId == input.ParentId)
                        .Select(p => new AreaResponseDto
                        {
                            Id = p.Id,
                            Name = p.Name,
                            ParentId = p.CountryId,
                            Code = p.Code,
                            Level = 3,
                            IsActive = p.IsActive,
                            // 判断是否有子级（城市）
                            HasChildren = _cityRepository.GetAll().Any(c => c.ProvinceId == p.Id)
                        })
                        .ToListAsync();

                    break;

                // 4. 加载城市（四级）
                case 4:
                    result = await _cityRepository.GetAll()
                       .Where(c => c.ProvinceId == input.ParentId)
                       .Select(c => new AreaResponseDto
                       {
                           Id = c.Id,
                           Name = c.Name,
                           ParentId = c.ProvinceId,
                           Code = c.Code,
                           Level = 4,
                           IsActive = c.IsActive,
                           // 判断是否有子级（区县）
                           HasChildren = _countyRepository.GetAll().Any(cy => cy.CityId == c.Id)
                       })
                       .ToListAsync();
                    break;

                // 5. 加载区县（五级，无下级）
                case 5:
                    result = await _countyRepository.GetAll()
                        .Where(cy => cy.CityId == input.ParentId)
                        .Select(cy => new AreaResponseDto
                        {
                            Id = cy.Id,
                            Name = cy.Name,
                            ParentId = cy.CityId,
                            Code = cy.Code,
                            Level = 5,
                            IsActive = cy.IsActive,
                            HasChildren = false // 区县无下级
                        })
                        .ToListAsync();
                    break;

                default:
                    throw new UserFriendlyException("无效的层级，仅支持1-5（大区/国家/省份/城市/区县）");
            }
            return result;
        }

        /// <summary>
        /// 新增区域、国家、省市区 信息（单个信息新增，同一个接口）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateAreaInfoAsync(CreateAreaInfoDto input)
        {
            // 1. 基础参数校验
            if (input.Level < 1 || input.Level > 5)
            {
                throw new UserFriendlyException("无效的层级，仅支持1-5（1=大区/2=国家/3=省份/4=城市/5=区县）");
            }
            if (input.Level > 1 && (!input.ParentId.HasValue || input.ParentId == Guid.Empty))
            {
                throw new UserFriendlyException($"新增层级{input.Level}的ParentId必须为有效Guid（不可为null/空Guid）");
            }

            // 2. 根据层级路由到对应表新增
            Guid newId = Guid.Empty;
            switch (input.Level)
            {
                // 1. 新增大区
                case 1:
                    var newArea = new Base_Area
                    {
                        Name = input.Name,
                        ParentId = null, // 大区平级，ParentId固定为Empty
                        EnglishName = input.EnglishName,
                        Code = input.Code,
                        Remark = input.Remark,
                        IsActive = input.IsActive,
                    };
                    await _areaRepository.InsertAsync(newArea);
                    newId = newArea.Id;
                    break;
                // 2. 新增国家（关联大区）
                case 2:
                    // 校验父级大区是否存在
                    var areaExists = await _areaRepository.GetAll().AnyAsync(a => a.Id == input.ParentId);
                    if (!areaExists)
                    {
                        throw new UserFriendlyException($"关联的大区ID({input.ParentId})不存在");
                    }
                    var newCountry = new Base_Country
                    {
                        Name = input.Name,
                        AreaId = input.ParentId.Value, // 关联大区ID
                        EnglishName = input.EnglishName,
                        Code = input.Code,
                        Remark = input.Remark,
                        IsActive = input.IsActive,
                    };
                    await _countryRepository.InsertAsync(newCountry);
                    newId = newCountry.Id;
                    break;
                // 3. 新增省份（关联国家）
                case 3:
                    // 校验父级国家是否存在
                    var countryInfo = await this._countryRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(p => p.Id == input.ParentId);
                    if (countryInfo == null)
                    {
                        throw new UserFriendlyException($"关联的国家ID({input.ParentId})不存在");
                    }

                    var newProvince = new Base_Province
                    {
                        Name = input.Name,
                        AreaId = countryInfo.AreaId,
                        CountryId = input.ParentId.Value, // 关联国家ID

                        EnglishName = input.EnglishName,
                        Code = input.Code,
                        Remark = input.Remark,
                        IsActive = input.IsActive,
                    };
                    await _provinceRepository.InsertAsync(newProvince);
                    newId = newProvince.Id;
                    break;
                // 4. 新增城市（关联省份）
                case 4:
                    // 校验父级省份是否存在
                    var provinceInfo = await this._provinceRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(p => p.Id == input.ParentId);
                    if (provinceInfo == null)
                    {
                        throw new UserFriendlyException($"关联的省份ID({input.ParentId})不存在");
                    }
                    var newCity = new Base_City
                    {
                        Name = input.Name,
                        AreaId = provinceInfo.AreaId,
                        CountryId = provinceInfo.CountryId,
                        ProvinceId = input.ParentId.Value, // 关联省份ID
                        EnglishName = input.EnglishName,
                        Code = input.Code,
                        Remark = input.Remark,
                        IsActive = input.IsActive,
                    };
                    await _cityRepository.InsertAsync(newCity);
                    newId = newCity.Id;
                    break;
                // 5. 新增区县（关联城市）
                case 5:
                    // 校验父级城市是否存在
                    var cityInfo = await this._cityRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(p => p.Id == input.ParentId);
                    if (cityInfo == null)
                    {
                        throw new UserFriendlyException($"关联的城市ID({input.ParentId})不存在");
                    }
                    var newCounty = new Base_County
                    {
                        Name = input.Name,
                        AreaId = cityInfo.AreaId,
                        CountryId = cityInfo.CountryId,
                        ProvinceId = cityInfo.ProvinceId,
                        // 关联城市ID
                        CityId = input.ParentId.Value,
                        EnglishName = input.EnglishName,
                        Code = input.Code,
                        Remark = input.Remark,
                        IsActive = input.IsActive,
                    };
                    await _countyRepository.InsertAsync(newCounty);
                    newId = newCounty.Id;
                    break;
            }
        }

        /// <summary>
        /// 修改区域、国家、省市区 信息（单个信息新增，同一个接口）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateAreaInfoAsync(CreateAreaInfoDto input)
        {
            // 1. 基础参数校验
            if (input.Level < 1 || input.Level > 5)
            {
                throw new UserFriendlyException("无效的层级，仅支持1-5（1=大区/2=国家/3=省份/4=城市/5=区县）");
            }
            if (input.Level > 1 && (!input.ParentId.HasValue || input.ParentId == Guid.Empty))
            {
                throw new UserFriendlyException($"新增层级{input.Level}的ParentId必须为有效Guid（不可为null/空Guid）");
            }

            // 2. 根据层级路由到对应表新增
            Guid newId = Guid.Empty;
            switch (input.Level)
            {
                // 1. 大区
                case 1:
                    var newArea = new Base_Area
                    {
                        Id = input.Id.Value,
                        Name = input.Name,
                        ParentId = Guid.Empty, // 大区平级，ParentId固定为Empty
                        EnglishName = input.EnglishName,
                        Code = input.Code,
                        Remark = input.Remark,
                        IsActive = input.IsActive,
                    };
                    await _areaRepository.UpdateAsync(newArea);
                    newId = newArea.Id;
                    break;
                // 2. 国家（关联大区）
                case 2:
                    // 校验父级大区是否存在
                    var areaExists = await _areaRepository.GetAll().AnyAsync(a => a.Id == input.ParentId);
                    if (!areaExists)
                    {
                        throw new UserFriendlyException($"关联的大区ID({input.ParentId})不存在");
                    }
                    var newCountry = new Base_Country
                    {
                        Id = input.Id.Value,
                        Name = input.Name,
                        AreaId = input.ParentId.Value, // 关联大区ID
                        EnglishName = input.EnglishName,
                        Code = input.Code,
                        Remark = input.Remark,
                        IsActive = input.IsActive,
                    };
                    await _countryRepository.UpdateAsync(newCountry);
                    newId = newCountry.Id;
                    break;
                // 3. 省份（关联国家）
                case 3:
                    // 校验父级国家是否存在
                    var countryInfo = await this._countryRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(p => p.Id == input.ParentId);
                    if (countryInfo == null)
                    {
                        throw new UserFriendlyException($"关联的国家ID({input.ParentId})不存在");
                    }

                    var newProvince = new Base_Province
                    {
                        Id = input.Id.Value,
                        Name = input.Name,
                        AreaId = countryInfo.AreaId,
                        CountryId = input.ParentId.Value, // 关联国家ID

                        EnglishName = input.EnglishName,
                        Code = input.Code,
                        Remark = input.Remark,
                        IsActive = input.IsActive,
                    };
                    await _provinceRepository.UpdateAsync(newProvince);
                    newId = newProvince.Id;
                    break;
                // 4. 城市（关联省份）
                case 4:
                    // 校验父级省份是否存在
                    var provinceInfo = await this._provinceRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(p => p.Id == input.ParentId);
                    if (provinceInfo == null)
                    {
                        throw new UserFriendlyException($"关联的省份ID({input.ParentId})不存在");
                    }
                    var newCity = new Base_City
                    {
                        Id = input.Id.Value,
                        Name = input.Name,
                        AreaId = provinceInfo.AreaId,
                        CountryId = provinceInfo.CountryId,
                        ProvinceId = input.ParentId.Value, // 关联省份ID
                        EnglishName = input.EnglishName,
                        Code = input.Code,
                        Remark = input.Remark,
                        IsActive = input.IsActive,
                    };
                    await _cityRepository.UpdateAsync(newCity);
                    newId = newCity.Id;
                    break;
                // 5. 区县（关联城市）
                case 5:
                    // 校验父级城市是否存在
                    var cityInfo = await this._cityRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(p => p.Id == input.ParentId);
                    if (cityInfo == null)
                    {
                        throw new UserFriendlyException($"关联的城市ID({input.ParentId})不存在");
                    }
                    var newCounty = new Base_County
                    {
                        Id = input.Id.Value,
                        Name = input.Name,
                        AreaId = cityInfo.AreaId,
                        CountryId = cityInfo.CountryId,
                        ProvinceId = cityInfo.ProvinceId,
                        // 关联城市ID
                        CityId = input.ParentId.Value,
                        EnglishName = input.EnglishName,
                        Code = input.Code,
                        Remark = input.Remark,
                        IsActive = input.IsActive,
                    };
                    await _countyRepository.UpdateAsync(newCounty);
                    newId = newCounty.Id;
                    break;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task DeleteAreaInfoAsync(GetInfoAreaDto input)
        {
            if (input.Level < 1 || input.Level > 5)
            {
                throw new UserFriendlyException("无效的层级，仅支持1-5（1=大区/2=国家/3=省份/4=城市/5=区县）");
            }
            if (input.Id == Guid.Empty)
            {
                throw new UserFriendlyException("参数错误，Id为空");
            }
            switch (input.Level)
            {
                case 1:
                    {
                        var info = await this._areaRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(p => p.Id == input.Id);
                        if (info == null)
                        {
                            throw new UserFriendlyException("操作失败，当前大区不存在");
                        }
                        await _areaRepository.DeleteAsync(input.Id);
                        break;
                    }
                case 2:
                    {
                        var info = await this._countryRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(p => p.Id == input.Id);
                        if (info == null)
                        {
                            throw new UserFriendlyException("操作失败，当前国家不存在");
                        }
                        await _countryRepository.DeleteAsync(input.Id);
                        break;
                    }
                case 3:
                    {
                        var info = await this._provinceRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(p => p.Id == input.Id);
                        if (info == null)
                        {
                            throw new UserFriendlyException("操作失败，当前省份不存在");
                        }
                        await _provinceRepository.DeleteAsync(input.Id);
                        break;
                    }
                case 4:
                    {
                        var info = await this._cityRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(p => p.Id == input.Id);
                        if (info == null)
                        {
                            throw new UserFriendlyException("操作失败，当前城市不存在");
                        }
                        await _cityRepository.DeleteAsync(input.Id);
                        break;
                    }
                case 5:
                    {
                        var info = await this._countyRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(p => p.Id == input.Id);
                        if (info == null)
                        {
                            throw new UserFriendlyException("操作失败，当前区县不存在");
                        }
                        await _countyRepository.DeleteAsync(input.Id);
                        break;
                    }
            }

        }

        /// <summary>
        /// 获取区域、国家、省市区 信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<AreaResponseDto> GetAreaDetailByIdAsync(GetInfoAreaDto input)
        {
            AreaResponseDto areaResponseDto = new AreaResponseDto();
            if (input.Level < 1 || input.Level > 5)
            {
                throw new UserFriendlyException("无效的层级，仅支持1-5（1=大区/2=国家/3=省份/4=城市/5=区县）");
            }
            if (input.Id == Guid.Empty)
            {
                throw new UserFriendlyException("参数错误，Id为空");
            }
            switch (input.Level)
            {
                case 1:
                    {
                        var info = await this._areaRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(p => p.Id == input.Id);
                        if (info == null)
                        {
                            throw new UserFriendlyException("操作失败，当前大区不存在");
                        }
                        areaResponseDto = new AreaResponseDto()
                        {
                            Code = info.Code,
                            Name = info.Name,
                            Id = info.Id,
                            Level = 1,
                            EnglishName = info.EnglishName,
                            ParentId = null,
                            Remark = info.Remark,
                            IsActive = info.IsActive
                        };
                        break;
                    }
                case 2:
                    {
                        var info = await this._countryRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(p => p.Id == input.Id);
                        if (info == null)
                        {
                            throw new UserFriendlyException("操作失败，当前国家不存在");
                        }
                        areaResponseDto = new AreaResponseDto()
                        {
                            Id = info.Id,
                            Code = info.Code,
                            Name = info.Name,
                            Level = 2,
                            EnglishName = info.EnglishName,
                            ParentId = info.AreaId,
                            Remark = info.Remark,
                            IsActive = info.IsActive
                        };
                        break;
                    }
                case 3:
                    {
                        var info = await this._provinceRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(p => p.Id == input.Id);
                        if (info == null)
                        {
                            throw new UserFriendlyException("操作失败，当前省份不存在");
                        }
                        areaResponseDto = new AreaResponseDto()
                        {
                            Code = info.Code,
                            Name = info.Name,
                            Id = info.Id,
                            Level = 3,
                            EnglishName = info.EnglishName,
                            ParentId = info.CountryId,
                            Remark = info.Remark,
                            IsActive = info.IsActive
                        };
                        break;
                    }
                case 4:
                    {
                        var info = await this._cityRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(p => p.Id == input.Id);
                        if (info == null)
                        {
                            throw new UserFriendlyException("操作失败，当前城市不存在");
                        }
                        areaResponseDto = new AreaResponseDto()
                        {
                            Code = info.Code,
                            Name = info.Name,
                            Id = info.Id,
                            Level = 4,
                            EnglishName = info.EnglishName,
                            ParentId = info.ProvinceId,
                            Remark = info.Remark,
                            IsActive = info.IsActive
                        };
                        break;
                    }
                case 5:
                    {
                        var info = await this._countyRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(p => p.Id == input.Id);
                        if (info == null)
                        {
                            throw new UserFriendlyException("操作失败，当前区县不存在");
                        }
                        areaResponseDto = new AreaResponseDto()
                        {
                            Code = info.Code,
                            Name = info.Name,
                            Id = info.Id,
                            Level = 5,
                            EnglishName = info.EnglishName,
                            ParentId = info.CityId,
                            Remark = info.Remark,
                            IsActive = info.IsActive
                        };
                        break;
                    }
            }

            return areaResponseDto;
        }
    }
}
