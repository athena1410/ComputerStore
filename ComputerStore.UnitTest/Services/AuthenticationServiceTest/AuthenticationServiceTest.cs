//-----------------------------------------------------------------------
// <copyright file="AuthenticationServiceTest.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------
using ComputerStore.BoundedContext.Entities;
using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Enums;
using ComputerStore.Structure.Helper;
using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Authentication;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using static ComputerStore.Structure.Constants.Constants;

namespace ComputerStore.UnitTest.Services.AuthenticationServiceTest
{
    [TestFixture]
    public class AuthenticationServiceTest
    {
        private IAuthenticationService authenticationService;
        private List<User> users;
        private Website website;
        private JwtSettings _jwtSetting;
        private string ipAddress;

        [SetUp]
        public void SetUp()
        {
            #region Seed data for unit test
            _jwtSetting = new JwtSettings
            {
                SecretKey = "this is my custom Secret key for authentication super admin",
                SuperAdminTimeExpires = 5,
                AdministratorTimeExpires = 10,
                UserTimeExpires = 15,
                RefreshTokenExpires = 7
            };

            users = new List<User>()
            {
                new User
                {
                    Id = 1,
                    Email = "sa@abc.com",
                    Password = Security.CreateHashPassword("sa@abc.com", "123456"),
                    RoleId = 1,
                    WebsiteId = null,
                    FirstName = "Administrator",
                    LastName = "",
                    Address = "Hà Nội",
                    Phone = "088666332",
                    Website = null,
                    RefreshToken = new List<RefreshToken>
                    {
                        new RefreshToken
                        {
                            Id = 1,
                            UserId = 1,
                            CreatedByIp = "127.0.0.1",
                            CreatedDate = DateTime.UtcNow,
                            Expires = DateTime.UtcNow.AddDays(_jwtSetting.RefreshTokenExpires),
                            Token = "tNpDTYIigKfJZ7Xd8QCa6uCWRKz0CHuVWZgExIXyy3FVpaGi6WQYyQ9k66uE/NA0LQyuNK8h1MClgMk7PWGT/w=="
                        }
                    }
                },
                new User
                {
                    Id = 2,
                    Email = "admin@gmail.com",
                    Password = Security.CreateHashPassword("admin@gmail.com", "123456"),
                    RoleId = 2,
                    WebsiteId = 1,
                    FirstName = "Fshop",
                    LastName = "Admin",
                    Address = "17 Duy Tân, Cầu Giấy, Hà Nội",
                    Phone = "0123456789",
                    Website = website,
                    RefreshToken = new List<RefreshToken>
                    {
                        new RefreshToken
                        {
                            Id = 2,
                            UserId = 2,
                            CreatedByIp = "127.0.0.1",
                            CreatedDate = DateTime.UtcNow,
                            Expires = DateTime.UtcNow.AddDays(_jwtSetting.RefreshTokenExpires),
                            Token = "4fDxFtzAl1tIMfuBfFlrdNSz8SYZNhdFJWCt9yej6NtFZqfR4pmbQW9h/DzmliWGYzLcbPn65+RNasUdep4vLg=="
                        }
                    }
                },
                new User
                {
                    Id = 3,
                    Email = "user@gmail.com",
                    Password = Security.CreateHashPassword("user@gmail.com", "123456"),
                    RoleId = 3,
                    WebsiteId = 1,
                    FirstName = "User",
                    LastName = "Fsoft",
                    Address = "19 Duy Tân, Cầu Giấy, Hà Nội",
                    Phone = "0987654321",
                    Website = website,
                    RefreshToken = new List<RefreshToken>
                    {
                        new RefreshToken
                        {
                            Id = 3,
                            UserId = 3,
                            CreatedByIp = "127.0.0.1",
                            CreatedDate = DateTime.UtcNow,
                            Expires = DateTime.UtcNow.AddDays(_jwtSetting.RefreshTokenExpires),
                            Token = "5kDjFmzAl1tIMfuBfFlrdNSz8SYZNhdFJWCt9yej6NtFZqfR4pmbQW9h/DzmliWGYzLcbPn65+RNasUdep4vLg=="
                        }
                    }
                }
            };

            website = new Website
            {
                Id = 1,
                CompanyId = 1,
                Name = "Fsoft Website",
                UrlPath = "fsoft",
                SecretKey = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.UtcNow
            };

            ipAddress = "127.0.0.1";
            #endregion

            //Build authentication service with unit of work
            authenticationService = new AuthenticationServiceBuilder()
                .WithUserRepositoryMock(users)
                .WithWebsiteRepositoryMock(website)
                .WithUnitOfWorkSetup()
                .Build();
        }

