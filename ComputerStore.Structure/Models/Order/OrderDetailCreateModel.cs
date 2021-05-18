using System.ComponentModel.DataAnnotations;

namespace ComputerStore.Structure.Models.Order
{
    public class OrderDetailCreateModel
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The field {0} must be greater than {1}.")]
        public int ProductId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The field {0} must be greater than {1}.")]
        public int Quantity { get; set; }
    }
}
