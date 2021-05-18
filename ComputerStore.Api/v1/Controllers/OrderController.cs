using ComputerStore.Api.Attribute;
using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Enums;
using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Order;
using ComputerStore.Structure.Models.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComputerStore.Api.v1.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class OrderController : BaseController
    {
        private readonly IOrderService orderService;

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        /// <summary>
        /// Create new order
        /// </summary>
        /// <param name="orderCreateModel"></param>
        /// <returns></returns>
        [Authorize(Roles = nameof(Role.User))]
        [SecondAuthorize(Order = 2)]
        [HttpPost]
        public async Task<IActionResult> Post(OrderCreateModel orderCreateModel)
        {
            orderCreateModel.WebsiteId = this.WebsiteId;
            orderCreateModel.UserId = this.UserId;
            await orderService.CreateAsync(orderCreateModel);
            return Ok(new ApiResponse<OrderCreateModel>());
        }

        /// <summary>
        /// Update order
        /// </summary>
        [Authorize(Roles = nameof(Role.Administrator))]
        [SecondAuthorize(Order = 2)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] OrderModel orderModel)
        {
            await orderService.UpdateAsync(this.WebsiteId, id, orderModel);
            return Ok(new ApiResponse<OrderModel>());
        }

        /// <summary>
        /// Get order and order detail by id
        /// </summary>
        [Authorize(Roles = "User, Administrator")]
        [SecondAuthorize(Order = 2)]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var order = await this.orderService.GetByIdAsync(this.WebsiteId, id);
            return Ok(new ApiResponse<OrderModel>(order));
        }

        /// <summary>
        /// Search order
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody] SearchModel<OrderSearchModel> searchModel)
        {
            var websiteId = TokenRole.Equals(nameof(Role.SuperAdmin)) ? (int?)null : WebsiteId;
            var orders = await this.orderService.SearchAsync(websiteId, searchModel);
            return Ok(new ApiResponse<PaginationResponse<List<OrderModel>>>(orders));
        }

        /// <summary>
        /// Get order history
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = nameof(Role.User))]
        [SecondAuthorize(Order = 2)]
        [HttpGet("order-history")]
        public async Task<IActionResult> GetOrderHistory()
        {
            var orders = await this.orderService.GetOrderHistoryAsync(this.WebsiteId, this.UserId);
            return Ok(new ApiResponse<List<OrderModel>>(orders));
        }

        /// <summary>
        /// Change order state
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = nameof(Role.Administrator))]
        [SecondAuthorize(Order = 2)]
        [HttpPut("ChangeOrderState/{id}")]
        public async Task<IActionResult> ChangeOrderState(int id, [FromBody] OrderState state)
        {
            await orderService.ChangeOrderStateAsync(this.WebsiteId, id, (int)state);
            return Ok(new ApiResponse<OrderModel>());
        }
    }
}
