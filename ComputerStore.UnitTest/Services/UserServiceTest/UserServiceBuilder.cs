//-----------------------------------------------------------------------
// <copyright file="UserServiceBuilder.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>DungBD5</author>
//-----------------------------------------------------------------------

using AutoMapper;
using ComputerStore.Api.Mappings;
using ComputerStore.BoundedContext.Entities;
using ComputerStore.Domain.Implement;
using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Extensions;
using ComputerStore.Structure.Models.Pagination;
using ComputerStore.UnitOfWork.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ComputerStore.UnitTest.Services.UserServiceTest
{
    public class UserServiceBuilder
    {
        private readonly Mock<IRepository<User>> _mockRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mapper _mapper;
        public UserServiceBuilder()
        {
            var mockRepositoryObject = new MockRepository(MockBehavior.Strict);
            _mockRepository = mockRepositoryObject.Create<IRepository<User>>();
            _mockUnitOfWork = mockRepositoryObject.Create<IUnitOfWork>();
            var mapperConfiguration = new MapperConfiguration(new MappingProfile());
            _mapper = new Mapper(mapperConfiguration);
        }
        /// <summary>
        /// With the repository setup
        /// </summary>
        /// <param name="users"></param>
        /// <param name="user"></param>
        /// <param name="pagingContext"></param>
        /// <returns>Service builder with EF core repository mockup</returns>
        public UserServiceBuilder WithRepositoryMock(List<User> users, User user, PagingContext pagingContext)
        {
            // 'GetAllAsync' repository mock
            _mockRepository.Setup(o => o.GetAllAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns((
                    Expression<Func<User, bool>> predicate) =>
                         Task.FromResult(users.Where(predicate.Compile()).ToList()
                                as IEnumerable<User>));
            // 'GetAsync' repository mock

            foreach (var item in users)
            {
                _mockRepository.Setup(x => x.GetAsync(item.Id)).ReturnsAsync(() => users.FirstOrDefault(x => x.Id == item.Id));
            }
            _mockRepository.Setup(x => x.GetAsync(10)).ReturnsAsync(() => null);

            //'FindByAsync'

            _mockRepository.Setup(x => x.FindByAsync(It.IsAny<Expression<Func<User,bool>>>(), It.IsAny<string>())).Returns(
                (Expression<Func<User,bool>> predicate,string include)=>
                Task.FromResult(users.Where(predicate.Compile()).FirstOrDefault()));
            _mockRepository.Setup(x => x.FindByAsync(It.IsAny<Expression<Func<User, bool>>>())).Returns(
               (Expression<Func<User, bool>> predicate) =>
               Task.FromResult(users.Where(predicate.Compile()).FirstOrDefault()));
            // 'Update' repository mock
            _mockRepository.Setup(x => x.Update(It.IsAny<User>())).Returns(It.IsAny<EntityState>());

            // 'Add' repository mock
            _mockRepository.Setup(x => x.Add(It.IsAny<User>())).Returns(EntityState.Added);

            //'GetAllAsync' repository mock with paging
            var pageSize = (pagingContext.PageNumber - 1) * pagingContext.NumberPerPage;
            _mockRepository.Setup(o => o.GetAllAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<PagingContext>()))
                .Returns((
                    Expression<Func<User, bool>> predicate, PagingContext paging) =>
                         Task.FromResult(users.Where(predicate.Compile())
                            .AsQueryable().Sort(pagingContext.SortColums, pagingContext.SortDirection)
                                .Skip(pageSize).Take(pagingContext.NumberPerPage) as IEnumerable<User>));
            //'CountAsync' repository mock
            _mockRepository.Setup(o => o.CountAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns((
                    Expression<Func<User, bool>> predicate) =>
                        Task.FromResult(users.Count(predicate.Compile())));

            //'ExistsAsync' repository mock
            _mockRepository.Setup(o => o.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns((
                        Expression<Func<User, bool>> predicate) =>
                    Task.FromResult(users.Where(predicate.Compile()).Any()));

            return this;
        }
        /// <summary>
        /// With the unit of work setup.
        /// </summary>
        /// <returns>Service builder with Unit Of Work mockup</returns>
        public UserServiceBuilder WithUnitOfWorkSetup()
        {
            _mockUnitOfWork.Setup(x => x.CommitAsync()).ReturnsAsync(1);
            _mockUnitOfWork.Setup(x => x.GetRepository<User>()).Returns(_mockRepository.Object);
            return this;
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns>Instance of User Service with mocked instances of unit of work</returns>
        public IUserService Build()
        {
            return new UserService(
                _mockUnitOfWork.Object, _mapper);
        }
    }
}
