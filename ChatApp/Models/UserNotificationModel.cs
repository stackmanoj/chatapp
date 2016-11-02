using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatApp.Models
{
    public class UserNotificationModel
    {
        public string NotificationType { get; set; }
        public int NotificationID { get; set; }
        public UserModel User { get; set; }
        public DateTime CreatedOn { get; set; }
        public string NotificationStatus { get; set; }
        public int TotalNotifications { get; set; }
    }
}