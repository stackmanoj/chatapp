using ChatApp.Domain.Concrete;
using ChatApp.Domain.Entity;
using ChatApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatApp
{
    public static class CommonFunctions
    {
        public static string GetProfilePicture(string profilePicture, string gender)
        {
            string profilePicturePath = "";
            if (string.IsNullOrEmpty(profilePicture))
            {
                if (gender == "Female")
                {
                    profilePicturePath = "/Content/Images/female-default-pic.jpg";
                }
                else
                {
                    profilePicturePath = "/Content/Images/male-default-pic.jpg";
                }
            }
            else
            {
                profilePicturePath = profilePicture;
            }
            return profilePicturePath;
        }
        public static UserModel GetUserModel(int id, User objentity = null, string friendRequestStatus = "", bool isRequestReceived = false)
        {
            var user = new User();
            if (objentity != null)
            {
                user = objentity;
            }
            else
            {
                EFUserRepository _UserRepo = new EFUserRepository();
                user = _UserRepo.GetUserById(id);
            }
            UserModel objmodel = new UserModel();
            if (user != null)
            {
                objmodel.IsRequestReceived = isRequestReceived;
                objmodel.FriendRequestStatus = friendRequestStatus;
                objmodel.UserID = user.UserID;
                objmodel.Name = user.Name;
                objmodel.ProfilePicture = CommonFunctions.GetProfilePicture(user.ProfilePicture, user.Gender);
                objmodel.Gender = user.Gender;
                objmodel.DOB = user.DOB.ToShortDateString();
                if (user.DOB != null)
                {
                    objmodel.Age = Convert.ToString(Math.Floor(DateTime.Now.Subtract(Convert.ToDateTime(user.DOB)).TotalDays / 365.0)) + " Years";
                }
                else
                {
                    objmodel.Age = "NaN";
                }
                objmodel.Bio = user.Bio;
            }
            return objmodel;
        }
        public static MessageModel GetMessageModel(ChatMessage objentity)
        {
            MessageModel objmodel = new MessageModel();
            objmodel.ChatMessageID = objentity.ChatMessageID;
            objmodel.FromUserID = objentity.FromUserID;
            objmodel.ToUserID = objentity.ToUserID;
            objmodel.Message = objentity.Message;
            objmodel.Status = objentity.Status;
            objmodel.CreatedOn =Convert.ToString(objentity.CreatedOn);
            objmodel.UpdatedOn = Convert.ToString(objentity.UpdatedOn);
            objmodel.ViewedOn = Convert.ToString(objentity.ViewedOn);
            objmodel.IsActive = objentity.IsActive;
            return objmodel;
        }
    }
}