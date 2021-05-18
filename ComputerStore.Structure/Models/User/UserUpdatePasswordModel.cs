using System;
using System.Collections.Generic;
using System.Text;

namespace ComputerStore.Structure.Models.User
{
    public class UserUpdatePasswordModel
    {
        public int Id { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public int? WebsiteId { get; set; }
    }
}
