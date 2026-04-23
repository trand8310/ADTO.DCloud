using ADTOSharp.Organizations;
using AutoMapper;
using System;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.Organizations.Dto
{
    public class UpdateOrganizationUnitInput
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(OrganizationUnit.MaxDisplayNameLength)]
        public string DisplayName { get; set; }
        /// <summary>
        /// 上级组织
        /// </summary>
        public Guid? ParentId { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool IsActive { get; set; }
        //
        // 摘要:
        //     公司/部门负责人Id
        public virtual Guid? ManagerUserId { get; set; }
    }
}