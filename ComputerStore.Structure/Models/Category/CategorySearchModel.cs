//-----------------------------------------------------------------------
// <copyright file="CategorySearch.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>BinhHTV</author>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;

namespace ComputerStore.Structure.Models.Category
{
    public class CategorySearchModel
    {
        [MaxLength(100, ErrorMessage = "Search string less than 100 characters")]
        public string Name { get; set; }
    }
}
