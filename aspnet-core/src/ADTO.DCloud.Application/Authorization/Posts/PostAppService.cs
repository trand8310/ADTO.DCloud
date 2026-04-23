using ADTO.DCloud.Authorization.Posts.Dto;
using ADTO.DCloud.Authorization.Users; 
using ADTO.DCloud.DataAuthorizes; 
using ADTO.DCloud.Infrastructure; 
using ADTOSharp.Application.Services.Dto; 
using ADTOSharp.Authorization; 
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Dependency; 
using ADTOSharp.Domain.Repositories; 
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Organizations; 
using ADTOSharp.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq; 
using System;
using System.Collections.Generic; 
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks; 

namespace ADTO.DCloud.Authorization.Posts;

/// <summary>
/// 岗位管理服务
/// </summary>
[ADTOSharpAuthorize]
public class PostAppService : DCloudAppServiceBase, IPostAppService
{
    private readonly IRepository<Post, Guid> _postRepository;
    private readonly IRepository<OrganizationUnit, Guid> _departmentRepository;
    private readonly IRepository<UserPost, Guid> _relationRepository;
    private readonly IRepository<User, Guid> _userRepository;
    private readonly DataAuthorizesAppService _dataAuthorizesAppService;

    public PostAppService(IRepository<Post, Guid> postRepository, IRepository<OrganizationUnit, Guid> departmentRepository,
        IRepository<UserPost, Guid> relationRepository,
            IRepository<User, Guid> userRepository,
            DataAuthorizesAppService dataAuthorizesAppService
       )
    {
        _postRepository = postRepository;
        _departmentRepository = departmentRepository;
        _relationRepository = relationRepository;
        _userRepository = userRepository;
        _dataAuthorizesAppService = dataAuthorizesAppService;
    }

