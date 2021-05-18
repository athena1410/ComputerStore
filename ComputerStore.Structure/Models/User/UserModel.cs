//-----------------------------------------------------------------------
// <copyright file="UserModel.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using ComputerStore.Structure.Models.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static ComputerStore.Structure.Constants.Constants;

namespace ComputerStore.Structure.Models.User
{
    public class UserModel
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(Utility.PasswordMinLength)]
        public string Password { get; set; }
        [JsonIgnore]
        public int RoleId { get; set; }
        [JsonIgnore]
        public string Role { get; set; }
        public int WebsiteId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string DisplayName { get; set; }
        public int? Status { get; set; }
        [MaxLength(20)]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(RegularExpression.Phone, ErrorMessage = MessageResponse.PhoneNumberInvalid)]
        public string Phone { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
