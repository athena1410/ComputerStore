//-----------------------------------------------------------------------
// <copyright file="RevokeTokenRequest.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace ComputerStore.Structure.Models.Authentication
{
    public class RevokeTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}
