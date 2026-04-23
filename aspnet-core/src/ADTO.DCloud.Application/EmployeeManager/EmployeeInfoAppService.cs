using ADTO.DCloud.ApplicationForm.Abs.Dto;
using ADTO.DCloud.Attendances.Attendance.Dto;
using ADTO.DCloud.Attendances.AttendanceLocations;
using ADTO.DCloud.Attendances.AttendanceTimeRules;
using ADTO.DCloud.Authorization;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Authorization.Users.Dto;
using ADTO.DCloud.Customers.Dto;
using ADTO.DCloud.DataAuthorizes;
using ADTO.DCloud.DataItem;
using ADTO.DCloud.DataSource.Dto;
using ADTO.DCloud.DeptRoles.Dto;
using ADTO.DCloud.Dto;
using ADTO.DCloud.EmployeeManager.Dto;
using ADTO.DCloud.EntityFrameworkCore.Repositories;
using ADTO.DCloud.Infrastructur;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.Media.UploadFiles;
using ADTO.DCloud.Migrations;
using ADTO.DCloud.Sessions;
using ADTO.DCloud.WorkFlow.Processs.Dto;
using ADTO.DistributedLocking;
using ADTOSharp;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Values;
using ADTOSharp.EntityFrameworkCore.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Organizations;
using ADTOSharp.Runtime.Caching;
using ADTOSharp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NuGet.Protocol.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ADTO.DCloud.EmployeeManager
{
    /// <summary>
    /// 员工信息
    /// </summary>
    [ADTOSharpAuthorize(PermissionNames.Pages_AdministrationHR_Employee)]
    public class EmployeeInfoAppService : DCloudAppServiceBase, IEmployeeInfoAppService
    {
        #region Fields
        protected IADTODistributedLock DistributedLock { get; }
        private readonly IRepository<EmployeeInfo, Guid> _repository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IRepository<EmployeeFamilies, Guid> _familiesRepository;
        private readonly IRepository<EmployeeContracts, Guid> _contractRepository;
        private readonly IRepository<EmployeeChangeLog, Guid> _changeLogRepository;
        private readonly IRepository<OrganizationUnit, Guid> _orgRepository;
        private readonly UserAppService _userAppService;
        private static IConfiguration _configuration;
        private readonly DataItemDetailAppService _dataitemDetailAppService;
        private readonly IAttendanceLocationsAppService _attendanceLocationsAppService;
        private readonly IAttendanceTimeRuleAppService _attendanceTimeRuleAppService;
        private readonly ICacheManager _cacheManager;
        private readonly IUploadFileAppService _uploadFileAppService;
        private readonly IGuidGenerator _guidGenerator;
        private readonly DataFilterService _dataAuthorizesApp;

        #endregion

        #region Ctor
        public EmployeeInfoAppService(IRepository<EmployeeInfo, Guid> repository,
            IRepository<User, Guid> userRepository,
            IRepository<EmployeeFamilies, Guid> familiesRepository,
            IRepository<EmployeeContracts, Guid> contractRepository,
            UserAppService userAppService,
            IRepository<EmployeeChangeLog, Guid> changeLogRepository,
            IADTODistributedLock distributedLock,
            IRepository<OrganizationUnit, Guid> orgRepository,
            DataItemDetailAppService dataitemDetailAppService,
            AttendanceLocationsAppService attendanceLocationsAppService,
            AttendanceTimeRuleAppService attendanceTimeRuleAppService,
            ICacheManager cacheManager,
            IGuidGenerator guidGenerator,
           IUploadFileAppService uploadFileAppService,
           DataFilterService dataAuthorizesApp
            )
        {
            _repository = repository;
            _userRepository = userRepository;
            _familiesRepository = familiesRepository;
            _contractRepository = contractRepository;
            _userAppService = userAppService;
            _changeLogRepository = changeLogRepository;
            DistributedLock = distributedLock;
            _orgRepository = orgRepository;
            _dataitemDetailAppService = dataitemDetailAppService;
            _attendanceLocationsAppService = attendanceLocationsAppService;
            _attendanceTimeRuleAppService = attendanceTimeRuleAppService;
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            _cacheManager = cacheManager;
            _guidGenerator = guidGenerator;
            _uploadFileAppService = uploadFileAppService;
            _dataAuthorizesApp = dataAuthorizesApp;
        }
        #endregion


        #region 数据查询
        /// <summary>
        /// 获取分页列表数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<EmployeeInfoDto>> GetAllPageListAsync(GetEmployeePagedInput input)
        {
            var query = from employee in _repository.GetAll()
                        join company in _orgRepository.GetAll() on employee.CompanyId equals company.Id into companys
                        from company in companys.DefaultIfEmpty()
                        join department in _orgRepository.GetAll() on employee.DepartmentId equals department.Id into departments
                        from department in departments.DefaultIfEmpty()
                        join contract in _contractRepository.GetAll() on employee.ContractId equals contract.Id into contracts
                        from contract in contracts.DefaultIfEmpty()
                        join familie in _familiesRepository.GetAll() on employee.FamilieId equals familie.Id into families
                        from familie in families.DefaultIfEmpty()
                        join manager in _userRepository.GetAll() on employee.ManagerId equals manager.Id into managers
                        from manager in managers.DefaultIfEmpty()
                        select new { employee, company, department, contract, familie, manager, CreationTime = employee.CreationTime };
            query = query.WhereIf(!string.IsNullOrWhiteSpace(input.KeyWord), q => q.employee.Name.Contains(input.KeyWord) || q.employee.UserName.Equals(input.KeyWord))
                .WhereIf(input.CompanyId.HasValue, q => q.company.Id.Equals(input.CompanyId))
                .WhereIf(input.DepartmentId.HasValue, q => q.department.Id.Equals(input.DepartmentId))
                .WhereIf(input.AccountApprovalStatus.HasValue, q => q.employee.AccountApprovalStatus.Equals(input.AccountApprovalStatus))//审批状态
                 .WhereIf(input.IsActive.HasValue, q => q.employee.IsActive == input.IsActive)//员工状态
                 .WhereIf(!string.IsNullOrWhiteSpace(input.Gender), q => q.employee.Gender == input.Gender)//性别
                  .WhereIf(input.ManagerId.HasValue, q => q.employee.ManagerId.Equals(input.ManagerId))//直属上级
                 .WhereIf(!string.IsNullOrWhiteSpace(input.PoliticalOutlook), q => q.familie.PoliticalOutlook == input.PoliticalOutlook)//政治面貌
                .WhereIf(input.InJobDate.HasValue, q => q.employee.InJobDate >= new DateTime(input.InJobDate.Value.Year, input.InJobDate.Value.Month, 1) && q.employee.InJobDate <= new DateTime(input.InJobDate.Value.Year, input.InJobDate.Value.Month + 1, 1).AddDays(-1))//入职日期按月查询
                .WhereIf(input.OutJobDate.HasValue, q => q.employee.OutJobDate >= new DateTime(input.OutJobDate.Value.Year, input.OutJobDate.Value.Month, 1) && q.employee.OutJobDate <= new DateTime(input.OutJobDate.Value.Year, input.OutJobDate.Value.Month + 1, 1).AddDays(-1))//离职日期按月查询
                .WhereIf(!string.IsNullOrWhiteSpace(input.IsAttType), q => q.employee.IsAttType == input.IsAttType)//打卡类型
                .WhereIf(input.EmployeeType.HasValue, q => q.employee.EmployeeType == input.EmployeeType.ToString())//人员类型
                .WhereIf(input.AttTimeRuleId.HasValue, q => q.employee.AttTimeRuleId.Equals(input.AttTimeRuleId))//（考勤时间）-对应之前的考勤区域
                .WhereIf(input.OfficeLocation.HasValue, q => q.employee.OfficeLocation.Equals(input.OfficeLocation))//办公地点
                .WhereIf(!string.IsNullOrWhiteSpace(input.ContractCompanyId), q => q.contract.ContractCompanyId.Equals(input.ContractCompanyId))//合同主体公司
                .WhereIf(input.RegularDate.HasValue, q => q.contract.RegularDate >= new DateTime(input.RegularDate.Value.Year, input.RegularDate.Value.Month, 1) && q.contract.RegularDate <= new DateTime(input.RegularDate.Value.Year, input.RegularDate.Value.Month + 1, 1).AddDays(-1))//转正日期
                .WhereIf(!string.IsNullOrWhiteSpace(input.SuperiorDepartmentId), q => q.employee.SuperiorDepartmentId.Equals(input.SuperiorDepartmentId))//上级部门
                .WhereIf(!string.IsNullOrWhiteSpace(input.DivisionCompany), q => q.employee.DivisionCompany.Equals(input.DivisionCompany))//所属事业部
                .WhereIf(!string.IsNullOrWhiteSpace(input.BusinessGroup), q => q.employee.BusinessGroup.Contains(input.BusinessGroup))//业务小组
                .WhereIf(!string.IsNullOrWhiteSpace(input.Position), q => q.employee.Position.Contains(input.Position))//职务
                .WhereIf(!string.IsNullOrWhiteSpace(input.PostLevelId), q => q.employee.Position.Contains(input.PostLevelId))//职级
                ;

            #region 
            //var query = _repository.GetAllIncluding(d => d.Department, c => c.Company, f => f.Familie, c1 => c1.Contract)
            //    .WhereIf(!string.IsNullOrWhiteSpace(input.KeyWord), q => q.Name.Contains(input.KeyWord) || q.UserName.Equals(input.KeyWord))
            //    .WhereIf(input.CompanyId.HasValue, q => q.Company.Id.Equals(input.CompanyId))
            //    .WhereIf(input.DepartmentId.HasValue, q => q.Department.Id.Equals(input.DepartmentId))
            //    .WhereIf(input.AccountApprovalStatus.HasValue, q => q.AccountApprovalStatus.Equals(input.AccountApprovalStatus))//审批状态
            //     .WhereIf(input.IsActive.HasValue, q => q.IsActive == input.IsActive)//员工状态
            //     .WhereIf(!string.IsNullOrWhiteSpace(input.Gender), q => q.Gender == input.Gender)//性别
            //      .WhereIf(input.ManagerId.HasValue, q => q.ManagerId.Equals(input.ManagerId))//直属上级
            //     .WhereIf(!string.IsNullOrWhiteSpace(input.PoliticalOutlook), q => q.Familie.PoliticalOutlook == input.PoliticalOutlook)//政治面貌
            //    .WhereIf(input.InJobDate.HasValue, q => q.InJobDate >= new DateTime(input.InJobDate.Value.Year, input.InJobDate.Value.Month, 1) && q.InJobDate <= new DateTime(input.InJobDate.Value.Year, input.InJobDate.Value.Month + 1, 1).AddDays(-1))//入职日期按月查询
            //    .WhereIf(input.OutJobDate.HasValue, q => q.OutJobDate >= new DateTime(input.OutJobDate.Value.Year, input.OutJobDate.Value.Month, 1) && q.OutJobDate <= new DateTime(input.OutJobDate.Value.Year, input.OutJobDate.Value.Month + 1, 1).AddDays(-1))//离职日期按月查询
            //    .WhereIf(!string.IsNullOrWhiteSpace(input.IsAttType), q => q.IsAttType == input.IsAttType)//打卡类型
            //    .WhereIf(input.EmployeeType.HasValue, q => q.EmployeeType == input.EmployeeType.ToString())//人员类型
            //    .WhereIf(input.AttTimeRuleId.HasValue, q => q.AttTimeRuleId.Equals(input.AttTimeRuleId))//（考勤时间）-对应之前的考勤区域
            //    .WhereIf(input.OfficeLocation.HasValue, q => q.OfficeLocation.Equals(input.OfficeLocation))//办公地点
            //    .WhereIf(!string.IsNullOrWhiteSpace(input.ContractCompanyId), q => q.Contract.ContractCompanyId.Equals(input.ContractCompanyId))//合同主体公司
            //    .WhereIf(input.RegularDate.HasValue, q => q.Contract.RegularDate >= new DateTime(input.RegularDate.Value.Year, input.RegularDate.Value.Month, 1) && q.Contract.RegularDate <= new DateTime(input.RegularDate.Value.Year, input.RegularDate.Value.Month + 1, 1).AddDays(-1))//转正日期
            //    .WhereIf(!string.IsNullOrWhiteSpace(input.SuperiorDepartmentId), q => q.SuperiorDepartmentId.Equals(input.SuperiorDepartmentId))//上级部门
            //    .WhereIf(!string.IsNullOrWhiteSpace(input.DivisionCompany), q => q.DivisionCompany.Equals(input.DivisionCompany))//所属事业部
            //    .WhereIf(!string.IsNullOrWhiteSpace(input.BusinessGroup), q => q.BusinessGroup.Contains(input.BusinessGroup))//业务小组
            //    .WhereIf(!string.IsNullOrWhiteSpace(input.Position), q => q.Position.Contains(input.Position))//职务
            //    .WhereIf(!string.IsNullOrWhiteSpace(input.PostLevelId), q => q.Position.Contains(input.PostLevelId))//职级
            //    ;
            #endregion


            if (input.Sorting != null)
                query = query.OrderBy(input.Sorting);
            var totalCount = query.Count();
            var list = await query.PageBy(input).ToListAsync();

            var postLevel = await _dataitemDetailAppService.GetItemDetailList(new DataItem.Dto.DataItemQueryDto() { ItemCode = "PostLevelId" });
            var locations = await _attendanceLocationsAppService.GetAllAsync(new Attendances.AttendanceLocations.Dto.GetAttendanceLocationInput() { IsActive = true });
            var attTimeRules = await _attendanceTimeRuleAppService.GetAllAsync(new Attendances.AttendanceTimeRules.Dto.GetAttendanceTimeRuleInput() { IsActive = true });
            //var users = await UserManager.GetUsersAsync();
            var listDtos = list.Select(item =>
            {
                var dto = ObjectMapper.Map<EmployeeInfoDto>(item.employee);
                dto.CompanyName = item.company?.DisplayName;
                dto.CompanyId = item.company?.Id;
                dto.Department = item.department?.DisplayName;
                dto.DepartmentId = item.department?.Id;
                dto.OfficeLocationText = locations.FirstOrDefault(q => q.Id == dto.OfficeLocation)?.LocationName;
                dto.PostLevel = postLevel.FirstOrDefault(q => q.ItemValue == dto.PostLevelId)?.ItemName;
                dto.AttTimeRule = attTimeRules.FirstOrDefault(q => q.Id == dto.AttTimeRuleId)?.RuleName;
                //if (item.manager!=null&&!string.IsNullOrEmpty(item.manager.Name))
                dto.ManagerName = item.manager?.Name; //users.FirstOrDefault(d => d.Id.Equals(item.employee.ManagerId))?.Name;
                return dto;
            }).ToList();
            return new PagedResultDto<EmployeeInfoDto>(totalCount, listDtos);
        }

        /// <summary>
        /// 获取员工信息(依员工Id)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<EmployeeInfoDto> GetAsync(EntityDto<Guid> input)
        {
            var query = from employee in this._repository.GetAllIncluding(f => f.Familie, c => c.Contract)
                        join user in _userRepository.GetAll() on employee.ManagerId equals user.Id into user
                        from users in user.DefaultIfEmpty()
                        join company in _orgRepository.GetAll() on employee.CompanyId equals company.Id into companys
                        from company in companys.DefaultIfEmpty()
                        join department in _orgRepository.GetAll() on employee.DepartmentId equals department.Id into departments
                        from department in departments.DefaultIfEmpty()
                        where employee.Id.Equals(input.Id)
                        select new { company, department, users, employee }
                        ;

            var info = await query.FirstOrDefaultAsync();

            var dto = ObjectMapper.Map<EmployeeInfoDto>(info.employee);
            dto.ManagerName = info.users?.Name;
            dto.Department = info.department?.DisplayName;
            dto.CompanyName = info.company?.DisplayName;
            if (info.employee.Familie != null && !string.IsNullOrWhiteSpace(info.employee.Familie.IdCardAttach) && info.employee.Familie.IdCardAttach != Guid.Empty.ToString())
            {
                var uploadFileList = await this._uploadFileAppService.GetFileUrlListByFolderId(Guid.Parse(info.employee.Familie.IdCardAttach));
                dto.Familie.IdCardAttachList = uploadFileList.Select(item =>
                {
                    UploadFilesInputDto uploadFilesDto = new UploadFilesInputDto()
                    {
                        FileId = item.Id,
                        Url = item.FullAddress,
                        Name = item.FileName
                    };
                    return uploadFilesDto;
                }).ToList();
            }
            return dto;
        }
        /// <summary>
        /// 获取员工信息(依员工Id)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<EmployeeInfoDto> GetEmployeeInfoAsync(EntityDto<Guid> input)
        {
            if (!ADTOSharpSession.UserId.HasValue)
                throw new ADTOSharpException(L("AbnormalUserLoginStatus"));

            var userId = input.Id == Guid.Empty ? ADTOSharpSession.UserId.Value : input.Id;
            var cacheManager = _cacheManager.GetCache($"DCloud.Employee");
            var cacheKey = $"GetEmployeeInfo.{userId}";
            var cacheVal = await cacheManager.GetOrDefaultAsync(cacheKey) as EmployeeInfoDto;
            if (cacheVal == null || cacheVal.Id == Guid.Empty)
            {
                var query = from employee in this._repository.GetAllIncluding(d => d.Department, com => com.Company, f => f.Familie, c => c.Contract)
                            join user in _userRepository.GetAll() on employee.UserId equals user.Id into user
                            from users in user.DefaultIfEmpty()
                            where employee.Id.Equals(input.Id)
                            select new { users, employee }
                     ;
                var info = await query.FirstOrDefaultAsync();

                var dto = ObjectMapper.Map<EmployeeInfoDto>(info.employee);
                dto.ManagerName = info.users.Name;
                dto.Department = info.employee.Department.DisplayName;
                dto.CompanyName = info.employee.Company.DisplayName;
                await cacheManager.SetAsync(cacheKey, dto);
                return dto;
                //var info = await _repository.GetAllIncluding(c => c.Company, d => d.Department).FirstOrDefaultAsync(q => q.Id.Equals(input.Id));
                //var dto = ObjectMapper.Map<EmployeeInfoDto>(info);
                //if (info.ManagerId.HasValue)
                //{
                //    var user = await UserManager.GetUserByIdAsync(info.ManagerId.Value);
                //    dto.ManagerName = user != null ? user.Name : "";
                //}
                //return dto;
            }
            else
            {
                return cacheVal;
            }
        }
        #endregion


        #region 获取员工信息
        /// <summary>
        /// 在职和离职人员，离职时间和入职时间在当前时间范围内的人员
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<GetUserIsActiveDto>> GetUserJobDate(GetJobDateRequestInput input)
        {
            var query = _repository.GetAll().Where(x => x.IsAttType != EnumUserCheckInType.NoCheckin.ToString());
            query = query.WhereIf(input.CompanyId.HasValue, x => x.CompanyId.Equals(input.CompanyId))
                .WhereIf(input.DepartmentId.HasValue, x => x.DepartmentId.Equals(input.DepartmentId))
                .WhereIf(!input.Keyword.IsNullOrEmpty(), x => x.Name.Equals(input.Keyword) || x.UserName.Equals(input.Keyword))
                .WhereIf(input.StartDate.HasValue && input.EndDate.HasValue, x => (x.IsActive && x.InJobDate <= input.EndDate && (x.OutJobDate >= input.EndDate || x.OutJobDate == null || x.InJobDate == null))
                || (x.IsActive.Equals(false) && x.InJobDate <= input.EndDate && x.OutJobDate >= input.EndDate)
                || (x.IsActive.Equals(false) && x.InJobDate <= input.EndDate && x.OutJobDate >= input.StartDate))
                .WhereIf(input.IsAttType.HasValue, x => x.IsAttType.Equals(input.IsAttType))
                .WhereIf(input.AttTimeRuleId.HasValue, x => x.AttTimeRuleId.Equals(input.AttTimeRuleId))//考勤时间
                .WhereIf(input.OfficeLocation.HasValue, x => x.OfficeLocation.Equals(input.OfficeLocation))//办公地点
                ;
            //获取总数
            var resultCount = await query.CountAsync();
            var list = query.PageBy(input).ToList();
            var listDtos = list.Select(item =>
            {
                var dto = ObjectMapper.Map<GetUserIsActiveDto>(item);
                return dto;
            }).ToList();
            return listDtos;
        }
        #endregion

        #region 员工开户信息
        /// <summary>
        /// 员工信息开户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateUserAccountAsync(CreateUserAccountDto input)
        {
            var entity = ObjectMapper.Map<EmployeeInfo>(input);
            if (input.CompanyId.HasValue)
                entity.Company = await _orgRepository.FirstOrDefaultAsync(input.CompanyId.Value);
            if (input.DepartmentId.HasValue)
                entity.Department = await _orgRepository.FirstOrDefaultAsync(input.DepartmentId.Value);
            await using (var handle = await DistributedLock.TryAcquireAsync(nameof(EmployeeInfoAppService)))
            {
                if (handle != null)
                {
                    var user = _repository.GetAll().Max(u => u.UserName);
                    entity.UserName = (user == null ? 40003 : int.Parse(user) + 1).ToString();
                    CreateOrUpdateUserInput userDto = new CreateOrUpdateUserInput()
                    {
                        User = new UserEditDto()
                        {
                            Name = entity.Name,
                            UserName = entity.UserName.ToString(),
                            CompanyId = entity.Company.Id,
                            Gender = entity.Gender,
                            PhoneNumber = entity.PhoneNumber,
                            EmailAddress = string.IsNullOrEmpty(entity.Email) ? $"{entity.UserName}.adtogroup.com" : entity.Email,
                            Password = string.IsNullOrEmpty(input.Password) ? _configuration["AuthServer:InitialPassword"] : input.Password,
                            IsActive = entity.IsActive,
                            ManagerId = input.ManagerId
                        },
                        AssignedRoleNames = ["普通员工"],
                        OrganizationUnits = [entity.Department.Id],
                    };
                    var _user = await _userAppService.CreateOrUpdateUserAsync(userDto);
                    entity.User = _user;
                    await _repository.InsertAsync(entity);
                }
            }
        }
        /// <summary>
        /// 员工开户基本信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateUserAccountAsync(UpdateUserAccountDto input)
        {
            var entity = await _repository.GetAllIncluding(con => con.Contract, f => f.Familie).FirstOrDefaultAsync(q => q.Id.Equals(input.Id));
            var employeeDto = ObjectMapper.Map<EmployeeInfo>(input);

            if (input.CompanyId.HasValue)
                entity.Company = await _orgRepository.FirstOrDefaultAsync(input.CompanyId.Value);
            if (input.DepartmentId.HasValue)
                entity.Department = await _orgRepository.FirstOrDefaultAsync(input.DepartmentId.Value);
            if (input.ManagerId.HasValue)
                entity.Manager = await _userRepository.FirstOrDefaultAsync(input.ManagerId.Value);

            await this.GetChanges<EmployeeInfo>(entity, employeeDto, "EmployeeInfo", entity.Id);
            CreateOrUpdateUserInput userDto = new CreateOrUpdateUserInput()
            {
                User = new UserEditDto()
                {
                    Id = entity.UserId,
                    Name = input.Name,
                    UserName = input.UserName,
                    CompanyId = input.CompanyId,
                    Gender = input.Gender,
                    PhoneNumber = input.PhoneNumber,
                    EmailAddress = string.IsNullOrEmpty(entity.Email) ? $"{entity.UserName}.adtogroup.com" : entity.Email,
                    IsActive = input.IsActive,
                    ManagerId = input.ManagerId
                },
                OrganizationUnits = [input.DepartmentId.Value],
            };
            var userid = await _userAppService.CreateOrUpdateUserAsync(userDto);
            ObjectMapper.Map(input, entity);
            await _repository.UpdateAsync(entity);
        }
        #endregion

        #region 提交数据

        /// <summary>
        /// 新增员工信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async Task CreateAsync(CreateEmployeeInfoDto input)
        {
            try
            {
                var entity = ObjectMapper.Map<EmployeeInfo>(input);
                if (input.CompanyId.HasValue)
                    entity.Company = await _orgRepository.FirstOrDefaultAsync(input.CompanyId.Value);
                if (input.DepartmentId.HasValue)
                    entity.Department = await _orgRepository.FirstOrDefaultAsync(input.DepartmentId.Value);
                //在这里执行的代码,由REDIS来管控全局可入,在同一个时间点内,不管多少个节点,只有一个节点可以进入这里.
                await using (var handle = await DistributedLock.TryAcquireAsync(nameof(EmployeeInfoAppService)))
                {
                    if (handle != null)
                    {
                        var user = _repository.GetAll().Max(u => u.UserName);
                        entity.UserName = (user == null ? 40003 : int.Parse(user) + 1).ToString();
                        CreateOrUpdateUserInput userDto = new CreateOrUpdateUserInput()
                        {
                            User = new UserEditDto()
                            {
                                Name = entity.Name,
                                UserName = entity.UserName.ToString(),
                                CompanyId = entity.Company.Id,
                                Gender = entity.Gender,
                                PhoneNumber = entity.PhoneNumber,
                                EmailAddress = string.IsNullOrWhiteSpace(entity.Email) ? $"{entity.UserName}@adtogroup.com" : entity.Email,
                                Password = string.IsNullOrEmpty(input.Password) ? _configuration["AuthServer:InitialPassword"] : input.Password,
                                IsActive = entity.IsActive,
                                ManagerId = input.ManagerId
                            },
                            AssignedRoleNames = ["普通员工"],
                            OrganizationUnits = [entity.Department.Id],
                        };
                        var _user = await _userAppService.CreateOrUpdateUserAsync(userDto);
                        entity.User = _user;
                        if (input.Familie != null)
                        {
                            #region 附件上传
                            //离职证明附件
                            var IdCardAttachId = await _uploadFileAppService.UploadFolderFileAsync(input.Familie.IdCardAttach, input.Familie.IdCardAttachList, _user.Id, input.Familie.Id);
                            entity.Familie.IdCardAttach = IdCardAttachId;
                            //毕业证
                            var diplomaAttachId = await _uploadFileAppService.UploadFolderFileAsync(input.Familie.DiplomaAttach, input.Familie.DiplomaAttachList, _user.Id, input.Familie.Id);
                            entity.Familie.DiplomaAttach = diplomaAttachId;
                            //学位证
                            var degreeDiplomaAttachId = await _uploadFileAppService.UploadFolderFileAsync(input.Familie.DegreeDiplomaAttach, input.Familie.DegreeDiplomaAttachList, _user.Id, input.Familie.Id);
                            entity.Familie.DegreeDiplomaAttach = degreeDiplomaAttachId;
                            //离职证明
                            var leavCertificateAttachId = await _uploadFileAppService.UploadFolderFileAsync(input.Familie.LeavCertificateAttach, input.Familie.LeavCertificateAttachList, _user.Id, input.Familie.Id);
                            entity.Familie.LeavCertificateAttach = leavCertificateAttachId;
                            //其他证明
                            var certificatesAttachId = await _uploadFileAppService.UploadFolderFileAsync(input.Familie.CertificatesAttach, input.Familie.CertificatesAttachList, _user.Id, input.Familie.Id);
                            entity.Familie.CertificatesAttach = certificatesAttachId;
                            #endregion
                        }
                        if (input.Contract != null)
                        {
                            //资料提交
                            var AttachmentId = await _uploadFileAppService.UploadFolderFileAsync(input.Contract.Attachment, input.Contract.AttachmentList, _user.Id, entity.ContractId);
                            entity.Contract.Attachment = AttachmentId;
                        }
                        await _repository.InsertAsync(entity);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// 修改员工信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateAsync(UpdateEmployeeInfoDto input)
        {
            var entity = await _repository.GetAsync(input.Id);
            //var entity = await _repository.GetAllIncluding(con => con.Contract, f => f.Familie).FirstOrDefaultAsync(q => q.Id.Equals(input.Id));
            if (entity == null || entity.Id == Guid.Empty)
            {
                throw new UserFriendlyException(L("employee.EmployeeInformationDoesNotExist"));
            }
            var user = await UserManager.GetUserByIdAsync(entity.UserId);
            if (user == null)
            {
                throw new UserFriendlyException(L("employee.EmployeeInformationDoesNotExist"));
            }
            var employeeDto = ObjectMapper.Map<EmployeeInfo>(input);
            if (input.CompanyId.HasValue)
                entity.Company = await _orgRepository.FirstOrDefaultAsync(input.CompanyId.Value);
            if (input.DepartmentId.HasValue)
                entity.Department = await _orgRepository.FirstOrDefaultAsync(input.DepartmentId.Value);
            if (input.ManagerId.HasValue)
                entity.Manager = await _userRepository.FirstOrDefaultAsync(input.ManagerId.Value);
            await this.GetChanges<EmployeeInfo>(entity, employeeDto, "EmployeeInfo", entity.Id);
            user.CompanyId = input.CompanyId;
            user.Gender = input.Gender;
            user.PhoneNumber = input.PhoneNumber;
            user.EmailAddress = string.IsNullOrWhiteSpace(entity.Email) ? $"{entity.UserName}@adtogroup.com" : entity.Email;
            user.IsActive = input.IsActive;
            user.ManagerId = input.ManagerId;
            await UserManager.UpdateAsync(user);
            ObjectMapper.Map(input, entity);


            if (input.Familie != null && input.Familie.Id != Guid.Empty)
            {
                var familie = await _familiesRepository.FirstOrDefaultAsync(input.Familie.Id);
                #region 附件上传
                //离职证明附件
                var idCardAttachId = await _uploadFileAppService.UploadFolderFileAsync(familie.IdCardAttach, input.Familie.IdCardAttachList, user.Id, familie.Id);
                input.Familie.IdCardAttach = idCardAttachId;
                //毕业证
                var diplomaAttachId = await _uploadFileAppService.UploadFolderFileAsync(familie.DiplomaAttach, input.Familie.DiplomaAttachList, user.Id, familie.Id);
                input.Familie.DiplomaAttach = diplomaAttachId;
                //学位证
                var degreeDiplomaAttachId = await _uploadFileAppService.UploadFolderFileAsync(familie.DegreeDiplomaAttach, input.Familie.DegreeDiplomaAttachList, user.Id, familie.Id);
                input.Familie.DegreeDiplomaAttach = degreeDiplomaAttachId;
                //离职证明
                var leavCertificateAttachId = await _uploadFileAppService.UploadFolderFileAsync(familie.LeavCertificateAttach, input.Familie.LeavCertificateAttachList, user.Id, familie.Id);
                input.Familie.LeavCertificateAttach = leavCertificateAttachId;
                //其他证明
                var certificatesAttachId = await _uploadFileAppService.UploadFolderFileAsync(familie.CertificatesAttach, input.Familie.CertificatesAttachList, user.Id, familie.Id);
                input.Familie.CertificatesAttach = certificatesAttachId;
                #endregion

                //新增修改记录
                await this.GetChanges<EmployeeFamilies>(entity.Familie, familie, "EmployeeFamilies", entity.Familie.Id);
                ObjectMapper.Map(input.Familie, familie);
                entity.Familie = familie;
            }

            if (input.Contract != null && input.Contract.Id != Guid.Empty)
            {
                var contract = await _contractRepository.FirstOrDefaultAsync(input.Contract.Id);// ObjectMapper.Map<EmployeeContracts>(input.Contract);
                //资料提交
                var AttachmentId = await _uploadFileAppService.UploadFolderFileAsync(contract.Attachment, input.Contract.AttachmentList, user.Id, contract.Id);
                contract.Attachment = AttachmentId;
                if (input.Contract.Id != Guid.Empty)
                {
                    await this.GetChanges<EmployeeContracts>(entity.Contract, contract, "EmployeeContracts", entity.Contract.Id);
                }
                ObjectMapper.Map(input.Contract, contract);
                entity.Contract = contract;
            }
            await _repository.UpdateAsync(entity);
        }
        /// <summary>
        /// 删除员工信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteAsync(EntityDto<Guid> input)
        {
            var entity = await _repository.GetAsync(input.Id);
            if (entity != null && entity.UserId != Guid.Empty)
                await _userAppService.DeleteUserAsync(new EntityDto<Guid>() { Id = entity.UserId });
            await _repository.DeleteAsync(input.Id);

        }
        /// <summary>
        /// 修改用户开户审批状态（默认改为已审批）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task UpdateApprovalStatusAsync(List<Guid> ids)
        {
            foreach (var item in ids)
            {
                var entity = await _repository.FirstOrDefaultAsync(item);
                if (entity != null)
                {
                    entity.AccountApprovalStatus = true;//用户开户已审批
                    await _repository.UpdateAsync(entity);
                    var user = await _userRepository.FirstOrDefaultAsync(entity.UserId);
                    if (user != null && user.Id != Guid.Empty)
                    {
                        user.IsActive = true;
                        await _userRepository.UpdateAsync(user);
                    }

                    if (!entity.AccountApprovalStatus)
                    {
                        //保存操作日志
                        await _changeLogRepository.InsertAsync(new EmployeeChangeLog
                        {
                            ObjectName = "EmployeeInfo",
                            Objectid = entity.Id,
                            PropertyName = "AccountApprovalStatus",
                            OldValue = entity.AccountApprovalStatus.ToString(),
                            NewValue = "true",
                        });
                    }
                }
            }
        }
        /// <summary>
        ///  禁用/启用员工状态  
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task UpdateIsActiveAsync(UpdateIsActiveDto input)
        {
            foreach (var item in input.Ids)
            {
                var entity = await _repository.FirstOrDefaultAsync(item);
                if (entity == null || entity.Id == Guid.Empty)
                    throw new UserFriendlyException(L("FailedToDisableEmployeeInformationDoesNotExist"));//操作失败，员工信息不存在
                if (entity != null && entity.Id != Guid.Empty)
                {
                    var user = await _userRepository.FirstOrDefaultAsync(entity.UserId);
                    if (user != null && user.Id != Guid.Empty)
                    {
                        user.IsActive = input.IsActive;
                        await _userRepository.UpdateAsync(user);
                    }
                    var dto = entity.ToJson();
                    await this.GetChanges<EmployeeInfo>(dto.ToObject<EmployeeInfo>(), entity, "EmployeeInfo", entity.Id);
                    //禁用账号的时候同时设置离职日期
                    if (!input.IsActive && input.OutJobDate.HasValue)
                        entity.OutJobDate = input.OutJobDate;
                    entity.IsActive = input.IsActive;//用户开户已审批
                    await _repository.UpdateAsync(entity);

                }
            }
        }
        #endregion

        #region 批量修改用户上级
        /// <summary>
        /// 根据员工Id批量修改直属上级
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateManagerAsync(UpdateManagerInput input)
        {
            var employees = await _repository.GetAll().Where(q => input.Ids.Contains(q.Id)).ToListAsync();
            var user = await _userRepository.GetAsync(input.UserManagerId);
            foreach (var item in employees)
            {
                item.Manager = user;
                await _repository.UpdateAsync(item);
            }
        }
        #endregion

        #region 获取用户年假信息
        /// <summary>
        /// 根据用户和时间获取当前用户年假
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<object> GetAnnualLeaveRecord(GetAnnualLeaveRecordInput input)
        {
            if (input.UserId == Guid.Empty)
            {
                throw new UserFriendlyException(L("AnnualLeaveRecord.UserNull"));
            }
            var userInfo = await _repository.GetAsync(input.UserId);
            if (userInfo.InJobDate == null)
            {
                return $"{userInfo.Name}{L("TheEmploymentDateIsBlank")}";
            }

            StringBuilder text = new StringBuilder();
            //入职时间
            var _jobindate = userInfo.InJobDate.Value;

            double days = 0;
            var isyear = false;
            var year = input.StartDate.Value.Year - userInfo.InJobDate.Value.Year;
            //默认年假为3天，满10年或者20年，按此计算方式5/12*入职满年限的次月到年底的月份数，超过10年+5天，超过20年+10天
            double annualleave = 3;

            double otherday = 0;
            //工作年限 
            int _workingYears = input.StartDate.Value.Year - userInfo.InJobDate.Value.Year;

            //入职月份的次月开始计算，+1是指包括自己
            //int _monthCount = (12 - (userInfo.detail.InJobDate.Value.Month == 12 ? _jobindate.AddMonths(1).Month : _jobindate.Month));
            int _monthCount = (12 - _jobindate.Month);
            //如果当前年份小于等于
            if ((_jobindate.Year == (input.StartDate.Value.Year - 1) || _jobindate.Year == input.StartDate.Value.Year) && input.StartDate.Value.Month <= _jobindate.Month)
                return $"{userInfo.Name}{_jobindate.ToString("yyyy-MM-dd")}{L("NoAnnualLeave")}";
            isyear = (input.StartDate.Value.Month - _jobindate.Month) <= 0 && _jobindate.Year != (input.StartDate.Value.Year - 1) ? true : false;
            //开始时间
            var _startime = isyear && _jobindate.Month < 12 ? input.StartDate.Value.AddYears(-1) : input.StartDate.Value;
            //结束时间
            var _endtime = isyear ? _jobindate.AddYears(year) : _jobindate.AddYears(year + 1);
            //年假开始月份
            int _starmonth = _jobindate.AddMonths(1).Month;
            //年假结束月份
            int _endmonth = _jobindate.Month;
            //2014年1月1日前入职的员工，年假区间是每年的1月1日-12月31日
            if (_jobindate.Year < 2014)
            {
                _startime = new DateTime(input.StartDate.Value.Year, 1, 1);
                _endtime = new DateTime(input.StartDate.Value.Year, 12, 31);
                _starmonth = 1;
                _endmonth = 12;
            }

            //入职年限满10年
            if (_workingYears > 10 && _workingYears <= 20)
            {
                //如果入职年限大于10小于20年假为10天
                annualleave = 8;
            }
            //入职年限满10年或者是20年,只有2014年1月1号之前入职的用户需要进行折算
            else if (_jobindate < ("2014-01-01").ToDate() && _workingYears == 10)
            {
                var count = Convert.ToDouble(5) / Convert.ToDouble(12) * _monthCount;
                otherday = (count - Math.Floor(count)) >= 0.5 ? (Math.Floor(count) + 0.5) : Math.Floor(count);
                annualleave = otherday + 3;
            }
            //入职年限满20年,只有2014年1月1号之前入职的用户需要进行折算
            else if (_jobindate < ("2014-01-01").ToDate() && _workingYears == 20)
            {
                var count = Convert.ToDouble(5) / Convert.ToDouble(12) * _monthCount;
                otherday = (count - Math.Floor(count)) >= 0.5 ? (Math.Floor(count) + 0.5) : Math.Floor(count);
                annualleave = otherday + 5;
            }
            else if (year == 10 && input.StartDate.Value.Month >= _jobindate.AddMonths(1).Month)
                annualleave = 8;
            else if (year == 20 && input.StartDate.Value.Month >= _jobindate.AddMonths(1).Month)
                annualleave = 13;
            //默认13天年假
            else if (_workingYears > 20)
                annualleave = 13;
            //未满1年没有年假
            else if (_workingYears < 1)
                annualleave = 0;
            //{0}于{1}入职,本次年假区间为{2}年{3}月1日-{4}年{5}月{6}日，可享受年假{7}天（除去春节已休的2天），
            text.Append(L("Employee.AnnualLeaveEeminder",
                userInfo.Name, _jobindate.ToString("yyyy-MM-dd"), _startime.Year, _starmonth, _endtime.Year, _endmonth, DateTime.DaysInMonth(_endtime.Year, _endmonth), annualleave));
            //其他年假大于0.5天表示10年或者是20年计算出的年假
            if (otherday >= 0.5)
                text.Append(string.Format(L("Employee.AnnualLeaveEeminderContinue"), otherday, _startime.Year, _jobindate.AddMonths(1).Month, _startime.Year));//其中{0}天年假需在{1}年{2}月1日~{3}年12月31日方可申请并休完。
            //获取已请假数据
            var edtime = new DateTime(_endtime.Year, _endmonth, DateTime.DaysInMonth(_endtime.Year, _endmonth)).AddSeconds(86399);

            return text.ToString();
        }
        #endregion

        #region 手机端开户信息提交
        /// <summary>
        /// 根据用户Id获取用户基本信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [ADTOSharpAllowAnonymous]
        public async Task<EmployeeBasicInformationDto> GetEmployeeBasicInformationAsync(EntityDto<Guid> input)
        {
            if (!ADTOSharpSession.UserId.HasValue)
                throw new ADTOSharpException(L("AbnormalUserLoginStatus"));
            var employee = await _repository.GetAllIncluding(t => t.Familie).FirstOrDefaultAsync(x => x.UserId.Equals(input.Id));
            if (employee == null || employee.Id == Guid.Empty)
                throw new UserFriendlyException(L("employee.EmployeeInformationDoesNotExist"));
            //if (employee.FamilieId.HasValue && employee.FamilieId != Guid.Empty)
            //{
            //    var family = await _familiesRepository.FirstOrDefaultAsync(x => x.Id.Equals(employee.FamilieId));
            //    employee.Familie = family;
            //}
            if (employee.AccountApprovalStatus)
                throw new UserFriendlyException(L("employee.EmployeeInformationAccountApprovalStatus"));


            return ObjectMapper.Map<EmployeeBasicInformationDto>(employee);
        }


        /// <summary>
        /// 员工开户基础信息提交-前端提交接口（手机端开户接口）
        /// </summary>c
        /// <param name="input"></param>
        /// <returns></returns>
        [ADTOSharpAllowAnonymous]
        public async Task SubmitEmployeeBasicInformation(SubmitEmployeeBasicInformationDto input)
        {
            if (!ADTOSharpSession.UserId.HasValue)
                throw new ADTOSharpException(L("AbnormalUserLoginStatus"));
            var entity = await _repository.GetAllIncluding(t => t.Familie).FirstOrDefaultAsync(x => x.Id.Equals(input.Id));
            if (entity == null || entity.Id == Guid.Empty)
            {
                throw new UserFriendlyException(L("employee.EmployeeInformationDoesNotExist"));
            }
            var user = await UserManager.GetUserByIdAsync(entity.UserId);
            if (user == null)
            {
                throw new UserFriendlyException(L("employee.EmployeeInformationDoesNotExist"));
            }
            var employeeDto = ObjectMapper.Map<EmployeeInfo>(input);
            await this.GetChanges<EmployeeInfo>(entity, employeeDto, "EmployeeInfo", entity.Id);
            user.Gender = input.Gender;
            user.PhoneNumber = input.PhoneNumber;
            user.EmailAddress = string.IsNullOrWhiteSpace(entity.Email) ? $"{entity.UserName}@adtogroup.com" : entity.Email;
            await UserManager.UpdateAsync(user);


            if (input.UserFamily != null)
            {
                //var familie = await _familiesRepository.GetAsync(input.UserFamily.Id);
                entity.Familie = entity.Familie ?? new EmployeeFamilies();
                #region 附件上传
                //离职证明附件
                var idCardAttachId = await _uploadFileAppService.UploadFolderFileAsync(entity.Familie.IdCardAttach, input.UserFamily.IdCardAttachList, user.Id, entity.Familie.Id);
                input.UserFamily.IdCardAttach = idCardAttachId;
                //毕业证
                var diplomaAttachId = await _uploadFileAppService.UploadFolderFileAsync(entity.Familie.DiplomaAttach, input.UserFamily.DiplomaAttachList, user.Id, entity.Familie.Id);
                input.UserFamily.DiplomaAttach = diplomaAttachId;
                //学位证
                var degreeDiplomaAttachId = await _uploadFileAppService.UploadFolderFileAsync(entity.Familie.DegreeDiplomaAttach, input.UserFamily.DegreeDiplomaAttachList, user.Id, entity.Familie.Id);
                input.UserFamily.DegreeDiplomaAttach = degreeDiplomaAttachId;
                //离职证明
                var leavCertificateAttachId = await _uploadFileAppService.UploadFolderFileAsync(entity.Familie.LeavCertificateAttach, input.UserFamily.LeavCertificateAttachList, user.Id, entity.Familie.Id);
                input.UserFamily.LeavCertificateAttach = leavCertificateAttachId;
                //其他证明
                var certificatesAttachId = await _uploadFileAppService.UploadFolderFileAsync(entity.Familie.CertificatesAttach, input.UserFamily.CertificatesAttachList, user.Id, entity.Familie.Id);
                input.UserFamily.CertificatesAttach = certificatesAttachId;
                #endregion
                //新增修改记录
                //ObjectMapper.Map(input.UserFamily, entity.Familie);
                //entity.Familie = familie;
            }

            ObjectMapper.Map(input, entity);
            await _repository.UpdateAsync(entity);
        }
        #endregion


        #region 二维码缓存
        /// <summary>
        /// 缓存二维码guid
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<string> EntryprocessQrcode(EntryprocessQrcodeInput input)
        {
            var cacheManager = _cacheManager.GetCache($"DCloud.Employee");
            var cacheKey = $"EntryprocessQrcode.{input.UserName}";
            var value = _guidGenerator.Create().ToString();
            var cacheVal = await cacheManager.GetOrDefaultAsync(cacheKey) as string;
            if (string.IsNullOrEmpty(cacheVal))
            {
                await cacheManager.SetAsync(cacheKey, value, TimeSpan.FromHours(2));//
                return value;
            }
            else
                return cacheVal;
        }
        #endregion

        #region 比较数据
        /// <summary>
        /// 比较两个实体数据-并且保存差异数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <param name="current"></param>
        /// <param name="type"></param>
        /// <param name="objectid"></param>
        /// <returns></returns>
        private async Task<int> GetChanges<T>(T original, T current, string type, Guid objectid)
        {
            if (original == null || current == null)
                return 0;
            var changes = new List<EmployeeChangeLog>();
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var oldValue = property.GetValue(original)?.ToString();
                var newValue = property.GetValue(current)?.ToString();
                if (!Equals(oldValue, newValue))
                {
                    changes.Add(new EmployeeChangeLog
                    {
                        ObjectName = type,
                        Objectid = objectid,
                        PropertyName = property.Name,
                        OldValue = oldValue,
                        NewValue = newValue
                    });
                }
            }
            await _changeLogRepository.InsertRangeAsync(changes);
            return changes.Count;
        }
        #endregion
    }
}
