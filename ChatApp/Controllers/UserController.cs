using ChatApp.Common;
using ChatApp.Domain.Abstract;
using ChatApp.Domain.Entity;
using ChatApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChatApp.Controllers
{
    public class UserController : BaseController
    {
        private IUser _UserRepo;
        public UserController(IUser UserRepo)
        {
            this._UserRepo = UserRepo;
        }
        public ActionResult Chat()
        {
            return View();
        }
        [HttpPost]
        public ActionResult getusers()
        {

            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Profile(int Id = 0)
        {
            if (Id == 0)
            {
                Id = MySession.Current.UserID;
            }
            var objmodel = CommonFunctions.GetUserModel(Id);
            if (Id != MySession.Current.UserID)
            {
                var friendInfo = _UserRepo.GetFriendRequestStatus(Id);
                if (friendInfo != null)
                {
                    objmodel.FriendRequestStatus = friendInfo.RequestStatus;
                    objmodel.FriendEndUserID = friendInfo.EndUserID;
                    objmodel.FriendRequestorID = friendInfo.RequestorUserID;
                    objmodel.FriendMappingID = friendInfo.FriendMappingID;
                }
            }
            return View(objmodel);
        }
        [HttpGet]
        public ActionResult EditProfile()
        {
            var objmodel = CommonFunctions.GetUserModel(MySession.Current.UserID);
            return View(objmodel);
        }
        [HttpPost]
        public ActionResult EditProfile(UserModel objmodel)
        {
            var objentity = _UserRepo.GetUserById(MySession.Current.UserID);
            objentity.Name = objmodel.Name;
            objentity.Gender = objmodel.Gender;
            objentity.DOB = Convert.ToDateTime(objmodel.DOB);
            objentity.Bio = objmodel.Bio;
            objentity.UpdatedOn = System.DateTime.Now;
            var result = _UserRepo.SaveUser(objentity);
            return RedirectToAction("Profile");
        }

        public ActionResult _UserSearchResult(string name)
        {
            var userList = _UserRepo.SearchUsers(name, MySession.Current.UserID);
            var objmodel = userList.Select(m => CommonFunctions.GetUserModel(m.UserInfo.UserID, m.UserInfo, m.FriendRequestStatus, m.IsRequestReceived)).ToList();
            return PartialView(objmodel);
        }
        public ActionResult _OnlineFriends()
        {
            var onlineFriends = _UserRepo.GetOnlineFriends(MySession.Current.UserID);
            var objmodel = onlineFriends.Select(m => new UserModel()
            {
                UserID = m.UserID,
                Name = m.Name,
                ProfilePicture = CommonFunctions.GetProfilePicture(m.ProfilePicture, m.Gender)
            }).ToList();
            return PartialView(objmodel);
        }
        public ActionResult _UserNotifications()
        {
            var notifications = _UserRepo.GetUserNotifications(MySession.Current.UserID);
            var objmodel = notifications.Select(m => new UserNotificationModel()
            {
                NotificationID = m.NotificationID,
                NotificationType = m.NotificationType,
                User = CommonFunctions.GetUserModel(0, m.User),
                NotificationStatus = m.NotificationStatus,
                CreatedOn = m.CreatedOn,
                TotalNotifications = m.TotalNotifications
            }).ToList();
            return PartialView(objmodel);
        }
        public ActionResult _RecentChats()
        {
            var recentChats = _UserRepo.GetRecentChats(MySession.Current.UserID);
            var objmodel = recentChats.Select(m => new UserModel()
            {
                UserID = m.UserID,
                Name = m.Name,
                ProfilePicture = CommonFunctions.GetProfilePicture(m.ProfilePicture, m.Gender),
                IsOnline = m.IsOnline,
                UnReadMessages = m.UnReadMessageCount > 0 ? Convert.ToString(m.UnReadMessageCount) : ""
            }).ToList();
            return PartialView(objmodel);
        }
        [HttpPost]
        public ActionResult UpdateProfilePicture(HttpPostedFileBase profilePicture, int userID)
        {
            try
            {
                string filePath = string.Empty;
                if (profilePicture != null)
                {
                    string folderpath = Server.MapPath("~/") + "Content/Images";
                    if (!System.IO.Directory.Exists(folderpath))
                    {
                        System.IO.Directory.CreateDirectory(folderpath);
                    }
                    string path = Server.MapPath("~/Content/Images/ProfilePictures");
                    if (!System.IO.Directory.Exists(path))
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }
                    Random r = new Random();
                    int randomNo = r.Next();
                    filePath = "/Content/Images/ProfilePictures/" + randomNo + "_" + profilePicture.FileName;
                    profilePicture.SaveAs(Server.MapPath("~/Content/Images/ProfilePictures/" + randomNo + "_" + profilePicture.FileName));
                    _UserRepo.UpdateUserProfilePicture(userID, filePath);
                    MySession.Current.ProfilePicture = filePath;
                    return Json(new { success = true, filePath = filePath }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, filePath = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, filePath = "" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Friends()
        {
            var friendUsers = _UserRepo.GetFriends(MySession.Current.UserID);
            var objmodel = friendUsers.Select(m => new UserModel()
            {
                UserID = m.UserID,
                Name = m.Name,
                ProfilePicture = CommonFunctions.GetProfilePicture(m.ProfilePicture, m.Gender),
                IsOnline = m.IsOnline,
            }).ToList();
            return View(objmodel);
        }
    }
}