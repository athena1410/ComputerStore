//-----------------------------------------------------------------------
// <copyright file="CategoryController.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using System;

namespace ComputerStore.Structure.Models.Pagination
{
    public class PaginationResponse<T>
    {
        public PaginationResponse(T data, int totalRecords, int pageSize)
        {
            Results = data;
            MetaData = new PagingResponse
            {
                TotalCount = totalRecords,
                PageSize = pageSize,
                HasNext = totalRecords > pageSize,
                TotalPages = (int)Math.Ceiling((decimal)totalRecords / pageSize)
            };
        }
        public T Results { get; set; }
        public PagingResponse MetaData { get; set; }
    }
}
