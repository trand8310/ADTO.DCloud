

using ADTOSharp;

namespace ADTO.DCloud.Friendships;
/// <summary>
/// 系统用户与好友之间的转换扩展
/// </summary>
public static class FriendshipExtensions
{
    public static UserIdentifier ToUserIdentifier(this Friendship friendship)
    {
        return new UserIdentifier(friendship.TenantId, friendship.UserId);
    }

    public static UserIdentifier ToFriendIdentifier(this Friendship friendship)
    {
        return new UserIdentifier(friendship.FriendTenantId, friendship.FriendUserId);
    }
}
