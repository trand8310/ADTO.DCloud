using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.DeptRoles;
using ADTO.DCloud.DeptRoles.Dto;
using ADTO.DCloud.EmployeeManager.CompanyPhones.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Organizations;
using ADTOSharp.Runtime.Caching;
using ADTOSharp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.EmployeeManager.CompanyPhones
{

    /// <summary>
    /// 公司号码
    /// </summary>
    //[ADTOSharpAuthorize(PermissionNames.Pages_AdministrationHR_Employee_CompanyPhone)]
    public class CompanyPhoneAppService : DCloudAppServiceBase, ICompanyPhoneAppService
    {
        #region ctor
        private readonly IRepository<CompanyPhone, Guid> _repository;
        private readonly IRepository<EmployeeInfo, Guid> _employeerepository;
        private readonly IRepository<User, Guid> _userrepository;
        private readonly IRepository<OrganizationUnit, Guid> _orgRepository;
        public CompanyPhoneAppService(IRepository<CompanyPhone, Guid> repository,
            IRepository<EmployeeInfo, Guid> employeerepository,
            IRepository<OrganizationUnit, Guid> orgRepository,
            IRepository<User, Guid> userrepository
            )
        {
            _repository = repository;
            _employeerepository = employeerepository;
            _orgRepository = orgRepository;
            _userrepository = userrepository;
        }
        #endregion

        #region 获取数据

        /// <summary>
        /// 查询所有公司号码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<CompanyPhoneDto>> GetPagedListAsync(GetPagedCompanyPhoneInput input)
        {
            var query = from phone in _repository.GetAll()
                        where phone.IsDeleted == false
                        join createUser in _userrepository.GetAll() on phone.CreatorUserId equals createUser.Id into createUserGroup
                        from createUser in createUserGroup.DefaultIfEmpty()
                        join lastUser in _userrepository.GetAll() on phone.LastModifierUserId equals lastUser.Id into lastUserGroup
                        from lastUser in lastUserGroup.DefaultIfEmpty()
                        select new { phone, createUser, lastUser };
            query = query.WhereIf(input.EmployeeId.HasValue && input.EmployeeId != Guid.Empty, q => q.phone.EmployeeId.Equals(input.EmployeeId))
              .WhereIf(!string.IsNullOrWhiteSpace(input.keyword), q => q.phone.Telephone.Equals(input.keyword))
              .OrderByDescending(x => x.phone.ProcessingDate);
            //获取总数
            var resultCount = await query.CountAsync();
            var list = await query.PageBy(input).ToListAsync();
            var result = list.Select
            (item =>
            {
                CompanyPhoneDto dto = ObjectMapper.Map<CompanyPhoneDto>(item.phone);
                dto.LastModifierUserName = item.lastUser!=null? item.lastUser.Name:"";
                dto.CreatorUserName = item.createUser != null ? item.createUser.Name : "";
                return dto;
            }).ToList();
            return new PagedResultDto<CompanyPhoneDto>(resultCount, result);
        }
        /// <summary>
        /// 根据公司号码Id查询公司号码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<CompanyPhoneDto> GetAsync(EntityDto<Guid> input)
        {
            var entity = await _repository.GetAsync(input.Id);
            var dto = ObjectMapper.Map<CompanyPhoneDto>(entity);
            return dto;
        }
        /// <summary>
        /// 查询用户公司号码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<CompanyPhoneDto>> GetEmployeeCompanyPhone(GetCompanyPhonePagedRequestInput input)
        {
            //var query = _repository.GetAllIncluding(d => d.Employee)
            //    .WhereIf(input.companyId.HasValue,q=>q.Employee.CompanyId.Equals(input.companyId))
            //    .WhereIf(input.DepartmentId.HasValue,q=>q.Employee.DepartmentId.Equals(input.DepartmentId))
            //    .WhereIf(input.IsActive.HasValue,q=>q.IsActive.Equals(input.IsActive))
            //    .WhereIf(input.EmployeeType.HasValue,q=>q.Employee.EmployeeType.Equals(input.EmployeeType))
            //    //.WhereIf(input.Status.HasValue,q=> (input.Status == 1 ? "(t.CompanyTelephone is not null and t.CompanyTelephone<>'')" : "(t.CompanyTelephone is  null or t.CompanyTelephone='')"))
            //   .WhereIf(input.Status.HasValue,q=>(input.Status==1?(q.)))
            //   ;
            var query =
    from elemployee in _employeerepository.GetAll()
    where elemployee.IsDeleted == false
    join department in _orgRepository.GetAll() on elemployee.DepartmentId equals department.Id into deptGroup
    from department in deptGroup.DefaultIfEmpty()
    join company in _orgRepository.GetAll() on elemployee.CompanyId equals company.Id into companyGroup
    from company in companyGroup.DefaultIfEmpty()
    let companyPhones = _repository.GetAll().Where(q => q.EmployeeId.Equals(elemployee.Id))
    select new
    {
        elemployee.Id,
        elemployee.Name,
        elemployee.UserName,
        elemployee.IsActive,
        Gender = elemployee.Gender,
        elemployee.DepartmentId,
        Department = department != null ? department.DisplayName : null,
        elemployee.CompanyId,
        CompanyName = company != null ? company.DisplayName : null,
        elemployee.PhoneNumber,
        EmployeeType = elemployee.EmployeeType,
        InJobDate = elemployee.InJobDate,
        elemployee.CreationTime,
        // 处理CompanyTelephone字符串连接
        CompanyTelephone = string.Join(",", companyPhones.Select(cp => cp.Telephone)),
        // 处理ProcessingDates字符串连接
        ProcessingDates = string.Join(",", companyPhones.Select(cp =>
            cp.ProcessingDate.HasValue ?
            cp.ProcessingDate.Value.ToString("yyyy-MM-dd") :
            null))
    };

            //获取总数
            var resultCount = await query.CountAsync();
            var list = query.PageBy(input).ToList();
            return new PagedResultDto<CompanyPhoneDto>(resultCount, ObjectMapper.Map<List<CompanyPhoneDto>>(list));
        }
        #endregion

        #region 提交
        /// <summary>
        /// 新增公司号码信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task CreateAsync(CreateCompanyPhoneDto input)
        {
            var employee = await _employeerepository.GetAsync(input.EmployeeId);
            if (employee == null || employee.Id == Guid.Empty)
            {
                throw new UserFriendlyException(L("employee.EmployeeInformationDoesNotExist"));
            }

            CompanyPhone entity = ObjectMapper.Map<CompanyPhone>(input);
            entity.Employee = employee;
            await _repository.InsertAsync(entity);
        }
        /// <summary>
        /// 修改公司号码信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task UpdateAsync(UpdateCompanyPhoneDto input)
        {
            var employee = await _employeerepository.GetAsync(input.EmployeeId);
            if (employee == null || employee.Id == Guid.Empty)
            {
                throw new UserFriendlyException(L("employee.EmployeeInformationDoesNotExist"));
            }
            var entity = _repository.Get(input.Id);
            ObjectMapper.Map(input, entity);
            entity.Employee = employee;
            await _repository.UpdateAsync(entity);
        }
        #endregion

        #region 删除 
        /// <summary>
        /// 删除公司号码信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task DeleteAsync(EntityDto<Guid> input)
        {
            var entity = _repository.Get(input.Id);
            await _repository.DeleteAsync(entity);
        }

        #endregion
    }
}
