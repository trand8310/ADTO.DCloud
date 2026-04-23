using ADTO.DCloud.ApplicationForm.EmailRequireForm.Dto;
using ADTO.DCloud.ApplicationForm.MerchandiseApplication.Dto;
using ADTO.DCloud.ApplicationForm.StockApplication.Dto;
using ADTO.DCloud.ApplicationForm.Stocks.Dto;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.CommodityStocks;
using ADTO.DCloud.DataAuthorizes;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.WorkFlow.NodeMethod;
using ADTO.DCloud.WorkFlow.Processes;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Organizations;
using ADTOSharp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm.MerchandiseApplication
{
    /// <summary>
    /// 总裁办物品申请
    /// </summary>
    [ADTOSharpAuthorize]
    public class OfficeSupplyApplicationAppService : DCloudAppServiceBase, IOfficeSupplyApplicationAppService
    {
        private readonly IRepository<Adto_OfficeSupplyApplication, Guid> _repository;
        private readonly IRepository<Adto_OfficeSupplyApplicationItem, Guid> _itemRepository;
        private readonly IRepository<CommodityStock, Guid> _commodityStockRepository;
        private readonly IRepository<OrganizationUnit, Guid> _orgRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IRepository<WorkFlowProcess, Guid> _processRepository;
        private readonly DataFilterService _dataAuthorizesApp;
        #region ctor
        public OfficeSupplyApplicationAppService(IRepository<Adto_OfficeSupplyApplication, Guid> repository,
            IRepository<Adto_OfficeSupplyApplicationItem, Guid> itemRepository,
            IRepository<CommodityStock, Guid> commodityStockRepository,
              IRepository<OrganizationUnit, Guid> orgRepository,
              IRepository<User, Guid> userRepository,
              DataFilterService dataAuthorizesApp,
              IRepository<WorkFlowProcess, Guid> processRepository)
        {
            _repository = repository;
            _itemRepository = itemRepository;
            _commodityStockRepository = commodityStockRepository;
            _orgRepository = orgRepository;
            _userRepository = userRepository;
            _processRepository = processRepository;
            _dataAuthorizesApp = dataAuthorizesApp;

        }
        #endregion

        #region 获取数据

        /// <summary>
        /// 单据管理-总裁办申请单据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DataAuthPermission("GetOfficeSupplyPagedList")]
        public async Task<PagedResultDto<OfficeSupplyApplicationDto>> GetOfficeSupplyPagedList(GetOfficeSupplyPagedListInput input)
        {
            if (input.StartDate == null)
            {
                //如果日期为空，默认显示近一个月
                input.StartDate = DateTime.Now.AddMonths(-1);
            }
            //因为添加日期为年与日，否则不会包含今天数据
            if (input.EndDate == null)
            {
                input.EndDate = DateTime.Now;
            }
            else
            {
                input.EndDate = input.EndDate.Value.Add(new TimeSpan(23, 59, 59));
            }
            var query = from r in this._repository.GetAll()
                        join p in this._processRepository.GetAll() on r.Id equals p.Id into p_j
                        from process in p_j.DefaultIfEmpty()  // 左连接 p 允许流程不存在，表单存在的情况,人事可作废申请表
                        join u in this._userRepository.GetAll() on r.UserId equals u.Id
                        join d in this._orgRepository.GetAll() on u.DepartmentId equals d.Id
                        join c in this._orgRepository.GetAll() on u.CompanyId equals c.Id
                        select new OfficeSupplyApplicationDto
                        {
                            Id = r.Id,
                            Title = r.Title,
                            UserId = r.UserId,
                            UserName = u.UserName,
                            Name = u.Name,
                            CompanyId = c.Id,
                            CompanyName = c.DisplayName,
                            DepartmentId = r.DepartmentId,
                            DepartmentName = d.DisplayName,
                            CreationTime = r.CreationTime,
                            CreatorUserId = r.CreatorUserId,
                            Remark = r.Remark,
                            IsFinished = process == null ? 0 : process.IsFinished ?? 0,//== 1 ? "结束" : "审核中",
                            SchemeCode = process == null ? "" : process.SchemeCode
                        };
            query = query.Where(x => x.CreationTime >= input.StartDate && x.CreationTime <= input.EndDate)
                     .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), p => p.UserName == input.Keyword || p.Name.Contains(input.Keyword))
                     .WhereIf(!string.IsNullOrWhiteSpace(input.Title), p => p.Title.Contains(input.Title))
                         .WhereIf(!string.IsNullOrWhiteSpace(input.CompanyId.ToString()), p => p.CompanyId == input.CompanyId)
                         .WhereIf(input.DepartmentId != null, p => p.DepartmentId == input.DepartmentId)
                    ;

            string permissionCode = PermissionHelper.GetPermissionCode(this.GetType().GetMethod(nameof(GetOfficeSupplyPagedList)));
            //数据权限
            query = await _dataAuthorizesApp.CreateDataFilteredQuery(query, permissionCode);

            //自定义排序
            if (!string.IsNullOrWhiteSpace(input.Sorting))
                query = query.OrderBy(input.Sorting);
            //获取总数
            var resultCount = await query.CountAsync();
            var taskList = query.PageBy(input).ToList();
            return new PagedResultDto<OfficeSupplyApplicationDto>(resultCount, taskList);
        }
        /// <summary>
        /// 获取分页列表总裁办申请单数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<OfficeSupplyApplicationDto>> GetAllPageListAsync(GetPagedOfficeSupplyApplicationApplyInput input)
        {
            var query = _repository.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.Filter), q => q.Title.Contains(input.Filter));
            if (input.Sorting != null)
                query = query.OrderBy(input.Sorting);
            var totalCount = query.Count();
            var list = await query.PageBy(input).ToListAsync();
            var listDtos = ObjectMapper.Map<List<OfficeSupplyApplicationDto>>(list);
            return new PagedResultDto<OfficeSupplyApplicationDto>(totalCount, listDtos);
        }

        /// <summary>
        /// 获取流程设计(依流程Id)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<OfficeSupplyApplicationDto> GetAsync(EntityDto<Guid> input)
        {
            var scheme = await _repository.GetAsync(input.Id);
            return ObjectMapper.Map<OfficeSupplyApplicationDto>(scheme);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 新增总裁办申请单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<OfficeSupplyApplicationDto> CreateAsync(CreateOfficeSupplyApplicationDto input)
        {
            try
            {
                var dto = ObjectMapper.Map<Adto_OfficeSupplyApplication>(input);
                await _repository.InsertAsync(dto);
                CurrentUnitOfWork.SaveChanges();
                return ObjectMapper.Map<OfficeSupplyApplicationDto>(dto);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        /// <summary>
        /// 新增总裁办申请明细
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<OfficeSupplyApplicationItemDto> CreateItemAsync(CreateOfficeSupplyApplicationItemDto input)
        {
            try
            {
                var entity = ObjectMapper.Map<Adto_OfficeSupplyApplicationItem>(input);
                var office = await _repository.GetAsync(input.OfficeSupplyApplicationId);
                entity.OfficeSupplyApplication = office;
                await _itemRepository.InsertAsync(entity);
                var kc = await _commodityStockRepository.FirstOrDefaultAsync(q => q.Id.Equals(input.ProductId));
                entity.Category = kc?.CategoryId;
                CurrentUnitOfWork.SaveChanges();
                return ObjectMapper.Map<OfficeSupplyApplicationItemDto>(entity);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        /// <summary>
        /// 修改总裁办申请单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<OfficeSupplyApplicationItemDto> UpdateItemAsync(CreateOfficeSupplyApplicationItemDto input)
        {
            try
            {
                var entity = await _itemRepository.GetAsync(input.Id);
                var stock = await _repository.GetAsync(input.OfficeSupplyApplicationId);
                ObjectMapper.Map(input, entity);
                entity.OfficeSupplyApplication = stock;
                var kc = await _commodityStockRepository.FirstOrDefaultAsync(q => q.Id.Equals(input.ProductId));
                entity.Category = kc?.CategoryId;
                await _itemRepository.UpdateAsync(entity);
                return ObjectMapper.Map<OfficeSupplyApplicationItemDto>(entity); ;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        /// <summary>
        /// 修改总裁办申请单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<OfficeSupplyApplicationDto> UpdateAsync(UpdateOfficeSupplyApplicationDto input)
        {
            try
            {
                var entity = await _repository.GetAsync(input.Id);

                ObjectMapper.Map(input, entity);
                await _repository.UpdateAsync(entity);
                return await GetAsync(new EntityDto<Guid>() { Id = entity.Id });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        /// <summary>
        /// 删除总裁办申请单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteAsync(EntityDto<Guid> input)
        {
            await _repository.DeleteAsync(input.Id);

        }
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task Execute(Guid id)
        {
            await this.GetAsync(new EntityDto<Guid>() { Id = id });
        }
        /// <summary>
        /// 新增流程表单
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>

        public async Task ExecuteInsert(string data)
        {
            var input = data.ToObject<WfMethodDto>();
            if (input.TableName == "Adto_OfficeSupplyApplications")
            {
                var dto = input.Data.ToObject<CreateOfficeSupplyApplicationDto>();
                await this.CreateAsync(dto);
            }
            else
            {
                var dto = input.Data.ToObject<CreateOfficeSupplyApplicationItemDto>();
                await this.CreateItemAsync(dto);
            }
        }

        /// <summary>
        /// 修改流程表单
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task ExecuteUpdate(string data)
        {
            var input = data.ToObject<WfMethodDto>();
            if (input.TableName == "Adto_OfficeSupplyApplications")
            {
                var dto = input.Data.ToObject<UpdateOfficeSupplyApplicationDto>();
                await this.UpdateAsync(dto);
            }
            else
            {
                var dto = input.Data.ToObject<CreateOfficeSupplyApplicationItemDto>();
                await this.UpdateItemAsync(dto);
            }
        }
        /// <summary>
        /// 删除表单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task ExecuteDelete(Guid id)
        {
            await this.DeleteAsync(new EntityDto<Guid>() { Id = id });
        }
        #endregion

    }
}
