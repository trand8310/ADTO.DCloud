using ADTO.DCloud.ApplicationForm;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.CommodityStocks.Dto;
using ADTO.DCloud.DataItem;
using ADTO.DCloud.EmployeeManager;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Organizations;
using ADTOSharp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.CommodityStocks
{
    /// <summary>
    /// 电脑库存管理
    /// </summary>
    public class CommodityStockAppService : DCloudAppServiceBase, ICommodityStockAppService
    {
        #region
        private IRepository<CommodityStock, Guid> _repository;
        private IRepository<CommodityStockCategory, Guid> _categoryRepository;
        private readonly IRepository<CommodityStocksRecord, Guid> _recordRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private IRepository<DataItemDetail, Guid> _dataItemDetailRepository;
        private IRepository<Adto_StockApplication, Guid> _stockApplicationRepository;
        private IRepository<Adto_StockApplicationItem, Guid> _stockApplicationItemRepository;

        private readonly IRepository<OrganizationUnit, Guid> _orgRepository;
        private readonly IRepository<EmployeeInfo, Guid> _employeeInfoRepository;
        private readonly IRepository<CompanyPhone, Guid> _companyPhoneRepository;

        public CommodityStockAppService(IRepository<CommodityStock, Guid> repository, IRepository<User, Guid> userRepository
            , IRepository<DataItemDetail, Guid> dataItemDetailRepository

            , IRepository<CommodityStockCategory, Guid> categoryRepository
            , IRepository<Adto_StockApplicationItem, Guid> stockApplicationItemRepository
            , IRepository<Adto_StockApplication, Guid> stockApplicationRepository
            , IRepository<CommodityStocksRecord, Guid> recordRepository
            , IRepository<OrganizationUnit, Guid> orgRepository
            , IRepository<EmployeeInfo, Guid> employeeInfoRepository
            , IRepository<CompanyPhone, Guid> companyPhoneRepository
            )
        {
            _repository = repository;
            _userRepository = userRepository;
            _dataItemDetailRepository = dataItemDetailRepository;
            _categoryRepository = categoryRepository;
            _recordRepository = recordRepository;
            _stockApplicationItemRepository = stockApplicationItemRepository;
            _stockApplicationRepository = stockApplicationRepository;
            _orgRepository = orgRepository;
            _employeeInfoRepository = employeeInfoRepository;
            _companyPhoneRepository = companyPhoneRepository;
        }

        #endregion

        #region 电脑库存管理

        /// <summary>
        /// 库存管理-分页列表（GetSQLCommodityStockPagedList对应老系统更换为EF方式）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<CommodityStockSQLDto>> GetCommodityStockPagedList(PagedCommodityStockSQLRequestDto input)
        {
            // 主表：库存表
            var stockQuery = _repository.GetAll();
            // 分类表
            var categoryQuery = _categoryRepository.GetAll().Where(p => p.Type == (int)EnumCommodityStockType.CommodityStocks);
            // 库存变更记录表
            var recordQuery = _recordRepository.GetAll();
            // 申请明细表
            var applyDetailQuery = _stockApplicationItemRepository.GetAll();

            // 1. 构建分组统计子查询
            // 领用申请数量分组
            var applyNumGroup = applyDetailQuery
                .Where(d => !d.IsDeleted && d.ReturnStatus == false && (d.ApplyType == "1" || d.ApplyType == "2"))
                .GroupBy(d => d.ProductId)
                .Select(g => new { ProductId = g.Key, StockApplyNum = (int?)g.Sum(d => d.ApplyCount) });

            // 异常数量分组(状态1/2/3/4)
            var abnormalNumGroup = recordQuery
                .Where(r => !r.IsDeleted && (r.Status == 1 || r.Status == 2 || r.Status == 3 || r.Status == 4))
                .GroupBy(r => r.CommodityStockId)
                .Select(g => new { ProductId = g.Key, AbnormalNum = (int?)g.Sum(r => r.Number) });

            // 新增数量分组(状态0/5)  新购0、其他新增5  ，这个也合计在库存数量里面，其他状态的变更记录，就为异常数量，总库存要减去异常数
            var newNumGroup = recordQuery
                .Where(r => !r.IsDeleted && (r.Status == 0 || r.Status == 5))
                .GroupBy(r => r.CommodityStockId)
                .Select(g => new { ProductId = g.Key, NewNum = (int?)g.Sum(r => r.Number) });

            // 2. EF Core 联表查询（左连接）
            var query = from t in stockQuery
                        join c in categoryQuery on t.CategoryId equals c.Id
                        // 左连接 申请数量
                        join detail in applyNumGroup on t.Id equals detail.ProductId into detailGroup
                        from detail in detailGroup.DefaultIfEmpty()
                            // 左连接 异常数量
                        join abnormal in abnormalNumGroup on t.Id equals abnormal.ProductId into abnormalGroup
                        from abnormal in abnormalGroup.DefaultIfEmpty()
                            // 左连接 新增数量
                        join new_r in newNumGroup on t.Id equals new_r.ProductId into newGroup
                        from new_r in newGroup.DefaultIfEmpty()

                        join t9 in _dataItemDetailRepository.GetAllIncluding(p => p.Item).Where(p => p.Item.ItemCode == "stockAddr") on t.Address equals t9.ItemValue into itemdetails
                        from itemdetail in itemdetails.DefaultIfEmpty()
                            // 基础条件
                        where !t.IsDeleted && c.Type == (int)EnumCommodityStockType.CommodityStocks
                        select new CommodityStockSQLDto
                        {
                            Id = t.Id,
                            ProductName = t.Name,
                            Category = t.CategoryId,
                            CategoryText = c.Name,
                            Model = t.Model,
                            SN = t.SN,
                            Price = t.Price,
                            Address = t.Address,
                            //Number = t.Number + (new_r == null ? 0 : new_r.NewNum),
                            Number = t.Number + (new_r.NewNum ?? 0),

                            Status = t.Status ?? 0,
                            Description = t.Description,
                            CreationTime = t.CreationTime,
                            CreatorUserId = t.CreatorUserId,
                            LastModificationTime = t.LastModificationTime,
                            LastModifierUserId = t.LastModifierUserId,

                            //StockApplyNum = detail.StockApplyNum,
                            //AbnormalNum =  abnormal.AbnormalNum,
                            //RemainingNum = t.Number + new_r.NewNum - detail.StockApplyNum - abnormal.AbnormalNum,
                            StockApplyNum = detail.StockApplyNum ?? 0,
                            AbnormalNum = abnormal.AbnormalNum ?? 0,
                            RemainingNum = t.Number + (new_r.NewNum ?? 0) - (detail.StockApplyNum ?? 0) - (abnormal.AbnormalNum ?? 0),
                            //RemainingNum = t.Number + (new_r == null ? 0 : new_r.NewNum) - (detail == null ? 0 : detail.StockApplyNum) - (abnormal == null ? 0 : abnormal.AbnormalNum),
                            Addr = itemdetail.ItemName
                        };

            // 3. 基础过滤
            query = query
                .WhereIf(input.CategoryId.HasValue && input.CategoryId.Value != Guid.Empty, t => t.Category == input.CategoryId.Value)
                .WhereIf(input.Status.HasValue, t => t.Status == input.Status.Value)
                .WhereIf(!string.IsNullOrEmpty(input.Model), t => t.Model.Contains(input.Model))
                .WhereIf(!string.IsNullOrEmpty(input.SN), t => t.SN.Contains(input.SN))
                .WhereIf(!string.IsNullOrEmpty(input.Address), t => t.Address == input.Address);

            if (!string.IsNullOrEmpty(input.Keyword))
            {
                var userQuery = _userRepository.GetAll();
                var applyQuery = _stockApplicationRepository.GetAll();
                var applyDetailAllQuery = _stockApplicationItemRepository.GetAll();

                var existsQuery = from a in applyQuery
                                  join b in applyDetailAllQuery on a.Id equals b.StockApplicationId
                                  join u in userQuery on a.UserId equals u.Id
                                  where !a.IsDeleted && !b.IsDeleted
                                  select new { b.ProductId, u.UserName, u.Name };

                query = query.Where(t =>
                    t.ProductName.Contains(input.Keyword)  // 商品名称模糊
                    || existsQuery.Any(x => x.ProductId == t.Id && (x.UserName == input.Keyword || x.Name.Contains(input.Keyword)))
                );
            }

            // 申请用户筛选（保持不变，SQL 原样）
            if (!string.IsNullOrEmpty(input.ApplyUser))
            {
                var userQuery = _userRepository.GetAll();
                var applyQuery = _stockApplicationRepository.GetAll();
                var applyDetailAllQuery = _stockApplicationItemRepository.GetAll();

                var existsQuery = from a in applyQuery
                                  join b in applyDetailAllQuery on a.Id equals b.StockApplicationId
                                  join u in userQuery on a.UserId equals u.Id
                                  where !a.IsDeleted && !b.IsDeleted
                                  select new { b.ProductId, u.UserName, u.Name };

                query = query.Where(t =>
                    existsQuery.Any(x => x.ProductId == t.Id && (x.UserName == input.ApplyUser || x.Name.Contains(input.ApplyUser)))
                );
            }

            // 排序
            if (!string.IsNullOrWhiteSpace(input.Sorting))
                query = query.OrderBy(input.Sorting);
            else
                query = query.OrderByDescending(t => t.CreationTime);


            var totalCount = await query.CountAsync();
            var items = await query.PageBy(input).ToListAsync();

            return new PagedResultDto<CommodityStockSQLDto>(totalCount, items);
        }

        #region 提交
        /// <summary>
        /// 新增库存信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPost]
        public async Task<CommodityStockDto> CreateAsync(CreateCommodityStockDto input)
        {
            CommodityStock entity = ObjectMapper.Map<CommodityStock>(input);

            if (input.CategoryId != Guid.Empty)
            {
                var catrgory = _categoryRepository.Get(input.CategoryId);
                entity.Category = catrgory;
            }

            Guid Id = await _repository.InsertAndGetIdAsync(entity);

            return ObjectMapper.Map<CommodityStockDto>(entity);
        }
        /// <summary>
        /// 修改库存信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPost]
        public async Task<CommodityStockDto> UpdateAsync(CreateCommodityStockDto input)
        {
            var entity = _repository.Get(input.Id.Value);

            ObjectMapper.Map(input, entity);
            if (input.CategoryId != Guid.Empty)
            {
                var catrgory = _categoryRepository.Get(input.CategoryId);
                entity.Category = catrgory;
            }

            await _repository.UpdateAsync(entity);


            return ObjectMapper.Map<CommodityStockDto>(entity);
        }

        /// <summary>
        /// 修改库存数量信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPost]
        public async Task<CommodityStockDto> UpdateNumberAsync(UpdateCommodityStockRequestDto input)
        {
            var entity = _repository.Get(input.Id);
            entity.Number = input.Number;
            await _repository.UpdateAsync(entity);

            return ObjectMapper.Map<CommodityStockDto>(entity);
        }
        #endregion

        #region 根据id获取当前实体 
        /// <summary>
        /// 获取-电脑库存-当前数据  编辑反显调用
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>

        public async Task<CommodityStockDto> GetStoockInfoAsync(EntityDto<Guid> input)
        {
            var entity = await _repository.GetAllIncluding(t => t.Category).Where(d => d.Id == input.Id).FirstOrDefaultAsync();
            var records = _recordRepository.GetAllIncluding().Where(q => q.CommodityStock.Id == entity.Id && (q.Status == 0 || q.Status == 5));
            var dto = ObjectMapper.Map<CommodityStockDto>(entity);
            dto.TotalNumber = (entity.Number ?? 0) + records.Sum(d => d.Number);
            return dto;
        }
        /// <summary>
        /// 获取当前数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>

        public async Task<CommodityStockDto> GetInfoAsync(EntityDto<Guid> input)
        {
            var entity = await _repository.GetAllIncluding(t => t.Category).Where(d => d.Id == input.Id).FirstOrDefaultAsync();
            var dto = ObjectMapper.Map<CommodityStockDto>(entity);
            return dto;
        }
        #endregion

        #endregion

        #region 电脑归还

        /// <summary>
        /// 电脑归还明细查询语句、库存管理单个商品领用明细
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private IQueryable<StockReturnRecoredDto> GetQuerySql(PagedStockReturnResultRequestDto input)
        {
            //公司电话
            var companyPhonesQuery = from cp in _companyPhoneRepository.GetAll()
                                     group cp by cp.EmployeeId into g
                                     select new { UserId = g.Key, Phones = string.Join(",", g.Select(x => x.Telephone)) };

            // 主查询
            var query = from t1 in _stockApplicationRepository.GetAll().Where(x => !x.IsDeleted)
                        join t2 in _stockApplicationItemRepository.GetAll().Where(x => !x.IsDeleted) on t1.Id equals t2.StockApplicationId

                        join t3 in _repository.GetAll() on t2.ProductId equals t3.Id

                        join t8 in this._categoryRepository.GetAll() on t3.CategoryId equals t8.Id into r_d
                        from category in r_d.DefaultIfEmpty()

                        join t4 in _employeeInfoRepository.GetAll() on t1.UserId equals t4.UserId into managers
                        from employeeInfo in managers.DefaultIfEmpty()

                        join t6 in _orgRepository.GetAll() on employeeInfo.CompanyId equals t6.Id into companys
                        from company in companys.DefaultIfEmpty()

                        join department in _orgRepository.GetAll() on employeeInfo.DepartmentId equals department.Id into departments
                        from department in departments.DefaultIfEmpty()

                        join t9 in _dataItemDetailRepository.GetAllIncluding(p => p.Item).Where(p => p.Item.ItemCode == "stockApplyType") on t2.ApplyType equals t9.ItemValue into itemdetails
                        from itemdetail in itemdetails.DefaultIfEmpty()

                        join phone in companyPhonesQuery on employeeInfo.UserId equals phone.UserId into phoneGroup
                        from phone in phoneGroup.DefaultIfEmpty()
                        select new StockReturnRecoredDto
                        {
                            DetailId = t2.Id,
                            ReturnStatus = t2.ReturnStatus,
                            ApplyType = t2.ApplyType,
                            ApplyCount = t2.ApplyCount,
                            ApplyTypeName = itemdetail.ItemName ?? "",
                            ProductId = t3.Id,
                            ProductName = t3.Name,
                            Model = t3.Model,
                            Category = t3.CategoryId,
                            CategoryName = category.Name ?? "",
                            SN = t3.SN,
                            Name = employeeInfo.Name ?? "",
                            UserName = employeeInfo.UserName ?? "",
                            OutJobDate = employeeInfo.OutJobDate,
                            IsActive = employeeInfo == null ? false : employeeInfo.IsActive,
                            CompanyName = company.DisplayName ?? "",
                            DepartName = department.DisplayName ?? "",
                            PhoneNumber = employeeInfo.PhoneNumber ?? "",
                            ProductApplyId = t1.Id,
                            CreationTime = t1.CreationTime,
                            BackTime = t2.BackTime,
                            BackUser = t2.BackUser,
                            BackRemark = t2.BackRemark,
                            Remarks = t2.Remark,
                            CompanyTelephone = phone.Phones ?? "",
                            //CompanyTelephone = phone==null?"": phone.Phones, 这种写法报错
                        };

            // 应用过滤条件
            if (!string.IsNullOrWhiteSpace(input.UserName))
            {
                query = query.Where(x => x.UserName == input.UserName ||
                                         (x.Name != null && x.Name.Contains(input.UserName)));
            }

            if (!string.IsNullOrWhiteSpace(input.Keyword))
            {
                query = query.Where(x => (x.UserName != null && x.UserName == input.Keyword) ||
                                         (x.Name != null && x.Name.Contains(input.Keyword)) ||
                                         (x.ProductName != null && x.ProductName.Contains(input.Keyword)) ||
                                         (x.Model != null && x.Model.Contains(input.Keyword)));
            }

            // 应用过滤条件
            if (!string.IsNullOrWhiteSpace(input.UserName))
            {
                query = query.Where(x => x.UserName == input.UserName ||
                                         (x.Name != null && x.Name.Contains(input.UserName)));
            }

            if (!string.IsNullOrWhiteSpace(input.Keyword))
            {
                query = query.Where(x => (x.UserName != null && x.UserName == input.Keyword) ||
                                         (x.Name != null && x.Name.Contains(input.Keyword)) ||
                                         (x.ProductName != null && x.ProductName.Contains(input.Keyword)) ||
                                         (x.Model != null && x.Model.Contains(input.Keyword)));
            }

            if (input.ApplyType >= 0)
            {
                query = query.Where(x => x.ApplyType == input.ApplyType.ToString());
            }

            if (input.ReturnStatus != null && input.ReturnStatus >= 0)
            {
                ////离职未归还
                if (input.ReturnStatus == 100)
                {
                    query = query.Where(x => x.ReturnStatus == false && x.IsActive == false);
                }
                else
                {
                    query = query.Where(x => x.ReturnStatus == Convert.ToBoolean(input.ReturnStatus));
                }
            }

            if (input.CategoryId != Guid.Empty)
            {
                query = query.Where(x => x.Category == input.CategoryId);
            }
            if (input.ProductId.HasValue && input.ProductId != Guid.Empty)
            {
                query = query.Where(x => x.ProductId == input.ProductId);
            }

            if (!string.IsNullOrWhiteSpace(input.Sorting))
                query = query.OrderBy(input.Sorting);
            else
                query = query.OrderByDescending(t => t.CreationTime);

            return query;
        }

        /// <summary>
        /// 电脑归还分页列表、库存管理单个商品领用明细
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[DataPermission("库存归还")]
        public async Task<PagedResultDto<StockReturnRecoredDto>> GetStockReturnPagedList(PagedStockReturnResultRequestDto input)
        {
            string keyword = string.IsNullOrWhiteSpace(input.Keyword) ? input.UserName : input.Keyword;
            var query = GetQuerySql(input);

            var totalCount = await query.CountAsync();
            var items = await query.PageBy(input).ToListAsync();

            foreach (var item in items)
            {
                item.ReturnStatusText = item.ReturnStatus == true ? "已归还" : "未归还";
                item.IsActiveText = item.IsActive == true ? "正常" : "禁用";
            }
            return new PagedResultDto<StockReturnRecoredDto>(totalCount, items);
        }

        /// <summary>
        /// 电脑归还-确认归还
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task SetStockReturn(SetStockReturnDto input)
        {
            if (ADTOSharpSession.UserId == null)
            {
                throw new UserFriendlyException($"请先登录！");
            }
            if (input.DetailIds == null || input.DetailIds.Count <= 0)
            {
                throw new UserFriendlyException($"请选择归还记录！");
            }
            //当前用户
            var user = _userRepository.Get(ADTOSharpSession.UserId.Value);

            foreach (var detailId in input.DetailIds)
            {
                var vInfo = await this._stockApplicationItemRepository.FirstOrDefaultAsync(p => p.Id == detailId);
                if (vInfo == null)
                {
                    throw new UserFriendlyException($"编号【{detailId}】的记录不存在！");
                }
                if (vInfo.ReturnStatus)
                {
                    throw new UserFriendlyException($"编号【{detailId}】已归还，不可以重复操作！");
                }
                await this._stockApplicationItemRepository.UpdateAsync(detailId, async entity =>
                {
                    entity.ReturnStatus = true;
                    entity.BackRemark = input.BackRemark;
                    entity.BackUserId = user.Id;
                    entity.BackUser = user.Name;
                    entity.BackTime = input.BackTime ?? DateTime.Now;
                });
            }
        }

        /// <summary>
        /// 电脑归还-导出统计
        /// 按人员分组，查该人员下所申请的所有产品 老StockReturnTJExportExcel
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<StockInventoryStatisticsDto>> StockInventoryStatisticsList(PagedStockReturnResultRequestDto input)
        {
            input.PageSize = int.MaxValue;
            string keyword = string.IsNullOrWhiteSpace(input.Keyword) ? input.UserName : input.Keyword;
            var query = GetQuerySql(input);

            // 2. 执行查询，获取数据
            var dataList = await query.ToListAsync();

            // 3. 按【人员】分组统计
            var result = dataList
                .GroupBy(x => new
                {
                    x.Name,
                    x.UserName,
                    x.DepartName
                })
                .Select(g => new StockInventoryStatisticsDto
                {
                    Name = g.Key.Name,
                    UserName = g.Key.UserName,
                    DepartName = g.Key.DepartName,

                    // 拼接该人员名下所有产品名（自动去重）
                    ProductNames = string.Join(",", g.Select(p => p.ProductName).Where(p => !string.IsNullOrEmpty(p)).Distinct())
                })
                .ToList();

            return result;
        }

        #endregion

        #region 库存统计
        /// <summary>
        /// 电脑库存类别统计
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[AbpAuthorize(PermissionNames.Pages_StockStatistics)]
        public async Task<PagedResultDto<StockCategoryStatisticsDto>> GetCommodityStockStatistics(PagedCommodityStockStatisticsRequestDto input)
        {
            // 子查询 
            var abnormalQuery = _recordRepository.GetAll()
                .Where(r => !r.IsDeleted && new[] { 1, 2, 3, 4 }.Contains(r.Status))
                .GroupBy(r => r.CommodityStockId)
                .Select(g => new { ProductId = g.Key, AbnormalNum = g.Sum(r => (int?)r.Number) })
                .AsNoTracking();

            var detailQuery = _stockApplicationItemRepository.GetAll()
                .Where(d => !d.IsDeleted && new[] { "1", "2" }.Contains(d.ApplyType))
                .GroupBy(d => d.ProductId)
                .Select(g => new { ProductId = g.Key, StockApplyNum = g.Sum(d => (int?)d.ApplyCount) })
                .AsNoTracking();

            var newRecordQuery = _recordRepository.GetAll()
                .Where(r => !r.IsDeleted && new[] { 0, 5 }.Contains(r.Status))
                .GroupBy(r => r.CommodityStockId)
                .Select(g => new { ProductId = g.Key, NewNum = g.Sum(r => (int?)r.Number) })
                .AsNoTracking();

            var query = from category in _categoryRepository.GetAll().AsNoTracking()
                        join stock in _repository.GetAll().AsNoTracking()
                            on category.Id equals stock.CategoryId
                        join abnormal in abnormalQuery
                            on stock.Id equals abnormal.ProductId into abnormalJoin
                        from abnormal in abnormalJoin.DefaultIfEmpty()
                        join detail in detailQuery
                            on stock.Id equals detail.ProductId into detailJoin
                        from detail in detailJoin.DefaultIfEmpty()
                        join newRecord in newRecordQuery
                            on stock.Id equals newRecord.ProductId into newRecordJoin
                        from newRecord in newRecordJoin.DefaultIfEmpty()
                        where category.Type == 1
                        select new { category, stock, abnormal, detail, newRecord };

            // 关键词过滤
            if (!string.IsNullOrWhiteSpace(input.Keyword))
            {
                query = query.Where(x => x.category.Name.Contains(input.Keyword));
            }

            // 分组聚合
            var groupedQuery = query
                .GroupBy(x => new { x.category.Id, x.category.Name })
                .Select(g => new StockCategoryStatisticsDto
                {
                    Id = g.Key.Id,
                    CategoryName = g.Key.Name,
                    Num = (g.Sum(x => x.stock.Number) ?? 0) + (g.Sum(x => x.newRecord.NewNum) ?? 0),
                    StockApplyNumber = g.Sum(x => x.detail.StockApplyNum) ?? 0,
                    AbnormalNum = g.Sum(x => x.abnormal.AbnormalNum) ?? 0,
                    RemainingNumber = (g.Sum(x => x.stock.Number) ?? 0)
                        - (g.Sum(x => x.detail.StockApplyNum) ?? 0)
                        - (g.Sum(x => x.abnormal.AbnormalNum) ?? 0)
                });

            var totalCount = await groupedQuery.CountAsync();
            var items = await groupedQuery.PageBy(input).ToListAsync();

            return new PagedResultDto<StockCategoryStatisticsDto>(totalCount, items);
        }
        #endregion


    }
}
