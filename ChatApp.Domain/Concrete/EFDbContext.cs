using ChatApp.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.Concrete
{
    public class EFDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<OnlineUser> OnlineUsers { get; set; }
        public DbSet<FriendMapping> FriendMappings { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<UserImage> UserImages { get; set; }
    }
}