        [TestCase]
        public void TestGenerateJwtToken_GenerateTokenForSuperAdmin()
        {
            var user = users.FirstOrDefault(x => x.WebsiteId == null && 
                        x.RoleId == (int)Structure.Enums.Role.SuperAdmin);
            var actual = this.authenticationService.GenerateJwtToken(user, null);
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(actual) as JwtSecurityToken;
            var mail = jsonToken.Claims.SingleOrDefault(c => c.Type == "email")?.Value;
            var userId = jsonToken.Claims.SingleOrDefault(c => c.Type == "nameid")?.Value;
            var websiteId = jsonToken.Claims.SingleOrDefault(c => c.Type.Contains("uri"))?.Value;
            var role = jsonToken.Claims.SingleOrDefault(c => c.Type == "role")?.Value;
            var kid = jsonToken.Header.LastOrDefault().Value;
            var expires = jsonToken.ValidTo;
            var expiresExpect = DateTime.UtcNow.AddMinutes(this._jwtSetting.SuperAdminTimeExpires).ToString("yyyy-MM-dd HH:mm");
            Assert.AreEqual(user.Email, mail);
            Assert.AreEqual(user.Id.ToString(), userId);
            Assert.AreEqual(((Structure.Enums.Role)user.Id).ToString(), role);
            Assert.AreEqual(string.Empty, websiteId);
            Assert.AreEqual(null, kid);
            Assert.AreEqual(expiresExpect, expires.ToString("yyyy-MM-dd HH:mm"));
        }

        [TestCase]
        public void TestGenerateJwtToken_GenerateTokenForAdministrator()
        {
            var user = users.FirstOrDefault(x => x.WebsiteId == website.Id &&
                                x.RoleId == (int)Structure.Enums.Role.Administrator);

            var actual = this.authenticationService.GenerateJwtToken(user, website);
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(actual) as JwtSecurityToken;
            var mail = jsonToken.Claims.SingleOrDefault(c => c.Type == "email")?.Value;
            var userId = jsonToken.Claims.SingleOrDefault(c => c.Type == "nameid")?.Value;
            var websiteId = jsonToken.Claims.SingleOrDefault(c => c.Type.Contains("uri"))?.Value;
            var role = jsonToken.Claims.SingleOrDefault(c => c.Type == "role")?.Value;
            var kid = jsonToken.Header.LastOrDefault().Value;
            var expires = jsonToken.ValidTo;
            var expiresExpect = DateTime.UtcNow.AddMinutes(this._jwtSetting.AdministratorTimeExpires).ToString("yyyy-MM-dd HH:mm");
            Assert.AreEqual(user.Email, mail);
            Assert.AreEqual(user.Id.ToString(), userId);
            Assert.AreEqual(((Structure.Enums.Role)user.Id).ToString(), role);
            Assert.AreEqual(website.Id.ToString(), websiteId);
            Assert.AreEqual(website.Id.ToString(), kid);
            Assert.AreEqual(expiresExpect, expires.ToString("yyyy-MM-dd HH:mm"));
        }

