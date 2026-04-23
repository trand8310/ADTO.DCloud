using System;
using ADTO.DCloud.DataIcons.Dto;
using System.Threading.Tasks;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Runtime.Caching;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ADTOSharp.UI;
using ADTO.DCloud.DataSource.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Linq.Extensions;

namespace ADTO.DCloud.DataIcons
{
    /// <summary>
    /// 系统图标相关方法
    /// </summary>
    public class DataIconsAppService : DCloudAppServiceBase, IDataIconsAppService
    {
        private readonly string key = "DCloud_icons_list";
        private readonly string keyVer = "DCloud_icons_ver";
        private readonly IRepository<DataIcons, Guid> _dataIconsRepository;
        private readonly IRepository<DataIconver, Guid> _dataIconsverRepository;
        private readonly ICacheManager _cacheManager;
        public DataIconsAppService(IRepository<DataIcons, Guid> dataIconsRepository, ICacheManager cacheManager, IRepository<DataIconver, Guid> dataIconsverRepository)
        {
            _dataIconsRepository = dataIconsRepository;
            _cacheManager = cacheManager;
            _dataIconsverRepository = dataIconsverRepository;

        }

        /// <summary>
        /// 获取系统所有图标  data/icons
        /// 如果版本号和前端保持一致，则不做请求，每次更新图片则更新版本号，否则版本号不变
        /// </summary>
        /// <param name="ver"></param>
        /// <returns></returns>
        public async Task<IconDto> GetDataIconsList(string ver)
        {
            try
            {
                var res = new IconDto();
                res.Ver = _cacheManager.GetCache(keyVer).ToString();
                if (string.IsNullOrEmpty(res.Ver))
                {
                    res.Ver = await this._dataIconsverRepository.GetAll().Where(p => p.Key == "id").Select(p => p.Ver).FirstOrDefaultAsync();
                    //if (!string.IsNullOrEmpty(res.Ver))
                    //{
                    //    await _cacheManager.(keyVer, res.Ver);
                    //}
                }
                if (string.IsNullOrEmpty(res.Ver) || res.Ver == ver)
                {
                    //return res;
                }

                //res.List = await _iCache.ReadAsync<IEnumerable<LrBaseIconsEntity>>(key);
                if (res.List == null)
                {
                    res.List = ObjectMapper.Map<List<DataIconsDto>>(await _dataIconsRepository.GetAll().ToListAsync());
                    //await _iCache.WriteAsync(key, res.List);
                }

                return res;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        /// <summary>
        /// 图标分页接口 data/icon/page
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<DataIconsDto>> GetDataIconsPageList(PagedDataIconsResultRequestDto input)
        {
            try
            {
                var query = this._dataIconsRepository.GetAll()
               .WhereIf(!string.IsNullOrWhiteSpace(input.KeyWord), p => p.Name.Contains(input.KeyWord) || p.Code.Contains(input.KeyWord));

                var totalCount = await query.CountAsync();
                var results = await query
                    .OrderBy(input.Sorting)
                    .PageBy(input)
                    .ToListAsync();

                return new PagedResultDto<DataIconsDto>(totalCount, ObjectMapper.Map<List<DataIconsDto>>(results));
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
