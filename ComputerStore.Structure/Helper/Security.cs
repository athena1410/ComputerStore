//-----------------------------------------------------------------------
// <copyright file="CategoryController.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Text;

namespace ComputerStore.Structure.Helper
{
    //Solution for security problem on project
    public class Security
    {
        //Create hash password for authentication
        public static string CreateHashPassword(string salt, string value)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                password: value,
                salt: Encoding.UTF8.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

            return Convert.ToBase64String(valueBytes);
        }

        //Validate password user provide
        public static bool ValidatePassword(string salt, string value, string hash)
            => CreateHashPassword(salt, value) == hash;
    }
}
