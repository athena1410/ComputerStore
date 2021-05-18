using AutoMapper;
using ComputerStore.Api.Mappings;
using ComputerStore.BoundedContext.Entities;
using ComputerStore.Domain.Implement;
using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Extensions;
using ComputerStore.Structure.Models.Pagination;
using ComputerStore.UnitOfWork.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ComputerStore.UnitTest.Services.OrderServiceTest
{
    public class OrderServiceBuilder
    {
        private readonly Mock<IRepository<Order>> _mockOrderRepository;
        private readonly Mock<IRepository<Product>> _mockProductRepository;
        private readonly Mock<IRepository<OrderDetail>> _mockOrderDetailRepository;
        private readonly Mock<IRepository<Cart>> _mockCartRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mapper _mapper;

        public OrderServiceBuilder()
        {
            var mockRepositoryObject = new MockRepository(MockBehavior.Strict);
            _mockOrderRepository = mockRepositoryObject.Create<IRepository<Order>>();
            _mockProductRepository = mockRepositoryObject.Create<IRepository<Product>>();
            _mockOrderDetailRepository = mockRepositoryObject.Create<IRepository<OrderDetail>>();
            _mockCartRepository = mockRepositoryObject.Create<IRepository<Cart>>();
            
            _mockUnitOfWork = mockRepositoryObject.Create<IUnitOfWork>();

            var mapperConfiguration = new MapperConfiguration(new MappingProfile());
            _mapper = new Mapper(mapperConfiguration);
        }

        /// <summary>
        /// With the order repository setup.
        /// </summary>
        /// <returns>Service builder with EF core repository mockup</returns>
        public OrderServiceBuilder WithOrderRepositoryMock(List<Order> orders, PagingContext pagingContext)
        {
            //'GetAllAsync' repository mock
            _mockOrderRepository.Setup(o => o.GetAllAsync(It.IsAny<Expression<Func<Order, bool>>>()))
                .Returns((
                    Expression<Func<Order, bool>> predicate) =>
                         Task.FromResult(orders.Where(predicate.Compile()).ToList()
                                as IEnumerable<Order>));

            //'GetAllAsync' repository mock with paging
            var pageSize = (pagingContext.PageNumber - 1) * pagingContext.NumberPerPage;
            _mockOrderRepository.Setup(o => o.GetAllAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<PagingContext>(), It.IsAny<string[]>()))
                .Returns((
                    Expression<Func<Order, bool>> predicate, PagingContext paging, string[] include) =>
                         Task.FromResult(orders.Where(predicate.Compile())
                            .AsQueryable().Sort(pagingContext.SortColums, pagingContext.SortDirection)
                                .Skip(pageSize).Take(pagingContext.NumberPerPage) as IEnumerable<Order>));
            _mockOrderRepository.Setup(o => o.GetAllAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<string>()))
                .Returns((
                    Expression<Func<Order, bool>> predicate, string include) =>
                         Task.FromResult(orders.Where(predicate.Compile())
                            .AsQueryable().Sort(pagingContext.SortColums, pagingContext.SortDirection)
                                .Skip(pageSize).Take(pagingContext.NumberPerPage) as IEnumerable<Order>));

            //'FindByAsync' repository mock
            _mockOrderRepository.Setup(x => x.FindByAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Func<IQueryable<Order>, IQueryable<Order>>>()))
                .Returns((
                Expression<Func<Order, bool>> predicate, Func<IQueryable<Order>, IQueryable<Order>> include) =>
                Task.FromResult(orders.Where(predicate.Compile()).FirstOrDefault()
                ));

            _mockOrderRepository.Setup(x => x.FindByAsync(It.IsAny<Expression<Func<Order, bool>>>()))
                .Returns((
                Expression<Func<Order, bool>> predicate) =>
                Task.FromResult(orders.Where(predicate.Compile()).FirstOrDefault()
                ));

            //'CountAsync' repository mock
            _mockOrderRepository.Setup(o => o.CountAsync(It.IsAny<Expression<Func<Order, bool>>>()))
                .Returns((
                    Expression<Func<Order, bool>> predicate) =>
                        Task.FromResult(orders.Count(predicate.Compile())));

            // 'Update' repository mock
            _mockOrderRepository.Setup(x => x.Update(It.IsAny<Order>())).Returns(It.IsAny<EntityState>());
            _mockProductRepository.Setup(x => x.Update(It.IsAny<Product>())).Returns(It.IsAny<EntityState>());

            // 'Add' repository mock
            _mockOrderRepository.Setup(x => x.Add(It.IsAny<Order>())).Returns(EntityState.Added);

            return this;
        }

        /// <summary>
        /// With the product repository setup.
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        public OrderServiceBuilder WithProductRepositoryMock(List<Product> products)
        {
            //'FindByAsync' repository mock
            _mockProductRepository.Setup(x => x.FindByAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<Func<IQueryable<Product>, IQueryable<Product>>>()))
                .Returns((
                Expression<Func<Product, bool>> predicate, Func<IQueryable<Product>, IQueryable<Product>> include) =>
                Task.FromResult(products.Where(predicate.Compile()).FirstOrDefault()
                ));

            _mockProductRepository.Setup(x => x.FindByAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .Returns((
                Expression<Func<Product, bool>> predicate) =>
                Task.FromResult(products.Where(predicate.Compile()).FirstOrDefault()
                ));

            // 'Update' repository mock
            _mockProductRepository.Setup(x => x.Update(It.IsAny<Product>())).Returns(It.IsAny<EntityState>());

            // 'GetAsync' repository mock
            _mockProductRepository.Setup(x => x.GetAsync(1)).ReturnsAsync(() => products[0]);
            _mockProductRepository.Setup(x => x.GetAsync(2)).ReturnsAsync(() => products[1]);
            return this;
        }

        /// <summary>
        /// With the order detail repository setup.
        /// </summary>
        /// <param name="orderDetails"></param>
        /// <returns></returns>
        public OrderServiceBuilder WithOrderDetailRepositoryMock(List<OrderDetail> orderDetails)
        {
            //'FindByAsync' repository mock
            _mockOrderDetailRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<OrderDetail, bool>>>()))
                .Returns((
                        Expression<Func<OrderDetail, bool>> predicate) =>
                    Task.FromResult(orderDetails.Where(predicate.Compile())
                    ));

            return this;
        }

        /// <summary>
        /// With the cart repository setup.
        /// </summary>
        /// <param name="orderDetails"></param>
        /// <returns></returns>
        public OrderServiceBuilder WithCartRepositoryMock(List<Cart> carts) 
        {
            //'FindByAsync' repository mock
            _mockCartRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<Cart, bool>>>()))
                .Returns((
                        Expression<Func<Cart, bool>> predicate) =>
                    Task.FromResult(carts.Where(predicate.Compile())
                    ));

            // 'Update' repository mock
            _mockCartRepository.Setup(x => x.Update(It.IsAny<Cart>())).Returns(It.IsAny<EntityState>());
            return this;
        }

        /// <summary>
        /// With the unit of work setup.
        /// </summary>
        /// <returns>Service builder with Unit Of Work mockup</returns>
        public OrderServiceBuilder WithUnitOfWorkSetup()
        {
            _mockUnitOfWork.Setup(x => x.CommitAsync()).ReturnsAsync(1);
            _mockUnitOfWork.Setup(x => x.GetRepository<Order>()).Returns(_mockOrderRepository.Object);
            _mockUnitOfWork.Setup(x => x.GetRepository<Product>()).Returns(_mockProductRepository.Object);
            _mockUnitOfWork.Setup(x => x.GetRepository<OrderDetail>()).Returns(_mockOrderDetailRepository.Object);
            _mockUnitOfWork.Setup(x => x.GetRepository<Cart>()).Returns(_mockCartRepository.Object);
            return this;
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns>Instance of Company Service with mocked instances of unit of work</returns>
        public IOrderService Build()
        {
            return new OrderService(
                _mockUnitOfWork.Object, _mapper);
        }
    }
}
