using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ChatApp.Models
{
    public class ChatMessageModel
    {
        public UserModel UserDetail { get; set; }
        public List<MessageModel> ChatMessages { get; set; }
        public bool IsOnline { get; set; }
        public string LastSeen { get; set; }
        public int LastChatMessageId { get; set; }
    }
    public class MessageModel
    {
        public int ChatMessageID { get; set; }
        public int FromUserID { get; set; }
        public string FromUserName { get; set; }
        public int ToUserID { get; set; }
        public string ToUserName { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public string CreatedOn { get; set; }
        public string UpdatedOn { get; set; }
        public string ReceivedOn { get; set; }
        public string ViewedOn { get; set; }
        public bool IsActive { get; set; }
    }
    public class MesageBlockModel
    {
        public string MessageAlign { get; set; }
        public string Name { get; set; }
        public string ProfilePicture { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}