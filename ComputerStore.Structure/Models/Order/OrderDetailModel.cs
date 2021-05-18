using ComputerStore.Structure.Models.Product;
using System.ComponentModel.DataAnnotations;

namespace ComputerStore.Structure.Models.Order
{
    public class OrderDetailModel
    {
        public int Id { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The field {0} must be greater than {1}.")]
        public int OrderId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The field {0} must be greater than {1}.")]
        public int ProductId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The field {0} must be greater than {1}.")]
        public int Quantity { get; set; }
        public float Price { get; set; }
        public float Discount { get; set; }
        public string ProductDisplayName { get; set; }

        public ProductModel Product { get; set; }
    }
}
