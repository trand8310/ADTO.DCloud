using ADTO.DCloud.Authorization.UserRelations.Dto;
using ADTOSharp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ADTO.DCloud.Authorization.UserRelations
{
    /// <summary>
    /// 用户关系
    /// </summary>
    public class UserRelationAppService : DCloudAppServiceBase, IUserRelationAppService
    {
        private readonly IRepository<UserRelation, Guid> _repository;
        public UserRelationAppService(IRepository<UserRelation, Guid> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 获取对象主键列表信息
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="category">分类:1-角色2-岗位</param>
        /// <returns></returns>
        public async Task<IEnumerable<UserRelationDto>> GetObjectIdList(GetObjectIdListInput input)
        {
            var list = await _repository.GetAll().Where(t => t.UserId == input.UserId && t.Category == input.Category).ToListAsync();
            return ObjectMapper.Map<List<UserRelationDto>>(list);
        }
        /// <summary>
        /// 获取用户主键列表信息
        /// </summary>
        /// <param name="objectId">关联角色或岗位组件</param>
        /// <returns></returns>
        public async Task<IEnumerable<UserRelationDto>> GetUserIdList(string objectId)
        {
            var list = await _repository.GetAll().Where(t => t.ObjectId == objectId).ToListAsync();
            return ObjectMapper.Map<List<UserRelationDto>>(list);
        }
        /// <summary>
        /// 保存用户对应对象数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="category"></param>
        /// <param name="userRelationEntityList"></param>
        /// <returns></returns>
        public async Task<IActionResult> SaveEntityList(CreateUserRelationDto input)
        {
            await _repository.DeleteAsync(t => t.UserId == input.UserId && t.Category == input.Category);
            foreach (CreateUserRelationInput dto in input.userRelationLlist)
            {
                UserRelation entity = new UserRelation()
                {
                    AttrCode = dto.AttrCode,
                    ObjectId = dto.ObjectId,
                    Category = input.Category,
                    UserId = input.UserId,
                    Name = input.UserName,
                    Type = 1
                };
                await _repository.InsertAsync(entity);
            }
            return new JsonResult(new
            {
                Message = $"保存成功",
                Success = true
            });
        }
    }
}

