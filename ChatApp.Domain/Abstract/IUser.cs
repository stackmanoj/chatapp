using ChatApp.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.Abstract
{
    public interface IUser
    {
        Tuple<User, string> SaveUser(User objentity);
        User CheckLogin(string userName, string password);
        void SaveUserOnlineStatus(OnlineUser objentity);
        string GetUserConnectionID(int UserID);
        List<string> GetUserConnectionID(int[] userIDs);
        List<User> GetAllUsers();
        List<OnlineUserDetails> GetOnlineFriends(int userID);
        User GetUserById(int userId);
        List<UserSearchResult> SearchUsers(string name, int userID);
        List<FriendRequests> GetSentFriendRequests(int userID);
        List<FriendRequests> GetReceivedFriendRequests(int userID);
        void SendFriendRequest(int endUserID, int loggedInUserID);
        int SaveUserNotification(string notificationType, int fromUserID, int toUserID);
        FriendMapping GetFriendRequestStatus(int userID);
        int ResponseToFriendRequest(int requestorID, string requestResponse, int endUserID);
        List<UserNotificationList> GetUserNotifications(int toUserID);
        int GetUserNotificationCounts(int toUserID);
        void ChangeNotificationStatus(int[] notificationIDs);
        FriendMapping RemoveFriendMapping(int friendMappingID);
        List<User> GetUsersByLinqQuery(Expression<Func<User, bool>> where);
        List<OnlineUserDetails> GetRecentChats(int currentUserID);
        OnlineUser GetUserOnlineStatus(int userID);
        void UpdateUserProfilePicture(int userID, string imagePath);
        void SaveUserImage(int userID, string imagePath, bool isProfilePicture);
        List<OnlineUserDetails> GetFriends(int userID);
    }
}
