using AutoMapper;
using ComputerStore.Api.Mappings;
using ComputerStore.BoundedContext.Entities;
using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Enums;
using ComputerStore.Structure.Exceptions;
using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Order;
using ComputerStore.Structure.Models.Pagination;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static ComputerStore.Structure.Constants.Constants;

namespace ComputerStore.UnitTest.Services.OrderServiceTest
{
    [TestFixture]
    public class OrderServiceTest
    {
        private IOrderService orderService;
        private List<Product> products;
        private List<Order> orders;
        private List<OrderDetail> orderDetails;
        private List<Cart> carts;
        private PagingContext pagingContext;
        private Mapper mapper;

        [SetUp]
        public void SetUp()
        {
            var mapperConfiguration = new MapperConfiguration(new MappingProfile());
            mapper = new Mapper(mapperConfiguration);

            #region Seed data for unit test
            //build list product
            products = new List<Product>()
            {
                new Product()
                {
                    Id = 1,
                    WebsiteId = 1,
                    CategoryId = 2,
                    Name = "Product Sub Category 1",
                    ProductCode = "Product01",
                    Quantity = 5
                },
                new Product()
                {
                    Id = 2,
                    WebsiteId = 1,
                    CategoryId = 2,
                    Name = "Product Sub Category 2",
                    ProductCode = "Product02",
                    Quantity = 10
                }
            };
            //Build List order
            orders = new List<Order>()
            {
                new Order
                {
                    Id = 1,
                    UserId = 6,
                    WebsiteId = 1,
                    ShipAddress = "Ha Noi 1",
                    Phone = "0886663346",
                    CreatedDate = DateTime.UtcNow,
                    Total = 350,
                    Status = 0,
                    OrderDetail = new List<OrderDetail>()
                    {
                        new OrderDetail
                        {
                            Id = 1,
                            OrderId = 1,
                            ProductId = 1,
                            Quantity = 2,
                            Price = 100,
                            Discount = 0
                        },
                        new OrderDetail
                        {
                            Id = 2,
                            OrderId = 1,
                            ProductId = 2,
                            Quantity = 1,
                            Price = 150,
                            Discount = 0
                        }
                    }
                },
                new Order
                {
                    Id = 2,
                    UserId = 7,
                    WebsiteId = 2,
                    ShipAddress = "Ha Noi 2",
                    Phone = "0886663346",
                    CreatedDate = DateTime.UtcNow,
                    Total = 150,
                    Status = 0,
                    OrderDetail = new List<OrderDetail>()
                    {
                        new OrderDetail
                        {
                            Id = 3,
                            OrderId = 2,
                            ProductId = 2,
                            Quantity = 1,
                            Price = 150,
                            Discount = 0
                        }
                    }
                },
                new Order
                {
                    Id = 3,
                    UserId = 6,
                    WebsiteId = 1,
                    ShipAddress = "Ha Noi 3",
                    Phone = "0886663346",
                    CreatedDate = DateTime.UtcNow,
                    Total = 150,
                    Status = 0,
                    OrderDetail = new List<OrderDetail>()
                    {
                        new OrderDetail
                        {
                            Id = 4,
                            OrderId = 2,
                            ProductId = 1,
                            Quantity = 2,
                            Price = 300,
                            Discount = 0
                        }
                    }
                }
            };
            //build list order detail
            orderDetails = new List<OrderDetail>
            {
                new OrderDetail
                {
                    Id = 1,
                    OrderId = 1,
                    ProductId = 1,
                    Quantity = 2,
                    Price = 100,
                    Discount = 0
                },
                new OrderDetail
                {
                    Id = 2,
                    OrderId = 1,
                    ProductId = 2,
                    Quantity = 1,
                    Price = 150,
                    Discount = 0
                },
                new OrderDetail
                {
                    Id = 3,
                    OrderId = 2,
                    ProductId = 2,
                    Quantity = 1,
                    Price = 150,
                    Discount = 0
                },
                new OrderDetail
                {
                    Id = 4,
                    OrderId = 2,
                    ProductId = 1,
                    Quantity = 2,
                    Price = 300,
                    Discount = 0
                }
            };
            //build list cart
            carts = new List<Cart>
            {
                new Cart
                {
                    Id = 1,
                    UserId = 1,
                    ProductId = 1,
                    WebsiteId = 1,
                    Quantity = 1
                },
                new Cart
                {
                    Id = 2,
                    UserId = 1,
                    ProductId = 2,
                    WebsiteId = 1,
                    Quantity = 3
                }
            };
            //build paging context
            pagingContext = new PagingContext()
            {
                NumberPerPage = 2,
                PageNumber = 1,
                SortColums = "Id",
                SortDirection = "asc"
            };
            #endregion

            //Build order service with unit of work
            orderService = new OrderServiceBuilder()
                .WithOrderRepositoryMock(orders, pagingContext)
                .WithProductRepositoryMock(products)
                .WithOrderDetailRepositoryMock(orderDetails)
                .WithCartRepositoryMock(carts)
                .WithUnitOfWorkSetup()
                .Build();
        }

