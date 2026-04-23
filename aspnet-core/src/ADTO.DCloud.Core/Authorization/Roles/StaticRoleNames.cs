namespace ADTO.DCloud.Authorization.Roles;

public static class StaticRoleNames
{
    public static class Host
    {
        /// <summary>
        /// 管理员
        /// </summary>
        public const string Admin = "Admin";
    }

    public static class Tenants
    {
        /// <summary>
        /// 管理员
        /// </summary>
        public const string Admin = "Admin";
        /// <summary>
        /// 用户
        /// </summary>
        public const string User = "User";

        /// <summary>
        /// 注册用户
        /// </summary>
        public const string Registered = "Registered";

        /// <summary>
        /// 访客
        /// </summary>
        public const string Guest = "Guest";


    }
}
