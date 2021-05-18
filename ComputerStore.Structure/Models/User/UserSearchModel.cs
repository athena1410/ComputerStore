//-----------------------------------------------------------------------
// <copyright file="UserSearchModel.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------
namespace ComputerStore.Structure.Models.User
{
    public class UserSearchModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public string DisplayName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public int? Status { get; set; }
    }
}
