using ADTO.DCloud.Customers;
using ADTO.DCloud.DataReports.Dto;
using ADTO.DCloud.Project;
using ADTO.DCloud.ProjectManage.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataReports
{
    /// <summary>
    /// 手机端首页面板数据统计接口
    /// </summary>
    public class MobileHomeStatAppService : DCloudAppServiceBase, IMobileHomeStatAppService
    {
        private readonly IRepository<Customer, Guid> _customerRepository;
        private readonly IRepository<ProjectInfo, Guid> _projectInfoRepository;
        private readonly IRepository<ProjectContract, Guid> _contractRepository;
        private static IConfiguration _configuration;
        private readonly IRepository<ProjectFollowRecord, Guid> _projectFollowRepository;
        private readonly IRepository<CustomerFollowRecord, Guid> _customerfollowRepository;
        public MobileHomeStatAppService(IRepository<Customer, Guid> customerRepository
            , IRepository<ProjectInfo, Guid> projectInfoRepository
            , IRepository<ProjectContract, Guid> contractRepository
            , IRepository<ProjectFollowRecord, Guid> projectFollowRepository
            , IRepository<CustomerFollowRecord, Guid> customerfollowRepository)
        {
            _customerRepository = customerRepository;
            _projectInfoRepository = projectInfoRepository;
            _contractRepository = contractRepository;
            _projectFollowRepository = projectFollowRepository;
            _customerfollowRepository = customerfollowRepository;
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
        }
        /// <summary>
        /// 得到首页统计类型
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetTimeRanges()
        {
            var dict = Enum.GetValues(typeof(StatTimeRange))
                .Cast<StatTimeRange>()
                .ToDictionary(
                    k => k.ToString(),  // 枚举项名称作为Key
                    v => v switch       // 自定义Value显示文本
                    {
                        StatTimeRange.Today => "今天",
                        StatTimeRange.CurrentMonth => "本月",
                        //StatTimeRange.PreviousMonth => "上月",
                        StatTimeRange.CurrentQuarter => "本季度",
                        StatTimeRange.CurrentYear => "本年度",
                        //StatTimeRange.PreviousYear => "上年度",
                        // 其他项映射...
                        _ => v.ToString()
                    });

            return dict;
        }

        /// <summary>
        /// 获取首页统计数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<MobileHomeStatDto>> GetHomeStatList(MobileHomeStatRequestDto input)
        {
            List<MobileHomeStatDto> homeStats = new List<MobileHomeStatDto>();

            DateTime beginTime = DateTime.Now;
            DateTime endTime = DateTime.Now;
            DateTime preBeginTime = DateTime.Now;
            DateTime preEndTime = DateTime.Now;
            string preName = "";
            //计算日期范围
            switch (input.DateType)
            {
                // 今天
                case nameof(StatTimeRange.Today):
                    beginTime = DateTime.Today;
                    endTime = DateTime.Today.AddDays(1);
                    preBeginTime = DateTime.Today.AddDays(-1);
                    preEndTime = DateTime.Today;
                    preName = "昨天";
                    break;
                // 本月
                case nameof(StatTimeRange.CurrentMonth):
                    beginTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    endTime = beginTime.AddMonths(1);
                    preBeginTime = beginTime.AddMonths(-1);
                    preEndTime = beginTime;
                    preName = "上月";
                    break;
                // 本季度
                case nameof(StatTimeRange.CurrentQuarter):
                    int currentQuarter = (DateTime.Today.Month - 1) / 3 + 1;
                    beginTime = new DateTime(DateTime.Today.Year, (currentQuarter - 1) * 3 + 1, 1);
                    endTime = beginTime.AddMonths(3);
                    preBeginTime = beginTime.AddMonths(-3);
                    preEndTime = beginTime;
                    preName = "上季度";
                    break;
                // 本年
                case nameof(StatTimeRange.CurrentYear):
                    beginTime = new DateTime(DateTime.Today.Year, 1, 1);
                    endTime = beginTime.AddYears(1);
                    preBeginTime = beginTime.AddYears(-1);
                    preEndTime = beginTime;
                    preName = "上年度";
                    break;
                default:
                    throw new ArgumentException("Invalid time range type", nameof(input.DateType));
            }

            #region 冗余写法
            ////客户数量
            //MobileHomeStatDto cust = new MobileHomeStatDto();
            //cust.Name = "新建客户";
            //cust.Nums = await this._customerRepository.GetAll().Where(p => p.CreationTime >= beginTime && p.CreationTime < endTime).CountAsync();
            //cust.PreNum = await this._customerRepository.GetAll().Where(p => p.CreationTime >= preBeginTime && p.CreationTime < preEndTime).CountAsync();
            //// 计算增长率（处理除零情况）
            ////cust.FloatRatio = cust.PreNum == 0 ? (cust.Nums == 0 ? 0 : float.PositiveInfinity) : (cust.Nums - cust.PreNum) / (float)cust.PreNum * 100;
            //cust.FloatRatio = CalculateGrowthRate(cust.Nums,cust.PreNum);
            //cust.IconUrl = "https://adtoDCloud.oss-cn-beijing.aliyuncs.com/appfile/custnums.png";
            //cust.PreName=preName;
            //homeStats.Add(cust);

            ////项目数量
            //MobileHomeStatDto project = new MobileHomeStatDto();
            //project.Name = "新建项目";
            //project.Nums = await this._projectInfoRepository.GetAll().Where(p => p.CreationTime >= beginTime && p.CreationTime < endTime).CountAsync();
            //project.PreNum = await this._projectInfoRepository.GetAll().Where(p => p.CreationTime >= preBeginTime && p.CreationTime < preEndTime).CountAsync();
            //project.FloatRatio = CalculateGrowthRate(project.Nums, project.PreNum);
            //project.IconUrl = "https://adtoDCloud.oss-cn-beijing.aliyuncs.com/appfile/projectnums.png";
            //project.PreName = preName;
            //homeStats.Add(project);

            ////合同数量
            //MobileHomeStatDto contract = new MobileHomeStatDto();
            //contract.Name = "合同签约";
            //contract.Nums = await this._contractRepository.GetAll().Where(p => p.CreationTime >= beginTime && p.CreationTime < endTime).CountAsync();
            //contract.PreNum = await this._contractRepository.GetAll().Where(p => p.CreationTime >= preBeginTime && p.CreationTime < preEndTime).CountAsync();
            //contract.FloatRatio = CalculateGrowthRate(contract.Nums, contract.PreNum);
            //contract.IconUrl = "https://adtoDCloud.oss-cn-beijing.aliyuncs.com/appfile/follownus.png";
            //contract.PreName = preName;
            //homeStats.Add(contract);
            #endregion
            var cdnDomainName = _configuration["Aliyun:cdnDomainName"];
            var cust = await CreateStatDtoAsync(
                this._customerRepository,
                "新建客户",
                cdnDomainName+"/appfile/custnums.png",
                preName,
                beginTime,
                endTime,
                preBeginTime,
                preEndTime);
            homeStats.Add(cust);

            var project = await CreateStatDtoAsync(
                this._projectInfoRepository,
                "新建项目",
                cdnDomainName + "/appfile/projectnums.png",
                preName,
                beginTime,
                endTime,
                preBeginTime,
                preEndTime);
            homeStats.Add(project);

            var contract = await CreateStatDtoAsync(
              this._contractRepository,
              "合同签约",
              cdnDomainName + "/appfile/follownus.png",
              preName,
              beginTime,
              endTime,
              preBeginTime,
              preEndTime);
            homeStats.Add(contract);

            //跟进记录数量（客户+项目）
            MobileHomeStatDto follow = new MobileHomeStatDto();
            follow.Name = "跟进记录";
            var projectFollowNums = await this._projectFollowRepository.GetAll().Where(p => p.CreationTime >= beginTime && p.CreationTime < endTime).CountAsync();
            var customerFollowNums = await this._customerfollowRepository.GetAll().Where(p => p.CreationTime >= beginTime && p.CreationTime < endTime).CountAsync();
            follow.Nums = projectFollowNums + customerFollowNums;

            var preProjectFollowNums = await this._projectFollowRepository.GetAll().Where(p => p.CreationTime >= preBeginTime && p.CreationTime < preEndTime).CountAsync();
            var preCustomerFollowNums = await this._customerfollowRepository.GetAll().Where(p => p.CreationTime >= preBeginTime && p.CreationTime < preEndTime).CountAsync();
            follow.PreNum = preProjectFollowNums + preCustomerFollowNums;
            follow.FloatRatio = CalculateGrowthRate(follow.Nums, follow.PreNum);
            follow.IconUrl = cdnDomainName + "appfile/contractnums.png";
            follow.PreName = preName;
            homeStats.Add(follow);

            return homeStats;
        }

        /// <summary>
        /// 计算增长率
        /// </summary>
        private float CalculateGrowthRate(int current, int previous)
        {
            if (previous == 0)
            {
                return current == 0 ? 0 : float.PositiveInfinity;
            }
            return (current - previous) / (float)previous * 100;
        }

        /// <summary>
        ///  查询指定对象日期范围内数据条数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="repository">仓储对象</param>
        /// <param name="name">查询类型名称</param>
        /// <param name="iconUrl">图标地址</param>
        /// <param name="preName">上月、上年度、上季度等</param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="preBeginTime"></param>
        /// <param name="preEndTime"></param>
        /// <returns></returns>
        private async Task<MobileHomeStatDto> CreateStatDtoAsync<T>(
            IRepository<T, Guid> repository,
            string name,
            string iconUrl,
            string preName,
            DateTime beginTime,
            DateTime endTime,
            DateTime preBeginTime,
            DateTime preEndTime)
            where T : class, IEntity<Guid>, IHasCreationTime  // 添加 IEntity<int> 约束
        {
            var dto = new MobileHomeStatDto
            {
                Name = name,
                Nums = await repository.GetAll()
                    .Where(p => p.CreationTime >= beginTime && p.CreationTime < endTime)
                    .CountAsync(),
                PreNum = await repository.GetAll()
                    .Where(p => p.CreationTime >= preBeginTime && p.CreationTime < preEndTime)
                    .CountAsync(),
                IconUrl = iconUrl,
                PreName = preName
            };
            dto.FloatRatio = CalculateGrowthRate(dto.Nums, dto.PreNum);
            return dto;
        }
        /// <summary>
        /// 首页项目展示列表-不授权分页项目列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<ProjectInfoDto>> GetHomeProjectPageList(PagedProjectInfoRequestDto input)
        {
            var query = _projectInfoRepository.GetAll();
            if (!string.IsNullOrWhiteSpace(input.KeyWord))
            {
                query = query.Where(p => p.Name.Contains(input.KeyWord) || p.Code == input.KeyWord);
            }
            if (input.StartDate.HasValue)
            {
                query = query.Where(p => p.CreationTime >= input.StartDate);
            }
            if (input.EndDate.HasValue)
            {
                query = query.Where(p => p.CreationTime <= input.EndDate.Value.AddDays(1));
            }
            if (input.MinAmount > 0)
            {
                query = query.Where(p => p.Amount >= input.MinAmount);
            }
            if (input.MaxAmount > 0)
            {
                query = query.Where(p => p.Amount <= input.MaxAmount);
            }
            var totalCount = await query.CountAsync();
            //根据更新日期排序
            var items = await query.OrderByDescending(p => p.CreationTime).PageBy(input).ToListAsync();
            var list = items.Select(item =>
            {
                var dto = ObjectMapper.Map<ProjectInfoDto>(item);
                return dto;
            }).ToList();
            return new PagedResultDto<ProjectInfoDto>(totalCount, list);
        }


    }
}
