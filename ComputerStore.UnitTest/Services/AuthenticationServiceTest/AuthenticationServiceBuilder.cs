//-----------------------------------------------------------------------
// <copyright file="AuthenticationServiceBuilder.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------
using AutoMapper;
using ComputerStore.Api.Mappings;
using ComputerStore.BoundedContext.Entities;
using ComputerStore.Domain.Implement;
using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Models;
using ComputerStore.UnitOfWork.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ComputerStore.UnitTest.Services.AuthenticationServiceTest
{
    public class AuthenticationServiceBuilder
    {
        private readonly Mock<IRepository<User>> _mockUserRepository;
        private readonly Mock<IRepository<Website>> _mockWebsiteRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mapper _mapper;
        private readonly IOptions<JwtSettings> _option;

        public AuthenticationServiceBuilder()
        {
            var mockRepositoryObject = new MockRepository(MockBehavior.Strict);
            _mockUserRepository = mockRepositoryObject.Create<IRepository<User>>();
            _mockWebsiteRepository = mockRepositoryObject.Create<IRepository<Website>>();
            _mockUnitOfWork = mockRepositoryObject.Create<IUnitOfWork>();

            var mapperConfiguration = new MapperConfiguration(new MappingProfile());
            _mapper = new Mapper(mapperConfiguration);
            var jwtSetting = new JwtSettings
            {
                SecretKey = "this is my custom Secret key for authentication super admin",
                SuperAdminTimeExpires = 5,
                AdministratorTimeExpires = 10,
                UserTimeExpires = 15,
                RefreshTokenExpires = 7
            };
            _option = Options.Create(jwtSetting);
        }

        /// <summary>
        /// With the user repository setup.
        /// </summary>
        /// <returns>Service builder with EF core repository mockup</returns>
        public AuthenticationServiceBuilder WithUserRepositoryMock(List<User> users)
        {
            // 'FindbyAsync' repository mock
            _mockUserRepository.Setup(x => x.FindByAsync(It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<string>()))
                .Returns((Expression<Func<User, bool>> predicate, string include) =>
                    Task.FromResult(users.FirstOrDefault(predicate.Compile())));

            // 'Update' repository mock
            _mockUserRepository.Setup(x => x.Update(It.IsAny<User>())).Returns(It.IsAny<EntityState>());

            return this;
        }

        /// <summary>
        /// With the website repository setup.
        /// </summary>
        /// <param name="website"></param>
        /// <returns></returns>
        public AuthenticationServiceBuilder WithWebsiteRepositoryMock(Website website)
        {
            // 'GetAsync' repository mock
            _mockWebsiteRepository.Setup(x => x.GetAsync((int?)1)).ReturnsAsync(() => website);

            return this;
        }

        /// <summary>
        /// With the unit of work setup.
        /// </summary>
        /// <returns>Service builder with Unit Of Work mockup</returns>
        public AuthenticationServiceBuilder WithUnitOfWorkSetup()
        {
            _mockUnitOfWork.Setup(x => x.CommitAsync()).ReturnsAsync(1);
            _mockUnitOfWork.Setup(x => x.GetRepository<User>()).Returns(_mockUserRepository.Object);
            _mockUnitOfWork.Setup(x => x.GetRepository<Website>()).Returns(_mockWebsiteRepository.Object);
            return this;
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns>Instance of Company Service with mocked instances of unit of work</returns>
        public IAuthenticationService Build()
        {
            return new AuthenticationService(
                _mockUnitOfWork.Object, _option, _mapper);
        }
    }
}