        [TestCase]
        public void TestGenerateJwtToken_GenerateTokenForUser()
        {
            var user = users.FirstOrDefault(x => x.WebsiteId == website.Id &&
                                x.RoleId == (int)Structure.Enums.Role.User);

            var actual = this.authenticationService.GenerateJwtToken(user, website);
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(actual) as JwtSecurityToken;
            var mail = jsonToken.Claims.SingleOrDefault(c => c.Type == "email")?.Value;
            var userId = jsonToken.Claims.SingleOrDefault(c => c.Type == "nameid")?.Value;
            var websiteId = jsonToken.Claims.SingleOrDefault(c => c.Type.Contains("uri"))?.Value;
            var role = jsonToken.Claims.SingleOrDefault(c => c.Type == "role")?.Value;
            var kid = jsonToken.Header.LastOrDefault().Value;
            var expires = jsonToken.ValidTo;
            var expiresExpect = DateTime.UtcNow.AddMinutes(this._jwtSetting.UserTimeExpires).ToString("yyyy-MM-dd HH:mm");
            Assert.AreEqual(user.Email, mail);
            Assert.AreEqual(user.Id.ToString(), userId);
            Assert.AreEqual(((Structure.Enums.Role)user.Id).ToString(), role);
            Assert.AreEqual(website.Id.ToString(), websiteId);
            Assert.AreEqual(website.Id.ToString(), kid);
            Assert.AreEqual(expiresExpect, expires.ToString("yyyy-MM-dd HH:mm"));
        }

