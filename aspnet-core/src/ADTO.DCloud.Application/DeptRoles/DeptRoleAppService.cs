
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using ADTOSharp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using ADTO.DCloud.DeptRoles.Dto;
using Microsoft.AspNetCore.Mvc;

namespace ADTO.DCloud.DeptRoles
{
    /// <summary>
    /// 部门角色管理
    /// </summary>
    public class DeptRoleAppService : DCloudAppServiceBase, IDeptRoleAppService
    {
        private readonly IRepository<DeptRole, Guid> _repository;

        public DeptRoleAppService(IRepository<DeptRole, Guid> repository)
        {
            _repository = repository;
        }

        #region 获取数据
        /// <summary>
        /// 获取部门角色数据列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IEnumerable<DeptRoleDto>> GetAllAsync(GetDeptRoleInput input)
        {
            var query = _repository.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.KeyWord), q => q.Name.Contains(input.KeyWord) || q.Code.Equals(input.KeyWord));
            var list = await query.ToListAsync();
            var listDtos = ObjectMapper.Map<List<DeptRoleDto>>(list);
            return listDtos;
        }


        /// <summary>
        /// 获取分页列表部门角色数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<DeptRoleDto>> GetAllPageListAsync(GetDeptRolePageInput input)
        {
            var query = _repository.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.KeyWord), q => q.Name.Contains(input.KeyWord) || q.Code.Equals(input.KeyWord))
                .WhereIf(!string.IsNullOrEmpty(input.Category), q => q.Category.Contains(input.Category))
                .WhereIf(input.IsActive.HasValue, q => q.IsActive == input.IsActive);
            if (input.Sorting != null)
                query = query.OrderBy(input.Sorting);
            var totalCount = query.Count();
            var list = await query.PageBy(input).ToListAsync();
            var listDtos = ObjectMapper.Map<List<DeptRoleDto>>(list);
            return new PagedResultDto<DeptRoleDto>(totalCount, listDtos);
        }

        /// <summary>
        /// 获取部门角色(依部门角色Id)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<DeptRoleDto> GetAsync(EntityDto<Guid> input)
        {
            var scheme = await _repository.GetAsync(input.Id);
            return ObjectMapper.Map<DeptRoleDto>(scheme);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 新增部门角色
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<DeptRoleDto> CreateAsync(CreateDeptRoleDto input)
        {
            var dto = ObjectMapper.Map<DeptRole>(input);

            await _repository.InsertAsync(dto);
            CurrentUnitOfWork.SaveChanges();
            return ObjectMapper.Map<DeptRoleDto>(dto);
        }
        /// <summary>
        /// 修改部门角色
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<DeptRoleDto> UpdateAsync(UpdateDeptRoleDto input)
        {
            var entity = await _repository.GetAsync(input.Id);
            ObjectMapper.Map(input, entity);
            await _repository.UpdateAsync(entity);
            return await GetAsync(new EntityDto<Guid>() { Id = entity.Id });
        }
        /// <summary>
        /// 删除部门角色
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[HttpDelete("workflow/scheme/{id}")]
        public async Task DeleteAsync(EntityDto<Guid> input)
        {
            await _repository.DeleteAsync(input.Id);

        }
        /// <summary>
        /// 修改部门角色禁用启用状态
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<DeptRoleDto> UpdateStateAsync(UpdateStateDeptRoleDto input)
        {
            var entity = await _repository.GetAsync(input.Id);
            entity.IsActive = input.IsActive;
            await _repository.UpdateAsync(entity);
            return await GetAsync(new EntityDto<Guid>() { Id = entity.Id });
        }

        #endregion
    }
}

