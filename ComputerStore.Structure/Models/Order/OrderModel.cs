using ComputerStore.Structure.Models.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ComputerStore.Structure.Models.Order
{
    public class OrderModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? WebsiteId { get; set; }
        public string ShipAddress { get; set; }
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(Constants.Constants.RegularExpression.Phone, ErrorMessage = Constants.Constants.MessageResponse.PhoneNumberInvalid)]
        public string Phone { get; set; }
        public bool? PaymentState { get; set; }
        public int OrderState { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? Status { get; set; }
        public string UserDisplayName { get; set; }
        public float Total { get; set; }
        public string Note { get; set; }

        public UserModel User { get; set; }
        public List<OrderDetailModel> OrderDetail { get; set; }
    }
}
