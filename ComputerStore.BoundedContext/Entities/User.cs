//-----------------------------------------------------------------------
// <copyright file="User.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace ComputerStore.BoundedContext.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public int? WebsiteId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? Status { get; set; }

        public virtual Role Role { get; set; }
        public virtual Website Website { get; set; }
        public virtual ICollection<Order> Order { get; set; }
        public virtual ICollection<RefreshToken> RefreshToken { get; set; }
        public virtual ICollection<Cart> Cart { get; set; }
    }
}
