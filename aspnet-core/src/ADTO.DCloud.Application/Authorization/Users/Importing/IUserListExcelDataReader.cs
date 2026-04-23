using System.Collections.Generic;
using ADTO.DCloud.Authorization.Users.Importing.Dto;
using ADTOSharp.Dependency;

namespace ADTO.DCloud.Authorization.Users.Importing
{
    public interface IUserListExcelDataReader: ITransientDependency
    {
        List<ImportUserDto> GetUsersFromExcel(byte[] fileBytes);
    }
}
