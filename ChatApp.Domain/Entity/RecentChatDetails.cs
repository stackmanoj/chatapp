using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.Entity
{
    public class RecentChatDetails
    {
        public List<OnlineUserDetails> Users { get; set; }
        public int LastUserID { get; set; }
    }
}
