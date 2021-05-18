//-----------------------------------------------------------------------
// <copyright file="JwtSettings.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

namespace ComputerStore.Structure.Models
{
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public int SuperAdminTimeExpires { get; set; }
        public int AdministratorTimeExpires { get; set; }
        public int UserTimeExpires { get; set; }
        public int RefreshTokenExpires { get; set; }
    }
}
