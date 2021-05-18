//-----------------------------------------------------------------------
// <copyright file="AnonymoustCartServiceTest.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using AutoMapper;
using ComputerStore.Api.Mappings;
using ComputerStore.BoundedContext.Entities;
using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Exceptions;
using ComputerStore.Structure.Models.Cart;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using static ComputerStore.Structure.Constants.Constants;

namespace ComputerStore.UnitTest.Services.AnonymousCartServiceTest
{
    [TestFixture]
    public class AnonymoustCartServiceTest
    {
        private List<AnonymousCart> anonymousCarts;
        private List<Product> products;
        private Mapper mapper;
        private AnonymousCartServiceBuilder anonymousCartServiceBuilder;
        private IAnonymousCartService anonymousCartService;
        private string identityCode;

        [SetUp]
        public void SetUp()
        {
            var mapperConfiguration = new MapperConfiguration(new MappingProfile());
            mapper = new Mapper(mapperConfiguration);

            #region Seed data
            products = new List<Product>()
            {
                new Product()
                {
                    Id = 1,
                    WebsiteId = 1,
                    CategoryId = 2,
                    Name = "Product Sub Category 1",
                    ProductCode = "Product01",
                    Quantity = 5
                },
                new Product()
                {
                    Id = 2,
                    WebsiteId = 1,
                    CategoryId = 2,
                    Name = "Product Sub Category 2",
                    ProductCode = "Product02",
                    Quantity = 10
                }
                ,
                new Product()
                {
                    Id = 3,
                    WebsiteId = 1,
                    CategoryId = 2,
                    Name = "Product Sub Category 2",
                    ProductCode = "Product03",
                    Quantity = 10
                }
            };

            identityCode = Guid.NewGuid().ToString();

            anonymousCarts = new List<AnonymousCart>
            {
                new AnonymousCart
                {
                    Id = 1,
                    WebsiteId = 1,
                    IdentityCode = identityCode,
                    ProductId = 1,
                    Quantity = 1
                },
                new AnonymousCart
                {
                    Id = 2,
                    WebsiteId = 1,
                    IdentityCode = identityCode,
                    ProductId = 2,
                    Quantity = 2
                }
            };
            #endregion

            //Build company service with unit of work
            anonymousCartServiceBuilder = new AnonymousCartServiceBuilder()
                .WithAnonymousCartRepositoryMock(anonymousCarts)
                .WithProductRepositoryMock(products)
                .WithUnitOfWorkSetup();
            anonymousCartService = anonymousCartServiceBuilder.Build();
        }

        [Test]
        public void TestCreateAsync_WithExistedCartHasSameIdentityCode_ShouldUpdateQuantity()
        {
            var cartCreateModel = new AnonymousCartCreateModel
            {
                IdentityCode = Guid.Parse(identityCode),
                ProductId = 1,
                Quantity = 5
            };

            Assert.DoesNotThrowAsync(() => anonymousCartService.CreateAsync(1, cartCreateModel));

            var existedCart = anonymousCarts.FirstOrDefault(x => x.ProductId == cartCreateModel.ProductId);
            Assert.AreEqual(6, existedCart.Quantity);
        }

        [Test]
        public void TestCreateAsync_WithNotFoundProduct_ShouldThrowNotFoundException()
        {
            var cartCreateModel = new AnonymousCartCreateModel
            {
                IdentityCode = Guid.Parse(identityCode),
                ProductId = 4,
                Quantity = 5
            };

            var ex = Assert.ThrowsAsync<NotFoundException>(() => anonymousCartService.CreateAsync(1, cartCreateModel));

            Assert.AreEqual(string.Format(MessageResponse.NotFoundError, nameof(Product),
                cartCreateModel.ProductId), ex.Message);
        }

        [Test]
        public void TestCreateAsync_WithValidData_ShouldNotThrowAnyException()
        {
            var cartCreateModel = new AnonymousCartCreateModel
            {
                IdentityCode = Guid.Parse(identityCode),
                ProductId = 3,
                Quantity = 5
            };

            Assert.DoesNotThrowAsync(() => anonymousCartService.CreateAsync(1, cartCreateModel));
        }

        [Test]
        public void TestUpdateAsync_WithNotFoundCart_ShouldThrowNotFoundException()
        {
            var cartModel = new AnonymousCartModel
            {
                Id = 3,
                IdentityCode = Guid.Parse(identityCode),
                WebsiteId = 1,
                ProductId = 1,
                Quantity = 1
            };

            var ex = Assert.ThrowsAsync<NotFoundException>(() => anonymousCartService.UpdateAsync(1, cartModel));
            Assert.AreEqual(string.Format(MessageResponse.NotFoundError, nameof(AnonymousCart),
                cartModel.Id), ex.Message);
        }

        [Test]
        public void TestUpdateAsync_WithExistedCart_ShouldUpdateQuantity()
        {
            var cartModel = new AnonymousCartModel
            {
                Id = 1,
                IdentityCode = Guid.Parse(identityCode),
                WebsiteId = 1,
                ProductId = 1,
                Quantity = 10
            };

            anonymousCartService.UpdateAsync(1, cartModel).GetAwaiter().GetResult();
            var actualQuantity = anonymousCarts.First(x => x.Id == 1).Quantity;
            Assert.AreEqual(cartModel.Quantity, actualQuantity);
        }

        [Test]
        public void TestDeleteAsync_WithNotFoundCart_ShouldThrowNotFoundException()
        {
            //Delete cart with id is 5
            var ex = Assert.ThrowsAsync<NotFoundException>(() => anonymousCartService.DeleteAsync(1, 5, identityCode));
            Assert.AreEqual(string.Format(MessageResponse.NotFoundError, nameof(AnonymousCart),
                5), ex.Message);
        }

        [Test]
        public void TestDeleteAsync_WithExistedCart_ShouldSetDeletedDateIsCurrentDate()
        {
            //Delete cart with id is 1
            anonymousCartService.DeleteAsync(1, 1, identityCode).GetAwaiter().GetResult();
            var actual = anonymousCarts.First(x => x.Id == 1).DeletedDate;
            Assert.NotNull(actual);
            Assert.AreEqual(DateTime.UtcNow.ToString("HH-mm-ss"), actual.Value.ToString("HH-mm-ss"));
        }

        [Test]
        public void TestEmptyAsync_ShouldDeleteAllCartOfUser()
        {
            anonymousCartService.EmptyAsync(1, identityCode).GetAwaiter().GetResult();
            var actual = anonymousCarts.Where(x => x.IdentityCode == identityCode).ToList();
            foreach (var cart in actual)
            {
                Assert.NotNull(cart.DeletedDate);
                Assert.AreEqual(DateTime.UtcNow.ToString("HH-mm-ss"), cart.DeletedDate.Value.ToString("HH-mm-ss"));
            }
        }

        [Test]
        public void TestGetAsync_ShouldReturnShoppingCart()
        {
            var actual = anonymousCartService.GetAsync(1, identityCode).GetAwaiter().GetResult();
            var expectDataJson = JsonConvert.SerializeObject(
               mapper.Map<List<CartItemModel>>(anonymousCarts.Where(x => x.WebsiteId == 1 && x.IdentityCode == identityCode)));
            var actualDataJson = JsonConvert.SerializeObject(actual.Items);

            Assert.AreEqual(2, actual.Items.Count);
            Assert.AreEqual(2, actual.TotalItems);
            Assert.AreEqual(expectDataJson, actualDataJson);
            Assert.AreEqual(actual.Items.Sum(x => x.Price * x.Quantity * (1 - x.Discount / 100)), actual.TotalPrice);
        }
    }
}

