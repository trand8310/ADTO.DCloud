using System;
using System.Collections.Generic;
using ADTOSharp.Runtime.Validation;

namespace ADTO.DCloud.Authorization.Users.Dto
{
    public class GetUsersToExcelInput: IShouldNormalize, IGetUsersInput
    {
        /// <summary>
        /// 用户列表
        /// </summary>
        public List<Guid> Ids { get; set; }
        public string Filter { get; set; }

        public List<string> Permissions { get; set; }
        public List<string> SelectedColumns { get; set; }

        public Guid? Role { get; set; }

        public bool OnlyLockedUsers { get; set; }

        public string Sorting { get; set; }
        public Guid? OrganizationUnitId { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Name";
            }
        }
    }
}
