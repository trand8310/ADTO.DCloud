using ADTO.DCloud.Attendances.AttendanceTimeRules.Dto;
using ADTO.DCloud.DataItem.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Runtime.Caching;
using ADTOSharp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ADTO.DCloud.DataItem
{
    /// <summary>
    /// 字典详情
    /// </summary>
    public class DataItemDetailAppService : DCloudAppServiceBase, IDataItemDetailAppService
    {
        private readonly IRepository<DataItemDetail, Guid> _dataItemDetailRepository;
        private readonly IRepository<DataItem, Guid> _dataItemRepository;
        private readonly ICacheManager _cacheManager;
        public DataItemDetailAppService(IRepository<DataItemDetail, Guid> dataItemDetailRepository, IRepository<DataItem, Guid> dataItemRepository, ICacheManager cacheManager)
        {
            _dataItemDetailRepository = dataItemDetailRepository;
            _dataItemRepository = dataItemRepository;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// 添加字典详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateDataItemDetailAsync(CreateDataItemDetailInputDto input)
        {
            if (string.IsNullOrWhiteSpace(input.ItemCode) && input.ItemId == Guid.Empty)
            {
                throw new UserFriendlyException("保存失败,所属分类编码不能为空！");
            }
            var existInfo = await _dataItemDetailRepository.GetAll().Where(p => p.ItemValue == input.ItemValue && p.ItemId == input.ItemId).FirstOrDefaultAsync();
            if (existInfo != null)
            {
                throw new UserFriendlyException("保存失败,字典值已存在！");
            }
            //所属分类Id,前端传递的是分类编码
            if (input.ItemId == Guid.Empty)
            {
                var itemInfo = await this._dataItemRepository.GetAll().Where(p => p.ItemCode == input.ItemCode).FirstOrDefaultAsync();
                if (itemInfo != null)
                {
                    input.ItemId = itemInfo.Id;
                }
                else
                {
                    throw new UserFriendlyException("无法找到对应的分类信息！");
                }
            }

            var dataItem = ObjectMapper.Map<DataItemDetail>(input);
            await _dataItemDetailRepository.InsertAsync(dataItem);


            var cacheManager = _cacheManager.GetCache($"ADTO.DCloud");
            var cacheKey = $".DataItemCode.GetItemDetailList.{input.ItemCode}";
            await cacheManager.RemoveAsync(cacheKey);

        }

      

        /// <summary>
        /// 修改字典详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateDataItemDetailAsync(CreateDataItemDetailInputDto input)
        {
            var info = await _dataItemDetailRepository.GetAsync(input.Id);
            if (info == null)
            {
                throw new UserFriendlyException("保存失败,记录不存在！");
            }
            if (string.IsNullOrWhiteSpace(input.ItemCode) && input.ItemId == Guid.Empty)
            {
                throw new UserFriendlyException("保存失败,所属分类编码不能为空！");
            }
            var existInfo = await _dataItemDetailRepository.GetAll().Where(p => p.ItemId.Equals(info.ItemId) && p.ItemValue == input.ItemValue && p.ItemId == input.ItemId && p.Id != input.Id).FirstOrDefaultAsync();
            if (existInfo != null)
            {
                throw new UserFriendlyException("保存失败,字典值已存在！");
            }
            //所属分类Id,前端传递的是分类编码
            if (input.ItemId == Guid.Empty)
            {
                var itemInfo = await this._dataItemRepository.GetAll().Where(p => p.ItemCode == input.ItemCode).FirstOrDefaultAsync();
                if (itemInfo != null)
                {
                    input.ItemId = itemInfo.Id;
                }
                else
                {
                    throw new UserFriendlyException("无法找到对应的分类信息！");
                }
            }
            // 转换一下，否则其它字段也会置空
            ObjectMapper.Map(input, info);

            await _dataItemDetailRepository.UpdateAsync(info);

            var cacheManager = _cacheManager.GetCache($"ADTO.DCloud");
            var cacheKey = $".DataItemCode.GetItemDetailList.{input.ItemCode}";
            await cacheManager.RemoveAsync(cacheKey);
        }

        /// <summary>
        /// 获取指定字典详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<DataItemDetailDto> GetDataItemDetailByIdAsync(EntityDto<Guid> input)
        {
            var role = await _dataItemDetailRepository.GetAsync(input.Id);
            return ObjectMapper.Map<DataItemDetailDto>(role);
        }

        /// <summary>
        /// 删除指定的字典详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteDataItemDetailAsync(EntityDto<Guid> input)
        {
            await _dataItemDetailRepository.DeleteAsync(input.Id);
        }

        /// <summary>
        /// 获取数据字典明细分页列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns></returns>

        public async Task<PagedResultDto<DataItemDetailDto>> GetItemDetailPageList(PagedDataItemDetailResultRequestDto input)
        {
            var query = _dataItemDetailRepository.GetAllIncluding(x => x.Item)
                .Where(q => q.Item.Id.Equals(input.ItemId))
                .WhereIf(!string.IsNullOrWhiteSpace(input.KeyWord), p => p.ItemName.Contains(input.KeyWord) || p.ItemName.Contains(input.KeyWord));

            var totalCount = await query.CountAsync();
            var items = await query.PageBy(input).ToListAsync();
            var list = items.Select(item =>
            {
                var dto = ObjectMapper.Map<DataItemDetailDto>(item);
                return dto;
            }).ToList();
            return new PagedResultDto<DataItemDetailDto>(totalCount, list);
        }

        /// <summary>
        /// 根据分类编码获取所有字典详情
        /// (表单设计器字典选择请求接口 GetDetailList)
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<List<DataItemDetailDto>> GetItemDetailList(DataItemQueryDto query)
        {
            var cacheManager = _cacheManager.GetCache($"ADTO.DCloud");
            var cacheKey = $".DataItemCode.GetItemDetailList.{query.ItemCode}";
            var cacheVal = await cacheManager.GetOrDefaultAsync(cacheKey) as List<DataItemDetailDto>;
            if (cacheVal == null || cacheVal.Count() <= 0)
            {
                var list = this._dataItemDetailRepository.GetAllIncluding(p => p.Item).Where(p => p.Item.ItemCode == query.ItemCode).OrderBy(p => p.DisplayOrder);
                var listDto = ObjectMapper.Map<List<DataItemDetailDto>>(await list.ToListAsync());
                await cacheManager.SetAsync(cacheKey, listDto);
                return listDto;
            }
            else
            {
                return cacheVal;
            }
        }

        /// <summary>
        /// 根据分类编码获取所有启用字典详情
        /// </summary>
        /// <param name="query"></param> 
        /// <returns></returns>
        public async Task<List<DataItemDetailDto>> GetItemDetailListByIsActive(DataItemQueryDto query)
        {
            var cacheManager = _cacheManager.GetCache($"ADTO.DCloud");
            var cacheKey = $".DataItemCode.GetItemDetailList.IsActive.{query.ItemCode}";
            var cacheVal = await cacheManager.GetOrDefaultAsync(cacheKey) as List<DataItemDetailDto>;
            if (cacheVal == null || cacheVal.Count() <= 0)
            {
                var list = this._dataItemDetailRepository.GetAllIncluding(p => p.Item).Where(p => p.IsActive == true).Where(p => p.Item.ItemCode == query.ItemCode).OrderBy(p => p.DisplayOrder);

                //await cacheManager.SetAsync(cacheKey, list);
                return ObjectMapper.Map<List<DataItemDetailDto>>(await list.ToListAsync());
            }
            else
            {
                return cacheVal;
            }
        }

        /// <summary>
        /// 根据分类获取数据字典详情树状列表
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<List<DataItemDetailTreeDto>> GetItemTreeViewAsync(string code)
        {
            var cacheManager = _cacheManager.GetCache($"ADTO.DCloud");
            var cacheKey = $".DataItemCode.GetItemTreeViewAsync.{code}";
            var cacheVal = await cacheManager.GetOrDefaultAsync(cacheKey) as List<DataItemDetailTreeDto>;
            if (cacheVal == null || cacheVal.Count() <= 0)
            {
                var list = await this._dataItemDetailRepository.GetAllIncluding(p => p.Item).Where(p => p.Item.ItemCode == code).OrderBy(p => p.DisplayOrder).ToListAsync();
                var items = list.Select(item =>
                {
                    var dto = ObjectMapper.Map<DataItemDetailDto>(item);
                    return dto;
                }).ToList();
                var data = GenerateModuleTreeView(items, null);
                await cacheManager.SetAsync(cacheKey, data);
                return data;
            }
            else return cacheVal;
        }

        private List<DataItemDetailTreeDto> GenerateModuleTreeView(List<DataItemDetailDto> list, Guid? parentId = null)
        {
            var query = list.Where(w => w.ParentId == parentId);
            return query.Select(item =>
            {
                var model = ObjectMapper.Map<DataItemDetailTreeDto>(item);
                model.ParentId = item.ParentId.HasValue ? item.ParentId : null;
                model.Children = GenerateModuleTreeView(list, item.Id);
                return model;
            }).ToList();
        }

    }
}

