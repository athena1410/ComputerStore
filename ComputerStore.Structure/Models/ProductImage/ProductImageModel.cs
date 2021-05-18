using System;

namespace ComputerStore.Structure.Models.ProductImage
{
   public class ProductImageModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? Status { get; set; }
    }
}
