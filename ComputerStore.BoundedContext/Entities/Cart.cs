//-----------------------------------------------------------------------
// <copyright file="Cart.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;

namespace ComputerStore.BoundedContext.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int WebsiteId { get; set; }
        public int ProductId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "The field {0} must be greater than or equal {1}.")]
        public int Quantity { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public virtual User User { get; set; }
        public virtual Product Product { get; set; }
        public virtual Website Website { get; set; }
    }
}
