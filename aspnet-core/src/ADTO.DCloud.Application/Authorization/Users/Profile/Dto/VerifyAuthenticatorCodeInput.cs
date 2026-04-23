namespace ADTO.DCloud.Authorization.Users.Profile.Dto
{
    /// <summary>
    /// ¹È¸èµÇÂ¼KEYµÄÐ£Ñé
    /// </summary>
    public class VerifyAuthenticatorCodeInput
    {
        public string Code { get; set; }
        public string GoogleAuthenticatorKey { get; set; }
    }
}