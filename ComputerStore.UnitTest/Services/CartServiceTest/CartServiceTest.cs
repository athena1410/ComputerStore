//-----------------------------------------------------------------------
// <copyright file="CartServiceTest.cs" company="Young">
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

namespace ComputerStore.UnitTest.Services.CartServiceTest
{
    [TestFixture]
    public class CartServiceTest
    {
        private ICartService cartService;
        private List<Cart> carts;
        private List<AnonymousCart> anonymousCarts;
        private List<Product> products;
        private Mapper mapper;
        private CartServiceBuilder cartServiceBuilder;
        private string identityCode;

        [SetUp]
        public void SetUp()
        {
            var mapperConfiguration = new MapperConfiguration(new MappingProfile());
            mapper = new Mapper(mapperConfiguration);

            #region Seed data
            carts = new List<Cart>
            {
                new Cart
                {
                    Id = 1,
                    UserId = 1,
                    WebsiteId = 1,
                    ProductId = 1,
                    Quantity = 1
                },
                new Cart
                {
                    Id = 2,
                    UserId = 1,
                    WebsiteId = 1,
                    ProductId = 2,
                    Quantity = 5
                }
            };

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
                    IdentityCode = identityCode,
                    ProductId = 1,
                    Quantity = 2
                }
            };
            #endregion

            //Build company service with unit of work
            cartServiceBuilder = new CartServiceBuilder()
                .WithCartRepositoryMock(carts)
                .WithAnonymousCartRepositoryMock(anonymousCarts)
                .WithProductRepositoryMock(products)
                .WithUnitOfWorkSetup();
            cartService = cartServiceBuilder.Build();
        }

        [Test]
        public void TestCreateAsync_WithExistedCartHasSameProduct_ShouldUpdateQuantity()
        {
            var cartCreateModel = new CartCreateModel
            {
                ProductId = 1,
                Quantity = 5
            };

            Assert.DoesNotThrowAsync(() => cartService.CreateAsync(1, 1, cartCreateModel));

            var existedCart = carts.FirstOrDefault(x => x.ProductId == cartCreateModel.ProductId);
            Assert.AreEqual(6, existedCart.Quantity);
        }

        [Test]
        public void TestCreateAsync_WithNotFoundProduct_ShouldThrowNotFoundException()
        {
            var cartCreateModel = new CartCreateModel
            {
                ProductId = 5,
                Quantity = 5
            };

            var ex = Assert.ThrowsAsync<NotFoundException>(() => cartService.CreateAsync(1, 1, cartCreateModel));

            Assert.AreEqual(string.Format(MessageResponse.NotFoundError, nameof(Product),
                cartCreateModel.ProductId), ex.Message);
        }

        [Test]
        public void TestCreateAsync_WithValidData_ShouldNotThrowAnyException()
        {
            var cartCreateModel = new CartCreateModel
            {
                ProductId = 3,
                Quantity = 5
            };

            Assert.DoesNotThrowAsync(() => cartService.CreateAsync(1, 1, cartCreateModel));
        }

        [Test]
        public void TestUpdateAsync_WithNotFoundCart_ShouldThrowNotFoundException()
        {
            var cartModel = new CartModel
            {
                Id = 3,
                UserId = 1,
                WebsiteId = 1,
                ProductId = 1,
                Quantity = 1
            };

            var ex = Assert.ThrowsAsync<NotFoundException>(() => cartService.UpdateAsync(1, cartModel));
            Assert.AreEqual(string.Format(MessageResponse.NotFoundError, nameof(Cart),
                cartModel.Id), ex.Message);
        }

        [Test]
        public void TestUpdateAsync_WithExistedCart_ShouldUpdateQuantity()
        {
            var cartModel = new CartModel
            {
                Id = 1,
                ProductId = 1,
                Quantity = 5
            };
            cartService.UpdateAsync(1, cartModel).GetAwaiter().GetResult();
            var actualQuantity = carts.First(x => x.Id == 1).Quantity;
            Assert.AreEqual(cartModel.Quantity, actualQuantity);
        }

        [Test]
        public void TestDeleteAsync_WithNotFoundCart_ShouldThrowNotFoundException()
        {
            //Delete cart with id is 5
            var ex = Assert.ThrowsAsync<NotFoundException>(() => cartService.DeleteAsync(1, 5));
            Assert.AreEqual(string.Format(MessageResponse.NotFoundError, nameof(Cart),
                5), ex.Message);
        }

        [Test]
        public void TestDeleteAsync_WithExistedCart_ShouldSetDeletedDateIsCurrentDate()
        {
            //Delete cart with id is 1
            cartService.DeleteAsync(1, 1).GetAwaiter().GetResult();
            var actual = carts.First(x => x.Id == 1).DeletedDate;
            Assert.NotNull(actual);
            Assert.AreEqual(DateTime.UtcNow.ToString("HH-mm-ss"), actual.Value.ToString("HH-mm-ss"));
        }

        [Test]
        public void TestEmptyAsync_ShouldDeleteAllCartOfUser()
        {
            cartService.EmptyAsync(1).GetAwaiter().GetResult();
            var actual = carts.Where(x => x.UserId == 1).ToList();
            foreach (var cart in actual)
            {
                Assert.NotNull(cart.DeletedDate);
                Assert.AreEqual(DateTime.UtcNow.ToString("HH-mm-ss"), cart.DeletedDate.Value.ToString("HH-mm-ss"));
            }
        }

        [Test]
        public void TestMergeAsync_WithExistedCartHasSameProduct_ShouldUpdateCartQuantity()
        {
            var anonymousCartQuantity = anonymousCarts.First().Quantity;
            var originalQuantity = carts.First(x => x.UserId == 1 && x.ProductId == 1).Quantity;
            cartService.MergeAsync(1, 1, identityCode).GetAwaiter().GetResult();
            
            Assert.AreEqual(anonymousCartQuantity + originalQuantity, carts.First().Quantity);
            Assert.IsNotNull(anonymousCarts.First().DeletedDate);
        }

        [Test]
        public void TestMergeAsync_WithNotExistedCartHasSameProduct_ShouldCreateNewCart()
        {
            anonymousCarts.Add(new AnonymousCart
            {
                Id = 2,
                WebsiteId = 1,
                ProductId = 3,
                Quantity = 3,
                IdentityCode = this.identityCode
            });

            cartService.MergeAsync(1, 1, identityCode).GetAwaiter().GetResult();
            var actual = carts.Where(x => x.UserId == 1 && x.WebsiteId == 1).ToList();
            Assert.IsNotNull(anonymousCarts.Last().DeletedDate);
        }

        [Test]
        public void TestGetAsync_ShouldReturnShoppingCart()
        {
            var actual = cartService.GetAsync(1).GetAwaiter().GetResult();
            var expectDataJson = JsonConvert.SerializeObject(
               mapper.Map<List<CartItemModel>>(carts.Where(x => x.WebsiteId == 1 && x.UserId == 1)));
            var actualDataJson = JsonConvert.SerializeObject(actual.Items);

            Assert.AreEqual(2, actual.Items.Count);
            Assert.AreEqual(2, actual.TotalItems);
            Assert.AreEqual(expectDataJson, actualDataJson);
            Assert.AreEqual(actual.Items.Sum(x => x.Price * x.Quantity * (1 - x.Discount / 100)), actual.TotalPrice);
        }
    }
}
