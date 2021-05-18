//-----------------------------------------------------------------------
// <copyright file="Product.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace ComputerStore.BoundedContext.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public int WebsiteId { get; set; }            
        public string Name { get; set; }
        public string ProductCode { get; set; }
        public string Description { get; set; }
        public float Discount { get; set; }
        public int Warranty { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public int ViewCount { get; set; }
        public string MetaData { get; set; }
        public string SpecificData { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int Status { get; set; }

        public virtual Category Category { get; set; }
        public virtual Website Website { get; set; }
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
        public virtual ICollection<ProductImage> ProductImage { get; set; }
        public virtual ICollection<Cart> Cart { get; set; }
        public virtual ICollection<AnonymousCart> AnonymousCart { get; set; }
    }
}
