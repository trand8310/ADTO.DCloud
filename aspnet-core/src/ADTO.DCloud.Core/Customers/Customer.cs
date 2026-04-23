using System;
using System.ComponentModel;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.Customers;
/// <summary>
///  客户信息
/// </summary>
[Description("客户信息"), Table("Customers")]
public class Customer : FullAuditedAggregateRoot<Guid>, IPassivable, IMayHaveTenant, IRemark, IDisplayOrder
{
    /// <summary>
    /// 客户编码
    /// </summary>
    [Description("客户编码")]
    public string Code { get; set; }

    /// <summary>
    /// 客户名称
    /// </summary>
    [Description("客户名称")]
    public string Name { get; set; }

    /// <summary>
    /// 公司规模(字典value)
    /// </summary>
    [Description("公司规模")]
    public string ScaleType { get; set; }

    /// <summary>
    /// 公司邮箱
    /// </summary>
    [Description("公司邮箱")]
    public string Email { get; set; }

    /// <summary>
    /// 客户电话
    /// </summary>
    [Description("客户电话")]
    public string Tel { get; set; }

    /// <summary>
    /// 客户来源
    /// </summary>
    [Description("客户来源")]
    public string SourceType { get; set; }

    /// <summary>
    /// 公司类型 字典
    /// </summary>
    [Description("公司类型")]
    public string CustomerType { get; set; }

    /// <summary>
    /// 客户状态 字典
    /// </summary>
    [Description("客户状态")]
    public string CustomerState { get; set; }

    /// <summary>
    /// 成立时间
    /// </summary>
    [Description("成立时间")]
    public DateTime RegistrationDate { get; set; }

    /// <summary>
    /// 主营行业 字典
    /// </summary>
    [Description("主营行业")]
    public string IndustryType { get; set; }

    /// <summary>
    /// 客户评级 字典
    /// </summary>
    [Description("客户评级")]
    public string CustomerLevel { get; set; }

    /// <summary>
    /// 年营业额
    /// </summary>
    [Description("年营业额")]
    public decimal AnnualAales { get; set; }

    /// <summary>
    /// 公司网址
    /// </summary>
    [Description("公司网址")]
    public string WebSite { get; set; }

    /// <summary>
    /// Whatapp
    /// </summary>
    [Description("Whatapp")]
    public string Whatapp { get; set; }

    /// <summary>
    /// 所在地区
    /// </summary>
    [Description("所在地区")]
    public int AreaId { get; set; }

    /// <summary>
    /// 所属国家
    /// </summary>
    [Description("所属国家")]
    public int CountryId { get; set; }

    /// <summary>
    /// 所属省份
    /// </summary>
    [Description("所属省份")]
    public int ProvinceId { get; set; }

    /// <summary>
    /// 所属市州
    /// </summary>
    [Description("所属市州")]
    public int CityId { get; set; }

    /// <summary>
    /// 所属区县
    /// </summary>
    [Description("所属区县")]
    public int CountyId { get; set; }

    /// <summary>
    /// 详细地址
    /// </summary>
    [Description("详细地址")]
    public string Address { get; set; }

    /// <summary>
    /// 地区国家省市区冗余字段
    /// </summary>
    [Description("详细地址")]
    public string LocationStr { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    [Description("是否激活")]
    public bool IsActive { get; set; }

    /// <summary>
    /// 租户Id
    /// </summary>
    [Description("租户Id")]
    public virtual Guid? TenantId { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [Description("备注")]
    public string Remark { get ; set ; }

    /// <summary>
    /// 审核状态
    /// </summary>
    [Description("审核状态")]
    public bool AuditStatus { get; set; }

    /// <summary>
    /// 审核时间
    /// </summary>
    [Description("审核时间")]
    public DateTime? AuditTime { get; set; }

    /// <summary>
    /// 审核人
    /// </summary>
    [Description("审核人")]
    public Guid? AuditUserId { get; set; }

    /// <summary>
    /// 所属、归属人
    /// </summary>
    [Description("所属人")]
    public Guid ResponsibilityUserId { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Description("排序")]
    public int DisplayOrder { get; set ; }

    /// <summary>
    /// 附件夹Id
    /// </summary>
    public Guid? FolderId { get; set; }

    //public  string Title { get; set; }
    /// <summary>
    ///  动态属性,这个字段的值做为一个整体以JSON格式存储在一个字段中
    /// </summary>
    //public ExtraPropertyDictionary ExtraProperties { get; set; }
}
