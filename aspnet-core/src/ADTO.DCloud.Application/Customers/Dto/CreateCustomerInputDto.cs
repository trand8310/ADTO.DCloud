using System;
using ADTOSharp.AutoMapper;
using System.Collections.Generic;
using ADTOSharp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using ADTO.DCloud.Customers.CustomerContactManage.Dto;
using ADTO.DCloud.Customers.CustomerFollowManage.Dto;
using ADTO.DCloud.ProjectManage.Dto;

namespace ADTO.DCloud.Customers.Dto
{
    /// <summary>
    /// 新增客户（包含客户主体信息、联系人、产品）
    /// </summary>
    [AutoMapTo(typeof(Customer))]
    public class CreateCustomerInputDto : EntityDto<Guid?>
    {
        public CreateCustomerInputDto()
        {
            CustomerContacts = new List<CreateCustomerContactsDto>();
            CustomerProducts = new List<CustomerProductDto>();
        }
        #region 客户主体信息
        /// <summary>
        /// 客户编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        [Required(ErrorMessage = "客户名称不能为空")]
        public string Name { get; set; }

        /// <summary>
        /// 公司规模(字典value)
        /// </summary>
        [Required(ErrorMessage = "公司规模不能为空")]
        public string ScaleType { get; set; }

        /// <summary>
        /// 公司邮箱
        /// </summary>
        [Required(ErrorMessage = "公司邮箱不能为空")]
        public string Email { get; set; }

        /// <summary>
        /// 公司电话
        /// </summary>
        [Required(ErrorMessage = "公司电话不能为空")]
        public string Tel { get; set; }

        /// <summary>
        /// 客户来源
        /// </summary>
        [Required(ErrorMessage = "客户来源不能为空")]
        public string SourceType { get; set; }

        /// <summary>
        /// 公司类型 字典
        /// </summary>
        [Required(ErrorMessage = "公司类型不能为空")]
        public string CustomerType { get; set; }

        /// <summary>
        /// 客户状态 字典
        /// </summary>
        [Required(ErrorMessage = "客户状态不能为空")]
        public string CustomerState { get; set; }

        /// <summary>
        /// 成立时间
        /// </summary>
        [Required(ErrorMessage = "成立时间不能为空")]
        public DateTime RegistrationDate { get; set; }

        /// <summary>
        /// 主营行业 字典
        /// </summary>
        [Required(ErrorMessage = "主营行业不能为空")]
        public string IndustryType { get; set; }

        /// <summary>
        /// 客户评级 字典
        /// </summary>
        [Required(ErrorMessage = "客户评级不能为空")]
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
        [Required(ErrorMessage = "所在地区不能为空")]
        public int AreaId { get; set; }

        /// <summary>
        /// 所属国家
        /// </summary>
        [Required(ErrorMessage = "所属国家不能为空")]
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
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }

        #endregion

        /// <summary>
        /// 客户联系人
        /// </summary>
        public List<CreateCustomerContactsDto> CustomerContacts { get; set; }

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
    }
}
