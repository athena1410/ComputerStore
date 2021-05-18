//-----------------------------------------------------------------------
// <copyright file="SearchModel.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using ComputerStore.Structure.Models.Pagination;
using System.ComponentModel.DataAnnotations;

namespace ComputerStore.Structure.Models
{
    public class SearchModel<T> : PagingContext
    {
        [Required]
        public T Data { get; set; }

        /// <summary>
        /// Extract paging context and search context from search model
        /// </summary>
        /// <returns></returns>
        public (T data, PagingContext pagingContext) Extract()
        {
            return (Data, ExtractPaging());
        }
    }
}