        [Test]
        public void TestSearchAsync_WithExistedOrders_ShouldReturnPaginationResponseWithListOrders()
        {
            var searchModel = new SearchModel<OrderSearchModel>()
            {
                Data = new OrderSearchModel()
                {
                    Id = 1
                },
                NumberPerPage = pagingContext.NumberPerPage,
                PageNumber = pagingContext.PageNumber,
                SortColums = pagingContext.SortColums,
                SortDirection = pagingContext.SortDirection
            };

            const int websiteId = 1;
            var actual = orderService.SearchAsync(websiteId, searchModel).GetAwaiter().GetResult();
            Assert.AreEqual(1, actual.MetaData.TotalPages);
            Assert.AreEqual(1, actual.MetaData.TotalCount);
            Assert.AreEqual(pagingContext.NumberPerPage, actual.MetaData.PageSize);
            Assert.True(!actual.MetaData.HasNext);

            var expectDataJson = JsonConvert.SerializeObject(
                mapper.Map<List<OrderModel>>(orders.Where(x => x.Id == searchModel.Data.Id).Take(searchModel.NumberPerPage)));
            var actualDataJson = JsonConvert.SerializeObject(actual.Results);
            Assert.AreEqual(expectDataJson, actualDataJson);
        }

        [Test]
        public void TestSearchAsync_WithNotExistedOrders_ShouldReturnPaginationResponseWithEmptyOrder()
        {
            var searchModel = new SearchModel<OrderSearchModel>()
            {
                Data = new OrderSearchModel()
                {
                    Id = 10
                },
                NumberPerPage = pagingContext.NumberPerPage,
                PageNumber = pagingContext.PageNumber,
                SortColums = pagingContext.SortColums,
                SortDirection = pagingContext.SortDirection
            };

            const int websiteId = 1;
            var actual = orderService.SearchAsync(websiteId, searchModel).GetAwaiter().GetResult();
            Assert.AreEqual(0, actual.MetaData.TotalPages);
            Assert.AreEqual(0, actual.MetaData.TotalCount);
            Assert.AreEqual(pagingContext.NumberPerPage, actual.MetaData.PageSize);
            Assert.IsEmpty(actual.Results);
        }

        [Test]
        public void TestSearchAsync_WithWrongWebsiteId_ShouldReturnPaginationResponseWithEmptyOrder()
        {
            var searchModel = new SearchModel<OrderSearchModel>()
            {
                Data = new OrderSearchModel()
                {
                    Id = 1
                },
                NumberPerPage = pagingContext.NumberPerPage,
                PageNumber = pagingContext.PageNumber,
                SortColums = pagingContext.SortColums,
                SortDirection = pagingContext.SortDirection
            };

            const int websiteId = 10;
            var actual = orderService.SearchAsync(websiteId, searchModel).GetAwaiter().GetResult();
            Assert.AreEqual(0, actual.MetaData.TotalPages);
            Assert.AreEqual(0, actual.MetaData.TotalCount);
            Assert.AreEqual(pagingContext.NumberPerPage, actual.MetaData.PageSize);
            Assert.IsEmpty(actual.Results);
        }

