using System.Collections.Generic;
using ADTO.DCloud.Authorization.Users.Importing.Dto;
using ADTO.DCloud.Dto;

namespace ADTO.DCloud.Authorization.Users.Importing
{
    public interface IInvalidUserExporter
    {
        FileDto ExportToFile(List<ImportUserDto> userListDtos);
    }
}
