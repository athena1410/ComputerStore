//-----------------------------------------------------------------------
// <copyright file="CartService.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using AutoMapper;
using ComputerStore.BoundedContext.Entities;
using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Enums;
using ComputerStore.Structure.Exceptions;
using ComputerStore.Structure.Models.Cart;
using ComputerStore.UnitOfWork.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ComputerStore.Structure.Constants.Constants;

namespace ComputerStore.Domain.Implement
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public CartService(
            IUnitOfWork unitOfWork,
            IMapper mapper
        )
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        /// <summary>
        /// Get shopping cart
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ShoppingCartModel> GetAsync(int userId)
        {
            var cartRepository = unitOfWork.GetRepository<Cart>();
            var carts = await cartRepository.GetAllAsync(x => !x.DeletedDate.HasValue &&
                                    x.UserId == userId, x => x.Include(x => x.Product).ThenInclude(x => x.ProductImage));
            var cartItems = mapper.Map<List<CartItemModel>>(carts);
            return mapper.Map<ShoppingCartModel>(cartItems);
        }

        /// <summary>
        /// Create new cart
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="userId"></param>
        /// <param name="cartCreateModel"></param>
        /// <returns></returns>
        public async Task CreateAsync(int websiteId, int userId, CartCreateModel cartCreateModel)
        {
            var cartRepository = unitOfWork.GetRepository<Cart>();
            var existedCart = await cartRepository.FindByAsync(x => !x.DeletedDate.HasValue && x.WebsiteId == websiteId &&
                                    x.UserId == userId && x.ProductId == cartCreateModel.ProductId);
            if (existedCart != null)
            {
                existedCart.Quantity += cartCreateModel.Quantity;
                cartRepository.Update(existedCart);
                await unitOfWork.CommitAsync();
                return;
            }

            var productRepository = unitOfWork.GetRepository<Product>();
            var product = await productRepository.FindByAsync(x => x.WebsiteId == websiteId &&
                               x.Status == (int)Status.ACTIVE && x.Id == cartCreateModel.ProductId);
            if (product == null)
            {
                throw new NotFoundException(string.Format(
                    MessageResponse.NotFoundError, nameof(Product), cartCreateModel.ProductId));
            }

            var cart = mapper.Map<Cart>(cartCreateModel);
            cart.WebsiteId = websiteId;
            cart.UserId = userId;
            cart.CreatedDate = DateTime.UtcNow;
            cartRepository.Add(cart);
            await unitOfWork.CommitAsync();
        }

        /// <summary>
        /// Update existed cart
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cartModel"></param>
        /// <returns></returns>
        public async Task UpdateAsync(int userId, CartModel cartModel)
        {
            var cartRepository = unitOfWork.GetRepository<Cart>();
            var cart = await cartRepository.FindByAsync(x => !x.DeletedDate.HasValue &&
                                    x.Id == cartModel.Id && x.UserId == userId);
            if (cart == null)
            {
                throw new NotFoundException(string.Format(
                   MessageResponse.NotFoundError, nameof(Cart), cartModel.Id));
            }

            cart = mapper.Map(cartModel, cart);
            cart.UpdatedDate = DateTime.UtcNow;
            cartRepository.Update(cart);
            await unitOfWork.CommitAsync();
        }

        /// <summary>
        /// Delete cart
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cartId"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int userId, int cartId)
        {
            var cartRepository = unitOfWork.GetRepository<Cart>();
            var cart = await cartRepository.FindByAsync(x => x.Id == cartId && x.UserId == userId);

            if (cart == null)
            {
                throw new NotFoundException(string.Format(
                   MessageResponse.NotFoundError, nameof(Cart), cartId));
            }

            cart.DeletedDate = DateTime.UtcNow;
            cartRepository.Update(cart);
            await unitOfWork.CommitAsync();
        }

        /// <summary>
        /// Empty cart
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task EmptyAsync(int userId)
        {
            var cartRepository = unitOfWork.GetRepository<Cart>();
            var carts = await cartRepository.GetAllAsync(x =>
                            !x.DeletedDate.HasValue && x.UserId == userId);

            foreach (var cart in carts)
            {
                cart.DeletedDate = DateTime.UtcNow;
                cartRepository.Update(cart);
            }

            await unitOfWork.CommitAsync();
        }

        /// <summary>
        /// Merge anonymous cart to cart
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="userId"></param>
        /// <param name="identityCode"></param>
        /// <returns></returns>
        public async Task MergeAsync(int websiteId, int userId, string identityCode)
        {
            var anounymousCartRepository = unitOfWork.GetRepository<AnonymousCart>();
            var cartRepository = unitOfWork.GetRepository<Cart>();

            var anonymousCarts = (await anounymousCartRepository.GetAllAsync(x => !x.DeletedDate.HasValue &&
                                    x.IdentityCode == identityCode)).ToList();
            if (!anonymousCarts.Any()) return;

            foreach (var anonymousCart in anonymousCarts)
            {
                var cart = await cartRepository.FindByAsync(x => !x.DeletedDate.HasValue &&
                               x.UserId == userId && x.ProductId == anonymousCart.ProductId);
                if (cart != null)
                {
                    cart.Quantity += anonymousCart.Quantity;
                    cart.UpdatedDate = DateTime.UtcNow;
                    cartRepository.Update(cart);
                }
                else
                {
                    cartRepository.Add(new Cart
                    {
                        ProductId = anonymousCart.ProductId,
                        UserId = userId,
                        WebsiteId = websiteId,
                        CreatedDate = DateTime.UtcNow,
                        Quantity = anonymousCart.Quantity
                    });
                }
                anonymousCart.DeletedDate = DateTime.UtcNow;
                anounymousCartRepository.Update(anonymousCart);
            }

            await unitOfWork.CommitAsync();
        }
    }
}
