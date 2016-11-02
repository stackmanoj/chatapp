using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.Entity
{
    public class OnlineUser
    {
        [Key]
        public int OnlineUserID { get; set; }
        public int UserID { get; set; }
        public string ConnectionID { get; set; }
        public bool IsOnline { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsActive { get; set; }
    }
}
