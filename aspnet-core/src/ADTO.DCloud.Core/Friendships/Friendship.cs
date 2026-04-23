using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ADTOSharp;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using ADTOSharp.Timing;

namespace ADTO.DCloud.Friendships;

/// <summary>
/// 好友
/// </summary>
[Table("Friendships"),Description("好友")]
public class Friendship : Entity<Guid>, IHasCreationTime, IMayHaveTenant
{
    /// <summary>
    /// 用户Id
    /// </summary>
    [Description("用户Id")]
    public Guid UserId { get; set; }
    /// <summary>
    /// 租户Id
    /// </summary>
    [Description("租户Id")]
    public Guid? TenantId { get; set; }
    /// <summary>
    /// 好友Id
    /// </summary>
    [Description("好友Id")]
    public Guid FriendUserId { get; set; }

    /// <summary>
    /// 好友租户Id
    /// </summary>
    [Description("好友租户Id")]
    public Guid? FriendTenantId { get; set; }
    /// <summary>
    /// 好友用户名
    /// </summary>
    [Description("好友用户名")]
    [Required]
    [MaxLength(ADTOSharpUserBase.MaxUserNameLength)]
    public string FriendUserName { get; set; }
    /// <summary>
    /// 好友租户名
    /// </summary>
    [Description("好友租户名")]
    public string FriendTenancyName { get; set; }
    /// <summary>
    /// 好友用户图像
    /// </summary>
    [Description("好友用户图像")]
    public Guid? FriendProfilePictureId { get; set; }
    /// <summary>
    /// 好友状态
    /// </summary>
    [Description("好友状态")]
    public FriendshipState State { get; set; }
    /// <summary>
    /// 创建时间
    /// </summary>
    [Description("创建时间")]
    public DateTime CreationTime { get; set; }

    public Friendship(UserIdentifier user, UserIdentifier probableFriend, string probableFriendTenancyName, string probableFriendUserName, Guid? probableFriendProfilePictureId, FriendshipState state)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        if (probableFriend == null)
        {
            throw new ArgumentNullException(nameof(probableFriend));
        }

        if (!Enum.IsDefined(typeof(FriendshipState), state))
        {
            throw new Exception("Invalid FriendshipState value: " + state);
        }

        UserId = user.UserId;
        TenantId = user.TenantId;
        FriendUserId = probableFriend.UserId;
        FriendTenantId = probableFriend.TenantId;
        FriendTenancyName = probableFriendTenancyName;
        FriendUserName = probableFriendUserName;
        State = state;
        FriendProfilePictureId = probableFriendProfilePictureId;

        CreationTime = Clock.Now;
    }

    protected Friendship()
    {

    }
}
