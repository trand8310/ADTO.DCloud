using ADTO.DCloud.Authorization.Posts.Dto;
using ADTOSharp.Application.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADTO.DCloud.Authorization.Posts
{
    public interface IPostAppService : IApplicationService
    {
        /// <summary>
        /// 根据用户获取所有岗位信息
        /// </summary>
        /// <returns></returns>
        public Task<List<PostDto>> GetPostByUser(GetPostByUserInput input);
        /// <summary>
        /// 保存用户所属的岗位信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberedPosts"></param>
        /// <returns></returns>
        Task<bool> SaveUserPostAsync(Guid userId, List<Guid> memberedPosts);
    }
}
