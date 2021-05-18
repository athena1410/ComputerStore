//-----------------------------------------------------------------------
// <copyright file="CartItemModel.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

namespace ComputerStore.Structure.Models.Cart
{
    public class CartItemModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string ProductImage { get; set; }
        public float Price { get; set; }
        public float Discount { get; set; }
    }
}
