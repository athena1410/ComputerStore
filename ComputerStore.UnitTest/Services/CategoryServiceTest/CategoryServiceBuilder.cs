//-----------------------------------------------------------------------
// <copyright file="CategoryServiceBuilder.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>BinhHTV</author>
//-----------------------------------------------------------------------

using AutoMapper;
using ComputerStore.Api.Mappings;
using ComputerStore.BoundedContext.Entities;
using ComputerStore.Domain.Implement;
using ComputerStore.Domain.Interfaces;
using ComputerStore.UnitOfWork.Interfaces;
using Moq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System;
using System.Threading.Tasks;
using System.Linq;
using ComputerStore.Structure.Models.Pagination;
using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Category;
using ComputerStore.Structure.Extensions;

namespace ComputerStore.UnitTest.Services.CategoryServiceTest
{
    public class CategoryServiceBuilder
    {
        private readonly Mock<IRepository<Category>> _mockRepository;
        private readonly Mock<IRepository<Product>> _mockRepositoryProduct;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mapper _mapper;

        public CategoryServiceBuilder()
        {
            var mockRepositoryObject = new MockRepository(MockBehavior.Strict);
            _mockRepository = mockRepositoryObject.Create<IRepository<Category>>();
            _mockRepositoryProduct = mockRepositoryObject.Create<IRepository<Product>>();

            _mockUnitOfWork = mockRepositoryObject.Create<IUnitOfWork>();

            var mapperConfiguration = new MapperConfiguration(new MappingProfile());
            _mapper = new Mapper(mapperConfiguration);
        }

        /// <summary>
        /// With the repository setup.
        /// </summary>
        /// <returns>Service builder with EF core repository mockup</returns>
        public CategoryServiceBuilder WithRepositoryMock(List<Category> categories, List<Product> products, PagingContext pagingContext)
        {
            //'GetAllAsync' repository mock
            _mockRepository.Setup(o => o.GetAllAsync(It.IsAny<Expression<Func<Category, bool>>>()))
                .Returns((
                    Expression<Func<Category, bool>> predicate) => 
                         Task.FromResult(categories.Where(predicate.Compile()).ToList() 
                                as IEnumerable<Category>));


            //'GetAllAsync' repository mock with paging
            var pageSize = (pagingContext.PageNumber - 1) * pagingContext.NumberPerPage;
            _mockRepository.Setup(o => o.GetAllAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<PagingContext>()))
                .Returns((
                    Expression<Func<Category, bool>> predicate, PagingContext paging) =>
                         Task.FromResult(categories.Where(predicate.Compile())
                            .AsQueryable().Sort(pagingContext.SortColums, pagingContext.SortDirection)
                                .Skip(pageSize).Take(pagingContext.NumberPerPage) as IEnumerable<Category>));


            //'CountAsync' repository mock
            _mockRepository.Setup(o => o.CountAsync(It.IsAny<Expression<Func<Category, bool>>>()))
                .Returns((
                    Expression<Func<Category, bool>> Predicate) =>
                        Task.FromResult(categories.Count(Predicate.Compile())));

            // 'GetAsync' repository mock
            foreach (var item in categories)
            {
                _mockRepository.Setup(x => x.GetAsync(item.Id)).ReturnsAsync(() => categories.FirstOrDefault(x=>x.Id == item.Id));
            }
            _mockRepository.Setup(x => x.GetAsync(10)).ReturnsAsync(() => null);

            //'FindByAsync' respository mock
            _mockRepository.Setup(o => o.FindByAsync(It.IsAny<Expression<Func<Category, bool>>>()))
                .Returns((
                    Expression<Func<Category, bool>> predicate) =>
                        Task.FromResult(categories.Where(predicate.Compile()).SingleOrDefault()));

            // 'Update' repository mock
            _mockRepository.Setup(x => x.Update(It.IsAny<Category>())).Returns(It.IsAny<EntityState>());

            // 'Add' repository mock
            _mockRepository.Setup(x => x.Add(It.IsAny<Category>())).Returns(EntityState.Added);

            //'ExistsAsync' respository mock
            _mockRepository.Setup(o => o.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>()))
                .Returns((
                    Expression<Func<Category, bool>> predicate) =>
                        Task.FromResult(categories.Where(predicate.Compile()).Any()));

            //'ExistsAsync' respository product mock
            _mockRepositoryProduct.Setup(o => o.ExistsAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .Returns((
                    Expression<Func<Product, bool>> predicate) =>
                        Task.FromResult(products.Where(predicate.Compile()).Any()));

            return this;
        }

        /// <summary>
        /// With the unit of work setup.
        /// </summary>
        /// <returns>Service builder with Unit Of Work mockup</returns>
        public CategoryServiceBuilder WithUnitOfWorkSetup()
        {
            _mockUnitOfWork.Setup(x => x.CommitAsync()).ReturnsAsync(1);
            _mockUnitOfWork.Setup(x => x.GetRepository<Category>()).Returns(_mockRepository.Object);
            _mockUnitOfWork.Setup(x => x.GetRepository<Product>()).Returns(_mockRepositoryProduct.Object);
            return this;
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns>Instance of Company Service with mocked instances of unit of work</returns>
        public ICategoryService Build()
        {
            return new CategoryService(
                _mockUnitOfWork.Object, _mapper);
        }
    }
}
