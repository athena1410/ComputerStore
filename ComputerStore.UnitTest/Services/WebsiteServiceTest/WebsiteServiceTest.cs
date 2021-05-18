using AutoMapper;
using ComputerStore.Api.Mappings;
using ComputerStore.BoundedContext.Entities;
using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Constants;
using ComputerStore.Structure.Enums;
using ComputerStore.Structure.Exceptions;
using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Pagination;
using ComputerStore.Structure.Models.Website;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ComputerStore.UnitTest.Services.WebsiteServiceTest
{
    [TestFixture]
    public class WebsiteServiceTest
    {
        private IWebsiteService websiteService;
        private List<Website> websites;
        private List<Company> companies;
        private PagingContext pagingContext;
        private Mapper mapper;

        [SetUp]
        public void SetUp()
        {
            var mapperConfiguration = new MapperConfiguration(new MappingProfile());
            mapper = new Mapper(mapperConfiguration);

            #region Seed data for unit test
            //Build List website
            websites = new List<Website>()
            {
                new Website
                {
                    Id = 1,
                    CompanyId = 1,
                    Name = "Fsoft Website",
                    UrlPath = "fsoft",
                    SecretKey = Guid.NewGuid().ToString(),
                    CreatedDate = DateTime.UtcNow,
                    Status = (int)Status.ACTIVE,
                    LogoUrl = "Logo1"
                },
                new Website
                {
                    Id = 2,
                    CompanyId = 2,
                    Name = "CMC Website",
                    UrlPath = "cmc",
                    SecretKey = Guid.NewGuid().ToString(),
                    CreatedDate = DateTime.UtcNow,
                    Status = (int)Status.ACTIVE,
                    LogoUrl = "Logo2"
                },
                new Website
                {
                    Id = 3,
                    CompanyId = 3,
                    Name = "SamSung Website",
                    UrlPath = "samsung",
                    SecretKey = Guid.NewGuid().ToString(),
                    CreatedDate = DateTime.UtcNow,
                    Status = (int)Status.DEACTIVATE,
                    LogoUrl = "Logo3"
                }
            };

            //build list company
            companies = new List<Company>()
            {
                new Company
                {
                    Id = 1,
                    Name = "Fsoft",
                    CreatedDate = DateTime.UtcNow,
                    Website = new Website
                    {
                        Id = 1,
                        CompanyId = 1,
                        Name = "Fsoft Website",
                        UrlPath = "fsoft",
                        SecretKey = Guid.NewGuid().ToString(),
                        CreatedDate = DateTime.UtcNow,
                        Status = (int)Status.ACTIVE,
                        LogoUrl = "Logo1"
                    }
                },
                new Company
                {
                    Id = 2,
                    Name = "CMC",
                    CreatedDate = DateTime.UtcNow
                },
                new Company
                {
                    Id = 3,
                    Name = "SamSung",
                    CreatedDate = DateTime.UtcNow
                }
            };

            //build pagingcontext
            pagingContext = new PagingContext()
            {
                NumberPerPage = 2,
                PageNumber = 1,
                SortColums = "Id",
                SortDirection = "asc"
            };
            #endregion

            //Build website service with unit of work
            websiteService = new WebsiteServiceBuilder()
                .WithRepositoryMock(websites, companies, pagingContext)
                .WithUnitOfWorkSetup()
                .Build();
        }

        [Test]
        public void TestCreateWebsiteAsync_ShouldNotThrowAnyException()
        {
            var websiteModel = new WebsiteModel()
            {
                CompanyId = 2,
                Name = "Website Create",
                UrlPath = "UrlPath Create",
            };

            Assert.DoesNotThrowAsync(() => websiteService.CreateAsync(websiteModel));
        }

        [Test]
        public void TestCreateWebsiteyAsync_WithNameOfWebsiteExisted_ShouldThrowValidationException()
        {
            var websiteModel = new WebsiteModel()
            {
                CompanyId = 1,
                Name = "SamSung Website",
                UrlPath = "UrlPath Create",
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => websiteService.CreateAsync(websiteModel));
            Assert.AreEqual(Constants.MessageResponse.WebsiteNameExisted, ex.Message);
        }

        [Test]
        public void TestCreateWebsiteyAsync_WithUrlPathOfWebsiteExisted_ShouldThrowValidationException()
        {
            var websiteModel = new WebsiteModel()
            {
                CompanyId = 1,
                Name = "Website Create",
                UrlPath = "fsoft",
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => websiteService.CreateAsync(websiteModel));
            Assert.AreEqual(Constants.MessageResponse.WebsiteUrlPathExisted, ex.Message);
        }

        [Test]
        public void TestCreateWebsiteAsync_WithNotExistedCompany_ShouldThrowNotFoundException()
        {
            var websiteModel = new WebsiteModel()
            {
                CompanyId = 10,
                Name = "Website Create",
                UrlPath = "UrlPath Create",
            };

            var ex = Assert.ThrowsAsync<NotFoundException>(() => websiteService.CreateAsync(websiteModel));
            Assert.AreEqual(string.Format(Constants.MessageResponse.NotFoundError, nameof(Company), websiteModel.CompanyId.ToString()), ex.Message);
        }

        [Test]
        public void TestCreateWebsiteAsync_WithCompanyAlreadyHasWebsite_ShouldThrowValidationException()
        {
            var websiteModel = new WebsiteModel()
            {
                CompanyId = 1,
                Name = "Website Create",
                UrlPath = "UrlPath Create",
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => websiteService.CreateAsync(websiteModel));
            Assert.AreEqual(Constants.MessageResponse.UniqueWebsitePerCompanyError, ex.Message);
        }

        [Test]
        public void TestUpdateWebsiteAsync_WithValidParameters_ShouldNotThrowAnyException()
        {
            var websiteId = 1;
            var websiteModel = new UpdateWebsiteModel()
            {
                CompanyId = 1,
                Name = "Website Update",
                UrlPath = "UrlPath Update",
                Note = "Note Update"
            };

            Assert.DoesNotThrowAsync(() => websiteService.UpdateAsync(websiteId, websiteModel));
        }

        [Test]
        public void TestUpdateWebsiteyAsync_WithNameOfWebsiteExisted_ShouldThrowValidationException()
        {
            var websiteId = 1;
            var websiteModel = new UpdateWebsiteModel()
            {
                CompanyId = 1,
                Name = "SamSung Website",
                UrlPath = "UrlPath Update",
                Note = "Note Update"
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => websiteService.UpdateAsync(websiteId, websiteModel));
            Assert.AreEqual(Constants.MessageResponse.WebsiteNameExisted, ex.Message);
        }

        [Test]
        public void TestUpdateWebsiteyAsync_WithUrlPathOfWebsiteExisted_ShouldThrowValidationException()
        {
            var websiteId = 1;
            var websiteModel = new UpdateWebsiteModel()
            {
                CompanyId = 1,
                Name = "Website Update",
                UrlPath = "cmc",
                Note = "Note Update"
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => websiteService.UpdateAsync(websiteId, websiteModel));
            Assert.AreEqual(Constants.MessageResponse.WebsiteUrlPathExisted, ex.Message);
        }

        [Test]
        public void TestUpdateWebsiteAsync_WithNotExistedCompany_ShouldThrowNotFoundException()
        {
            var websiteId = 1;
            var websiteModel = new UpdateWebsiteModel()
            {
                CompanyId = 10,
                Name = "Website Update",
                UrlPath = "UrlPath Update",
                Note = "Note Update"
            };

            var ex = Assert.ThrowsAsync<NotFoundException>(() => websiteService.UpdateAsync(websiteId, websiteModel));
            Assert.AreEqual(string.Format(Constants.MessageResponse.NotFoundError, nameof(Company), websiteModel.CompanyId.ToString()), ex.Message);
        }

        [Test]
        public void TestUpdateWebsiteAsync_WithCompanyAlreadyHasWebsite_ShouldThrowValidationException()
        {
            var websiteId = 1;
            var websiteModel = new UpdateWebsiteModel()
            {
                CompanyId = 2,
                Name = "Website Update",
                UrlPath = "UrlPath Update",
                Note = "Note Update"
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => websiteService.UpdateAsync(websiteId, websiteModel));
            Assert.AreEqual(Constants.MessageResponse.UniqueWebsitePerCompanyError, ex.Message);
        }

        [Test]
        public void TestUpdateWebsiteAsync_WithNotExistedWebsite_ShouldThrowNotFoundException()
        {
            var websiteId = 10;
            var websiteModel = new UpdateWebsiteModel()
            {
                CompanyId = 1,
                Name = "Website Update",
                UrlPath = "UrlPath Update",
                Note = "Note Update"
            };

            var ex = Assert.ThrowsAsync<NotFoundException>(() => websiteService.UpdateAsync(websiteId, websiteModel));
            Assert.AreEqual(string.Format(Constants.MessageResponse.NotFoundError,
                        nameof(Website), websiteId.ToString()), ex.Message);
        }

        [Test]
        public void TestGetByIdAsync_WithExistedWebsite_ShouldReturnWebiste()
        {
            const int websiteId = 1;
            var actual = websiteService.GetByIdAsync(websiteId).GetAwaiter().GetResult();
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<WebsiteModel>(actual);
            var actualJson = JsonConvert.SerializeObject(actual);
            var expectJson = JsonConvert.SerializeObject(mapper.Map<WebsiteModel>(websites.Find(x=>x.Id == websiteId)));
            Assert.AreEqual(expectJson, actualJson);
        }

        [Test]
        public void TestGetByIdAsync_WithNotFoundWebsite_ShouldReturnNull()
        {
            const int websiteId = 10;
            var actual = websiteService.GetByIdAsync(websiteId).GetAwaiter().GetResult();
            Assert.Null(actual);
        }

        [Test]
        public void TestGetAllAsync_WithActiveStatus_ShouldReturnListWebsitesActive()
        {
            var actual = websiteService.GetAllAsync(Status.ACTIVE).GetAwaiter().GetResult();
            Assert.IsInstanceOf<List<WebsiteModel>>(actual);
            Assert.AreEqual(websites.Where(x=>x.Status == (int)Status.ACTIVE).ToList().Count, actual.Count);
            var actualJson = JsonConvert.SerializeObject(actual);
            var expectJson = JsonConvert.SerializeObject(mapper.Map<List<WebsiteModel>>(websites.Where(x => x.Status == (int)Status.ACTIVE).ToList()));
            Assert.AreEqual(expectJson, actualJson);
        }

        [Test]
        public void TestGetAllAsync_WithDeactiveStatus_ShouldReturnListWebsitesDeactive()
        {
            var actual = websiteService.GetAllAsync(Status.DEACTIVATE).GetAwaiter().GetResult();
            Assert.IsInstanceOf<List<WebsiteModel>>(actual);
            Assert.AreEqual(websites.Where(x => x.Status == (int)Status.DEACTIVATE).ToList().Count, actual.Count);
            var actualJson = JsonConvert.SerializeObject(actual);
            var expectJson = JsonConvert.SerializeObject(mapper.Map<List<WebsiteModel>>(websites.Where(x => x.Status == (int)Status.DEACTIVATE).ToList()));
            Assert.AreEqual(expectJson, actualJson);
        }

        [Test]
        public void TestChangeStatusWebsiteAsync_WithNotValidParameters_ShouldThrowNotFoundException()
        {
            const int websiteId = 10;
            var ex = Assert.ThrowsAsync<NotFoundException>(() => websiteService.ChangeStatusAsync(websiteId, 1));
            Assert.AreEqual(string.Format(Constants.MessageResponse.NotFoundError, nameof(Website), websiteId.ToString()), ex.Message);
        }

        [Test]
        public void TestChangeStatusWebsiteAsync_WithValidParameters_ShouldChangeWebsiteStatus()
        {
            const int websiteId = 1;
            websiteService.ChangeStatusAsync(websiteId, 1);
            Assert.AreEqual((int)Status.DEACTIVATE, websiteService.GetByIdAsync(websiteId).Result.Status);
        }

        [Test]
        public void TestSearchAsync_WithExistedWebsites_ShouldReturnPaginationResponseWithLisWebsites()
        {
            var searchModel = new SearchModel<WebsiteSearchModel>()
            {
                Data = new WebsiteSearchModel()
                {
                    Name = "Fsoft"
                },
                NumberPerPage = pagingContext.NumberPerPage,
                PageNumber = pagingContext.PageNumber,
                SortColums = pagingContext.SortColums,
                SortDirection = pagingContext.SortDirection
            };

            var actual = websiteService.SearchAsync(searchModel).GetAwaiter().GetResult();
            Assert.AreEqual(1, actual.MetaData.TotalPages);
            Assert.AreEqual(1, actual.MetaData.TotalCount);
            Assert.AreEqual(pagingContext.NumberPerPage, actual.MetaData.PageSize);
            Assert.True(!actual.MetaData.HasNext);

            var expectDataJson = JsonConvert.SerializeObject(
                mapper.Map<List<WebsiteModel>>(websites.Where(x => x.Name.Contains(searchModel.Data.Name)).Take(searchModel.NumberPerPage)));
            var actualDataJson = JsonConvert.SerializeObject(actual.Results);
            Assert.AreEqual(expectDataJson, actualDataJson);
        }

        [Test]
        public void TestSearchAsync_WithNotExistedWebsites_ShouldReturnPaginationResponseWithEmptyWebsites()
        {
            var searchModel = new SearchModel<WebsiteSearchModel>()
            {
                Data = new WebsiteSearchModel()
                {
                    Name = "Empty Website"
                },
                NumberPerPage = pagingContext.NumberPerPage,
                PageNumber = pagingContext.PageNumber,
                SortColums = pagingContext.SortColums,
                SortDirection = pagingContext.SortDirection
            };
            var actual = websiteService.SearchAsync(searchModel).GetAwaiter().GetResult();
            Assert.AreEqual(0, actual.MetaData.TotalPages);
            Assert.AreEqual(0, actual.MetaData.TotalCount);
            Assert.AreEqual(pagingContext.NumberPerPage, actual.MetaData.PageSize);
            Assert.IsEmpty(actual.Results);
        }

        [Test]
        public void TestGetLogoUrl_WithNotFoundWebsite_ShouldReturnNull()
        {
            const int websiteId = 10;
            var actual = websiteService.GetLogoUrl(websiteId).GetAwaiter().GetResult();
            Assert.Null(actual);
        }

        [Test]
        public void TestGetLogoUrl_WithExistedWebsite_ShouldReturnLogoUrl()
        {
            const int websiteId = 1;
            var actual = websiteService.GetLogoUrl(websiteId).GetAwaiter().GetResult();
            Assert.AreEqual("Logo1", actual);
        }
    }
}
