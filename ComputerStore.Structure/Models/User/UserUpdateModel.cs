using System;
using System.ComponentModel.DataAnnotations;
using static ComputerStore.Structure.Constants.Constants;

namespace ComputerStore.Structure.Models.User
{
    public class UserUpdateModel
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Required]
        public int WebsiteId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public int? Status { get; set; }
        [MaxLength(20)]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(RegularExpression.Phone, ErrorMessage = MessageResponse.PhoneNumberInvalid)]
        public string Phone { get; set; }
    }
}
