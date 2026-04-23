using System;
using ADTOSharp;

namespace ADTO.DCloud.Authorization.Users.Dto
{
    public class ImportUsersFromExcelJobArgs
    {
        public Guid? TenantId { get; set; }

        public Guid BinaryObjectId { get; set; }

        public UserIdentifier User { get; set; }
    }
}
