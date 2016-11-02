using ChatApp.Domain.Abstract;
using ChatApp.Domain.Concrete;
using ChatApp.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.Concrete
{
    public class EFUserRepository : IUser
    {
        EFDbContext _context = new EFDbContext();
        public Tuple<User, string> SaveUser(User objentity)
        {
            var obj = _context.Users.Where(m => m.UserID == objentity.UserID).FirstOrDefault();
            if (obj != null)
            {
                obj.Name = objentity.Name;
                obj.Gender = objentity.Gender;
                obj.DOB = objentity.DOB;
                obj.Bio = objentity.Bio;
                obj.UpdatedOn = System.DateTime.Now;
            }
            else
            {
                var existUserName = _context.Users.Where(m => m.UserName == objentity.UserName && m.IsActive == true).FirstOrDefault();
                if (existUserName != null)
                {
                    return new Tuple<User, string>(objentity, "User name is already exist. Please try with another user name.");
                }
                _context.Users.Add(objentity);
            }
            _context.SaveChanges();
            return new Tuple<User, string>(objentity, "");
        }
        public User CheckLogin(string userName, string password)
        {
            var obj = _context.Users.Where(m => m.UserName == userName && m.Password == password && m.IsActive == true).FirstOrDefault();
            return obj;
        }
        public void SaveUserOnlineStatus(OnlineUser objentity)
        {
            var obj = _context.OnlineUsers.Where(m => m.UserID == objentity.UserID && m.IsActive == true).FirstOrDefault();
            if (obj != null)
            {
                obj.IsOnline = objentity.IsOnline;
                obj.UpdatedOn = System.DateTime.Now;
                obj.ConnectionID = objentity.ConnectionID;
            }
            else
            {
                objentity.CreatedOn = System.DateTime.Now;
                objentity.UpdatedOn = System.DateTime.Now;
                objentity.IsActive = true;
                _context.OnlineUsers.Add(objentity);
            }
            _context.SaveChanges();
        }
        public string GetUserConnectionID(int UserID)
        {
            var obj = _context.OnlineUsers.Where(m => m.UserID == UserID && m.IsActive == true && m.IsOnline == true).Select(m => m.ConnectionID).FirstOrDefault();
            return obj;
        }
        public List<string> GetUserConnectionID(int[] userIDs)
        {
            var obj = _context.OnlineUsers.Where(m => userIDs.Contains(m.UserID) && m.IsActive == true && m.IsOnline == true).Select(m => m.ConnectionID).ToList();
            return obj;
        }
        public List<User> GetAllUsers()
        {
            var objList = _context.Users.ToList();
            return objList;
        }
        public List<OnlineUserDetails> GetOnlineFriends(int userID)
        {
            int[] friends = GetFriendUserIds(userID);
            var obj = (from u in _context.OnlineUsers
                       join v in _context.Users on u.UserID equals v.UserID
                       where u.IsOnline == true && friends.Contains(v.UserID)
                       select new OnlineUserDetails
                       {
                           UserID = u.UserID,
                           Name = v.Name,
                           ConnectionID = u.ConnectionID,
                           ProfilePicture = v.ProfilePicture,
                           Gender = v.Gender
                       }).OrderBy(m => m.Name).ToList();
            return obj;
        }
        public User GetUserById(int userId)
        {
            var obj = _context.Users.Where(m => m.UserID == userId).FirstOrDefault();
            return obj;
        }
        public int[] GetFriendUserIds(int userID)
        {
            var arr = _context.FriendMappings.Where(m => (m.RequestorUserID == userID || m.EndUserID == userID) && m.RequestStatus == "Accepted" && m.IsActive == true).Select(m => m.RequestorUserID == userID ? m.EndUserID : m.RequestorUserID).ToArray();
            return arr;
        }
        public List<FriendRequests> GetSentFriendRequests(int userID)
        {
            var list = (from u in _context.FriendMappings
                        join v in _context.Users on u.EndUserID equals v.UserID
                        where u.RequestorUserID == userID && u.RequestStatus == "Sent" && u.IsActive == true
                        select new FriendRequests()
                        {
                            UserInfo = v,
                            RequestStatus = u.RequestStatus,
                            EndUserID = u.EndUserID,
                            RequestorUserID = u.RequestorUserID
                        }).ToList();
            return list;
        }
        public List<FriendRequests> GetReceivedFriendRequests(int userID)
        {
            var list = (from u in _context.FriendMappings
                        join v in _context.Users on u.RequestorUserID equals v.UserID
                        where u.EndUserID == userID && u.RequestStatus == "Sent" && u.IsActive == true
                        select new FriendRequests()
                        {
                            UserInfo = v,
                            RequestStatus = u.RequestStatus,
                            EndUserID = u.EndUserID,
                            RequestorUserID = u.RequestorUserID
                        }).ToList();
            return list;
        }

        public List<FriendRequests> GetAllSentFriendRequests()
        {
            var list = (from u in _context.FriendMappings
                        join v in _context.Users on u.EndUserID equals v.UserID
                        where u.RequestStatus == "Sent" && u.IsActive == true
                        select new FriendRequests()
                        {
                            UserInfo = v,
                            RequestStatus = u.RequestStatus,
                            EndUserID = u.EndUserID,
                            RequestorUserID = u.RequestorUserID
                        }).ToList();
            return list;
        }
        public List<UserSearchResult> SearchUsers(string name, int userID)
        {
            int[] friendIds = GetFriendUserIds(userID);
            var objList = _context.Users.Where(m => m.Name.ToLower().Contains(name.ToLower()) && m.UserID != userID && !friendIds.Contains(m.UserID)).ToList();
            var receivedRequests = GetReceivedFriendRequests(userID);
            var sentRequests = GetSentFriendRequests(userID);
            List<UserSearchResult> list = new List<UserSearchResult>();
            foreach (var item in objList)
            {
                bool isReceived = false;
                var receivedRequest = receivedRequests.Where(x => x.UserInfo.UserID == item.UserID).FirstOrDefault();
                if (receivedRequest != null)
                {
                    isReceived = true;
                }
                var userInfo = new UserSearchResult();
                userInfo.IsRequestReceived = isReceived;
                userInfo.UserInfo = item;
                var sentRequest = sentRequests.Where(m => m.UserInfo.UserID == item.UserID).FirstOrDefault();
                if (sentRequest != null)
                {
                    userInfo.FriendRequestStatus = sentRequest.RequestStatus; ;
                }
                list.Add(userInfo);
            }
            return list;
        }
        public void SendFriendRequest(int endUserID, int loggedInUserID)
        {
            FriendMapping objentity = new FriendMapping();
            objentity.CreatedOn = System.DateTime.Now;
            objentity.EndUserID = endUserID;
            objentity.IsActive = true;
            objentity.RequestorUserID = loggedInUserID;
            objentity.RequestStatus = "Sent";
            objentity.UpdatedOn = System.DateTime.Now;
            _context.FriendMappings.Add(objentity);
            _context.SaveChanges();
        }
        public int SaveUserNotification(string notificationType, int fromUserID, int toUserID)
        {
            UserNotification notification = new UserNotification();
            notification.CreatedOn = System.DateTime.Now;
            notification.IsActive = true;
            notification.NotificationType = notificationType;
            notification.FromUserID = fromUserID;
            notification.Status = "New";
            notification.UpdatedOn = System.DateTime.Now;
            notification.ToUserID = toUserID;
            _context.UserNotifications.Add(notification);
            _context.SaveChanges();
            return notification.NotificationID;
        }
        public FriendMapping GetFriendRequestStatus(int userID)
        {
            var obj = _context.FriendMappings.Where(m => (m.EndUserID == userID || m.RequestorUserID == userID) && m.IsActive == true).FirstOrDefault();
            return obj;
        }
        public int ResponseToFriendRequest(int requestorID, string requestResponse, int endUserID)
        {
            var request = _context.FriendMappings.Where(m => m.EndUserID == endUserID && m.RequestorUserID == requestorID && m.IsActive == true).FirstOrDefault();
            if (request != null)
            {
                request.RequestStatus = requestResponse;
                request.UpdatedOn = System.DateTime.Now;
                _context.SaveChanges();
            }
            var notification = _context.UserNotifications.Where(m => m.ToUserID == endUserID && m.FromUserID == requestorID && m.IsActive == true && m.NotificationType == "FriendRequest").FirstOrDefault();
            if (notification != null)
            {
                notification.IsActive = false;
                notification.UpdatedOn = System.DateTime.Now;
                _context.SaveChanges();
                return notification.NotificationID;
            }
            return 0;
        }
        public List<UserNotificationList> GetUserNotifications(int toUserID)
        {
            var listQuery = (from u in _context.UserNotifications
                             join v in _context.Users on u.FromUserID equals v.UserID
                             where u.ToUserID == toUserID && u.IsActive == true
                             select new UserNotificationList()
                             {
                                 NotificationID = u.NotificationID,
                                 NotificationType = u.NotificationType,
                                 User = v,
                                 NotificationStatus = u.Status,
                                 CreatedOn = u.CreatedOn
                             }).OrderByDescending(m => m.CreatedOn);
            int totalNotifications = listQuery.Count();
            var list = listQuery.Take(3).ToList();
            list.ForEach(m => m.TotalNotifications = totalNotifications);
            return list;
        }
        public int GetUserNotificationCounts(int toUserID)
        {
            int count = _context.UserNotifications.Where(m => m.Status == "New" && m.ToUserID == toUserID && m.IsActive == true).Count();
            return count;
        }
        public void ChangeNotificationStatus(int[] notificationIDs)
        {
            _context.UserNotifications.Where(m => notificationIDs.Contains(m.NotificationID)).ToList().ForEach(m => m.Status = "Read");
            _context.SaveChanges();
        }
        public FriendMapping RemoveFriendMapping(int friendMappingID)
        {
            var obj = _context.FriendMappings.Where(m => m.FriendMappingID == friendMappingID).FirstOrDefault();
            if (obj != null)
            {
                obj.IsActive = false;
                _context.SaveChanges();
            }
            return obj;
        }
        public void UpdateProfilePicture(int userID, string profilePicturePath)
        {

        }
        public List<User> GetUsersByLinqQuery(Expression<Func<User, bool>> where)
        {
            var objList = _context.Users.Where(where).ToList();
            return objList;
        }
        public List<OnlineUserDetails> GetRecentChats(int currentUserID)
        {
            int[] friends = GetFriendUserIds(currentUserID);
            var recentMessages = _context.ChatMessages.Where(m => m.IsActive == true && (m.ToUserID == currentUserID || m.FromUserID == currentUserID)).OrderByDescending(m => m.CreatedOn).ToList();
            var userIds = recentMessages.Select(m => (m.ToUserID == currentUserID ? m.FromUserID : m.ToUserID)).Distinct().ToArray();
            var userIdsList = userIds.ToList();
            var messagesByUserId = recentMessages.Where(m => m.ToUserID == currentUserID && m.Status == "Sent").ToList();
            var newMessagesCount = (from p in messagesByUserId
                                    group p by p.FromUserID into g
                                    select new { FromUserID = g.Key, Count = g.Count() }).ToList();
            var onlineUserIDs = _context.OnlineUsers.Where(m => friends.Contains(m.UserID) && userIds.Contains(m.UserID) && m.IsActive == true && m.IsOnline == true).Select(m => m.UserID).ToArray();
            var users = (from m in _context.Users
                         join v in userIdsList on m.UserID equals v
                         select new OnlineUserDetails
                         {
                             UserID = m.UserID,
                             Name = m.Name,
                             ProfilePicture = m.ProfilePicture,
                             Gender = m.Gender,
                             IsOnline = onlineUserIDs.Contains(m.UserID) ? true : false
                         }).ToList();
            users.ForEach(m =>
            {
                m.UnReadMessageCount = newMessagesCount.Where(x => x.FromUserID == m.UserID).Select(x => x.Count).FirstOrDefault();
            });
            users = users.OrderBy(d => userIdsList.IndexOf(d.UserID)).ToList();
            return users;
        }
        public OnlineUser GetUserOnlineStatus(int userID)
        {
            var obj = _context.OnlineUsers.Where(m => m.UserID == userID).FirstOrDefault();
            return obj;
        }
        public void UpdateUserProfilePicture(int userID, string imagePath)
        {
            var obj = _context.Users.Where(m => m.UserID == userID).FirstOrDefault();
            if (obj != null)
            {
                obj.ProfilePicture = imagePath;
                obj.UpdatedOn = System.DateTime.Now;
                _context.SaveChanges();
                SaveUserImage(userID, imagePath, true);
            }
        }
        public void SaveUserImage(int userID, string imagePath, bool isProfilePicture)
        {
            if (isProfilePicture)
            {
                var existingProfilePicture = _context.UserImages.Where(m => m.UserID == userID && m.IsActive == true && m.IsProfilePicture == true).FirstOrDefault();
                if (existingProfilePicture != null)
                {
                    existingProfilePicture.IsProfilePicture = false;
                    _context.SaveChanges();
                }
            }
            UserImage objentity = new UserImage();
            objentity.CreatedOn = System.DateTime.Now;
            objentity.ImagePath = imagePath;
            objentity.IsActive = true;
            objentity.IsProfilePicture = isProfilePicture;
            objentity.UserID = userID;
            _context.UserImages.Add(objentity);
            _context.SaveChanges();
        }
        public List<OnlineUserDetails> GetFriends(int userID)
        {
            var friendIds = GetFriendUserIds(userID);
            var onlineUserIDs = _context.OnlineUsers.Where(m => friendIds.Contains(m.UserID) && m.IsActive == true && m.IsOnline == true).Select(m => m.UserID).ToArray();
            var users = _context.Users.Where(m => friendIds.Contains(m.UserID)).Select(m => new OnlineUserDetails
                         {
                             UserID = m.UserID,
                             Name = m.Name,
                             ProfilePicture = m.ProfilePicture,
                             Gender = m.Gender,
                             IsOnline = onlineUserIDs.Contains(m.UserID) ? true : false
                         }).ToList();
            return users;
        }
    }
}
