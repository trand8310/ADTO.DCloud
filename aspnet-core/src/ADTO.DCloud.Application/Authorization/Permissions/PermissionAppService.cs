using ADTO.DCloud.Authorization.Permissions.Dto;
using ADTO.DCloud.Authorization.Roles.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using Humanizer;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace ADTO.DCloud.Authorization.Permissions;

/// <summary>
/// 权限管理服务
/// </summary>
public class PermissionAppService : DCloudAppServiceBase, IPermissionAppService
{

    private List<JObject> InternalPermissionTree(IReadOnlyList<Permission> permissions, List<string> excludes, int level, Permission parent)
    {
        var result = new List<JObject>();
        var list = parent == null ? permissions.Where(w => w.Parent == null) : permissions.Where(w => w.Parent != null && w.Parent.Name == parent.Name);
        foreach (var item in list)
        {
            var model = ObjectMapper.Map<PermissionDto>(item);
            var json = new JObject();
            json["id"] = $"{model.Name.ToLower()}_{level}";
            json["name"] = model.Name;
            json["displayName"] = model.DisplayName;
            json["description"] = model.Description;
            json["isGrantedByDefault"] = false;
            if (item.Children != null && item.Children.Count > 0)
            {
                var children = InternalPermissionTree(permissions, excludes, level + 1, item);
                //if (!excludes.Contains(model.Name))
                //{
                //    children.Insert(0, JObject.FromObject(new { id = model.Id, name = model.Name, displayName = model.DisplayName, descriptiion = model.Description, isGrantedByDefault = false }));
                //}
                json["children"] = JArray.FromObject(children);
            }
            result.Add(json);
        }
        return result;
    }

    /// <summary>
    /// 获取权限列表,树状数据结构
    /// </summary>
    /// <returns></returns>
    public List<JObject> GetPermissionsTreeList()
    {
        var excludes = new List<string>() { "Pages", "Pages.Administration" };
        var permissions = PermissionManager.GetAllPermissions();
        var level = 0;
        var result = InternalPermissionTree(permissions, excludes, level, null);
        return result;
    }

    /// <summary>
    /// 获取权限列表,层级结构
    /// </summary>
    /// <returns></returns>
    public ListResultDto<FlatPermissionWithLevelDto> GetAllPermissions()
    {
        var permissions = PermissionManager.GetAllPermissions();
        var rootPermissions = permissions.Where(p => p.Parent == null);
        var result = new List<FlatPermissionWithLevelDto>();
        foreach (var rootPermission in rootPermissions)
        {
            var level = 0;
            AddPermission(rootPermission, permissions, result, level);
        }
        return new ListResultDto<FlatPermissionWithLevelDto>
        {
            Items = result
        };
    }
    /// <summary>
    /// 添加权限
    /// </summary>
    /// <param name="permission"></param>
    /// <param name="allPermissions"></param>
    /// <param name="result"></param>
    /// <param name="level"></param>
    private void AddPermission(Permission permission, IReadOnlyList<Permission> allPermissions, List<FlatPermissionWithLevelDto> result, int level)
    {
        var flatPermission = ObjectMapper.Map<FlatPermissionWithLevelDto>(permission);
        flatPermission.Level = level;
        result.Add(flatPermission);

        if (permission.Children == null)
        {
            return;
        }

        var children = allPermissions.Where(p => p.Parent != null && p.Parent.Name == permission.Name).ToList();

        foreach (var childPermission in children)
        {
            AddPermission(childPermission, allPermissions, result, level + 1);
        }
    }
}
