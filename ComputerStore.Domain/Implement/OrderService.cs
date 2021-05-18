using AutoMapper;
using ComputerStore.BoundedContext.Entities;
using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Constants;
using ComputerStore.Structure.Enums;
using ComputerStore.Structure.Exceptions;
using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Order;
using ComputerStore.Structure.Models.Pagination;
using ComputerStore.UnitOfWork.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Role = ComputerStore.Structure.Enums.Role;

namespace ComputerStore.Domain.Implement
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        /// <summary>
        /// Get order by id
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<OrderModel> GetByIdAsync(int websiteId, int orderId)
        {
            var orderRepository = unitOfWork.GetRepository<Order>();
            var order = await orderRepository
                                .FindByAsync(x => x.Id == orderId && x.WebsiteId == websiteId,
                                    x => x.Include(u => u.User)
                                          .Include(o => o.OrderDetail)
                                          .ThenInclude(p => p.Product));
            return mapper.Map<Order, OrderModel>(order);
        }

        /// <summary>
        /// Update order
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="orderId"></param>
        /// <param name="orderModel"></param>
        /// <returns></returns>
        public async Task UpdateAsync(int websiteId, int orderId, OrderModel orderModel)
        {
            
            var orderRepository = unitOfWork.GetRepository<Order>();
            var productRepository = unitOfWork.GetRepository<Product>();

            var order = await orderRepository
                                .FindByAsync(x => x.Id == orderId && x.WebsiteId == websiteId,
                                    x => x.Include(o => o.OrderDetail));
            if (order == null)
            {
                throw new NotFoundException(string.Format(
                        Constants.MessageResponse.NotFoundError, nameof(Order), orderId));
            }

            //Only allow update order for order has state is in-progress
            if (order.OrderState != (int)OrderState.InProgress)
            {
                throw new ValidationException(string.Format(
                    Constants.MessageResponse.InvalidUpdateOrderOperationError, ((OrderState)order.OrderState).ToString()));
            }

            order = mapper.Map(orderModel, order);
            //Update order detail
            foreach (var oderDetailModel in orderModel.OrderDetail)
            {
                var orderDetail = order.OrderDetail.FirstOrDefault(x => x.Id == oderDetailModel.Id);

                if (orderDetail == null)
                {
                    throw new NotFoundException(string.Format(
                        Constants.MessageResponse.NotFoundError, nameof(OrderDetail), oderDetailModel.Id));
                }

                var quantityChange = oderDetailModel.Quantity - orderDetail.Quantity;
                //If not change continue
                if (quantityChange == 0) continue;

                var product = await productRepository.FindByAsync(
                    x => x.Id == orderDetail.ProductId &&
                         x.Status == (int)Status.ACTIVE);

                if (product == null)
                {
                    throw new NotFoundException(string.Format(
                        Constants.MessageResponse.NotFoundError, nameof(Product), orderDetail.ProductId));
                }

                if (product.Quantity < quantityChange)
                {
                    throw new ValidationException(string.Format(
                        Constants.MessageResponse.MissingInventoryData, nameof(Product), product.Name));
                }
                //update quantity detail
                orderDetail.Quantity = oderDetailModel.Quantity;

                //update quantity of product in inventory
                product.Quantity -= quantityChange;
                product.UpdatedDate = DateTime.UtcNow;
                productRepository.Update(product);
            }
            order.Total = order.OrderDetail.Sum(od => (od.Price * od.Quantity) * (1 - (od.Discount / 100)));
            order.UpdatedDate = DateTime.UtcNow;
            orderRepository.Update(order);
            await unitOfWork.CommitAsync();
        }

        /// <summary>
        /// Search orders
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public async Task<PaginationResponse<List<OrderModel>>> SearchAsync(int? websiteId, SearchModel<OrderSearchModel> searchModel)
        {
            var roleId = websiteId == null ? (int)Role.Administrator : (int)Role.User;
            var orderRepository = unitOfWork.GetRepository<Order>();
            //Extract search model generate order search model and paging context
            var (orderSearchModel, pagingContext) = searchModel.Extract();

            Expression<Func<Order, bool>> predicate = x =>
            (orderSearchModel.UserDisplayName == null || x.User.FirstName.ToLower().Contains(orderSearchModel.UserDisplayName.ToLower())
            || x.User.LastName.ToLower().Contains(orderSearchModel.UserDisplayName.ToLower()))
            && (!orderSearchModel.Id.HasValue || x.Id == orderSearchModel.Id)
            && (roleId == (int)Role.Administrator || x.WebsiteId == websiteId);

            var includes = new[] { nameof(User), nameof(OrderDetail) };
            var orders = await orderRepository.GetAllAsync(predicate, pagingContext, includes);
            var totalRecord = await orderRepository.CountAsync(predicate);
            return new PaginationResponse<List<OrderModel>>(
                mapper.Map<List<OrderModel>>(orders), totalRecord, searchModel.NumberPerPage);
        }

        /// <summary>
        /// Create new order
        /// </summary>
        /// <param name="orderCreateModel"></param>
        /// <returns></returns>
        public async Task CreateAsync(OrderCreateModel orderCreateModel)
        {
            var orderRepository = unitOfWork.GetRepository<Order>();
            var cartRepository = unitOfWork.GetRepository<Cart>();
            var productRepository = unitOfWork.GetRepository<Product>();
            var order = mapper.Map<Order>(orderCreateModel);
            var carts = (await cartRepository.GetAllAsync(x => x.UserId == orderCreateModel.UserId &&
                                                        x.WebsiteId == orderCreateModel.WebsiteId &&
                                                       !x.DeletedDate.HasValue)).ToList();
            //Prevent create order with empty cart
            if (!carts.Any())
            {
                throw new ValidationException(Constants.MessageResponse.CheckOutWithEmptyCartError);
            }

            foreach (var cart in carts)
            {
                var product = await productRepository.FindByAsync(
                    x => x.Id == cart.ProductId &&
                         x.Status == (int)Status.ACTIVE && x.WebsiteId == orderCreateModel.WebsiteId);

                if (product == null)
                {
                    throw new NotFoundException(string.Format(
                        Constants.MessageResponse.NotFoundError, nameof(Product), cart.ProductId));
                }

                if (product.Quantity < cart.Quantity)
                {
                    throw new ValidationException(string.Format(
                        Constants.MessageResponse.MissingInventoryData, nameof(Product), product.Name));
                }
                //create new order detail
                order.OrderDetail.Add(new OrderDetail
                {
                    ProductId = cart.ProductId,
                    Quantity = cart.Quantity,
                    Price = product.Price,
                    Discount = product.Discount
                });

                //update quantity of product in inventory
                product.Quantity -= cart.Quantity;
                product.UpdatedDate = DateTime.UtcNow;
                productRepository.Update(product);

                //remove cart 
                cart.DeletedDate = DateTime.UtcNow;
                cartRepository.Update(cart);
            }

            order.Total = order.OrderDetail.Sum(od => (od.Price * od.Quantity) * (1 - (od.Discount / 100)));
            order.Status = (int)Status.ACTIVE;
            order.CreatedDate = DateTime.UtcNow;
            orderRepository.Add(order);
            await unitOfWork.CommitAsync();
        }

        /// <summary>
        /// Get order history
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<OrderModel>> GetOrderHistoryAsync(int websiteId, int userId)
        {
            var orderRepository = unitOfWork.GetRepository<Order>();
            var orders = await orderRepository.GetAllAsync(x => x.UserId == userId &&
                            x.WebsiteId == websiteId, nameof(OrderDetail));
            return this.mapper.Map<List<OrderModel>>(orders);
        }

        /// <summary>
        /// Change order state
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="orderId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task ChangeOrderStateAsync(int websiteId, int orderId, int state)
        {
            var orderRepository = unitOfWork.GetRepository<Order>();
            var order = await orderRepository.FindByAsync(x => x.Id == orderId &&
                            x.WebsiteId == websiteId && x.Status == (int)Status.ACTIVE);

            if (order == null)
            {
                throw new NotFoundException(string.Format(
                    Constants.MessageResponse.NotFoundError, nameof(Order), orderId));
            }

            //Only allow change state of order with state is in-progress
            if (order.OrderState != (int)OrderState.InProgress)
            {
                throw new ValidationException(string.Format(
                    Constants.MessageResponse.InvalidUpdateOrderOperationError, ((OrderState)order.OrderState).ToString()));
            }

            //When change order state to rejected => re-update products quantity in stock
            if (state == (int)OrderState.Rejected)
            {
                var orderDetailRepository = unitOfWork.GetRepository<OrderDetail>();
                var orderDetails = (await orderDetailRepository.GetAllAsync(x => x.OrderId == order.Id)).ToList();
                if (orderDetails.Any())
                {
                    var productRepository = unitOfWork.GetRepository<Product>();
                    foreach (var orderDetail in orderDetails)
                    {
                        var product = await productRepository.GetAsync(orderDetail.ProductId);
                        if (product == null) continue;

                        product.Quantity += orderDetail.Quantity;
                        product.UpdatedDate = DateTime.UtcNow;
                        productRepository.Update(product);
                    }
                }
            }
            order.OrderState = state;
            order.PaymentState = state == (int)OrderState.Completed;
            order.UpdatedDate = DateTime.UtcNow;
            orderRepository.Update(order);
            await unitOfWork.CommitAsync();
        }
    }
}
