using System;
using System.ComponentModel.DataAnnotations;
using ADTOSharp.Extensions;
using ADTOSharp.Runtime.Validation;

namespace ADTO.DCloud.Authorization.Users.Profile.Dto
{
    public class UpdateProfilePictureInput : ICustomValidate
    {
        /// <summary>
        /// 文件TOKEN
        /// </summary>
        [MaxLength(400)]
        public string FileToken { get; set; }
        /// <summary>
        /// 是否使用标准用户图像,如果启用,则上传文件将无效,会使用一个默认的图标做为用户图像.
        /// </summary>
        public bool UseGravatarProfilePicture { get; set; }
        
        public Guid? UserId { get; set; }
        
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (!UseGravatarProfilePicture && FileToken.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(FileToken));
            }
        }
    }
}