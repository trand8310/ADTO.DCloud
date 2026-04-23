using System.Collections.Generic;
using ADTOSharp.Runtime.Validation;
using ADTO.DCloud.Dto;
using System;

namespace ADTO.DCloud.Authorization.Users.Dto
{
    public class GetUsersInput : PagedAndSortedInputDto, IShouldNormalize, IGetUsersInput
    {

        /// <summary>
        /// 组织架构Id
        /// </summary>
        public Guid? OrganizationUnitId { get; set; }
        public string Filter { get; set; }

        public List<string> Permissions { get; set; }
        /// <summary>
        /// 用户列表
        /// </summary>
        public List<Guid> Ids { get; set; }
        public Guid? Role { get; set; }

        public bool OnlyLockedUsers { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime desc";
            }

            Filter = Filter?.Trim();
        }
    }
}
