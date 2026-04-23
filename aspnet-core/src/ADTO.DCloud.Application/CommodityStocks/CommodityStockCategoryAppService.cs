using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADTO.DCloud.CommodityStocks.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;


namespace ADTO.DCloud.CommodityStocks
{
    /// <summary>
    /// 库存类别
    /// </summary>
    public class CommodityStockCategoryAppService : DCloudAppServiceBase, ICommodityStockCategoryAppService
    {
        private readonly IRepository<CommodityStockCategory, Guid> _repository;
        public CommodityStockCategoryAppService(IRepository<CommodityStockCategory, Guid> repository
          )
        {
            _repository = repository;

        }

        #region 获取库存类别树状结构
        /// <summary>
        /// 获取库存类别树状结构
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<JObject>> GetTree(GetRequestTreeInput input)
        {
            //var cache = _cacheManager.GetCache($"{OAConsts.DefaultCacheName}CommodityStockCategory");
            //var cacheKey = $"GetDetailTree-{input.Type}";
            //return await cache.GetAsync(cacheKey, async (t) =>
            //{
            var query = await _repository.GetAll()
           .Where(w => w.Type == input.Type)
           .OrderBy(o => o.SortCode)
           .ToListAsync();
            var list = ObjectMapper.Map<List<CommodityStockCategoryDto>>(query);
            return BuildTreeOptimized(list);

            //}) as List<JObject>;
        }

        private List<JObject> BuildTreeOptimized(List<CommodityStockCategoryDto> list)
        {
            if (list == null || !list.Any())
                return new List<JObject>();
            var parentGroup = list.ToLookup(x => x.ParentId);
            List<JObject> BuildChildNodes(Guid? parentId)
            {
                var children = parentGroup[parentId];
                return children.Select(item =>
                {
                    var childNodes = BuildChildNodes(item.Id);
                    bool hasChildren = childNodes != null && childNodes.Count > 0;

                    return new JObject
                    {
                        ["id"] = item.Id.ToString(),
                        ["text"] = item.Name,
                        ["name"] = item.Name,
                        ["value"] = item.Id.ToString(),
                        ["parentId"] = item.ParentId.ToString(),
                        ["creationTime"] = item.CreationTime,
                        ["sortcode"] = item.SortCode,
                        ["description"] = item.Description,
                        ["children"] = hasChildren ? JArray.FromObject(childNodes) : null,
                        ["hasChildren"] = hasChildren,
                        ["type"] = item.ParentId == null ? "root" : (hasChildren ? "folder" : "file"),

                        ["icon"] = null,
                        ["showcheck"] = false,
                        ["checkstate"] = 0,
                        ["isexpand"] = true,
                        ["complete"] = true
                    };
                }).ToList();
            }
            return BuildChildNodes(null);
        }

        /// <summary>
        /// 原本方式，已更改为BuildTreeOptimized
        /// 存在多级一级类，数据就为空
        /// </summary>
        /// <param name="list"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private List<JObject> InternalGetTree(List<CommodityStockCategoryDto> list, Guid id)
        {
            var query = from c in list
                        where c.ParentId == id
                        select c;
            return query.Select(s =>
            {
                var m = new JObject();
                m["id"] = s.Id + "";
                m["text"] = s.Name;
                m["name"] = s.Name;
                m["value"] = s.Id + "";
                m["icon"] = null;
                m["showcheck"] = false;
                m["checkstate"] = 0;
                m["isexpand"] = true;
                m["complete"] = true;
                m["parentId"] = s.ParentId.ToString();
                m["creationTime"] = s.CreationTime;
                m["sortcode"] = s.SortCode;
                m["description"] = s.Description;
                m["children"] = JArray.FromObject(InternalGetTree(list, s.Id).ToList());
                m["hasChildren"] = m["children"] != null && m["children"].Count() > 0;
                m["type"] = ((s.ParentId == Guid.Empty) ? "root" : s.ParentId == id ? "folder" : "file");
                return m;
            }).ToList();
        }
        #endregion

