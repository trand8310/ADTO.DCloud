using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.EmployeeManager.Dto
{
    /// <summary>
    /// 员工开户前端Dto
    /// </summary>
    [AutoMap(typeof(EmployeeInfo))]
    public class EmployeeBasicInformationDto : EntityDto<Guid>
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// 员工姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 员工工号
        /// </summary>
        [StringLength(50)]
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 性别
        /// </summary>	
        public string Gender { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        [MaxLength(120)]
        public string IDCard { get; set; }
        /// <summary>
        /// 生日
        /// </summary>	
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 日历
        /// </summary>
        public string Calendar { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 公司号码
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// QQ号
        /// </summary>	
        [MaxLength(120)]
        public string OICQ { get; set; }
        /// <summary>
        /// 微信号
        /// </summary>	
        [MaxLength(120)]
        public string WeChat { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 职务
        /// </summary>
        [MaxLength(120)]
        public string Position { get; set; }


        /// <summary>
        /// 用户家庭信息
        /// </summary>
        public EmployeeFamilieDto UserFamily { get; set; }
    }
}
