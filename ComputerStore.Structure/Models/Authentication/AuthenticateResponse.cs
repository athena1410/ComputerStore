//-----------------------------------------------------------------------
// <copyright file="AuthenticateResponse.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using ComputerStore.Structure.Models.User;

namespace ComputerStore.Structure.Models.Authentication
{
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }  
        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public AuthenticateResponse(UserModel user, string jwtToken, string refreshToken)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.Email;
            Role = ((Enums.Role)user.RoleId).ToString();
            Token = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}
