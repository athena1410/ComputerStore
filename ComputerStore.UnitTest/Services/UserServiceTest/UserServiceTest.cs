//-----------------------------------------------------------------------
// <copyright file="UserServiceTest.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>DungBD5</author>
//-----------------------------------------------------------------------
using AutoMapper;
using ComputerStore.Api.Mappings;
using ComputerStore.BoundedContext.Entities;
using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Constants;
using ComputerStore.Structure.Exceptions;
using ComputerStore.Structure.Helper;
using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Pagination;
using ComputerStore.Structure.Models.User;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ComputerStore.UnitTest.Services.UserServiceTest
{
    [TestFixture]
    public class UserServiceTest
    {
        private IUserService userService;
        private User user;
        private List<User> users;
        private Mapper mapper;
        private UserServiceBuilder userServiceBuilder;
        private PagingContext pagingContext;

        [SetUp]
        public void Setup()
        {
            var mapperConfiguration = new MapperConfiguration(new MappingProfile());
            mapper = new Mapper(mapperConfiguration);

            #region Seed data for unit test
            //Build User
            user = new User()
            {
                Id = 1,
                Address = "Ha Noi",
                Email = "email@gmail.com",
                FirstName = "Dung",
                LastName = "Bui",
                Phone = "0968285322",
                Status = 0,
                Password = "zxe9I4uWD9TngXQ6OljmHJk1TZrHH6HlsICnFaIARCk=",
                WebsiteId = 1,

                Website = new Website
                {
                    Id = 1,
                    CompanyId = 1,
                    Name = "Fsoft Website",
                    UrlPath = "fsoft",
                    SecretKey = Guid.NewGuid().ToString(),
                    Status = 0,
                    CreatedDate = DateTime.UtcNow
                }
            };

            //Build List Company
            var comanyId = Guid.NewGuid().ToString();
            users = new List<User>()
            {
                new User()
                {
                    Id = 1,
                    Address="Ha Noi",
                    Email="nguyentuan@gmail.com",
                    FirstName="Dung",
                    LastName="Bui",
                    Phone="0968285322",
                    Password="zxe9I4uWD9TngXQ6OljmHJk1TZrHH6HlsICnFaIARCk=",
                    Status = 0,
                    WebsiteId=1,
                    RoleId=3,
                    Website = new Website
                    {
                        Id = 1,
                        CompanyId = 1,
                        Name = "Fsoft Website",
                        UrlPath = "fsoft",
                        SecretKey = comanyId,
                        Status = 0,
                        CreatedDate = DateTime.UtcNow
                    }
                },
                new User()
                {
                    Id = 2,
                    Address="Da Nang",
                    Email="email2@gmail.com",
                    FirstName="Dung",
                    LastName="Bui",
                    Phone="0968285323",
                    Status = 0,
                    RoleId=3,
                    Password="123456",
                    WebsiteId=1,
                    Website = new Website
                    {
                        Id = 1,
                        CompanyId = 1,
                        Name = "Fsoft Website",
                        UrlPath = "fsoft",
                        SecretKey = comanyId,
                        Status = 0,
                        CreatedDate = DateTime.UtcNow
                    }
                },
                new User()
                {
                    WebsiteId=1,
                    Id = 3,
                    RoleId=3,
                    Address="HCM",
                    Email="email3@gmail.com",
                    FirstName="Dung3",
                    LastName="Bui3",
                    Password="123456",
                    Phone="0968285324",
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

            userServiceBuilder = new UserServiceBuilder()
                .WithRepositoryMock(users, user, pagingContext)
                .WithUnitOfWorkSetup();
            //Build user service with unit of work
            userService = userServiceBuilder.Build();
        }
        #region Test GetByEmailAndSite function in UserService

        /// <summary>
        /// Get  user by email and site Id return valid data
        /// </summary>
        [Test]
        public void TestGetByEmailAndSite_ValidInput_ShouldReturnUser()
        {
            var email = "nguyentuan@gmail.com";
            var websiteId = 1;
            var user = userService.GetByEmailAndSite(email, websiteId).GetAwaiter().GetResult();
            Assert.AreEqual(email, user.Email);
            Assert.AreEqual(websiteId, user.WebsiteId);
            //Assert.NotNull(user);
        }
        /// <summary>
        /// Get  user by email and site Id return null
        /// </summary>
        [Test]
        public void TestGetByEmailAndSite_InvalidInput_ShouldReturnNull()
        {
            var email = "hello@gmail.com";
            var websiteId = 1;
            var user = userService.GetByEmailAndSite(email, websiteId).GetAwaiter().GetResult();
            Assert.IsNull(user);
        }
        #endregion

        #region Test CreateUserAsync function in UserService
        /// <summary>
        /// Create user error because email existed
        /// </summary>
        [Test]
        public void TestCreateUserAsync_EmailExisted_ShouldThrowValidationException()
        {
            var userModel = new UserModel()
            {
                Address = "Ha Noi",
                Email = "nguyentuan@gmail.com",
                FirstName = "Dung",
                LastName = "Bui",
                Phone = "0968285322",
                Status = 0,
                WebsiteId = 1
            };
            ValidationException ex = Assert.Throws<ValidationException>(() => userService.CreateAsync(1, Structure.Enums.Role.User, userModel).GetAwaiter().GetResult());
            Assert.AreEqual("User with Email existed in server", ex.Message);
        }
        /// <summary>
        /// Create success user
        /// </summary>
        [Test]
        public void TestCreateUserAsync_ValidInput_ShouldNotThrowAnyException()
        {
            var userModel = new UserModel()
            {
                Address = "Ha Noi",
                Email = "doandung2701@gmail.com",
                FirstName = "Dung",
                LastName = "Bui",
                Phone = "0968285322",
                Status = 0,
                WebsiteId = 1,
                Password = "123456789"
            };
            Assert.DoesNotThrowAsync(() => userService.CreateAsync(1, Structure.Enums.Role.User, userModel));
        }

        #endregion

        #region Test UpdateUserAsync function in UserService

        /// <summary>
        /// Test update user, but user not found
        /// </summary>
        [Test]
        public void TestUpdateUserAsync_UserWithIdNotFound_ShouldThrowNotFoundException()
        {
            var websiteId = 1;
            var userId = 10;
            var userModel = new UserUpdateModel()
            {
                Id = 10,
                Address = "Ha Noi",
                Email = "doandung2701@gmail.com",
                FirstName = "Dung",
                LastName = "Bui",
                Phone = "0968285322",
                Status = 0,
                WebsiteId = 1,
                Password = "123456789"
            };
            NotFoundException ex = Assert.Throws<NotFoundException>(() => userService.UpdateAsync(websiteId, userId, userModel).GetAwaiter().GetResult());
            Assert.AreEqual(string.Format(
                    Constants.MessageResponse.NotFoundError, nameof(User), nameof(userId)), ex.Message);
        }

        /// <summary>
        /// Update success user
        /// </summary>
        [Test]
        public void TestUpdateUserAsync_ShouldNotThrowAnyException()
        {
            var userModel = new UserUpdateModel()
            {
                Id = 1,
                Address = "Ha Noi",
                Email = "doandung2701@gmail.com",
                FirstName = "Dung",
                LastName = "Bui",
                Phone = "0968285322",
                Status = 0,
                WebsiteId = 1,
                Password = null
            };
            Assert.DoesNotThrowAsync(() => userService.UpdateAsync(1, 1, userModel));
        }
        /// <summary>
        ///   Update user with password not null,update success
        /// </summary>
        [Test]
        public void TestUpdateUserAsync_PasswordNotNull_ShouldNotThrowAnyException()
        {
            var userModel = new UserUpdateModel()
            {
                Id = 1,
                Address = "Ha Noi",
                Email = "doandung2701@gmail.com",
                FirstName = "Dung",
                LastName = "Bui",
                Phone = "0968285322",
                Status = 0,
                WebsiteId = 1,
                Password = "123456789123"
            };
            Assert.DoesNotThrowAsync(() => userService.UpdateAsync(1, 1, userModel));
        }
        #endregion

        #region Test GetAllAsync
        /// <summary>
        /// get all user with website has no user
        /// </summary>
        [Test]
        public void TestGetAllAsync_InvalidWebsiteId_ShouldReturnEmptyArray()
        {
            const int websiteId = 2;
            var users = userService.GetAllAsync(websiteId).GetAwaiter().GetResult();
            Assert.IsTrue(users.Count == 0);
        }
        /// <summary>
        /// get all user with website had users
        /// </summary>
        [Test]
        public void TestGetAllAsync_ValidWebsiteId_ShouldReturnExpectedArray()
        {
            const int websiteId = 1;
            //var users = userService.GetAllAsync(websiteId).GetAwaiter().GetResult();
            //Assert.IsTrue(users.Count == 3);
            var actual = userService.GetAllAsync(websiteId).GetAwaiter().GetResult();
            Assert.IsInstanceOf<List<UserModel>>(actual);
            Assert.AreEqual(users.Count, actual.Count);
            var actualJson = JsonConvert.SerializeObject(actual);
            var expectJson = JsonConvert.SerializeObject(mapper.Map<List<UserModel>>(users));
            Assert.AreEqual(expectJson, actualJson);
        }
        #endregion

        #region Test GetByIdAsync
        /// <summary>
        /// Get User By Id but not found
        /// </summary>
        [Test]
        public void TestGetByIdAsync_InvalidUserIdAndWebsiteId_ShouldReturnNull()
        {
            const int userId = 1;
            const int websiteId = 2;
            var user = userService.GetByIdAsync(websiteId, userId).GetAwaiter().GetResult();
            Assert.IsNull(user);
        }
        /// <summary>
        /// get user by id success
        /// </summary>
        [Test]
        public void TestGetByIdAsync_ValidUserIdAndWebsiteId_ShouldReturnExpectedUser()
        {
            const int userId = 1;
            const int websiteId = 1;
            var user = userService.GetByIdAsync(websiteId, userId).GetAwaiter().GetResult();
            Assert.AreEqual(user.Id, userId);
        }
        #endregion

        #region Test SearchAsync
        /// <summary>
        /// Search user by DisplayName, pageSize = 1, numberPerPage = 2
        /// </summary>
        [Test]
        public void TestSearchAsync_ValidData_ExistsUserByName_Page1PerPage2()
        {
            const int websiteId = 1;
            var searchModel = new SearchModel<UserSearchModel>()
            {
                Data = new UserSearchModel()
                {
                    DisplayName = "Dung"
                },
                PageNumber = 1,
                NumberPerPage = 2
            };
            var result = userService.SearchAsync(websiteId, searchModel);
            var users = result.Result.Results;
            Assert.IsNotEmpty(users);
            Assert.AreEqual(users.Count, 2);
            Assert.AreEqual(users[0].Id, 1);
            Assert.AreEqual(users[0].WebsiteId, 1);
            Assert.AreEqual(users[0].Status, 0);

            Assert.AreEqual(users[1].Id, 2);
            Assert.AreEqual(users[1].WebsiteId, 1);
            Assert.AreEqual(users[1].Status, 0);
        }
        #endregion

        #region Change Status Async
        /// <summary>
        /// Change status success
        /// </summary>
        [Test]
        public void TestChangeStatusAsync_ValidInput_StatusChanged()
        {
            const int websiteId = 1;
            const int userId = 1;
            const int status = (int)Structure.Enums.Status.DEACTIVATE;

            var oldUser = users.SingleOrDefault(x => x.Id == userId && x.WebsiteId == 1);
            Assert.AreEqual(oldUser.Status, 0);
            Assert.AreEqual(oldUser.Id, 1);
            Assert.AreEqual(oldUser.WebsiteId, 1);

            Assert.DoesNotThrowAsync(() => userService.ChangeStatusAsync(websiteId, userId, status));

            var userChangedStatus = users.SingleOrDefault(x => x.Id == userId && x.WebsiteId == 1);
            Assert.AreEqual(userChangedStatus.Id, 1);
            Assert.AreEqual(userChangedStatus.WebsiteId, 1);
            Assert.AreEqual(userChangedStatus.Status, 1);
        }
        #endregion

        #region Updatepassword
        /// <summary>
        /// Update password but user not found
        /// </summary>
        [Test]
        public void TestUpdatePasswordAsync_UserIdNotExisted_ShouldThrowNotFoundException()
        {
            var websiteId = 1;
            var userId = 10;
            var userUpdatePasswordModel = new UserUpdatePasswordModel()
            {
                OldPassword = "123456",
                NewPassword = "123456789",
                WebsiteId = 1
            };
            NotFoundException ex = Assert.Throws<NotFoundException>(() => userService.UpdatePassword(websiteId, userId, userUpdatePasswordModel).GetAwaiter().GetResult());

            Assert.AreEqual(
                    string.Format(
                        Constants.MessageResponse.NotFoundError, nameof(User), nameof(userUpdatePasswordModel.Id)),
                    ex.Message);
        }

        /// <summary>
        /// update password but oldpass not match 
        /// </summary>
        [Test]
        public void TestUpdatePasswordAsync_OldPasswordIncorrect_ShouldThrowValidationException()
        {
            var websiteId = 1;
            var userId = 1;
            var userUpdatePasswordModel = new UserUpdatePasswordModel()
            {
                OldPassword = "1234567",
                NewPassword = "123456789",
                WebsiteId = 1
            };
            ValidationException ex = Assert.Throws<ValidationException>(() => userService.UpdatePassword(websiteId, userId, userUpdatePasswordModel).GetAwaiter().GetResult());

            Assert.AreEqual(
                    string.Format(
                Constants.MessageResponse.OldPasswordIncorrect, userUpdatePasswordModel.OldPassword),
                    ex.Message);
        }
        /// <summary>
        /// Update password by new pass is null
        /// </summary>
        [Test]
        public void TestUpdatePasswordAsync_NewPasswordNull_ShouldThrowValidationException()
        {
            var websiteId = 1;
            var userId = 1;
            var userUpdatePasswordModel = new UserUpdatePasswordModel()
            {
                OldPassword = "1234566",
                NewPassword = null,
                WebsiteId = 1
            };
            ValidationException ex = Assert.Throws<ValidationException>(() => userService.UpdatePassword(websiteId, userId, userUpdatePasswordModel).GetAwaiter().GetResult());

            Assert.AreEqual(
                    Constants.MessageResponse.NewPasswordNotMeetExpected,
                    ex.Message);
        }
        /// <summary>
        /// Update password but length of new password not meet expected
        /// </summary>
        [Test]
        public void TestUpdatePasswordAsync_NewPasswordLengthNotMeetExpected_ShouldThrowValidationException()
        {
            var websiteId = 1;
            var userId = 1;
            var userUpdatePasswordModel = new UserUpdatePasswordModel()
            {
                OldPassword = "1234566",
                NewPassword = "123",
                WebsiteId = 1
            };
            ValidationException ex = Assert.Throws<ValidationException>(() => userService.UpdatePassword(websiteId, userId, userUpdatePasswordModel).GetAwaiter().GetResult());

            Assert.AreEqual(
                    Constants.MessageResponse.NewPasswordNotMeetExpected,
                    ex.Message);
        }

        /// <summary>
        /// update password but old pass not match in database
        /// </summary>
        [Test]
        public void TestUpdatePasswordAsync_OldPasswordNotEqual_ShouldThrowValidationException()
        {
            var websiteId = 1;
            var userId = 1;
            var userUpdatePasswordModel = new UserUpdatePasswordModel()
            {
                OldPassword = "1234566",
                NewPassword = "123456789",
                WebsiteId = 1
            };
            ValidationException ex = Assert.Throws<ValidationException>(() => userService.UpdatePassword(websiteId, userId, userUpdatePasswordModel).GetAwaiter().GetResult());

            Assert.AreEqual(
                    Constants.MessageResponse.OldPasswordIncorrect,
                    ex.Message);
        }
        /// <summary>
        /// Update success password
        /// </summary>
        [Test]
        public void TestUpdatePasswordAsync_ValidInput_ShouldNewPasswordChanged()
        {
            var websiteId = 1;
            var userId = 1;
            var userUpdatePasswordModel = new UserUpdatePasswordModel()
            {
                OldPassword = "123456",
                NewPassword = "123456789",
                WebsiteId = 1
            };
            Assert.DoesNotThrowAsync(() => userService.UpdatePassword(websiteId, userId, userUpdatePasswordModel));
            var userUpdated = users.SingleOrDefault(x => x.Id == userId && x.WebsiteId == websiteId);
            var isValid = Security.ValidatePassword(userUpdated.Email, userUpdatePasswordModel.NewPassword, userUpdated.Password);
            Assert.IsTrue(isValid);

        }
        #endregion
    }
}
