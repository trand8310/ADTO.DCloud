
using ADTOSharp.AutoMapper;
using System;

namespace ADTO.DCloud.Storage.Dto
{
    /// <summary>
    /// 文件共享权限
    /// </summary>
    [AutoMap(typeof(SharedFileAuthorizes))]
    public class SharedFileAuthorizeDto
    {
        /// <summary>
        /// 共享类别Id
        /// </summary>
        public Guid SharedFileCategory { get; set; }

        /// <summary>
        /// 共享权限对象Id
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// 共享权限对象名称
        /// </summary>
        public string ObjectName { get; set; }

        /// <summary>
        /// 共享权限类型（公司，部门，用户），暂时默认部门
        /// </summary>
        public string ObjectType { get; set; }
    }
}
