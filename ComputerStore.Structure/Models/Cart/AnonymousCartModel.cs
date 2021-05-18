//-----------------------------------------------------------------------
// <copyright file="AnonymousCartModel.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;

namespace ComputerStore.Structure.Models.Cart
{
    public class AnonymousCartModel
    {
        public int Id { get; set; }
        public int WebsiteId { get; set; }
        [Required]
        public Guid? IdentityCode { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "The field {0} must be greater than or equal {1}.")]
        public int ProductId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "The field {0} must be greater than or equal {1}.")]
        public int Quantity { get; set; }
    }
}
