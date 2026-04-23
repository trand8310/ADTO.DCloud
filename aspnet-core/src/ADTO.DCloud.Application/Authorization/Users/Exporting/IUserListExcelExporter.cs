using System.Collections.Generic;
using ADTO.DCloud.Authorization.Users.Dto;
using ADTO.DCloud.Dto;

namespace ADTO.DCloud.Authorization.Users.Exporting
{
    public interface IUserListExcelExporter
    {
        FileDto ExportToFile(List<UserListDto> userListDtos, List<string> selectedColumns);
    }
}