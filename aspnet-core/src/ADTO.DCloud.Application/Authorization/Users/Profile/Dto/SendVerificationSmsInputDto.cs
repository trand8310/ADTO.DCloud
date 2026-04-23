namespace ADTO.DCloud.Authorization.Users.Profile.Dto
{
    /// <summary>
    /// 手机短信校验
    /// </summary>
    public class SendVerificationSmsInputDto
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        public string PhoneNumber { get; set; }
    }
}