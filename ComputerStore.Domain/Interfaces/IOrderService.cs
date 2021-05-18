using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Order;
using ComputerStore.Structure.Models.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComputerStore.Domain.Interfaces
{
    public interface IOrderService
    {
        /// <summary>
        /// Update order
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="orderId"></param>
        /// <param name="orderModel"></param>
        /// <returns></returns>
        Task UpdateAsync(int websiteId, int orderId, OrderModel orderModel);
        /// <summary>
        /// Get order by id
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<OrderModel> GetByIdAsync(int websiteId, int orderId);
        /// <summary>
        /// Search order
        /// </summary>
        /// <param name="websiteId"></param>
        /// <returns></returns>
        Task<PaginationResponse<List<OrderModel>>> SearchAsync(int? websiteId, SearchModel<OrderSearchModel> searchModel);

        /// <summary>
        /// Create new order
        /// </summary>
        /// <param name="orderCreateModel"></param>
        /// <returns></returns>
        Task CreateAsync(OrderCreateModel orderCreateModel);

        /// <summary>
        /// Get order history
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<OrderModel>> GetOrderHistoryAsync(int websiteId, int userId);

        /// <summary>
        /// Change order state
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="orderId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        Task ChangeOrderStateAsync(int websiteId, int orderId, int state);
    }
}
