using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.Entity
{
    public class FriendRequests
    {
        public User UserInfo { get; set; }
        public string RequestStatus { get; set; }
        public int RequestorUserID { get; set; }
        public int EndUserID { get; set; }
    }
}
