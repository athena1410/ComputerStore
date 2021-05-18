using System;
using System.ComponentModel.DataAnnotations;
//-----------------------------------------------------------------------
// <copyright file="CartCreateModel.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

namespace ComputerStore.Structure.Models.Cart
{
    public class CartCreateModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "The field {0} must be greater than or equal {1}.")]
        public int ProductId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "The field {0} must be greater than or equal {1}.")]
        public int Quantity { get; set; }
    }
}
