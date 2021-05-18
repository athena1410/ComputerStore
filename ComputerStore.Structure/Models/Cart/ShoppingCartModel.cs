//-----------------------------------------------------------------------
// <copyright file="ShoppingCartModel.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace ComputerStore.Structure.Models.Cart
{
    public class ShoppingCartModel
    {
        public float TotalPrice { get; set; }
        public int TotalItems { get; set; }
        public List<CartItemModel> Items { get; set; }

        public ShoppingCartModel()
        {
            Items = new List<CartItemModel>();
        }
    }
}
