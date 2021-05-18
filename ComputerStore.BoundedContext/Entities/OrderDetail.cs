//-----------------------------------------------------------------------
// <copyright file="OrderDetail.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

namespace ComputerStore.BoundedContext.Entities
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public float Discount { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
