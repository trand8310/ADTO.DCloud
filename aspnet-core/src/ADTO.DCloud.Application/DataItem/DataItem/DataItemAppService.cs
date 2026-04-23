using System;
using ADTO.DCloud.Dto;
using System.Linq;
using System.Threading.Tasks;
using ADTO.DCloud.DataItem.Dto;
using System.Collections.Generic;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using ADTOSharp.UI;

namespace ADTO.DCloud.DataItem
{
    /// <summary>
    /// 字典类别相关操作
    /// </summary>
    public class DataItemAppService : DCloudAppServiceBase, IDataItemAppService
    {
        private readonly IRepository<DataItem, Guid> _dataItemRepository;
        public DataItemAppService(IRepository<DataItem, Guid> dataItemRepository)
        {
            _dataItemRepository = dataItemRepository;
        }

        /// <summary>
        /// 添加字典分类
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateDataItemAsync(CreateDataItemInputDto input)
        {
            try
            {
                var existInfo = await this._dataItemRepository.GetAll().Where(p => p.ItemCode == input.ItemCode).FirstOrDefaultAsync();
                if (existInfo != null)
                {
                    throw new UserFriendlyException("保存失败,分类编码已存在！");
                }
                var dataItem = ObjectMapper.Map<DataItem>(input);
                await _dataItemRepository.InsertAsync(dataItem);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 修改字典分类
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateDataItemAsync(CreateDataItemInputDto input)
        {
            var existInfo = await this._dataItemRepository.GetAll().Where(p => p.ItemCode == input.ItemCode && p.Id != input.Id).FirstOrDefaultAsync();
            if (existInfo != null)
            {
                throw new UserFriendlyException("保存失败,分类编码已存在！");
            }
            var info = this._dataItemRepository.Get(input.Id.Value);
            ObjectMapper.Map(input, info);

            //转换一下，否则其它字段也会置空
            await _dataItemRepository.UpdateAsync(info);
        }

        /// <summary>
        /// 获取指定字典分类
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<DataItemDto> GetDataItemByIdAsync(EntityDto<Guid> input)
        {
            var info = await _dataItemRepository.GetAsync(input.Id);
            return ObjectMapper.Map<DataItemDto>(info);
        }

        /// <summary>
        /// 根据分类编码获取字典分类详情
        /// </summary>
        /// <param name="ItemCode">分类编码</param>
        /// <returns></returns>
        public async Task<DataItemDto> GetDataItemByCodeAsync(string ItemCode)
        {
            var info = await _dataItemRepository.GetAll().Where(p => p.ItemCode == ItemCode).FirstOrDefaultAsync();
            return ObjectMapper.Map<DataItemDto>(info);
        }

        /// <summary>
        /// 删除指定的字典分类
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteDataItemAsync(EntityDto<Guid> input)
        {
            await _dataItemRepository.DeleteAsync(input.Id);
        }

        /// <summary>
        /// 分类列表
        /// 设计器获取数据字典下拉框data/dataitem/classifys
        /// </summary>
        /// <returns></returns>
        public async Task<List<DataItemDto>> GetClassifyList()
        {
            var list = await this._dataItemRepository.GetAll().OrderBy(p=>p.DisplayOrder).ToListAsync();
            return ObjectMapper.Map<List<DataItemDto>>(list);
        }

        /// <summary>
        /// 获取字典分类列表(树结构列表)
        /// </summary>
        /// <returns></returns>
        public async Task<List<DataItemTreeDto>> GetDataItemList(string keyword)
        {
            var query = await _dataItemRepository.GetAllIncludingAsync(d => d.Parent);
            var pageList = query.WhereIf(!string.IsNullOrWhiteSpace(keyword), q => q.ItemCode.Equals(keyword) || q.ItemName.Equals(keyword) || q.Id.Equals(keyword)).ToList();

            var list = pageList.Select(item =>
            {
                var dto = ObjectMapper.Map<DataItemTreeDto>(item);
                if (item.Parent != null)
                {
                    dto.ParentName = item.Parent.ItemName;
                    dto.Parent = ObjectMapper.Map<DataItemTreeDto>(item.Parent);
                }
                return dto;
            }).ToList();
            var result = InternalTreeList(list, null);
            return result;
        }

        private List<DataItemTreeDto> InternalTreeList(List<DataItemTreeDto> list, string parentId = null)
        {
            var query = list.AsQueryable()
                   .WhereIf(parentId != null && parentId != "", w => w.Parent != null && w.ParentId == parentId)
                   .WhereIf(parentId == null || parentId == "", w => w.Parent == null)
                   .ToList();

            return query.Select(item =>
            {
                item.children = InternalTreeList(list, item.Id.ToString());
                return item;
            }).ToList();
        }

        /// <summary>
        /// 获取字典分类列表(树结构)
        /// </summary>
        /// <returns></returns>
        public async Task<List<TreeModelDto>> GetDataItemTree(EntityDto<string> input)
        {
            var dataItems = await _dataItemRepository.GetAll().WhereIf(!string.IsNullOrEmpty(input.Id), d => d.Id.ToString() == input.Id).ToListAsync();
            List<TreeModelDto> treeList = new List<TreeModelDto>();
            foreach (var item in dataItems)
            {
                TreeModelDto node = new TreeModelDto();
                node.id = item.Id.ToString();
                node.text = item.ItemName;
                node.value = item.ItemCode;
                node.showcheck = false;
                node.checkstate = 0;
                node.isexpand = true;
                node.parentId = item.Parent != null ? item.Parent.Id.ToString() : null;
                treeList.Add(node);
            }
            return treeList.ToTree();
        }


    }
}
