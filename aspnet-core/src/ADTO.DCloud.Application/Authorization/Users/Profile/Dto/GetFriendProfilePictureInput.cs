using System;
using ADTOSharp;

namespace ADTO.DCloud.Authorization.Users.Profile.Dto
{
    /// <summary>
    /// 好友图像查询
    /// </summary>
    public class GetFriendProfilePictureInput
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        public Guid? TenantId { get; set; }

        public UserIdentifier ToUserIdentifier()
        {
            return new UserIdentifier(TenantId, UserId);
        }
    }
}
