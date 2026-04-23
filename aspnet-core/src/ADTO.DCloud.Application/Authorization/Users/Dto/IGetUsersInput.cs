using System;
using System.Collections.Generic;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Runtime.Validation;

namespace ADTO.DCloud.Authorization.Users.Dto
{
    public interface IGetUsersInput : ISortedResultRequest
    {
        /// <summary>
        /// 用户列表
        /// </summary>
        List<Guid> Ids { get; set; }
        /// <summary>
        /// 组织架构Id
        /// </summary>
        Guid? OrganizationUnitId { get; set; }
        string Filter { get; set; }

        List<string> Permissions { get; set; }

        Guid? Role { get; set; }

        bool OnlyLockedUsers { get; set; }
    }
}