        [Test]
        public void TestGetByIdAsync_WithExistedOrder_ShouldReturnOrder()
        {
            const int websiteId = 1;
            const int orderId = 1;
            var actual = orderService.GetByIdAsync(websiteId, orderId).GetAwaiter().GetResult();
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<OrderModel>(actual);
            var actualJson = JsonConvert.SerializeObject(actual);
            var expectJson = JsonConvert.SerializeObject(mapper.Map<OrderModel>(orders.Find(x => x.Id == orderId)));
            Assert.AreEqual(expectJson, actualJson);
        }

        [Test]
        public void TestGetByIdAsync_WithNotFoundOrder_ShouldReturnNull()
        {
            const int websiteId = 1;
            const int orderId = 10;
            var actual = orderService.GetByIdAsync(websiteId, orderId).GetAwaiter().GetResult();
            Assert.Null(actual);
        }

        [Test]
        public void TestGetByIdAsync_WithWrongWebsiteId_ShouldReturnNull()
        {
            const int websiteId = 3;
            const int orderId = 1;
            var actual = orderService.GetByIdAsync(websiteId, orderId).GetAwaiter().GetResult();
            Assert.Null(actual);
        }

        [Test]
        public void TestGetOrderHistoryAsync_WithNotExistedOrders_ShouldReturnEmptyOrders()
        {
            var websiteId = 1;
            var userId = 10;
            var actual = orderService.GetOrderHistoryAsync(websiteId, userId).GetAwaiter().GetResult();
            Assert.IsEmpty(actual);
        }

        [Test]
        public void TestGetOrderHistoryAsync_WithWrongWebsiteId_ShouldReturnEmptyOrders()
        {
            const int websiteId = 10;
            const int userId = 1;
            var actual = orderService.GetOrderHistoryAsync(websiteId, userId).GetAwaiter().GetResult();
            Assert.IsEmpty(actual);
        }

        [Test]
        public void TestGetOrderHistoryAsync_WithExistedOrders_ShouldReturnOrders()
        {
            const int websiteId = 1;
            const int userId = 6;
            var actual = orderService.GetOrderHistoryAsync(websiteId, userId).GetAwaiter().GetResult();
            var expectDataJson = JsonConvert.SerializeObject(
                mapper.Map<List<OrderModel>>(orders.Where(x => x.UserId == userId && x.WebsiteId == websiteId)));
            var actualDataJson = JsonConvert.SerializeObject(actual);
            Assert.AreEqual(expectDataJson, actualDataJson);
        }

        [Test]
        public void TestCreateAsync_WithValidData_ShouldNotThrowAnyException()
        {
            var orderCreateModel = new OrderCreateModel
            {
                WebsiteId = 1,
                UserId = 1,
                Phone = "0906322555",
                ShipAddress = "Ha Noi"
            };

            Assert.DoesNotThrowAsync(() => orderService.CreateAsync(orderCreateModel));
            Assert.AreEqual(4, products[0].Quantity);
            Assert.AreEqual(7, products[1].Quantity);
            var actualCart = carts.Where(x => x.UserId == orderCreateModel.UserId &&
                                x.WebsiteId == orderCreateModel.WebsiteId && !x.DeletedDate.HasValue);
            //Verify delete cart item
            foreach (var cart in actualCart)
            {
                Assert.NotNull(cart.DeletedDate);
            }
        }

        [Test]
        public void TestCreateAsync_WithNotExistedProduct_ShouldThrowNotFoundException()
        {
            var orderCreateModel = new OrderCreateModel
            {
                WebsiteId = 1,
                UserId = 1,
                Phone = "0906322555",
                ShipAddress = "Ha Noi"
            };
            //delete product with id = 1
            var product = products.FirstOrDefault(x => x.Id == 1);
            product.Status = (int)Status.DEACTIVATE;

            var ex = Assert.ThrowsAsync<NotFoundException>(() => orderService.CreateAsync(orderCreateModel));
            Assert.AreEqual(string.Format(MessageResponse.NotFoundError, nameof(Product),
                product.Id), ex.Message);
        }

