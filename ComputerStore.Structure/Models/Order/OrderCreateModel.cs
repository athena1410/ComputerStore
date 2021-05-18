//-----------------------------------------------------------------------
// <copyright file="OrderCreateModel.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static ComputerStore.Structure.Constants.Constants;

namespace ComputerStore.Structure.Models.Order
{
    public class OrderCreateModel
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The field {0} must be greater than {1}.")]
        public int WebsiteId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The field {0} must be greater than {1}.")]
        public int UserId { get; set; }
        [RegularExpression(RegularExpression.Phone)]
        public string Phone { get; set; }
        [Required]
        public string ShipAddress { get; set; }
    }
}