        /// <summary>
        /// 得到库存类别列表，非分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public List<CommodityStockCategoryDto> GetStockCategoryList(PagedCommodityStockCategoryRequestDto input)
        {
            var taskList = this._repository.GetAll().WhereIf(input.Type > 0, x => x.Type == input.Type).OrderBy(o => o.SortCode);
            return ObjectMapper.Map<List<CommodityStockCategoryDto>>(taskList);
        }

        /// <summary>
        /// 根据编号查询对应实体
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<CommodityStockCategoryDto> GetAsync(EntityDto<Guid> input)
        {
            var entity = await _repository.GetAsync(input.Id);
            var dto = ObjectMapper.Map<CommodityStockCategoryDto>(entity);

            if (dto.ParentId == Guid.Empty)
            {
                dto.ParentId = null;
            }
            return dto;
        }

        #region 根据传过来的参数-新增修改
        /// <summary>
        /// 新增-库存类别
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPost]
        public async Task CreateAsync(CreateCommodityStockCategoryDto input)
        {
            CommodityStockCategory entity = ObjectMapper.Map<CommodityStockCategory>(input);
            await _repository.InsertAndGetIdAsync(entity);

        }
        /// <summary>
        /// 编辑-库存类别
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPost]
        public async Task UpdateAsync(CreateCommodityStockCategoryDto input)
        {
            var entity = _repository.Get(input.Id.Value);
            ObjectMapper.Map(input, entity);
            await _repository.UpdateAsync(entity);

        }

        #endregion

        #region 电脑库存 
        /// <summary>
        /// 新增-电脑库存
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPost]
        public async Task CreateCommodityStocksAsync(CreateCommodityStockCategoryDto input)
        {
            CommodityStockCategory entity = ObjectMapper.Map<CommodityStockCategory>(input);
            entity.Type = (int)EnumCommodityStockType.CommodityStocks;
            await _repository.InsertAndGetIdAsync(entity);
        }
        /// <summary>
        /// 编辑-电脑库存
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPost]
        public async Task UpdateCommodityStocksAsync(CommodityStockCategoryDto input)
        {
            var entity = _repository.Get(input.Id);
            entity.Type = (int)EnumCommodityStockType.CommodityStocks;
            ObjectMapper.Map(input, entity);
            await _repository.UpdateAsync(entity);
        }

        #endregion

        #region 总裁办库存
        /// <summary>
        /// 新增-总裁办库存
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPost]
        public async Task CreatePresidentOfficeInventoryAsync(CreateCommodityStockCategoryDto input)
        {
            CommodityStockCategory entity = ObjectMapper.Map<CommodityStockCategory>(input);
            entity.Type = (int)EnumCommodityStockType.PresidentOfficeInventory;
            await _repository.InsertAndGetIdAsync(entity);

        }
        /// <summary>
        /// 修改-总裁办库存
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPost]
        public async Task UpdatePresidentOfficeInventoryAsync(CommodityStockCategoryDto input)
        {
            var entity = _repository.Get(input.Id);
            ObjectMapper.Map(input, entity);
            entity.Type = (int)EnumCommodityStockType.PresidentOfficeInventory;
            await _repository.UpdateAsync(entity);
        }
        #endregion


        #region 删除 
        /// <summary>
        /// 删除库存类别信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task DeleteAsync(EntityDto<Guid> input)
        {
            var entity = _repository.Get(input.Id);
            if (entity == null || entity.Id == Guid.Empty)
            {
                throw new UserFriendlyException($"参数异常");
            }
            await _repository.DeleteAsync(entity);
        }
        /// <summary>
        /// 多删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task DeleteListAsync(List<EntityDto<Guid>> ids)
        {
            foreach (var item in ids)
            {
                var entity = _repository.Get(item.Id);
                if (entity == null || entity.Id == Guid.Empty)
                {
                    throw new UserFriendlyException($"参数异常");
                }
                await _repository.DeleteAsync(entity);
            }
        }
        #endregion
    }
}
