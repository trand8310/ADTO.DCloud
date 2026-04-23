namespace ADTO.DCloud.Authorization.Users.Profile.Dto
{
    /// <summary>
    /// 手机短信登录的校验KEY
    /// </summary>
    public class VerifySmsCodeInputDto
    {
        public string Code { get; set; }

        public string PhoneNumber { get; set; }
    }
}