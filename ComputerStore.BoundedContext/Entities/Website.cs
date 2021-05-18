//-----------------------------------------------------------------------
// <copyright file="Website.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ComputerStore.BoundedContext.Entities
{
    public class Website
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
        public string UrlPath { get; set; }
        public string LogoUrl { get; set; }
        public string Note { get; set; }
        [MinLength(20)]
        public string SecretKey { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? Status { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<Order> Order { get; set; }
        public virtual ICollection<Product> Product { get; set; }
        public virtual ICollection<User> User { get; set; }
        public virtual ICollection<Cart> Cart { get; set; }
        public virtual ICollection<AnonymousCart> AnonymousCart { get; set; }
    }
}
