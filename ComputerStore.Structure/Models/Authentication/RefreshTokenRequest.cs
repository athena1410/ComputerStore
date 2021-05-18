//-----------------------------------------------------------------------
// <copyright file="RefreshTokenRequest.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------
using System.ComponentModel.DataAnnotations;

namespace ComputerStore.Structure.Models.Authentication
{
    public class RefreshTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}
