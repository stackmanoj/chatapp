using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.Entity
{
    public class MessageRecords
    {
        public List<ChatMessage> Messages { get; set; }
        public int TotalMessages { get; set; }
        public int LastChatMessageId { get; set; }
    }
}
