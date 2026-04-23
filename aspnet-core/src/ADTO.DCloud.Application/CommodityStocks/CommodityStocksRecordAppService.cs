using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.CommodityStocks.Dto;
using ADTO.DCloud.DataItem;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ADTO.DCloud.CommodityStocks
{
    /// <summary>
    /// 库存操作记录、变更记录
    /// </summary>
    public class CommodityStocksRecordAppService : DCloudAppServiceBase, ICommodityStocksRecordAppService
    {
        private readonly IRepository<CommodityStocksRecord, Guid> _repository;
        private IRepository<CommodityStock, Guid> _stockRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private IRepository<DataItemDetail, Guid> _dataItemDetailRepository;
        public CommodityStocksRecordAppService(IRepository<CommodityStocksRecord, Guid> repository
            , IRepository<CommodityStock, Guid> stockRepository,
            IRepository<DataItemDetail, Guid> dataItemDetailRepository
           , IRepository<User, Guid> userRepository)
        {
            _repository = repository;
            _stockRepository = stockRepository;
            _userRepository = userRepository;
            _dataItemDetailRepository = dataItemDetailRepository;

        }
        /// <summary>
        /// 获取库存变更记录-分页列表
        /// </summary>
        /// <param name="input">查询实体</param>
        /// <returns></returns>

        public async Task<PagedResultDto<CommodityStocksRecordDto>> GetStocksRecordPagedAllList(PagedCommodityStockRecordRequestDto input)
        {
            var itemQuery = _dataItemDetailRepository.GetAllIncluding(p => p.Item)
                        .Where(p => p.Item.ItemCode == "CommodityStocksRecord");

            var query = from record in this._repository.GetAllIncluding(d => d.CommodityStock)
                        join u1 in _userRepository.GetAll() on record.CreatorUserId equals u1.Id into u
                        from user in u.DefaultIfEmpty()

                        join item in itemQuery on record.Status.ToString() equals item.ItemValue into itemdetails
                        from itemdetail in itemdetails.DefaultIfEmpty()

                        select new { record, user, itemdetail };

            query = query.WhereIf(input.CommodityStockId.HasValue && input.CommodityStockId != Guid.Empty, q => q.record.CommodityStock.Id.Equals(input.CommodityStockId))
                .WhereIf(input.Status.HasValue, q => q.record.Status == input.Status);

            //获取总数
            var resultCount = await query.CountAsync();

            var list = query.PageBy(input).ToList().Select(item =>
            {
                var dto = ObjectMapper.Map<CommodityStocksRecordDto>(item.record);
                dto.CreatorUser = item.record != null ? item.user?.Name : "";
                dto.ProductName = dto.CommodityStock.Name;
                dto.StatusText = item.itemdetail.ItemName;
                return dto;
            }).ToList();

            var ResultList = ObjectMapper.Map<List<CommodityStocksRecordDto>>(list);
            return new PagedResultDto<CommodityStocksRecordDto>(resultCount, ResultList);
        }

        /// <summary>
        /// 新增变更记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPost]
        public async Task CreateStocksRecordAsync(CreateCommodityStocksRecord input)
        {
            CommodityStocksRecord entity = ObjectMapper.Map<CommodityStocksRecord>(input);
            if (input.CommodityStockId != Guid.Empty)
            {
                var stock = _stockRepository.Get(input.CommodityStockId);
                entity.CommodityStock = stock;
            }

            await _repository.InsertAsync(entity);

        }

        /// <summary>
        /// 编辑变更记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        //[AbpAuthorize("Pages.Stock.RecordList.Update")]
        [HttpPost]
        public async Task UpdateStocksRecordAsync(UpdateCommodityStocksRecord input)
        {
            var entity = _repository.Get(input.Id);
            if (input.CommodityStockId != Guid.Empty)
            {
                var stock = _stockRepository.Get(input.CommodityStockId);
                entity.CommodityStock = stock;
            }
            ObjectMapper.Map(input, entity);
            await _repository.UpdateAsync(entity);

        }

        #region 删除
        /// <summary>
        /// 删除变更记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[AbpAuthorize("Pages.Stock.RecordList.Delete")]
        [HttpPost]
        public async Task DeleteStocksRecordAsync(EntityDto<Guid> input)
        {
            var entity = this._repository.Get(input.Id);
            if (entity == null || entity.Id == Guid.Empty)
            {
                throw new UserFriendlyException($"操作失败！");
            }
            await _repository.DeleteAsync(entity);
        }
        #endregion

        /// <summary>
        /// 获取变更记录详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<CommodityStocksRecordDto> GetStocksRecordInfoAsync(EntityDto<Guid> input)
        {
            var entity = await _repository.GetAllIncluding(t => t.CommodityStock).Where(d => d.Id == input.Id).FirstOrDefaultAsync();
            var dto = ObjectMapper.Map<CommodityStocksRecordDto>(entity);
            return dto;
        }
    }
}