        [Test]
        public void TestCreateAsync_WithNotEnoughQuantityOfProduct_ShouldThrowMissingInventoryDataException()
        {
            var orderCreateModel = new OrderCreateModel
            {
                UserId = 1,
                WebsiteId = 1,
                Phone = "0906322555",
                ShipAddress = "Ha Noi"
            };
            //Set quantity of product with id = 1 is 0 (sold out)
            var product = products.FirstOrDefault(x => x.Id == 1);
            product.Quantity = 0;
            var ex = Assert.ThrowsAsync<ValidationException>(() => orderService.CreateAsync(orderCreateModel));
            Assert.AreEqual(string.Format(MessageResponse.MissingInventoryData, nameof(Product), product.Name), ex.Message);
        }

        [Test]
        public void TestCreateAsync_WithEmptyCart_ShouldThrowValidationException()
        {
            var orderCreateModel = new OrderCreateModel
            {
                UserId = 2,
                WebsiteId = 1,
                Phone = "0906322555",
                ShipAddress = "Ha Noi"
            };
            var ex = Assert.ThrowsAsync<ValidationException>(() => orderService.CreateAsync(orderCreateModel));
            Assert.AreEqual(MessageResponse.CheckOutWithEmptyCartError, ex.Message);
        }

        [Test]
        public void TestChangeStateAsync_WithNotFoundOrder_ShouldThrowNotFoundException()
        {
            const int websiteId = 1;
            const int orderId = 4;
            const int state = 1;

            var ex = Assert.ThrowsAsync<NotFoundException>(() => orderService.ChangeOrderStateAsync(websiteId, orderId, state));
            Assert.AreEqual(string.Format(MessageResponse.NotFoundError, nameof(Order), orderId), ex.Message);
        }

        [Test]
        public void TestChangeStateAsync_WithOrderHasStateIsNotInProgress_ShouldThrowValidationException()
        {
            const int websiteId = 1;
            const int orderId = 1;
            const int state = 0;

            //Setup order with id = 1 has order state = completed
            var order = orders.First(x => x.Id == orderId);
            order.OrderState = 1;

            var ex = Assert.ThrowsAsync<ValidationException>(() => orderService.ChangeOrderStateAsync(websiteId, orderId, state));
            Assert.AreEqual(string.Format(MessageResponse.InvalidUpdateOrderOperationError,
                ((Structure.Enums.OrderState)order.OrderState).ToString()), ex.Message);
        }

        [Test]
        public void TestChangeStateAsync_ToCompleted_ShouldNotThrowAnyException()
        {
            const int websiteId = 1;
            const int orderId = 1;
            const int state = (int)Structure.Enums.OrderState.Completed;

            orderService.ChangeOrderStateAsync(websiteId, orderId, state).GetAwaiter().GetResult();

            var actual = orderService.GetByIdAsync(websiteId, orderId).GetAwaiter().GetResult();
            Assert.AreEqual(1, actual.OrderState);
            Assert.IsTrue(actual.PaymentState);
        }

        [Test]
        public void TestChangeStateAsync_ToRejected_ShouldReUpdateQuantityInInventory()
        {
            const int websiteId = 1;
            const int orderId = 1;
            const int state = (int)Structure.Enums.OrderState.Rejected;
            orderService.ChangeOrderStateAsync(websiteId, orderId, state).GetAwaiter().GetResult();

            var actual = orderService.GetByIdAsync(websiteId, orderId).GetAwaiter().GetResult();
            Assert.AreEqual((int)Structure.Enums.OrderState.Rejected, actual.OrderState);
            Assert.IsFalse(actual.PaymentState);
            Assert.AreEqual(7, products[0].Quantity);
            Assert.AreEqual(11, products[1].Quantity);
        }