        [TestCase]
        public void TestGenerateRefreshToken_ShouldReturnNotNull()
        {
            var actual = authenticationService.GenerateRefreshToken(ipAddress);
            Assert.IsInstanceOf(typeof(RefreshToken), actual);
            Assert.AreEqual(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm"), actual.CreatedDate.ToString("yyyy-MM-dd HH:mm"));
            Assert.AreEqual(DateTime.UtcNow.AddDays(_jwtSetting.RefreshTokenExpires).ToString("yyyy-MM-dd HH:mm"), actual.Expires.ToString("yyyy-MM-dd HH:mm"));
            Assert.AreEqual(ipAddress, actual.CreatedByIp);
            var check = string.IsNullOrEmpty(actual.Token);
            Assert.IsFalse(check);
        }

        [TestCase]
        public void TestAuthenticate_ForSuperAdminWithValidData_ShouldReturnAuthenticateResponse()
        {
            var user = users.FirstOrDefault(x => x.WebsiteId == null && 
                            x.RoleId == (int)Structure.Enums.Role.SuperAdmin);
            var authenticateRequest = new AuthenticateRequest
            {
                Email = "sa@abc.com",
                Password = "123456"
            };
            int? websiteId = null;

            var actual = authenticationService.Authenticate(authenticateRequest, websiteId, ipAddress)
                            .GetAwaiter().GetResult();

            Assert.AreEqual(user.Id, actual.Id);
            Assert.AreEqual(user.FirstName, actual.FirstName);
            Assert.AreEqual(user.LastName, actual.LastName);
            Assert.NotNull(actual.RefreshToken);
            Assert.AreEqual(nameof(Structure.Enums.Role.SuperAdmin), actual.Role);
            Assert.AreEqual(authenticationService.GenerateJwtToken(user, null), actual.Token);
        }

        [TestCase]
        public void TestAuthenticate_ForSuperAdminWithNotExistedUser_ShouldReturnNull()
        {
            var user = users.FirstOrDefault(x => x.WebsiteId == null);
            var authenticateRequest = new AuthenticateRequest
            {
                Email = "sa123@abc.com",
                Password = "123456"
            };
            int? websiteId = null;

            var actual = authenticationService.Authenticate(authenticateRequest, websiteId, ipAddress)
                            .GetAwaiter().GetResult();

            Assert.IsNull(actual);
        }

        [TestCase]
        public void TestAuthenticate_ForSuperAdminWithNotValidPassword_ShouldReturnNull()
        {
            var user = users.FirstOrDefault(x => x.WebsiteId == null);
            var authenticateRequest = new AuthenticateRequest
            {
                Email = "sa@abc.com",
                Password = "12345678"
            };
            int? websiteId = null;

            var actual = authenticationService.Authenticate(authenticateRequest, websiteId, ipAddress)
                            .GetAwaiter().GetResult();

            Assert.IsNull(actual);
        }

        [TestCase]
        public void TestAuthenticate_ForAdministratorWithValidData_ShouldReturnAuthenticateResponse()
        {
            var user = users.FirstOrDefault(x => x.WebsiteId == website.Id &&
                            x.RoleId == (int)Structure.Enums.Role.Administrator);
            var authenticateRequest = new AuthenticateRequest
            {
                Email = "admin@gmail.com",
                Password = "123456"
            };

            var actual = authenticationService.Authenticate(authenticateRequest, website.Id, ipAddress)
                            .GetAwaiter().GetResult();

            Assert.AreEqual(user.Id, actual.Id);
            Assert.AreEqual(user.FirstName, actual.FirstName);
            Assert.AreEqual(user.LastName, actual.LastName);
            Assert.NotNull(actual.RefreshToken);
            Assert.AreEqual(nameof(Structure.Enums.Role.Administrator), actual.Role);
        }

        [TestCase]
        public void TestAuthenticate_ForAdministratorWithNotExistedUser_ShouldReturnNull()
        {
            var user = users.FirstOrDefault(x => x.WebsiteId == website.Id &&
                            x.RoleId == (int)Structure.Enums.Role.Administrator);
            var authenticateRequest = new AuthenticateRequest
            {
                Email = "admin123@gmail.com",
                Password = "123456"
            };

            var actual = authenticationService.Authenticate(authenticateRequest, website.Id, ipAddress)
                            .GetAwaiter().GetResult();

            Assert.IsNull(actual);
        }

        [TestCase]
        public void TestAuthenticate_ForAdministratorWithNotValidPassword_ShouldReturnNull()
        {
            var user = users.FirstOrDefault(x => x.WebsiteId == website.Id &&
                            x.RoleId == (int)Structure.Enums.Role.Administrator);
            var authenticateRequest = new AuthenticateRequest
            {
                Email = "admin@gmail.com",
                Password = "12345678"
            };

            var actual = authenticationService.Authenticate(authenticateRequest, website.Id, ipAddress)
                            .GetAwaiter().GetResult();

            Assert.IsNull(actual);
        }

        [TestCase]
        public void TestAuthenticate_ForAdministratorWithInvalidWebsite_ShouldThrowValidationException()
        {
            this.website.Status = (int)Status.DEACTIVATE;
            var user = users.FirstOrDefault(x => x.WebsiteId == website.Id &&
                            x.RoleId == (int)Structure.Enums.Role.Administrator);
            var authenticateRequest = new AuthenticateRequest
            {
                Email = "admin@gmail.com",
                Password = "123456"
            };

            var actual = Assert.ThrowsAsync<ValidationException>(() => authenticationService.Authenticate(authenticateRequest, website.Id, ipAddress));
            Assert.AreEqual(MessageResponse.WebsiteNotValid, actual.Message);
        }

        [TestCase]
        public void TestAuthenticate_ForUserWithValidData_ShouldReturnAuthenticateResponse()
        {
            var user = users.FirstOrDefault(x => x.WebsiteId == website.Id &&
                            x.RoleId == (int)Structure.Enums.Role.User);
            var authenticateRequest = new AuthenticateRequest
            {
                Email = "user@gmail.com",
                Password = "123456"
            };

            var actual = authenticationService.Authenticate(authenticateRequest, website.Id, ipAddress)
                            .GetAwaiter().GetResult();

            Assert.AreEqual(user.Id, actual.Id);
            Assert.AreEqual(user.FirstName, actual.FirstName);
            Assert.AreEqual(user.LastName, actual.LastName);
            Assert.NotNull(actual.RefreshToken);
            Assert.AreEqual(nameof(Structure.Enums.Role.User), actual.Role);
            Assert.AreEqual(authenticationService.GenerateJwtToken(user, website), actual.Token);
        }

        [TestCase]
        public void TestAuthenticate_ForUserWithNotExistedUser_ShouldReturnNull()
        {
            var user = users.FirstOrDefault(x => x.WebsiteId == website.Id &&
                            x.RoleId == (int)Structure.Enums.Role.User);
            var authenticateRequest = new AuthenticateRequest
            {
                Email = "user123@gmail.com",
                Password = "123456"
            };

            var actual = authenticationService.Authenticate(authenticateRequest, website.Id, ipAddress)
                            .GetAwaiter().GetResult();

            Assert.IsNull(actual);
        }

        [TestCase]
        public void TestAuthenticate_ForUserWithNotValidPassword_ShouldReturnNull()
        {
            var user = users.FirstOrDefault(x => x.WebsiteId == website.Id &&
                            x.RoleId == (int)Structure.Enums.Role.User);
            var authenticateRequest = new AuthenticateRequest
            {
                Email = "user@gmail.com",
                Password = "12345678"
            };

            var actual = authenticationService.Authenticate(authenticateRequest, website.Id, ipAddress)
                            .GetAwaiter().GetResult();

            Assert.IsNull(actual);
        }

        [TestCase]
        public void TestAuthenticate_ForUserWithInvalidWebsite_ShouldThrowValidationException()
        {
            this.website.Status = (int)Status.DEACTIVATE;
            var user = users.FirstOrDefault(x => x.WebsiteId == website.Id &&
                            x.RoleId == (int)Structure.Enums.Role.User);
            var authenticateRequest = new AuthenticateRequest
            {
                Email = "user@gmail.com",
                Password = "123456"
            };

            var actual = Assert.ThrowsAsync<ValidationException>(() => authenticationService.Authenticate(authenticateRequest, website.Id, ipAddress));
            Assert.AreEqual(MessageResponse.WebsiteNotValid, actual.Message);
        }

        [TestCase]
        public void TestRefreshToken_ForSuperAdminWithValidToken_ShouldReturnAuthenticateResponse()
        {
            var user = users.FirstOrDefault(x => x.WebsiteId == null &&
                        x.RoleId == (int)Structure.Enums.Role.SuperAdmin);
            var refreshToken = user.RefreshToken.FirstOrDefault().Token;
            var actual = authenticationService.RefreshToken(refreshToken, ipAddress).GetAwaiter().GetResult();

            //verify 
            Assert.AreEqual(user.Id, actual.Id);
            Assert.AreEqual(user.FirstName, actual.FirstName);
            Assert.AreEqual(user.LastName, actual.LastName);
            Assert.NotNull(actual.RefreshToken);
            Assert.AreEqual(nameof(Structure.Enums.Role.SuperAdmin), actual.Role);
            Assert.AreEqual(authenticationService.GenerateJwtToken(user, website), actual.Token);
            var check = string.IsNullOrEmpty(actual.RefreshToken);
            Assert.IsFalse(check);
        }

        [TestCase]
        public void TestRefreshToken_ForSuperAdminWithNoUserFoundWithToken_ShouldReturnNull()
        {
            var user = users.FirstOrDefault(x => x.WebsiteId == null &&
                        x.RoleId == (int)Structure.Enums.Role.SuperAdmin);
            var refreshToken = "superadminrefreshtokeninvalid";

            var actual = authenticationService.RefreshToken(refreshToken, ipAddress).GetAwaiter().GetResult();
            Assert.Null(actual);
        }

        [TestCase]
        public void TestRefreshToken_ForSuperAdminWithExpiresRefreshToken_ShouldReturnNull()
        {
            var user = users.FirstOrDefault(x => x.WebsiteId == null &&
                        x.RoleId == (int)Structure.Enums.Role.SuperAdmin);
            user.RefreshToken.FirstOrDefault().Expires = DateTime.UtcNow.AddDays(-1);
            var refreshToken = user.RefreshToken.FirstOrDefault().Token;

            var actual = authenticationService.RefreshToken(refreshToken, ipAddress).GetAwaiter().GetResult();
            Assert.Null(actual);
        }

        [TestCase]
        public void TestRefreshToken_ForAdministratorWithValidToken_ShouldReturnAuthenticateResponse()
        {
            var user = users.FirstOrDefault(x => x.WebsiteId == website.Id &&
                        x.RoleId == (int)Structure.Enums.Role.Administrator);
            var refreshToken = user.RefreshToken.FirstOrDefault().Token;
            var ipAddress = "127.0.0.1";
            var actual = authenticationService.RefreshToken(refreshToken, ipAddress).GetAwaiter().GetResult();

            //verify 
            Assert.AreEqual(user.Id, actual.Id);
            Assert.AreEqual(user.FirstName, actual.FirstName);
            Assert.AreEqual(user.LastName, actual.LastName);
            Assert.NotNull(actual.RefreshToken);
            Assert.AreEqual(nameof(Structure.Enums.Role.Administrator), actual.Role);
            Assert.AreEqual(authenticationService.GenerateJwtToken(user, website), actual.Token);
            var check = string.IsNullOrEmpty(actual.RefreshToken);
            Assert.IsFalse(check);
        }

        [TestCase]
        public void TestRefreshToken_ForAdministratorWithNoUserFoundWithToken_ShouldReturnNull()
        {
            var user = users.FirstOrDefault(x => x.WebsiteId == website.Id &&
                        x.RoleId == (int)Structure.Enums.Role.Administrator);
            var refreshToken = "administratorrefreshtokeninvalid";
            var ipAddress = "127.0.0.1";
            var actual = authenticationService.RefreshToken(refreshToken, ipAddress).GetAwaiter().GetResult();
            Assert.Null(actual);
        }

        [TestCase]
        public void TestRefreshToken_ForAdministratorWithExpiresRefreshToken_ShouldReturnNull()
        {
            var user = users.FirstOrDefault(x => x.WebsiteId == website.Id &&
                        x.RoleId == (int)Structure.Enums.Role.Administrator);
            user.RefreshToken.FirstOrDefault().Expires = DateTime.UtcNow.AddDays(-1);
            var refreshToken = user.RefreshToken.FirstOrDefault().Token;

            var actual = authenticationService.RefreshToken(refreshToken, ipAddress).GetAwaiter().GetResult();
            Assert.Null(actual);
        }

        [TestCase]
        public void TestRefreshToken_ForAdministratorWithInvalidWebsite_ShouldThrowValidationException()
        {
            var user = users.FirstOrDefault(x => x.WebsiteId == website.Id &&
                        x.RoleId == (int)Structure.Enums.Role.Administrator);
            website.Status = (int)Status.DEACTIVATE;
            var refreshToken = user.RefreshToken.FirstOrDefault().Token;

            var actual = Assert.ThrowsAsync<ValidationException>(() => authenticationService.RefreshToken(refreshToken, ipAddress));
            Assert.AreEqual(MessageResponse.WebsiteNotValid, actual.Message);
        }

        [TestCase]
        public void TestRefreshToken_ForUserWithValidToken_ShouldReturnAuthenticateResponse()
        {
            var user = users.FirstOrDefault(x => x.WebsiteId == website.Id &&
                        x.RoleId == (int)Structure.Enums.Role.User);
            var refreshToken = user.RefreshToken.FirstOrDefault().Token;

            var actual = authenticationService.RefreshToken(refreshToken, ipAddress).GetAwaiter().GetResult();

            //verify 
            Assert.AreEqual(user.Id, actual.Id);
            Assert.AreEqual(user.FirstName, actual.FirstName);
            Assert.AreEqual(user.LastName, actual.LastName);
            Assert.NotNull(actual.RefreshToken);
            Assert.AreEqual(nameof(Structure.Enums.Role.User), actual.Role);
            Assert.AreEqual(authenticationService.GenerateJwtToken(user, website), actual.Token);
            var check = string.IsNullOrEmpty(actual.RefreshToken);
            Assert.IsFalse(check);
        }

        [TestCase]
        public void TestRefreshToken_ForUserWithNoUserFoundWithToken_ShouldReturnNull()
        {
            var user = users.FirstOrDefault(x => x.WebsiteId == website.Id &&
                        x.RoleId == (int)Structure.Enums.Role.User);
            var refreshToken = "userrefreshtokeninvalid";

            var actual = authenticationService.RefreshToken(refreshToken, ipAddress).GetAwaiter().GetResult();
            Assert.Null(actual);
        }

        [TestCase]
        public void TestRefreshToken_ForUserWithExpiresRefreshToken_ShouldReturnNull()
        {
            var user = users.FirstOrDefault(x => x.WebsiteId == website.Id &&
                        x.RoleId == (int)Structure.Enums.Role.User);
            user.RefreshToken.FirstOrDefault().Expires = DateTime.UtcNow.AddDays(-1);
            var refreshToken = user.RefreshToken.FirstOrDefault().Token;

            var actual = authenticationService.RefreshToken(refreshToken, ipAddress).GetAwaiter().GetResult();
            Assert.Null(actual);
        }

        [TestCase]
        public void TestRefreshToken_ForUserWithInvalidWebsite_ShouldThrowValidationException()
        {
            var user = users.FirstOrDefault(x => x.WebsiteId == website.Id &&
                        x.RoleId == (int)Structure.Enums.Role.User);
            website.Status = (int)Status.DEACTIVATE;
            var refreshToken = user.RefreshToken.FirstOrDefault().Token;

            var actual = Assert.ThrowsAsync<ValidationException>(() => authenticationService.RefreshToken(refreshToken, ipAddress));
            Assert.AreEqual(MessageResponse.WebsiteNotValid, actual.Message);
        }

        [TestCase]
        public void TestRevokeToken_WithValidToken_ShouldReturnTrue()
        {
            var user = users.FirstOrDefault(x => x.WebsiteId == website.Id &&
                        x.RoleId == (int)Structure.Enums.Role.User);

            var token = user.RefreshToken.FirstOrDefault().Token;

            var actual = authenticationService.RevokeToken(token, ipAddress).GetAwaiter().GetResult();
            Assert.True(actual);
        }

        [TestCase]
        public void TestRevokeToken_WithNoUserFoundWithToken_ShouldReturnFalse()
        {
            var user = users.FirstOrDefault(x => x.WebsiteId == website.Id &&
                        x.RoleId == (int)Structure.Enums.Role.User);

            var token = "userrefreshtokeninvalid";

            var actual = authenticationService.RevokeToken(token, ipAddress).GetAwaiter().GetResult();
            Assert.IsFalse(actual);
        }

        [TestCase]
        public void TestRevokeToken_WithExpiresToken_ShouldReturnFalse()
        {
            var user = users.FirstOrDefault(x => x.WebsiteId == website.Id &&
                        x.RoleId == (int)Structure.Enums.Role.User);
            user.RefreshToken.FirstOrDefault().Expires = DateTime.UtcNow.AddDays(-1);

            var token = user.RefreshToken.FirstOrDefault().Token;

            var actual = authenticationService.RevokeToken(token, ipAddress).GetAwaiter().GetResult();
            Assert.IsFalse(actual);
        }
    }
}