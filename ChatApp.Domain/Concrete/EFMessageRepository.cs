using ChatApp.Domain.Abstract;
using ChatApp.Domain.Concrete;
using ChatApp.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.Concrete
{
    public class EFMessageRepository : IMessage
    {
        EFDbContext _context = new EFDbContext();
        public ChatMessage SaveChatMessage(ChatMessage objentity)
        {
            _context.ChatMessages.Add(objentity);
            _context.SaveChanges();
            return objentity;
        }
        public MessageRecords GetChatMessagesByUserID(int currentUserID, int toUserID, int lastMessageID = 0)
        {
            MessageRecords obj = new MessageRecords();
            var messages = _context.ChatMessages.Where(m => m.IsActive == true && (m.ToUserID == toUserID || m.FromUserID == toUserID) && (m.ToUserID == currentUserID || m.FromUserID == currentUserID)).OrderByDescending(m => m.CreatedOn);
            if (lastMessageID > 0)
            {
                obj.Messages = messages.Where(m => m.ChatMessageID < lastMessageID).Take(20).ToList().OrderBy(m => m.CreatedOn).ToList();
            }
            else
            {
                obj.Messages = messages.Take(20).ToList().OrderBy(m => m.CreatedOn).ToList();
            }
            obj.LastChatMessageId = obj.Messages.OrderBy(m => m.ChatMessageID).Select(m => m.ChatMessageID).FirstOrDefault();
            return obj;
        }
        public void UpdateMessageStatusByUserID(int fromUserID, int currentUserID)
        {
            var unreadMessages = _context.ChatMessages.Where(m => m.Status == "Sent" && m.ToUserID == currentUserID && m.FromUserID == fromUserID && m.IsActive == true).ToList();
            unreadMessages.ForEach(m =>
            {
                m.Status = "Viewed";
                m.ViewedOn = System.DateTime.Now;
            });
            _context.SaveChanges();
        }
        public void UpdateMessageStatusByMessageID(int messageID)
        {
            var unreadMessages = _context.ChatMessages.Where(m => m.ChatMessageID == messageID).FirstOrDefault();
            if (unreadMessages != null)
            {
                unreadMessages.Status = "Viewed";
                unreadMessages.ViewedOn = System.DateTime.Now;
                _context.SaveChanges();
            }
        }
    }
}
