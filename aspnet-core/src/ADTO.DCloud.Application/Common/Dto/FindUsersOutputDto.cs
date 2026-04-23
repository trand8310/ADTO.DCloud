using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Organizations.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System;


namespace ADTO.DCloud.Common.Dto;

[AutoMapFrom(typeof(User))]
public class FindUsersOutputDto : EntityDto<Guid>, IPassivable, IHasCreationTime
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; }
    /// <summary>
    /// 姓名
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 用户图像
    /// </summary>
    public string UserProfilePicture { get; set; }
    /// <summary>
    /// 手机号码
    /// </summary>
    public string PhoneNumber { get; set; }
    /// <summary>
    /// 邮件件地址
    /// </summary>
    public string EmailAddress { get; set; }

    public OrganizationUnitSampleDto Department { get; set; }
    public OrganizationUnitSampleDto Company { get; set; }
    /// <summary>
    /// 人员状态
    /// </summary>
    public bool IsActive { get; set; }
    /// <summary>
    /// 创建事件
    /// </summary>
    public DateTime CreationTime { get; set; }
    /// <summary>
    /// 公司Id
    /// </summary>
    public Guid? CompanyId { get; set; }
    /// <summary>
    /// 部门Id
    /// </summary>
    public Guid? DepartmentId { get; set; }
}