//-----------------------------------------------------------------------
// <copyright file="CategoryController.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;

namespace ComputerStore.Structure.Models.Company
{
    public class CompanyModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(20)]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^((\\+84-?)|0)?[0-9]{10}$", ErrorMessage = "Not a valid phone number")]
        public string Phone { get; set; }
        [MaxLength(100)]
        public string Address { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
