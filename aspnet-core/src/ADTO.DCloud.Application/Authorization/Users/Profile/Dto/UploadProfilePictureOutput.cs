using ADTOSharp.Web.Models;

namespace ADTO.DCloud.Authorization.Users.Profile.Dto
{
    /// <summary>
    /// 用户图像上传返回
    /// </summary>
    public class UploadProfilePictureOutput : ErrorInfo
    {
        public UploadProfilePictureOutput()
        {
            
        }

        public UploadProfilePictureOutput(ErrorInfo error)
        {
            Code = error.Code;
            Details = error.Details;
            Message = error.Message;
            ValidationErrors = error.ValidationErrors;
        }
    }
}