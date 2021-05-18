//-----------------------------------------------------------------------
// <copyright file="RefreshToken.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------
using System;

namespace ComputerStore.BoundedContext.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByIp { get; set; }
        public DateTime? Revoked { get; set; }
        public string RevokedByIp { get; set; }
        public string ReplacedByToken { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public bool IsActive => Revoked == null && !IsExpired;
        public virtual User User { get; set; }
    }
}
