//-----------------------------------------------------------------------
// <copyright file="AnonymousCartServiceBuilder.cs" company="Young">
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

namespace ComputerStore.UnitTest.Services.AnonymousCartServiceTest
{
    public class AnonymousCartServiceBuilder
    {
        private readonly Mock<IRepository<AnonymousCart>> _mockAnonymousCartRepository;
        private readonly Mock<IRepository<Product>> _mockProductRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mapper _mapper;

        public AnonymousCartServiceBuilder()
        {
            var mockRepositoryObject = new MockRepository(MockBehavior.Strict);
            _mockAnonymousCartRepository = mockRepositoryObject.Create<IRepository<AnonymousCart>>();
            _mockProductRepository = mockRepositoryObject.Create<IRepository<Product>>();
            _mockUnitOfWork = mockRepositoryObject.Create<IUnitOfWork>();

            var mapperConfiguration = new MapperConfiguration(new MappingProfile());
            _mapper = new Mapper(mapperConfiguration);
        }

        /// <summary>
        /// With product repository setup.
        /// </summary>
        /// <returns>Service builder with EF core repository mockup</returns>
        public AnonymousCartServiceBuilder WithProductRepositoryMock(List<Product> products)
        {
            // 'FindByAsync' repository mock
            _mockProductRepository.Setup(x => x.FindByAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .Returns((Expression<Func<Product, bool>> predicate) =>
                    Task.FromResult(products.Where(predicate.Compile()).FirstOrDefault()
                ));

            return this;
        }

        /// <summary>
        /// With the anonymoust cart repository setup.
        /// </summary>
        /// <returns>Service builder with EF core repository mockup</returns>
        public AnonymousCartServiceBuilder WithAnonymousCartRepositoryMock(List<AnonymousCart> anonymousCarts)
        {
            // 'GetAllAsync' repository mock
            _mockAnonymousCartRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<AnonymousCart, bool>>>()))
                .Returns((Expression<Func<AnonymousCart, bool>> predicate) =>
                    Task.FromResult(anonymousCarts.Where(predicate.Compile()).ToList() as IEnumerable<AnonymousCart>
                ));

            //'GetAllAsync' repository mock
            _mockAnonymousCartRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<AnonymousCart, bool>>>(),
                It.IsAny<Func<IQueryable<AnonymousCart>, IQueryable<AnonymousCart>>>()))
                .Returns((
                Expression<Func<AnonymousCart, bool>> predicate, Func<IQueryable<AnonymousCart>, IQueryable<AnonymousCart>> include) =>
                Task.FromResult(anonymousCarts.Where(predicate.Compile()).ToList() as IEnumerable<AnonymousCart>
                ));

            // 'Update' repository mock
            _mockAnonymousCartRepository.Setup(x => x.Update(It.IsAny<AnonymousCart>())).Returns(It.IsAny<EntityState>());

            // 'Add' repository mock
            _mockAnonymousCartRepository.Setup(x => x.Add(It.IsAny<AnonymousCart>())).Returns(EntityState.Added);

            // 'FindByAsync' repository mock
            _mockAnonymousCartRepository.Setup(x => x.FindByAsync(It.IsAny<Expression<Func<AnonymousCart, bool>>>()))
                .Returns((Expression<Func<AnonymousCart, bool>> predicate) =>
                    Task.FromResult(anonymousCarts.Where(predicate.Compile()).FirstOrDefault()
                ));

            return this;
        }

        /// <summary>
        /// With the unit of work setup.
        /// </summary>
        /// <returns>Service builder with Unit Of Work mockup</returns>
        public AnonymousCartServiceBuilder WithUnitOfWorkSetup()
        {
            _mockUnitOfWork.Setup(x => x.CommitAsync()).ReturnsAsync(1);
            _mockUnitOfWork.Setup(x => x.GetRepository<AnonymousCart>()).Returns(_mockAnonymousCartRepository.Object);
            _mockUnitOfWork.Setup(x => x.GetRepository<Product>()).Returns(_mockProductRepository.Object);
            return this;
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns>Instance of Company Service with mocked instances of unit of work</returns>
        public IAnonymousCartService Build()
        {
            return new AnonymousCartService(
                _mockUnitOfWork.Object, _mapper);
        }
    }
}
