using System;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.Authorization.Users.Dto
{
    public class ResetPasswordDto
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        [Required]
        public string NewPassword { get; set; }
    }
}
