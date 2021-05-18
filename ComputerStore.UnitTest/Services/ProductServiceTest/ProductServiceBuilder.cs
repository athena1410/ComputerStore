using AutoMapper;
using ComputerStore.Api.Mappings;
using ComputerStore.BoundedContext.Entities;
using ComputerStore.Domain.Implement;
using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Models.Category;
using ComputerStore.Structure.Models.Pagination;
using ComputerStore.UnitOfWork.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ComputerStore.UnitTest.Services.ProductServiceTest
{
	public class ProductServiceBuilder
	{
		private readonly Mock<IRepository<Product>> _mockRepository;
		private readonly Mock<IUnitOfWork> _mockUnitOfWork;
		private readonly Mock<ICategoryService> _mockCategoryService;
		private readonly Mapper _mapper;

		public ProductServiceBuilder()
		{
			var mockRepositoryObject = new MockRepository(MockBehavior.Strict);

			_mockRepository = mockRepositoryObject.Create<IRepository<Product>>();

			_mockCategoryService = mockRepositoryObject.Create<ICategoryService>();
			_mockUnitOfWork = mockRepositoryObject.Create<IUnitOfWork>();

			var mapperConfiguration = new MapperConfiguration(new MappingProfile());
			_mapper = new Mapper(mapperConfiguration);
		}

		/// <summary>
		/// With the repository setup.
		/// </summary>
		/// <returns>Service builder with EF core repository mockup</returns>
		public ProductServiceBuilder WithRepositoryMock(List<Category> categories, List<Product> products, PagingContext pagingContext)
		{
			// [GetAsync] repository mock
			_mockRepository.Setup(x => x.GetAsync(100)).ReturnsAsync(() => null);

			foreach (var item in products)
			{
				_mockRepository.Setup(x => x.GetAsync(item.Id)).ReturnsAsync(() => products.FirstOrDefault(x => x.Id == item.Id));
			}

			// [Update] repository mock
			_mockRepository.Setup(x => x.Update(It.IsAny<Product>())).Returns(It.IsAny<EntityState>());

			// [Add] repository mock
			_mockRepository.Setup(x => x.Add(It.IsAny<Product>())).Returns(EntityState.Added);

			// [GetAllAsync] mock
			_mockRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<Product, bool>>>()))
				.ReturnsAsync(products.Where(o => o.WebsiteId == 1));

			// [FindByAsync] mock
			_mockRepository.Setup(o => o.FindByAsync(It.IsAny<Expression<Func<Product, bool>>>()))
			   .Returns(
				(Expression<Func<Product, bool>> predicate) =>
					   Task.FromResult(products.Where(predicate.Compile()).FirstOrDefault()));

			// [FindByAsync] mock
			_mockRepository.Setup(o => o.FindByAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<string>()))
			   .Returns(
				(Expression<Func<Product, bool>> predicate, string include) =>
					   Task.FromResult(products.Where(predicate.Compile()).FirstOrDefault()));

			// [GetMaxAsync] mock
			_mockRepository.Setup(o => o.GetMaxAsync(It.IsAny<Expression<Func<Product, float>>>()))
				.ReturnsAsync(products.Max(o => o.Price));

			// [GetMinAsync] mock
			_mockRepository.Setup(o => o.GetMinAsync(It.IsAny<Expression<Func<Product, float>>>()))
				.ReturnsAsync(products.Min(o => o.Price));

			// [GetAllAsync]
			_mockRepository.Setup(o => o.GetAllAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<PagingContext>(), It.IsAny<string[]>()))
			   .Returns(
				(Expression<Func<Product, bool>> predicate, PagingContext pagingContext, string[] includes) =>
					   Task.FromResult(products.Where(predicate.Compile())));

			// [CountAsync]
			_mockRepository.Setup(o => o.CountAsync(It.IsAny<Expression<Func<Product, bool>>>()))
			   .Returns(
				(Expression<Func<Product, bool>> predicate) =>
					   Task.FromResult(products.Where(predicate.Compile()).ToList().Count));

			//'ExistsAsync' repository mock
			_mockRepository.Setup(o => o.ExistsAsync(It.IsAny<Expression<Func<Product, bool>>>()))
				.Returns((
						Expression<Func<Product, bool>> predicate) =>
					Task.FromResult(products.Where(predicate.Compile()).Any()));

			return this;
		}

		/// <summary>
		/// With the unit of work setup.
		/// </summary>
		/// <returns>Service builder with Unit Of Work mockup</returns>
		public ProductServiceBuilder WithUnitOfWorkSetup()
		{
			_mockUnitOfWork.Setup(x => x.CommitAsync()).ReturnsAsync(1);
			_mockUnitOfWork.Setup(x => x.GetRepository<Product>()).Returns(_mockRepository.Object);
			return this;
		}

		public ProductServiceBuilder WithCategoryService(List<Category> categories)
		{
			// GetChildrenAsync with has children
			_mockCategoryService.Setup(x => x.GetChildrenAsync(1, 1))
				.ReturnsAsync(categories.Where(o => o.ParentId == 1 && o.WebsiteId == 1)
										.Select(o => _mapper.Map<CategoryModel>(o))
										.ToList());

			// GetChildrenAsync with has'nt children
			_mockCategoryService.Setup(x => x.GetChildrenAsync(1, 2))
				.ReturnsAsync(categories.Where(o => o.ParentId == 2 && o.WebsiteId == 1)
										.Select(o => _mapper.Map<CategoryModel>(o))
										.ToList());

			// GetByIdAsync nothing todo
			_mockCategoryService.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<int>()))
				.Returns(Task.FromResult<CategoryModel>(null));

			return this;
		}

		/// <summary>
		/// Builds this instance.
		/// </summary>
		/// <returns>Instance of Company Service with mocked instances of unit of work</returns>
		public IProductService Build()
		{
			return new ProductService(_mockUnitOfWork.Object, _mapper, _mockCategoryService.Object);
		}
	}
}
