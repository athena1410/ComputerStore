using ComputerStore.Structure.Extensions;
using System.ComponentModel.DataAnnotations;
using static ComputerStore.Structure.Constants.Constants;

namespace ComputerStore.Structure.Models.Pagination
{
    public class PagingContext
    {
        public int PageNumber { get; set; } = Paging.PageInit;
        public int NumberPerPage { get; set; } = Paging.PageSizeDefault;
        public string SortColums { get; set; }

        [RegularExpression("asc|desc", ErrorMessage = "The sort direction must be either 'asc' or 'desc' only.")]
        public string SortDirection { get; set; }

        public PagingContext ExtractPaging()
        {
            if (PageNumber == Paging.NoPageSize)
            {
                PageNumber = Paging.PageInit;
            }

            if (NumberPerPage == Paging.NoPageSize)
            {
                NumberPerPage = Paging.PageSizeDefault;
            }

            if (SortColums != null && !string.IsNullOrEmpty(SortColums))
            {
                SortColums = SortColums.FirstCharToUpper();
            }
            return this;
        }
    }
}
