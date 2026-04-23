using ADTO.DCloud.ApplicationForm;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.CommodityStocks.Dto;
using ADTO.DCloud.CommodityStocks.PresidentofficeStock.Dto;
using ADTO.DCloud.DataItem;
using ADTO.DCloud.Dto;
using ADTO.DCloud.EmployeeManager;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Organizations;
using ADTOSharp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.CommodityStocks.PresidentofficeStock
{
    /// <summary>
    /// 总裁办库存相关方法
    /// </summary>
    public class PresidentofficeStockAppService : DCloudAppServiceBase, IPresidentofficeStockAppService
    {
        #region
        private IRepository<CommodityStock, Guid> _stockRepository;
        private IRepository<CommodityStockCategory, Guid> _categoryRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private IRepository<DataItemDetail, Guid> _dataItemDetailRepository;
        private IRepository<Adto_OfficeSupplyApplication, Guid> _officeSupplyRepository;
        private IRepository<Adto_OfficeSupplyApplicationItem, Guid> _officeSupplyItemRepository;

        private readonly IRepository<OrganizationUnit, Guid> _orgRepository;
        private readonly IRepository<EmployeeInfo, Guid> _employeeInfoRepository;

        public PresidentofficeStockAppService(IRepository<CommodityStock, Guid> stockRepository
            , IRepository<User, Guid> userRepository
            , IRepository<DataItemDetail, Guid> dataItemDetailRepository
            , IRepository<CommodityStockCategory, Guid> categoryRepository
            , IRepository<Adto_OfficeSupplyApplication, Guid> officeSupplyRepository
            , IRepository<Adto_OfficeSupplyApplicationItem, Guid> officeSupplyItemRepository
                , IRepository<OrganizationUnit, Guid> orgRepository
            , IRepository<EmployeeInfo, Guid> employeeInfoRepository
            )
        {
            _stockRepository = stockRepository;
            _userRepository = userRepository;
            _dataItemDetailRepository = dataItemDetailRepository;
            _categoryRepository = categoryRepository;
            _officeSupplyRepository = officeSupplyRepository;
            _officeSupplyItemRepository = officeSupplyItemRepository;
            _orgRepository = orgRepository;
            _employeeInfoRepository = employeeInfoRepository;
        }

        #endregion


        /// <summary>
        /// 总裁办库存管理 对应原接口GetSQLPresidentOfficePagedList
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<PresidentOfficeDto>> GetOfficeStockPagedList(PagedPresidentofficeStockRequestDto input)
        {
            // 主表：库存表
            var stockQuery = _stockRepository.GetAll();
            // 分类表（固定类型：总统办库存）
            var categoryQuery = _categoryRepository.GetAll()
                .Where(p => p.Type == (int)EnumCommodityStockType.PresidentOfficeInventory);

            // 申请明细表
            var applyDetailQuery = _officeSupplyItemRepository.GetAll();

            // 1. 构建分组统计子查询（和你SQL子查询完全一致）
            var applyNumGroup = applyDetailQuery
                .Where(d => !d.IsDeleted)
                .GroupBy(d => d.ProductId)
                .Select(g => new { ProductId = g.Key, StockApplyNum = (int?)g.Sum(d => d.ApplyCount) });

            // 2. EF Core 联表查询（左连接 + 内连接 完全对齐SQL）
            var query = from t in stockQuery
                        join c in categoryQuery on t.CategoryId equals c.Id
                        // 左连接 申请数量（和SQL LEFT JOIN一致）
                        join detail in applyNumGroup on t.Id equals detail.ProductId into detailGroup
                        from detail in detailGroup.DefaultIfEmpty()

                        join user in _userRepository.GetAll() on t.CreatorUserId equals user.Id into createUser
                        from userinfo in createUser.DefaultIfEmpty()

                        join t9 in _dataItemDetailRepository.GetAllIncluding(p => p.Item).Where(p => p.Item.ItemCode == "stockAddr") on t.Address equals t9.ItemValue into itemdetails
                        from itemdetail in itemdetails.DefaultIfEmpty()
                            // 基础条件（和SQL WHERE一致）
                        where !t.IsDeleted
                        select new PresidentOfficeDto
                        {
                            Id = t.Id,
                            ProductName = t.Name,
                            Category = t.CategoryId,
                            CategoryText = c.Name,
                            Model = t.Model,
                            Price = t.Price,
                            Number = t.Number,
                            Status = t.Status ?? 0,
                            Description = t.Description,
                            CreationTime = t.CreationTime,
                            CreatorUserId = t.CreatorUserId,
                            LastModificationTime = t.LastModificationTime,
                            LastModifierUserId = t.LastModifierUserId,

                            StockApplyNum = detail.StockApplyNum ?? 0,
                            RemainingNum = (t.Number ?? 0) - (detail.StockApplyNum ?? 0),
                            TotalPrice = (t.Price ?? 0) * (t.Number ?? 0),
                            Addr = t.Address,

                            AddrText = itemdetail.ItemName,

                            CreateUser = userinfo.Name ?? ""
                        };

            // 3. 动态条件过滤（和你SQL完全一致）
            query = query
                .WhereIf(input.CategoryId.HasValue && input.CategoryId.Value != Guid.Empty, t => t.Category == input.CategoryId.Value)
                .WhereIf(!string.IsNullOrEmpty(input.Keyword), t => t.ProductName.Contains(input.Keyword))
                .WhereIf(!string.IsNullOrEmpty(input.Model), t => t.Model.Contains(input.Model));
            //.WhereIf(input.Status.HasValue, t => t.Status == input.Status.Value)
            //.WhereIf(!string.IsNullOrEmpty(input.SN), t => t.SN.Contains(input.SN));

            // 申请用户筛选（和你SQL EXISTS 逻辑 100% 一模一样）
            if (!string.IsNullOrEmpty(input.ApplyUser))
            {
                var userQuery = _userRepository.GetAll();
                var applyQuery = _officeSupplyRepository.GetAll();
                var applyDetailAllQuery = _officeSupplyItemRepository.GetAll();

                var existsQuery = from a in applyQuery
                                  join b in applyDetailAllQuery on a.Id equals b.OfficeSupplyApplicationId
                                  join u in userQuery on a.UserId equals u.Id
                                  where !a.IsDeleted && !b.IsDeleted
                                  select new { b.ProductId, u.UserName, u.Name };

                query = query.Where(t =>
                    existsQuery.Any(x => x.ProductId == t.Id
                                      && (x.UserName == input.ApplyUser || x.Name == input.ApplyUser))
                );
            }

            // 只查询剩余库存大于0的数据（和SQL一致）
            //query = query.WhereIf(input.IsRemainingNum == 1, t => t.RemainingNum > 0);

            // 排序
            if (!string.IsNullOrWhiteSpace(input.Sorting))
                query = query.OrderBy(input.Sorting);
            else
                query = query.OrderByDescending(t => t.CreationTime);

            // 分页
            var totalCount = await query.CountAsync();
            var items = await query.PageBy(input).ToListAsync();

            return new PagedResultDto<PresidentOfficeDto>(totalCount, items);
        }

        /// <summary>
        /// 新增总裁办库存
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPost]
        public async Task CreateOfficeStockStockAsync(CreateCommodityStockDto input)
        {
            var info = ObjectMapper.Map<CommodityStock>(input);
            await _stockRepository.InsertAsync(info);
        }

        /// <summary>
        /// 修改总裁办库存
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPost]
        public async Task UpdateOfficeStockAsync(CreateCommodityStockDto input)
        {
            var entity = _stockRepository.Get(input.Id.Value);
            if (entity == null)
            {
                throw new UserFriendlyException($"记录不存在！");
            }

            ObjectMapper.Map(input, entity);
            await _stockRepository.UpdateAsync(entity);
        }

        /// <summary>
        /// 获取总裁办库存详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<CommodityStockDto> GeteOfficeStockInfoAsync(EntityDto<Guid> input)
        {
            var entity = await _stockRepository.GetAllIncluding(t => t.Category).Where(d => d.Id == input.Id).FirstOrDefaultAsync();
            var dto = ObjectMapper.Map<CommodityStockDto>(entity);
            return dto;
        }

        /// <summary>
        /// 删除总裁办库存
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        [HttpPost]
        public async Task DeleteOfficeStockAsync(EntityDto<Guid> input)
        {
            var info = await this._stockRepository.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
            if (info == null)
            {
                throw new UserFriendlyException("操作失败，记录不存在！");
            }
            await _stockRepository.DeleteAsync(input.Id);
        }

        /// <summary>
        /// 总裁办库存-物品申请管理、总裁办库存列表展开详情 对应原接口GetPresidentOfficeApplyDetailList
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<OfficeSupplyApplicationDetailDto>> GetOfficeSupplyApplicationPagedList(GetOfficeSupplyApplicationPagedRequest input)
        {
            // 1. 连表查询（和 SQL 完全一致：内连接 + 左连接）
            var query = from t in _officeSupplyRepository.GetAll()
                        join u in _userRepository.GetAll() on t.UserId equals u.Id
                        join c in _orgRepository.GetAll() on u.CompanyId equals c.Id
                        join d in _orgRepository.GetAll() on u.DepartmentId equals d.Id
                        //总裁办物品申请明细表
                        join a in _officeSupplyItemRepository.GetAll() on t.Id equals a.OfficeSupplyApplicationId
                        // 左连接商品库存表
                        join e in _stockRepository.GetAll() on a.ProductId equals e.Id into eGroup
                        from e in eGroup.DefaultIfEmpty()
                            // 左连接商品分类表
                        join cate in _categoryRepository.GetAll() on a.Category equals cate.Id into cateGroup
                        from cate in cateGroup.DefaultIfEmpty()
                            // SQL 基础条件
                        where (e == null || !e.IsDeleted) && !a.IsDeleted
                        select new OfficeSupplyApplicationDetailDto
                        {
                            // 用户信息
                            Name = u.Name,
                            UserName = u.UserName,
                            DepartmentName = d.DisplayName,
                            CompanyName = c.DisplayName,
                            PhoneNumber = u.PhoneNumber,

                            // 申请明细
                            ProductAppyDetailId = a.Id,
                            ProductName = a.ProductName,
                            ProductId = a.ProductId.Value,
                            ApplyType = a.ApplyType.Value,
                            Remarks = a.Remark,
                            ApplyCount = a.ApplyCount,
                            //主记录Id
                            Id = t.Id,
                            Title = t.Title,
                            Summary = t.Remark,
                            CreationTime = t.CreationTime,

                            //// 分类
                            CategoryName = cate.Name,
                            CategoryId = cate.Id,

                            // 商品信息
                            Price = e.Price.Value,
                            Model = e.Model,

                            // 计算字段
                            TotalPrice = a.ApplyCount * (e.Price ?? 0),

                            // 归还时间/操作人\扣费人
                            BackTime = a.LastModificationTime,
                            LastModifierUserId = a.LastModifierUserId
                        };

            // 2. 动态筛选条件（和 SQL 完全一致）
            query = query
              //扣费状态(有些为null )
              .WhereIf(input.ApplyType.HasValue, t => input.ApplyType == 0 ? (t.ApplyType == 0 || t.ApplyType == null) : t.ApplyType == input.ApplyType)
               //关联商品
               .WhereIf(input.ProductId.HasValue && input.ProductId != Guid.Empty, t => t.ProductId == input.ProductId.Value)
               //商品类别
               .WhereIf(input.CategoryId.HasValue && input.CategoryId != Guid.Empty, t => t.CategoryId == input.CategoryId.Value)
               //申请人（工号或者名称）
               .WhereIf(!string.IsNullOrEmpty(input.ApplyUser), t => t.Name == input.ApplyUser || t.UserName == input.ApplyUser)
               .WhereIf(!string.IsNullOrEmpty(input.Keyword), t =>
                   t.ProductName.Contains(input.Keyword) ||
                   t.Name.Contains(input.Keyword) ||
                   t.UserName == input.Keyword ||
                   t.Title.Contains(input.Keyword))
               .WhereIf(input.BeginTime.HasValue && input.EndTime.HasValue,
                   t => t.CreationTime >= input.BeginTime && t.CreationTime <= input.EndTime);

            // 3. 排序
            if (!string.IsNullOrWhiteSpace(input.Sorting))
                query = query.OrderBy(input.Sorting);
            else
                query = query.OrderByDescending(t => t.CreationTime);

            // 4. 分页
            var totalCount = await query.CountAsync();
            var items = await query.PageBy(input).ToListAsync();

            // 5. 赋值归还人（和你原生SQL逻辑一致）
            foreach (var item in items)
            {
                if (item.LastModifierUserId.HasValue && item.LastModifierUserId.Value != Guid.Empty)
                {
                    item.BackUser = (await _userRepository.GetAsync(item.LastModifierUserId.Value))?.Name ?? string.Empty;
                }
                item.ApplyTypeText = item.ApplyType == 1 ? "已扣费" : "未扣费";
            }

            return new PagedResultDto<OfficeSupplyApplicationDetailDto>(totalCount, items);
        }

        /// <summary>
        /// 修改物品费用信息状态、确定扣费 RteurnPresidentOffice
        /// </summary>
        /// <param name="input"></param>
        [HttpPost]
        public async Task SetReturnOfficeSupply(EntityDto<Guid> input)
        {
            var entity = await _officeSupplyItemRepository.FirstOrDefaultAsync(p => p.Id == input.Id);
            if (entity == null)
            {
                throw new UserFriendlyException($"记录不存在！");
            }
            await this._officeSupplyItemRepository.UpdateAsync(input.Id, async entity => { entity.ApplyType = 1; });

        }
    }
}
