using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataAuthorizes.Dto
{
    /// <summary>
    /// 登录者信息字段枚举（用于动态查询或权限控制）
    /// </summary>
    public enum EnumLoginUserField
    {
        /// <summary>
        /// 文本（自由输入）
        /// </summary>
        [Description("文本")]
        Text=1,

        /// <summary>
        /// 登录者ID
        /// </summary>
        [Description("登录者ID")]
        UserId=2,

        /// <summary>
        /// 登录者账号
        /// </summary>
        [Description("登录者账号")]
        UserName=3,

        /// <summary>
        /// 登录者公司
        /// </summary>
        [Description("登录者公司")]
        Company=4,
        /// <summary>
        /// 登录者公司及下属公司
        /// </summary>
        [Description("登录者公司及下属公司")]
        CompanyIn=41,
        /// <summary>
        /// 固定公司
        /// </summary>
        [Description("固定公司")]
        CompanyFixed = 42,
        /// <summary>
        /// 登录者部门
        /// </summary>
        [Description("登录者部门")]
        Department=5,
        /// <summary>
        /// 登录者部门及下属部门
        /// </summary>
        [Description("登录者部门及下属部门")]
        DepartmentIn=51,
        /// <summary>
        /// 固定部门
        /// </summary>
        [Description("固定部门")]
        DepartmentFixed = 52,
        /// <summary>
        /// 登录者岗位
        /// </summary>
        [Description("登录者岗位")]
        PostId=6,

        /// <summary>
        /// 登录者角色
        /// </summary>
        [Description("登录者角色")]
        Role=7,
    }
}