        [Test]
        public void TestUpdateAsync_WithValidData_ShouldNotThrowAnyException()
        {
            const int websiteId = 1;
            const int orderId = 1;
            var orderModel = new OrderModel
            {
                UserId = 6,
                WebsiteId = 1,
                ShipAddress = "Ha Noi 123",
                Phone = "0886663345",
                OrderDetail = new List<OrderDetailModel>()
                {
                    new OrderDetailModel
                    {
                        Id = 1,
                        OrderId = 1,
                        ProductId = 1,
                        Quantity = 3
                    },
                    new OrderDetailModel
                    {
                        Id = 2,
                        OrderId = 1,
                        ProductId = 2,
                        Quantity = 3
                    }
                }
            };

            orderService.UpdateAsync(websiteId, orderId, orderModel).GetAwaiter().GetResult();

            var actual = orderService.GetByIdAsync(websiteId, orderId).GetAwaiter().GetResult();
            //Verify change order information
            Assert.AreEqual(orderModel.ShipAddress, actual.ShipAddress);
            Assert.AreEqual(orderModel.Phone, actual.Phone);
            Assert.AreEqual(750, actual.Total);
            Assert.AreEqual(orderModel.OrderDetail[0].Quantity, actual.OrderDetail[0].Quantity);
            Assert.AreEqual(orderModel.OrderDetail[1].Quantity, actual.OrderDetail[1].Quantity);

            //Verify update product quantity in inventory
            Assert.AreEqual(4, products[0].Quantity);
            Assert.AreEqual(8, products[1].Quantity);
        }

        [Test]
        public void TestUpdateAsync_WithNotFoundOrder_ShouldThrowNotFoundException()
        {
            const int websiteId = 1;
            const int orderId = 4;
            var orderModel = new OrderModel
            {
                UserId = 6,
                WebsiteId = 1,
                ShipAddress = "Ha Noi 123",
                Phone = "0886663345",
                OrderDetail = new List<OrderDetailModel>()
                {
                    new OrderDetailModel
                    {
                        Id = 1,
                        OrderId = 1,
                        ProductId = 1,
                        Quantity = 3
                    },
                    new OrderDetailModel
                    {
                        Id = 2,
                        OrderId = 1,
                        ProductId = 2,
                        Quantity = 3
                    }
                }
            };

            var ex = Assert.ThrowsAsync<NotFoundException>(
                () => orderService.UpdateAsync(websiteId, orderId, orderModel));
            Assert.AreEqual(string.Format(MessageResponse.NotFoundError, 
                nameof(Order), orderId), ex.Message);
        }

        [Test]
        public void TestUpdateAsync_WithOrderHasStateIsCompleted_ShouldThrowValidationException()
        {
            const int websiteId = 1;
            const int orderId = 1;

            //Setup order with id = 1 has order state = completed
            var order = orders.First(x => x.Id == orderId);
            order.OrderState = 1;
            var orderModel = new OrderModel
            {
                UserId = 6,
                WebsiteId = 1,
                ShipAddress = "Ha Noi 123",
                Phone = "0886663345",
                OrderDetail = new List<OrderDetailModel>()
                {
                    new OrderDetailModel
                    {
                        Id = 1,
                        OrderId = 1,
                        ProductId = 1,
                        Quantity = 3
                    },
                    new OrderDetailModel
                    {
                        Id = 2,
                        OrderId = 1,
                        ProductId = 2,
                        Quantity = 3
                    }
                }
            };

            var ex = Assert.ThrowsAsync<ValidationException>(
                () =>orderService.UpdateAsync(websiteId, orderId, orderModel));
            Assert.AreEqual(string.Format(MessageResponse.InvalidUpdateOrderOperationError, 
                ((OrderState)order.OrderState).ToString()), ex.Message);
        }

