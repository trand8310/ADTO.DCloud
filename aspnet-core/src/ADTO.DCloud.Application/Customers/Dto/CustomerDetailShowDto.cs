using System;
using System.Collections.Generic;
using System.ComponentModel;
using ADTO.DCloud.Customers.CustomerContactManage.Dto;
using ADTO.DCloud.ProjectManage.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;

namespace ADTO.DCloud.Customers.Dto
{
    /// <summary>
    /// 客户展示详情
    /// </summary>
    [AutoMap(typeof(Customer))]
    public class CustomerDetailShowDto : EntityDto<Guid>
    {
        public CustomerDetailShowDto() { UploadFilesDtos = new List<CustomerUploadFilesDto> { }; }

        #region 客户主体信息

        /// <summary>
        /// 客户编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 公司规模(字典value)
        /// </summary>
        public string ScaleType { get; set; }

        /// <summary>
        /// 公司邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 客户电话
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 客户来源
        /// </summary>
        public string SourceType { get; set; }

        /// <summary>
        /// 公司类型 字典
        /// </summary>
        public string CustomerType { get; set; }

        /// <summary>
        /// 客户状态 字典
        /// </summary>
        public string CustomerState { get; set; }

        /// <summary>
        /// 成立时间
        /// </summary>
        public DateTime RegistrationDate { get; set; }

        /// <summary>
        /// 主营行业 字典
        /// </summary>
            public string IndustryType { get; set; }

        /// <summary>
        /// 客户评级 字典
        /// </summary>
        public string CustomerLevel { get; set; }

        /// <summary>
        /// 年营业额
        /// </summary>
        public decimal AnnualAales { get; set; }

        /// <summary>
        /// 公司网址
        /// </summary>
        public string WebSite { get; set; }

        /// <summary>
        /// Whatapp
        /// </summary>
        public string Whatapp { get; set; }

        /// <summary>
        /// 所在地区
        /// </summary>
        public Guid AreaId { get; set; }

        /// <summary>
        /// 所属国家
        /// </summary>
        public Guid CountryId { get; set; }

        /// <summary>
        /// 所属省份
        /// </summary>
        public Guid ProvinceId { get; set; }

        /// <summary>
        /// 所属市州
        /// </summary>
        public Guid CityId { get; set; }

        /// <summary>
        /// 所属区县
        /// </summary>
        public Guid CountyId { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 地区国家省市区冗余字段
        /// </summary>
        public string LocationStr { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        public virtual Guid? TenantId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public bool AuditStatus { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime AuditTime { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public Guid AuditUserId { get; set; }

        /// <summary>
        /// 所属人
        /// </summary>
        public Guid ResponsibilityUserId { get; set; }

        /// <summary>
        /// 所属人
        /// </summary>
        public string ResponsibilityUserName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public Guid CreatorUserId { get; set; }

        /// <summary>
        /// 创建人名称
        /// </summary>
        public string CreatorUserName { get; set; }

        /// <summary>
        /// 最后跟进时间
        /// </summary>
        public DateTime? LastFollowTime { get; set; }
        #endregion

        /// <summary>
        /// 客户联系人
        /// </summary>
        public List<CustomerContactsDto> CustomerContacts { get; set; }

        /// <summary>
        /// 客户相关产品
        /// </summary>
        public List<CustomerProductDto> CustomerProducts { get; set; }
        /// <summary>
        /// 附件夹Id（手机端中有客户新增有附件）
        /// </summary>
        public Guid? FolderId { get; set; }
        /// <summary>
        /// 图片集合
        /// </summary>
        public List<CustomerUploadFilesDto> UploadFilesDtos { get; set; }

        /// <summary>
        /// 区域国家省市区  最后一级的中文名
        /// 有些国家没有多级，用于小程序端反显
        /// </summary>
        public string LastLocationName { get; set; }
    }
}
