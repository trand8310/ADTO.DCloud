namespace ADTO.DCloud.Authorization.Users.Profile.Dto
{
    /// <summary>
    /// 用户图像
    /// </summary>
    public class GetProfilePictureOutput
    {
        public string ProfilePicture { get; set; }

        public GetProfilePictureOutput(string profilePicture)
        {
            ProfilePicture = profilePicture;
        }
    }
}