    #region 查询
    /// <summary>
    /// 获取文件路径
    /// </summary>
    /// <returns></returns>
    private string GetRequestPath()
    {
        return IocManager.Instance.Resolve<IHttpContextAccessor>().HttpContext.Request.Path;
    }
    /// <summary>
    /// 获取所有岗位信息-不分页
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<List<PostDto>> GetAllAsync(GetPostsInput input)
    {
        var query = _postRepository.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.KeyWord), p => p.Name.Contains(input.KeyWord) || p.EnCode.Contains(input.KeyWord))
            .WhereIf(input.DepartmentId.HasValue && !input.DepartmentId.IsEmpty(), p => p.DepartmentId.Equals(input.DepartmentId))
             .WhereIf(input.CompanyId.HasValue && !input.CompanyId.IsEmpty(), p => p.CompanyId.Equals(input.CompanyId));
        query = await _dataAuthorizesAppService.CreateDataFilteredQuery<Post>(query, GetRequestPath());
        return ObjectMapper.Map<List<PostDto>>(query);
    }
    /// <summary>
    /// 岗位列表树状结构-返回了部门名称及公司名称
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<List<PostTreeDto>> GetAllTreeAsync(GetPostsInput input)
    {
        var query = from post in _postRepository.GetAll()
                    join depart in _departmentRepository.GetAll()
                        on post.DepartmentId equals depart.Id into departGroup
                    from depart in departGroup.DefaultIfEmpty()
                    join company in _departmentRepository.GetAll()
                        on post.CompanyId equals company.Id into companyGroup
                    from company in companyGroup.DefaultIfEmpty()
                    select new PostTreeDto()
                    {
                        Id = post.Id,
                        ParentId = post.ParentId,
                        Name = post.Name,
                        EnCode = post.EnCode,
                        DisplayOrder = post.DisplayOrder,
                        CompanyId = company == null ? Guid.Empty : company.Id,      // 注意处理null
                        CompanyName = company == null ? "" : company.DisplayName,
                        DepartmentId = post.DepartmentId,
                        DepartmentName = depart == null ? "" : depart.DisplayName
                    };
        query = query.WhereIf(input.CompanyId.HasValue && input.CompanyId != Guid.Empty, q => q.CompanyId.Equals(input.CompanyId))
             .WhereIf(input.DepartmentId.HasValue, q => q.DepartmentId.Equals(input.DepartmentId))
             .WhereIf(!string.IsNullOrWhiteSpace(input.KeyWord), q => q.Name.Contains(input.KeyWord) || q.EnCode.Contains(input.KeyWord))
            ;
        var list = await _dataAuthorizesAppService.CreateDataFilteredQuery<PostTreeDto>(query, GetRequestPath());
        return GetElmentTreeDtos(list.ToList(), null);
    }
    private List<PostTreeDto> GetElmentTreeDtos(List<PostTreeDto> list, Guid? id)
    {
        var query = from c in list
                    where id.HasValue && id.Value != Guid.Empty ? c.ParentId == id : c.ParentId == null
                    select c;
        return query.Select(item =>
        {
            item.Children = GetElmentTreeDtos(list, item.Id).ToList();
            return item;
        }).ToList();
    }

    /// <summary>
    /// 获取岗位列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<List<JObject>> GetTree(GetPostsInput input)
    {
        var query = _postRepository.GetAll().Join(_departmentRepository.GetAll(), post => post.DepartmentId, depart => depart.Id, (post, depart) => new { post, depart })
            .WhereIf(input.CompanyId.HasValue, q => q.post.CompanyId.Equals(input.CompanyId))
            .WhereIf(input.DepartmentId.HasValue, q => q.post.DepartmentId.Equals(input.DepartmentId))
            .WhereIf(!string.IsNullOrWhiteSpace(input.KeyWord), q => q.post.Name.Contains(input.KeyWord) || q.post.EnCode.Contains(input.KeyWord))
           ;
        var filteredQuery = await _dataAuthorizesAppService.CreateDataFilteredQuery(query, GetRequestPath());
        var list = filteredQuery.ToList().OrderBy(o => o.post.DisplayOrder).Select(item =>
        {
            PostDto dto = ObjectMapper.Map<PostDto>(item.post);
            dto.DepartmentName = item.depart.DisplayName;
            return dto;
        }).ToList();

        return GetElmentTreeList(list, Guid.Empty);
    }
    private List<JObject> GetElmentTreeList(List<PostDto> list, Guid? id)
    {
        var query = from c in list
                    where id.HasValue && id.Value != Guid.Empty ? c.ParentId == id : c.ParentId == null
                    select c;
        return query.Select(s =>
        {
            var m = new JObject();
            m["id"] = s.Id;
            m["text"] = s.Name;
            m["value"] = s.Id;
            m["parentId"] = s.ParentId;
            m["ChildNodes"] = JArray.FromObject(GetElmentTreeList(list, s.Id).ToList());
            return m;
        }).ToList();
    }
    /// <summary>
    /// 获取岗位(依Id)
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<PostDto> GetPostAsync(EntityDto<Guid> input)
    {
        var post = await _postRepository.GetAsync(input.Id);
        var depart = await _departmentRepository.FirstOrDefaultAsync(post.DepartmentId);
        var postDto = ObjectMapper.Map<PostDto>(post);
        postDto.DepartmentName = depart == null ? "" : depart.DisplayName;
        return postDto;
    }
    /// <summary>
    /// 获取岗位(依Id)-编辑的时候获取的数据，过滤了不用维护的字段
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<UpdatePostEditDto> GetPostEditAsync(EntityDto<Guid> input)
    {
        var post = await _postRepository.GetAsync(input.Id);
        var depart = await _departmentRepository.GetAsync(post.DepartmentId);
        var postDto = ObjectMapper.Map<UpdatePostEditDto>(post);
        postDto.DepartmentName = depart == null ? "" : depart.DisplayName;
        return postDto;
    }

    /// <summary>
    /// 获取岗位成员
    /// </summary>
    /// <returns></returns>
    public async Task<List<UserPostDto>> GetUserIdList(GetUserIdListInput input)
    {
        var query = await _relationRepository.GetAll()
            .WhereIf(input.PostId != Guid.Empty, q => q.PostId.Equals(input.PostId))
           .ToDynamicListAsync();
        return ObjectMapper.Map<List<UserPostDto>>(query);
    }
    /// <summary>
    /// 获取岗位成员,关联岗位组件-根据多个用户Id
    /// </summary>
    /// <returns></returns>
    public async Task<List<UserPostDto>> GetUserByPostId(GetByUserIdListByUserIdsInput input)
    {
        var query = await _relationRepository.GetAll()
            .Where(q => input.ObjectIdList.Contains(q.PostId))
           .ToDynamicListAsync();
        return ObjectMapper.Map<List<UserPostDto>>(query);
    }
    /// <summary>
    /// 获取岗位数据列表(根据主键串)organization/posts
    /// </summary>
    /// <param name="postIds">根据主键串</param>
    /// <returns></returns>
    //[HttpPost]
    public async Task<IEnumerable<PostDto>> GetListByPostIds(GetListByPostIdsInput input)
    {
        Guid[] postIdList = input.Ids
          .Split(new[] { '，', ',' }, StringSplitOptions.RemoveEmptyEntries)
          .Select(p => Guid.TryParse(p.Trim(), out Guid guid) ? guid : Guid.Empty)
          .ToArray();

        var query = from post in _postRepository.GetAll()
                    join c in _departmentRepository.GetAll() on post.CompanyId equals c.Id into c
                    from company in c.DefaultIfEmpty()
                    join d in _departmentRepository.GetAll() on post.DepartmentId equals d.Id into d
                    from department in d.DefaultIfEmpty()
                    where postIdList.Contains(post.Id)
                    select new { post, company, department };
        var list = await query.ToListAsync();
        var res = list.Select(item =>
         {
             PostDto dto = ObjectMapper.Map<PostDto>(item.post);
             dto.DepartmentName = item.department.DisplayName;
             dto.CompanyName = item.company.DisplayName;
             return dto;
         }).ToList();
        return res;
    }
    /// <summary>
    /// 根据用户获取所有岗位信息
    /// </summary>
    /// <returns></returns>
    public async Task<List<PostDto>> GetPostByUser(GetPostByUserInput input)
    {
        var query = await _relationRepository.GetAll()
            .Join(_postRepository.GetAll(), user => user.PostId, post => post.Id, (user, post) => new { user, post })
             .Where(temp => temp.post.IsDeleted == false)
            .Join(_departmentRepository.GetAll(), temp => temp.post.CompanyId, company => company.Id, (temp, company) => new
            {
                user = temp.user,
                post = temp.post,
                company = company
            })
            .ToListAsync();
        var list = query.Where(q => q.user.UserId.Equals(input.UserId))
          ;
        var result = list.Select(item =>
        {
            var dto = ObjectMapper.Map<PostDto>(item.post);
            dto.CompanyName = item.company.DisplayName;
            return dto;
        }).ToList();

        return result;
    }
    #endregion

    #region 提交
    /// <summary>
    /// 新增岗位
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<PostDto> CreatePostAsync(CreatePostInput input)
    {
        if (input.CompanyId == Guid.Empty)
            throw new UserFriendlyException("所属岗位公司为空，请选择岗位公司！");
        if (input.DepartmentId == Guid.Empty)
            throw new UserFriendlyException("所属岗位部门为空，请选择岗位部门！");
        var post = ObjectMapper.Map<Post>(input);
        if (input.ParentId.HasValue)
        {
            var parent = await _postRepository.GetAsync(input.ParentId.Value);
            if (parent != null)
                post.Parent = parent;
        }
        await _postRepository.InsertAsync(post);
        CurrentUnitOfWork.SaveChanges();
        return ObjectMapper.Map<PostDto>(post);
    }
    /// <summary>
    /// 修改岗位
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<PostDto> UpdatePostAsync(UpdatePostInput input)
    {
        if (input.CompanyId == Guid.Empty)
            throw new UserFriendlyException("所属岗位公司为空，请选择岗位公司！");
        if (input.DepartmentId == Guid.Empty)
            throw new UserFriendlyException("所属岗位部门为空，请选择岗位部门！");
        var module = await _postRepository.GetAsync(input.Id);
        ObjectMapper.Map(input, module);
        if (input.ParentId.HasValue)
        {
            var parent = await _postRepository.GetAsync(input.ParentId.Value);
            if (parent != null)
                module.Parent = parent;
        }
        await _postRepository.UpdateAsync(module);
        return await GetPostAsync(input);
    }

    /// <summary>
    /// 删除岗位数据
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task DeletePostAsync(EntityDto<Guid> input)
    {
        await _postRepository.DeleteAsync(input.Id);
    }

    /// <summary>
    /// 保存岗位对应用户数据
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<IActionResult> SavePostForUserAsync(UserPostInputDto input)
    {
        //获取岗位现有的用户信息
        var userPosts = await _relationRepository.GetAll().Where(w => w.PostId == input.PostId).ToListAsync();
        //删除不再需要的岗位信息
        await _relationRepository.DeleteAsync(d => d.PostId.Equals(input.PostId) && !input.UserIds.Any(a => a.Equals(d.UserId)));
        //新增需要插入的岗位信息
        foreach (var userId in input.UserIds.Where(w => !userPosts.Any(a => a.UserId.Equals(w))))
        {
            await _relationRepository.InsertAsync(new UserPost() { UserId = userId, PostId = input.PostId });
        }
        return new JsonResult(new
        {
            Message = "保存成功！",
            Success = true
        });
    }

    /// <summary>
    /// 保存用户所属的岗位信息
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="memberedPosts"></param>
    /// <returns></returns>
    public async Task<bool> SaveUserPostAsync(Guid userId, List<Guid> memberedPosts)
    {
        //获取用户现有的岗位信息

        var userPosts = ObjectMapper.Map<List<UserPostDto>>(await _relationRepository.GetAll().Where(w => w.UserId == userId).ToListAsync());
        //删除不再需要的岗位信息
        await _relationRepository.DeleteAsync(d => d.UserId.Equals(userId) && !memberedPosts.Any(a => a.Equals(d.PostId)));
        //新增需要插入的岗位信息
        foreach (var postId in memberedPosts.Where(w => !userPosts.Any(a => a.PostId.Equals(w))))
        {
            await _relationRepository.InsertAsync(new UserPost() { UserId = userId, PostId = postId });
        }
        return true;
    }
    /// <summary>
    /// 修改用户岗位排序
    /// </summary>
    /// <returns></returns>

    public async Task<bool> UpdateUserPostDisplayOrder(UpdateUserPostDisplayOrderDto input)
    {
        var module = await _relationRepository.GetAll().Where(q => q.UserId.Equals(input.UserId) && q.PostId.Equals(input.PostId)).FirstOrDefaultAsync();
        ObjectMapper.Map(input, module);
        await _relationRepository.UpdateAsync(module);
        return true;
    }
    #endregion

    #region 扩展方法
    /// <summary>
    /// 判断是否是有关联
    /// </summary>
    /// <param name="beginId"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    private async Task<bool> HasRelation(Guid beginId, Dictionary<Guid, int> list)
    {
        bool res = false;
        var entity = await _postRepository.FirstOrDefaultAsync(p => p.Id == beginId);
        if (entity == null || entity.Parent != null)
        {
            res = false;
        }
        else if (list.ContainsKey(entity.Parent.Id))
        {
            res = true;
        }
        else
        {
            res = await HasRelation(entity.Parent.Id, list);
        }
        return res;
    }

    /// <summary>
    /// 判断是否是上级
    /// </summary>
    /// <param name="myId">自己的岗位</param>
    /// <param name="otherIds">对方的岗位</param>
    /// <returns></returns>
    public async Task<bool> IsUp(Guid myId, List<Guid> otherIds)
    {
        bool res = false;
        if (Guid.Empty == myId)
        {
            Dictionary<Guid, int> map = new Dictionary<Guid, int>();
            foreach (var otherItem in otherIds)
            {
                if (!map.ContainsKey(otherItem))
                {
                    map.Add(otherItem, 1);
                }
            }
            if (await HasRelation(myId, map))
            {
                res = true;
            }
        }
        return res;
    }
    /// <summary>
    /// 判断是否是下级
    /// </summary>
    /// <param name="myId">自己的岗位</param>
    /// <param name="otherIds">对方的岗位</param>
    /// <returns></returns>
    public async Task<bool> IsDown(Guid myId, List<Guid> otherIds)
    {
        bool res = false;
        if (Guid.Empty == myId)
        {
            Dictionary<Guid, int> map = new Dictionary<Guid, int>();
            map.Add(myId, 1);
            foreach (var otherItem in otherIds)
            {
                if (await HasRelation(otherItem, map))
                {
                    res = true;
                    break;
                }
            }
        }
        return res;
    }
    /// <summary>
    /// 获取上级岗位人员ID
    /// </summary>
    /// <param name="postId">岗位</param>
    /// <param name="level">级数</param>
    /// <returns></returns>
    public async Task<IEnumerable<Guid>> GetUpIdList(Guid postId, int level)
    {
        List<Guid> res = new List<Guid>();
        if (postId != Guid.Empty && level > 0 && level < 6)
        {
            // 现在支持1-5级查找
            bool isHave = false; // 判断是否指定级数的职位
            var entity = await _postRepository.GetAllIncluding(t => t.Parent).Where(t => t.Id == postId).FirstOrDefaultAsync();
            if (entity != null)
            {
                Guid? parentId = entity.ParentId;
                for (int i = 0; i < level; i++)
                {
                    if (entity.Parent != null)
                    {
                        parentId = entity.Parent.Id;
                        if (i == (level - 1))
                        {
                            isHave = true;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                if (isHave)
                {
                    if (entity.Parent != null)
                    {
                        res.Add(entity.Parent.Id);
                    }
                }
            }
        }
        return res;
    }
    /// <summary>
    /// 获取下级岗位人员ID
    /// </summary>
    /// <param name="postId">岗位</param>
    /// <param name="level">级数</param>
    /// <returns></returns>
    public async Task<IEnumerable<Guid>> GetDownIdList(Guid postId, int level)
    {
        List<Guid> res = new List<Guid>();
        if (Guid.Empty == postId && level > 0 && level < 6)
        {// 现在支持1-5级查找
            bool isHave = false; // 判断是否指定级数的职位
            List<Guid> parentList = new List<Guid>();
            parentList.Add(postId);
            for (int i = 0; i < level; i++)
            {
                var list = _postRepository.GetAll().Where(q => parentList.Contains(q.Id));
                parentList = (List<Guid>)list.Select(q => q.Id);
                if (parentList.Count > 0)
                {
                    if (i == (level - 1))
                    {
                        isHave = true;
                    }
                }
                else
                {
                    break;
                }
            }
            if (isHave)
            {
                res.AddRange(parentList);
            }
        }
        return res;
    }
    #endregion


}