        [Test]
        public void TestUpdateAsync_WithOrderHasStateIsRejected_ShouldThrowValidationException()
        {
            const int websiteId = 1;
            const int orderId = 1;

            //Setup order with id = 1 has order state = completed
            var order = orders.First(x => x.Id == orderId);
            order.OrderState = 2;
            var orderModel = new OrderModel
            {
                UserId = 6,
                WebsiteId = 1,
                ShipAddress = "Ha Noi 123",
                Phone = "0886663345",
                OrderDetail = new List<OrderDetailModel>()
                {
                    new OrderDetailModel
                    {
                        Id = 1,
                        OrderId = 1,
                        ProductId = 1,
                        Quantity = 3
                    },
                    new OrderDetailModel
                    {
                        Id = 2,
                        OrderId = 1,
                        ProductId = 2,
                        Quantity = 3
                    }
                }
            };

            var ex = Assert.ThrowsAsync<ValidationException>(
                () => orderService.UpdateAsync(websiteId, orderId, orderModel));
            Assert.AreEqual(string.Format(MessageResponse.InvalidUpdateOrderOperationError,
                ((OrderState)order.OrderState).ToString()), ex.Message);
        }

        [Test]
        public void TestUpdateAsync_WithNotExistedOrderDetail_ShouldThrowNotFoundException()
        {
            const int websiteId = 1;
            const int orderId = 1;
            var orderModel = new OrderModel
            {
                UserId = 6,
                WebsiteId = 1,
                ShipAddress = "Ha Noi 123",
                Phone = "0886663345",
                OrderDetail = new List<OrderDetailModel>()
                {
                    new OrderDetailModel
                    {
                        Id = 3,
                        OrderId = 1,
                        ProductId = 1,
                        Quantity = 3
                    },
                    new OrderDetailModel
                    {
                        Id = 2,
                        OrderId = 1,
                        ProductId = 2,
                        Quantity = 3
                    }
                }
            };

            var ex = Assert.ThrowsAsync<NotFoundException>(
                () => orderService.UpdateAsync(websiteId, orderId, orderModel));
            Assert.AreEqual(string.Format(MessageResponse.NotFoundError, nameof(OrderDetail), 3), ex.Message);
        }

        [Test]
        public void TestUpdateAsync_WithDeactivateProduct_ShouldReturnNotFoundException()
        {
            const int websiteId = 1;
            const int orderId = 1;
            //Setup de-active product
            var product = products.First(x => x.Id == 1);
            product.Status = (int) Status.DEACTIVATE;

            var orderModel = new OrderModel
            {
                UserId = 6,
                WebsiteId = 1,
                ShipAddress = "Ha Noi 123",
                Phone = "0886663345",
                OrderDetail = new List<OrderDetailModel>()
                {
                    new OrderDetailModel
                    {
                        Id = 1,
                        OrderId = 1,
                        ProductId = 1,
                        Quantity = 3
                    },
                    new OrderDetailModel
                    {
                        Id = 2,
                        OrderId = 1,
                        ProductId = 2,
                        Quantity = 3
                    }
                }
            };

            var ex = Assert.ThrowsAsync<NotFoundException>(
                () => orderService.UpdateAsync(websiteId, orderId, orderModel));
            Assert.AreEqual(string.Format(MessageResponse.NotFoundError, nameof(Product), 1), ex.Message);
        }

        [Test]
        public void TestUpdateAsync_WithNotEnoughProduct_ShouldReturnValidationException()
        {
            const int websiteId = 1;
            const int orderId = 1;
            var orderModel = new OrderModel
            {
                UserId = 6,
                WebsiteId = 1,
                ShipAddress = "Ha Noi 123",
                Phone = "0886663345",
                OrderDetail = new List<OrderDetailModel>()
                {
                    new OrderDetailModel
                    {
                        Id = 1,
                        OrderId = 1,
                        ProductId = 1,
                        Quantity = 20
                    },
                    new OrderDetailModel
                    {
                        Id = 2,
                        OrderId = 1,
                        ProductId = 2,
                        Quantity = 3
                    }
                }
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => orderService.UpdateAsync(websiteId, orderId, orderModel));
            Assert.AreEqual(string.Format(MessageResponse.MissingInventoryData, nameof(Product), products[0].Name), ex.Message);
        }
    }
}
