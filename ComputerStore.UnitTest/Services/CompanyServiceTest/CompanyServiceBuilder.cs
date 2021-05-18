//-----------------------------------------------------------------------
// <copyright file="CompanyServiceBuilder.cs" company="Young">
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
using Moq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;
using ComputerStore.Structure.Models.Pagination;
using ComputerStore.Structure.Extensions;

namespace ComputerStore.UnitTest.Services.CompanyServiceTest
{
    public class CompanyServiceBuilder
    {
        private readonly Mock<IRepository<Company>> _mockRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mapper _mapper;

        public CompanyServiceBuilder()
        {
            var mockRepositoryObject = new MockRepository(MockBehavior.Strict);
            _mockRepository = mockRepositoryObject.Create<IRepository<Company>>();
            _mockUnitOfWork = mockRepositoryObject.Create<IUnitOfWork>();

            var mapperConfiguration = new MapperConfiguration(new MappingProfile());
            _mapper = new Mapper(mapperConfiguration);
        }

        /// <summary>
        /// With the repository setup.
        /// </summary>
        /// <returns>Service builder with EF core repository mockup</returns>
        public CompanyServiceBuilder WithRepositoryMock(List<Company> companies, Company company, PagingContext pagingContext)
        {
            // 'GetAllAsync' repository mock
            _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(() => companies);
            // 'GetAsync' repository mock
            _mockRepository.Setup(x => x.GetAsync(1, x => x.Website)).ReturnsAsync(() => company);
            _mockRepository.Setup(x => x.GetAsync(1)).ReturnsAsync(() => company);
            _mockRepository.Setup(x => x.GetAsync(10, x => x.Website)).ReturnsAsync(() => null);
            _mockRepository.Setup(x => x.GetAsync(10)).ReturnsAsync(() => null);
            // 'Update' repository mock
            _mockRepository.Setup(x => x.Update(It.IsAny<Company>())).Returns(It.IsAny<EntityState>());

            // 'Add' repository mock
            _mockRepository.Setup(x => x.Add(It.IsAny<Company>())).Returns(EntityState.Added);

            //'GetAllAsync' repository mock with paging
            var pageSize = (pagingContext.PageNumber - 1) * pagingContext.NumberPerPage;
            _mockRepository.Setup(o => o.GetAllAsync(It.IsAny<Expression<Func<Company, bool>>>(), It.IsAny<PagingContext>()))
                .Returns((
                    Expression<Func<Company, bool>> predicate, PagingContext paging) =>
                         Task.FromResult(companies.Where(predicate.Compile())
                            .AsQueryable().Sort(pagingContext.SortColums, pagingContext.SortDirection)
                                .Skip(pageSize).Take(pagingContext.NumberPerPage) as IEnumerable<Company>));

            //'CountAsync' repository mock
            _mockRepository.Setup(o => o.CountAsync(It.IsAny<Expression<Func<Company, bool>>>()))
                .Returns((
                    Expression<Func<Company, bool>> predicate) =>
                        Task.FromResult(companies.Count(predicate.Compile())));

            //'ExistsAsync' repository mock
            _mockRepository.Setup(o => o.ExistsAsync(It.IsAny<Expression<Func<Company, bool>>>()))
                .Returns((
                        Expression<Func<Company, bool>> predicate) =>
                    Task.FromResult(companies.Where(predicate.Compile()).Any()));

            return this;
        }

        /// <summary>
        /// With the unit of work setup.
        /// </summary>
        /// <returns>Service builder with Unit Of Work mockup</returns>
        public CompanyServiceBuilder WithUnitOfWorkSetup()
        {
            _mockUnitOfWork.Setup(x => x.CommitAsync()).ReturnsAsync(1);
            _mockUnitOfWork.Setup(x => x.GetRepository<Company>()).Returns(_mockRepository.Object);
            return this;
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns>Instance of Company Service with mocked instances of unit of work</returns>
        public ICompanyService Build()
        {
            return new CompanyService(
                _mockUnitOfWork.Object, _mapper);
        }
    }
}
