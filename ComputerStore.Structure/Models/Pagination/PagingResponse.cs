namespace ComputerStore.Structure.Models.Pagination
{
    public class PagingResponse
    {
        public bool HasNext { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
