//-----------------------------------------------------------------------
// <copyright file="CartServiceBuilder.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using AutoMapper;
using ComputerStore.Api.Mappings;
using ComputerStore.BoundedContext.Entities;
using ComputerStore.Domain.Implement;
using ComputerStore.Domain.Interfaces;
using ComputerStore.UnitOfWork.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ComputerStore.UnitTest.Services.CartServiceTest
{
    public class CartServiceBuilder
    {
        private readonly Mock<IRepository<Cart>> _mockCartRepository;
        private readonly Mock<IRepository<AnonymousCart>> _mockAnonymousCartRepository;
        private readonly Mock<IRepository<Product>> _mockProductRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mapper _mapper;

        public CartServiceBuilder()
        {
            var mockRepositoryObject = new MockRepository(MockBehavior.Strict);
            _mockCartRepository = mockRepositoryObject.Create<IRepository<Cart>>();
            _mockAnonymousCartRepository = mockRepositoryObject.Create<IRepository<AnonymousCart>>();
            _mockProductRepository = mockRepositoryObject.Create<IRepository<Product>>();
            _mockUnitOfWork = mockRepositoryObject.Create<IUnitOfWork>();

            var mapperConfiguration = new MapperConfiguration(new MappingProfile());
            _mapper = new Mapper(mapperConfiguration);
        }

        /// <summary>
        /// With the repository setup.
        /// </summary>
        /// <returns>Service builder with EF core repository mockup</returns>
        public CartServiceBuilder WithCartRepositoryMock(List<Cart> carts)
        {
            // 'GetAllAsync' repository mock
            _mockCartRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<Cart, bool>>>()))
                .Returns((Expression<Func<Cart, bool>> predicate) =>
                    Task.FromResult(carts.Where(predicate.Compile()).ToList() as IEnumerable<Cart>
                ));

            //'GetAllAsync' repository mock
            _mockCartRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<Cart, bool>>>(),
                It.IsAny<Func<IQueryable<Cart>, IQueryable<Cart>>>()))
                .Returns((
                Expression<Func<Cart, bool>> predicate, Func<IQueryable<Cart>, IQueryable<Cart>> include) =>
                Task.FromResult(carts.Where(predicate.Compile()).ToList() as IEnumerable<Cart>
                ));

            // 'Update' repository mock
            _mockCartRepository.Setup(x => x.Update(It.IsAny<Cart>())).Returns(It.IsAny<EntityState>());

            // 'Add' repository mock
            _mockCartRepository.Setup(x => x.Add(It.IsAny<Cart>())).Returns(EntityState.Added);

            // 'FindByAsync' repository mock
            _mockCartRepository.Setup(x => x.FindByAsync(It.IsAny<Expression<Func<Cart, bool>>>()))
                .Returns((Expression<Func<Cart, bool>> predicate) =>
                    Task.FromResult(carts.Where(predicate.Compile()).FirstOrDefault()
                ));

            return this;
        }

        /// <summary>
        /// With product repository setup.
        /// </summary>
        /// <returns>Service builder with EF core repository mockup</returns>
        public CartServiceBuilder WithProductRepositoryMock(List<Product> products)
        {
            // 'FindByAsync' repository mock
            _mockProductRepository.Setup(x => x.FindByAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .Returns((Expression<Func<Product, bool>> predicate) =>
                    Task.FromResult(products.Where(predicate.Compile()).FirstOrDefault()
                ));

            return this;
        }


        /// <summary>
        /// With the repository setup.
        /// </summary>
        /// <returns>Service builder with EF core repository mockup</returns>
        public CartServiceBuilder WithAnonymousCartRepositoryMock(List<AnonymousCart> anonymousCarts)
        {
            // 'GetAllAsync' repository mock
            _mockAnonymousCartRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<AnonymousCart, bool>>>()))
                .Returns((Expression<Func<AnonymousCart, bool>> predicate) =>
                    Task.FromResult(anonymousCarts.Where(predicate.Compile()).ToList() as IEnumerable<AnonymousCart>
                ));

            // 'Update' repository mock
            _mockAnonymousCartRepository.Setup(x => x.Update(It.IsAny<AnonymousCart>())).Returns(It.IsAny<EntityState>());

            return this;
        }

        /// <summary>
        /// With the unit of work setup.
        /// </summary>
        /// <returns>Service builder with Unit Of Work mockup</returns>
        public CartServiceBuilder WithUnitOfWorkSetup()
        {
            _mockUnitOfWork.Setup(x => x.CommitAsync()).ReturnsAsync(1);
            _mockUnitOfWork.Setup(x => x.GetRepository<Cart>()).Returns(_mockCartRepository.Object);
            _mockUnitOfWork.Setup(x => x.GetRepository<AnonymousCart>()).Returns(_mockAnonymousCartRepository.Object);
            _mockUnitOfWork.Setup(x => x.GetRepository<Product>()).Returns(_mockProductRepository.Object);
            return this;
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns>Instance of Company Service with mocked instances of unit of work</returns>
        public ICartService Build()
        {
            return new CartService(
                _mockUnitOfWork.Object, _mapper);
        }
    }
}
