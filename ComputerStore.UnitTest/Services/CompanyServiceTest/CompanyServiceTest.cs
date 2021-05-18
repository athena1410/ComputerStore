//-----------------------------------------------------------------------
// <copyright file="CompanyServiceTest.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using AutoMapper;
using ComputerStore.Api.Mappings;
using ComputerStore.BoundedContext.Entities;
using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Constants;
using ComputerStore.Structure.Enums;
using ComputerStore.Structure.Exceptions;
using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Company;
using ComputerStore.Structure.Models.Pagination;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ComputerStore.UnitTest.Services.CompanyServiceTest
{
    [TestFixture]
    public class CompanyServiceTest
    {
        private ICompanyService companyService;
        private Company company;
        private List<Company> companies;
        private Mapper mapper;
        private CompanyServiceBuilder companyServiceBuilder;
        private PagingContext pagingContext;
        [SetUp]
        public void SetUp()
        {
            var mapperConfiguration = new MapperConfiguration(new MappingProfile());
            mapper = new Mapper(mapperConfiguration);

            #region Seed data for unit test
            //Build Company
            company = new Company()
            {
                Id = 1,
                Name = "Company",
                Phone = "0123456781",
                Address = "Ha Noi",
                Status = 0,
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
            companies = new List<Company>()
            {
                new Company()
                {
                    Id = 1,
                    Name = "Company 1",
                    Phone = "0123456781",
                    Address = "Ha Noi",
                    Status = 0,
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
                },
                new Company()
                {
                    Id = 2,
                    Name = "Fsoft Telecom",
                    Phone = "0123456782",
                    Address = "Da Nang"
                },
                new Company()
                {
                    Id = 3,
                    Name = "Fsoft",
                    Phone = "0123456783",
                    Address = "Ho Chi Minh",
                }
            };

            //build paging context
            pagingContext = new PagingContext()
            {
                NumberPerPage = 1,
                PageNumber = 1,
                SortColums = "Id",
                SortDirection = "asc"
            };
            #endregion

            companyServiceBuilder = new CompanyServiceBuilder()
                .WithRepositoryMock(companies, company, pagingContext)
                .WithUnitOfWorkSetup();
            //Build company service with unit of work
            companyService = companyServiceBuilder.Build();
        }

        [Test]
        public void TestCreateCompanyAsync_ShouldNotThrowAnyException()
        {
            var companyModel = new CompanyModel()
            {
                Name = "Company Test",
                Phone = "0906198862",
                Address = "Ha Noi"
            };

            Assert.DoesNotThrowAsync(() => companyService.CreateAsync(companyModel));
        }

        [Test]
        public void TestCreateCompanyAsync_WithNameOfCompanyExisted_ShouldThrowValidationException()
        {
            var companyModel = new CompanyModel()
            {
                Name = "Company 1",
                Phone = "0906198862",
                Address = "Ha Noi"
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => companyService.CreateAsync(companyModel));
            Assert.AreEqual(Constants.MessageResponse.CompanyNameExisted, ex.Message);
        }

        [Test]
        public void TestUpdateCompanyAsync_WithValidParameters_ShouldNotThrowAnyException()
        {
            var companyModel = new CompanyModel()
            {
                Id = 1,
                Name = "Company Test",
                Phone = "0906198863",
                Address = "Ho Chi Minh"
            };

            Assert.DoesNotThrowAsync(() => companyService.UpdateAsync(1, companyModel));
            var actual = companyService.GetByIdAsync(1).GetAwaiter().GetResult();
            Assert.AreEqual(companyModel.Name, actual.Name);
            Assert.AreEqual(companyModel.Phone, actual.Phone);
            Assert.AreEqual(companyModel.Address, actual.Address);
        }

        [Test]
        public void TestUpdateCompanyAsync_WithNotExistedCompany_ShouldThrowNotFoundException()
        {
            var companyModel = new CompanyModel()
            {
                Id = 1,
                Name = "Company Test",
                Phone = "0906198862",
                Address = "Ha Noi"
            };
            const int companyId = 10;
            var ex = Assert.ThrowsAsync<NotFoundException>(() => companyService.UpdateAsync(companyId, companyModel));
            Assert.AreEqual($"Not found Company with id: {companyId}", ex.Message);
        }

        [Test]
        public void TestUpdateCompanyAsync_WithNameOfCompanyExisted_ShouldThrowValidationException()
        {
            var companyModel = new CompanyModel()
            {
                Name = "Fsoft Telecom",
                Phone = "0906198862",
                Address = "Ha Noi"
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => companyService.UpdateAsync(1, companyModel));
            Assert.AreEqual(Constants.MessageResponse.CompanyNameExisted, ex.Message);
        }

        [Test]
        public void TestUpdateCompanyAsync_WithChangedStatus_ShouldChangeRelatedWebsiteStatus()
        {
            var companyModel = new CompanyModel()
            {
                Id = 1,
                Name = "Company 4",
                Phone = "0123456783",
                Address = "Ha Noi",
                Status = 1
            };
            Assert.DoesNotThrowAsync(() => companyService.UpdateAsync(1, companyModel));
            Assert.AreEqual(company.Website.Status, (int)Status.DEACTIVATE);
        }

        [Test]
        public void TestGetByIdAsync_WithExistedCompany_ShouldReturnCompany()
        {
            const int companyId = 1;
            var actual = companyService.GetByIdAsync(companyId).GetAwaiter().GetResult();
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<CompanyModel>(actual);
            var actualJson = JsonConvert.SerializeObject(actual);
            var expectJson = JsonConvert.SerializeObject(mapper.Map<CompanyModel>(company));
            Assert.AreEqual(expectJson, actualJson);
        }

        [Test]
        public void TestGetByIdAsync_WithNotFoundCompany_ShouldReturnNull()
        {
            const int companyId = 10;
            var actual = companyService.GetByIdAsync(companyId).GetAwaiter().GetResult();
            Assert.Null(actual);
        }

        [Test]
        public void TestGetAllAsync_ShouldReturnListCompanies()
        {
            var actual = companyService.GetAllAsync().GetAwaiter().GetResult();
            Assert.IsInstanceOf<List<CompanyModel>>(actual);
            Assert.AreEqual(companies.Count, actual.Count);
            var actualJson = JsonConvert.SerializeObject(actual);
            var expectJson = JsonConvert.SerializeObject(mapper.Map<List<CompanyModel>>(companies));
            Assert.AreEqual(expectJson, actualJson);
        }

        [Test]
        public void TestChangeStatusCompanyAsync_ShouldChangeStatusOfRelatedWebsite()
        {
            Assert.DoesNotThrowAsync(() => companyService.ChangeStatusCompanyAsync(1, 1));
            Assert.AreEqual((int)Status.DEACTIVATE, company.Status);
            Assert.AreEqual((int)Status.DEACTIVATE, company.Website.Status);
        }

        [Test]
        public void TestChangeStatusCompanyAsync_WithNotValidParameters_ShouldThrowNotFoundException()
        {
            const int companyId = 10;
            var ex = Assert.ThrowsAsync<NotFoundException>(() => companyService.ChangeStatusCompanyAsync(companyId, 1));
            Assert.AreEqual(string.Format(Constants.MessageResponse.NotFoundError, nameof(Company), companyId.ToString()), ex.Message);
        }

        [Test]
        public void TestSearchAsync_WithExistedCompanies_ShouldReturnPaginationResponseWithListCompanies()
        {
            var searchModel = new SearchModel<CompanySearchModel>()
            {
                Data = new CompanySearchModel()
                {
                    Name = "Fsoft"
                },
                NumberPerPage = pagingContext.NumberPerPage,
                PageNumber = pagingContext.PageNumber,
                SortColums = pagingContext.SortColums,
                SortDirection = pagingContext.SortDirection
            };

            var actual = companyService.SearchAsync(searchModel).GetAwaiter().GetResult();
            Assert.AreEqual(2, actual.MetaData.TotalPages);
            Assert.AreEqual(2, actual.MetaData.TotalCount);
            Assert.AreEqual(pagingContext.NumberPerPage, actual.MetaData.PageSize);
            Assert.True(actual.MetaData.HasNext);

            var expectDataJson = JsonConvert.SerializeObject(
                mapper.Map<List<CompanyModel>>(companies.Where(x => x.Name.Contains(searchModel.Data.Name)).Take(searchModel.NumberPerPage)));
            var actualDataJson = JsonConvert.SerializeObject(actual.Results);
            Assert.AreEqual(expectDataJson, actualDataJson);
        }   

        [Test]
        public void TestSearchAsync_WithNotExistedCompanies_ShouldReturnPaginationResponseWithEmptyCompanies()
        {
            var searchModel = new SearchModel<CompanySearchModel>()
            {
                Data = new CompanySearchModel()
                {
                    Name = "Empty Company"
                },
                NumberPerPage = pagingContext.NumberPerPage,
                PageNumber = pagingContext.PageNumber,
                SortColums = pagingContext.SortColums,
                SortDirection = pagingContext.SortDirection
            };
            var actual = companyService.SearchAsync(searchModel).GetAwaiter().GetResult();
            Assert.AreEqual(0, actual.MetaData.TotalPages);
            Assert.AreEqual(0, actual.MetaData.TotalCount);
            Assert.AreEqual(pagingContext.NumberPerPage, actual.MetaData.PageSize);
            Assert.IsEmpty(actual.Results);
        }

        [Test]
        public void TestExistedByName_WithExistedCompanyWithName_ShouldReturnTrue()
        {
            const string name = "Company 1";

            var actual = companyService.ExistedByName(name).GetAwaiter().GetResult();
            Assert.True(actual);
        }

        [Test]
        public void TestExistedByName_WithNotExistedCompanyWithName_ShouldReturnFalse()
        {
            const string name = "Company 5";
            var actual = companyService.ExistedByName(name).GetAwaiter().GetResult();
            Assert.False(actual);
        }
    }
}
