using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.Entity
{
    public class ChatMessage
    {
        [Key]
        public int ChatMessageID { get; set; }
        public int FromUserID { get; set; }
        public int ToUserID { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public DateTime ViewedOn { get; set; }
        public bool IsActive { get; set; }
    }
}
