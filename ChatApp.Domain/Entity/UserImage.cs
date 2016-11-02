using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.Entity
{
    public class UserImage
    {
        [Key]
        public int ImageID { get; set; }
        public int UserID { get; set; }
        public string ImagePath { get; set; }
        public bool IsProfilePicture { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }

    }
}
