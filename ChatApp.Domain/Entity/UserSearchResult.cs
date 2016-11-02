using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.Entity
{
    public class UserSearchResult
    {
        public User UserInfo { get; set; }
        public string FriendRequestStatus { get; set; }
        public bool IsRequestReceived { get; set; }
    }
}
