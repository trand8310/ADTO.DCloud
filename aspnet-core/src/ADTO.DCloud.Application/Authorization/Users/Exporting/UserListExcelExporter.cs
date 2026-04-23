using System.Collections.Generic;
using System.Linq;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Runtime.Session;
using ADTOSharp.Timing.Timezone;
using ADTO.DCloud.Authorization.Users.Dto;
using ADTO.DCloud.DataExporting.Excel.MiniExcel;
using ADTO.DCloud.Dto;
using ADTO.DCloud.Storage;
using System.Collections;
using System.Reflection;
using System;

namespace ADTO.DCloud.Authorization.Users.Exporting;

public class UserListExcelExporter : MiniExcelExcelExporterBase, IUserListExcelExporter
{
    private readonly IPropertyInfoHelper _propertyInfoHelper;

    public UserListExcelExporter(ITempFileCacheManager tempFileCacheManager, IPropertyInfoHelper propertyInfoHelper) : base(tempFileCacheManager)
    {
        _propertyInfoHelper = propertyInfoHelper;
    }

    public FileDto ExportToFile(List<UserListDto> userList, List<string> selectedColumns)
    {
        var items = new List<Dictionary<string, object>>();

        foreach (var item in userList)
        {
            if (selectedColumns is { Count: 0 })
            {
                break;
            }

            var rowItem = new Dictionary<string, object>();

            foreach (var selectedColumn in selectedColumns)
            {
                // if the property is found, it will be added to the list of items
                if (typeof(UserListDto).GetProperty(selectedColumn) is { } property)
                {
                    rowItem.Add(L(property.Name), _propertyInfoHelper.GetConvertedPropertyValue(property, item, HandleLists) ?? "");
                }
            }

            items.Add(rowItem);
        }

        return CreateExcelPackage("UserList.xlsx", items);
    }
#nullable enable
    private static string? HandleLists(PropertyInfo property, object item)
    {
        var propertyType = property.PropertyType;

        if (!typeof(IEnumerable).IsAssignableFrom(propertyType) &&
            !propertyType.IsGenericType &&
            propertyType.GetGenericTypeDefinition() != typeof(List<>))
        {
        }

        var genericType = propertyType.GetGenericArguments()[0];

        return genericType switch
        {
            { } when genericType == typeof(UserListRoleDto) => HandleUserListRoleDto(genericType, item),
            _ => null
        };
    }

    private static string? HandleUserListRoleDto(Type genericType, object item)
    {
        if (genericType != typeof(UserListRoleDto)) return null;

        var complexType = (UserListDto)item;
        return complexType.Roles.Select(r => r.RoleName).JoinAsString(", ");
    }
#nullable disable
}