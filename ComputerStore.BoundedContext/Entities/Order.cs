//-----------------------------------------------------------------------
// <copyright file="Order.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace ComputerStore.BoundedContext.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? WebsiteId { get; set; }
        public string ShipAddress { get; set; }
        public string Phone { get; set; }
        public bool? PaymentState { get; set; }
        public int OrderState { get; set; }
        public float Total { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? Status { get; set; }
        public string Note { get; set; }

        public virtual User User { get; set; }
        public virtual Website Website { get; set; }
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
    }
}
