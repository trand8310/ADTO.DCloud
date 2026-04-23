using System.Linq;
using ADTO.DCloud.Chat;
using ADTO.DCloud.Friendships.Cache;
using ADTOSharp;
using ADTOSharp.Dependency;
using ADTOSharp.RealTime;
using ADTOSharp.Threading;

namespace ADTO.DCloud.Friendships
{
    /// <summary>
    /// 负责观看聊天用户的在线/离线状态更改。当用户上线或离线时，该类抓取状态更改并通知相关用户的朋友
    /// </summary>
    public class ChatUserStateWatcher : ISingletonDependency
    {
        private readonly IChatCommunicator _chatCommunicator;
        private readonly IUserFriendsCache _userFriendsCache;
        private IOnlineClientManager _onlineClientManager;

        public ChatUserStateWatcher(
            IChatCommunicator chatCommunicator,
            IUserFriendsCache userFriendsCache,
            IOnlineClientManager onlineClientManager
            )
        {
            _chatCommunicator = chatCommunicator;
            _userFriendsCache = userFriendsCache;
            _onlineClientManager = onlineClientManager;
        }

        public void Initialize()
        {
           // _onlineClientManager = IocManager.Instance.Resolve<IOnlineClientManager<ChatChannel>>();

            // _onlineClientManager

            _onlineClientManager.UserConnected += OnlineClientManager_UserConnected;
            _onlineClientManager.UserDisconnected += OnlineClientManager_UserDisconnected;
        }

        private void OnlineClientManager_UserConnected(object sender, OnlineUserEventArgs e)
        {
            NotifyUserConnectionStateChange(e.User, true);
        }

        private void OnlineClientManager_UserDisconnected(object sender, OnlineUserEventArgs e)
        {
            NotifyUserConnectionStateChange(e.User, false);
        }

        private async void NotifyUserConnectionStateChange(UserIdentifier user, bool isConnected)
        {
            var cacheItem = _userFriendsCache.GetCacheItem(user);

            foreach (var friend in cacheItem.Friends)
            {
                var friendUserClients = await _onlineClientManager.GetAllByUserIdAsync(new UserIdentifier(friend.FriendTenantId, friend.FriendUserId));
                if (!friendUserClients.Any())
                {
                    continue;
                }

                AsyncHelper.RunSync(() => _chatCommunicator.SendUserConnectionChangeToClients(friendUserClients, user, isConnected));
            }
        }
    }
}