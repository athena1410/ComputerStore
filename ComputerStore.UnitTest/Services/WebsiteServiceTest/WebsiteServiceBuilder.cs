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

namespace ComputerStore.UnitTest.Services.WebsiteServiceTest
{
    public class WebsiteServiceBuilder
    {
        private readonly Mock<IRepository<Website>> _mockRepositoryWebsite;
        private readonly Mock<IRepository<Company>> _mockRepositoryCompany;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mapper _mapper;

        public WebsiteServiceBuilder()
        {
            var mockRepositoryObject = new MockRepository(MockBehavior.Strict);
            _mockRepositoryWebsite = mockRepositoryObject.Create<IRepository<Website>>();
            _mockRepositoryCompany = mockRepositoryObject.Create<IRepository<Company>>();

            _mockUnitOfWork = mockRepositoryObject.Create<IUnitOfWork>();

            var mapperConfiguration = new MapperConfiguration(new MappingProfile());
            _mapper = new Mapper(mapperConfiguration);
        }

        /// <summary>
        /// With the repository setup.
        /// </summary>
        /// <returns>Service builder with EF core repository mockup</returns>
        public WebsiteServiceBuilder WithRepositoryMock(List<Website> websites, List<Company> companies, PagingContext pagingContext)
        {
            //'GetAllAsync' repository mock
            _mockRepositoryWebsite.Setup(o => o.GetAllAsync(It.IsAny<Expression<Func<Website, bool>>>()))
                .Returns((
                    Expression<Func<Website, bool>> predicate) =>
                         Task.FromResult(websites.Where(predicate.Compile()).ToList()
                                as IEnumerable<Website>));

            //'FindByAsync' repository mock
            _mockRepositoryCompany.Setup(x => x.FindByAsync(It.IsAny<Expression<Func<Company, bool>>>(), It.IsAny<string>()))
                .Returns((
                Expression<Func<Company, bool>> predicate, string include) =>
                Task.FromResult(companies.Where(predicate.Compile()).FirstOrDefault()
                ));

            //'GetAllAsync' repository mock with paging
            var pageSize = (pagingContext.PageNumber - 1) * pagingContext.NumberPerPage;
            _mockRepositoryWebsite.Setup(o => o.GetAllAsync(It.IsAny<Expression<Func<Website, bool>>>(), It.IsAny<PagingContext>(), It.IsAny<string[]>()))
                .Returns((
                    Expression<Func<Website, bool>> predicate, PagingContext paging, string[] includes) =>
                         Task.FromResult(websites.Where(predicate.Compile())
                            .AsQueryable().Sort(pagingContext.SortColums, pagingContext.SortDirection)
                                .Skip(pageSize).Take(pagingContext.NumberPerPage) as IEnumerable<Website>));

            //'CountAsync' repository mock
            _mockRepositoryWebsite.Setup(o => o.CountAsync(It.IsAny<Expression<Func<Website, bool>>>()))
                .Returns((
                    Expression<Func<Website, bool>> Predicate) =>
                        Task.FromResult(websites.Count(Predicate.Compile())));

            // 'GetAsync' repository mock
            _mockRepositoryWebsite.Setup(x => x.GetAsync(10)).ReturnsAsync(() => null);
            foreach (var item in websites)
            {
                _mockRepositoryWebsite.Setup(x => x.GetAsync(item.Id)).ReturnsAsync(() => websites.FirstOrDefault(x => x.Id == item.Id));
            }
            _mockRepositoryCompany.Setup(x => x.GetAsync(10)).ReturnsAsync(() => null);
            foreach (var item in companies)
            {
                _mockRepositoryCompany.Setup(x => x.GetAsync(item.Id)).ReturnsAsync(() => companies.FirstOrDefault(x => x.Id == item.Id));
            }

            // 'Update' repository mock
            _mockRepositoryWebsite.Setup(x => x.Update(It.IsAny<Website>())).Returns(It.IsAny<EntityState>());

            // 'Add' repository mock
            _mockRepositoryWebsite.Setup(x => x.Add(It.IsAny<Website>())).Returns(EntityState.Added);

            //'ExistsAsync' repository mock
            _mockRepositoryWebsite.Setup(o => o.ExistsAsync(It.IsAny<Expression<Func<Website, bool>>>()))
                .Returns((
                        Expression<Func<Website, bool>> predicate) =>
                    Task.FromResult(websites.Where(predicate.Compile()).Any()));

            return this;
        }

        /// <summary>
        /// With the unit of work setup.
        /// </summary>
        /// <returns>Service builder with Unit Of Work mockup</returns>
        public WebsiteServiceBuilder WithUnitOfWorkSetup()
        {
            _mockUnitOfWork.Setup(x => x.CommitAsync()).ReturnsAsync(1);
            _mockUnitOfWork.Setup(x => x.GetRepository<Website>()).Returns(_mockRepositoryWebsite.Object);
            _mockUnitOfWork.Setup(x => x.GetRepository<Company>()).Returns(_mockRepositoryCompany.Object);
            return this;
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns>Instance of Website Service with mocked instances of unit of work</returns>
        public IWebsiteService Build()
        {
            return new WebsiteService(
                _mockUnitOfWork.Object, _mapper);
        }
    }
}
