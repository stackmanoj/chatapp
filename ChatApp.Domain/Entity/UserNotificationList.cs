using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.Entity
{
    public class UserNotificationList
    {
        public string NotificationType { get; set; }
        public int NotificationID { get; set; }
        public User User { get; set; }
        public DateTime CreatedOn { get; set; }
        public string NotificationStatus { get; set; }
        public int TotalNotifications { get; set; }
    }
}
