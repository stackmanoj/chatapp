using ChatApp.Common;
using ChatApp.Domain.Abstract;
using ChatApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChatApp.Controllers
{
    public class ChatController : Controller
    {
        private IUser _UserRepo;
        private IMessage _MessageRepo;
        public ChatController(IUser UserRepo, IMessage MessageRepo)
        {
            this._UserRepo = UserRepo;
            this._MessageRepo = MessageRepo;
        }
        public ActionResult _Messages(int Id)
        {
            var userModel = CommonFunctions.GetUserModel(Id);
            var messages = _MessageRepo.GetChatMessagesByUserID(MySession.Current.UserID, Id);
            var objmodel = new ChatMessageModel();
            objmodel.UserDetail = userModel;
            objmodel.ChatMessages = messages.Messages.Select(m => CommonFunctions.GetMessageModel(m)).ToList();
            objmodel.LastChatMessageId = messages.LastChatMessageId;
            var onlineStatus = _UserRepo.GetUserOnlineStatus(Id);
            if (onlineStatus != null)
            {
                objmodel.IsOnline = onlineStatus.IsOnline;
                objmodel.LastSeen = Convert.ToString(onlineStatus.UpdatedOn);
            }
            return View(objmodel);
        }
        public ActionResult GetRecentMessages(int Id, int lastChatMessageId)
        {
            var messages = _MessageRepo.GetChatMessagesByUserID(MySession.Current.UserID, Id, lastChatMessageId);
            var objmodel = new ChatMessageModel();
            objmodel.ChatMessages = messages.Messages.Select(m => CommonFunctions.GetMessageModel(m)).ToList();
            objmodel.LastChatMessageId = messages.LastChatMessageId;
            return Json(objmodel, JsonRequestBehavior.AllowGet);
        }
    }
}
