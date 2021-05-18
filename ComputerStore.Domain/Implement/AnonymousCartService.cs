//-----------------------------------------------------------------------
// <copyright file="AnonymousCartService.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using AutoMapper;
using ComputerStore.BoundedContext.Entities;
using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Exceptions;
using ComputerStore.Structure.Models.Cart;
using ComputerStore.UnitOfWork.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ComputerStore.Structure.Constants.Constants;

namespace ComputerStore.Domain.Implement
{
    public class AnonymousCartService : IAnonymousCartService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public AnonymousCartService(
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
        /// <param name="websiteId"></param>
        /// <param name="identityCode"></param>
        /// <returns></returns>
        public async Task<ShoppingCartModel> GetAsync(int websiteId, string identityCode)
        {
            var anonymousCartRepository = unitOfWork.GetRepository<AnonymousCart>();
            var anonymousCarts = await anonymousCartRepository.GetAllAsync(x => !x.DeletedDate.HasValue && x.WebsiteId == websiteId &&
                                    x.IdentityCode == identityCode, x => x.Include(x => x.Product).ThenInclude(x => x.ProductImage));
            var cartItems = mapper.Map<List<CartItemModel>>(anonymousCarts);
            return mapper.Map<ShoppingCartModel>(cartItems);
        }

        /// <summary>
        /// Create new anonymous cart
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="anonymousCartModel"></param>
        /// <returns></returns>
        public async Task CreateAsync(int websiteId, AnonymousCartCreateModel anonymousCartModel)
        {
            var anonymousCartRepository = unitOfWork.GetRepository<AnonymousCart>();
            var existedCart = await anonymousCartRepository.FindByAsync(x => !x.DeletedDate.HasValue && x.WebsiteId == websiteId &&
                                    x.IdentityCode == anonymousCartModel.IdentityCode.ToString() && x.ProductId == anonymousCartModel.ProductId);
            if (existedCart != null)
            {
                existedCart.Quantity += anonymousCartModel.Quantity;
                anonymousCartRepository.Update(existedCart);
                await unitOfWork.CommitAsync();
                return;
            }

            var productRepository = unitOfWork.GetRepository<Product>();
            var product = await productRepository.FindByAsync(x => x.WebsiteId == websiteId &&
                                x.Id == anonymousCartModel.ProductId);
            if (product == null)
            {
                throw new NotFoundException(string.Format(
                    MessageResponse.NotFoundError, nameof(Product), anonymousCartModel.ProductId));
            }

            var anonymousCart = mapper.Map<AnonymousCart>(anonymousCartModel);
            anonymousCart.WebsiteId = websiteId;
            anonymousCart.CreatedDate = DateTime.UtcNow;
            anonymousCartRepository.Add(anonymousCart);
            await unitOfWork.CommitAsync();
        }

        /// <summary>
        /// Update existed anonymous cart
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="anonymousCartModel"></param>
        /// <returns></returns>
        public async Task UpdateAsync(int websiteId, AnonymousCartModel anonymousCartModel)
        {
            var anonymousCartRepository = unitOfWork.GetRepository<AnonymousCart>();
            var anonymousCart = await anonymousCartRepository.FindByAsync(x => !x.DeletedDate.HasValue &&
                                    x.Id == anonymousCartModel.Id && x.IdentityCode == anonymousCartModel.IdentityCode.ToString());
            if (anonymousCart == null)
            {
                throw new NotFoundException(string.Format(
                   MessageResponse.NotFoundError, nameof(AnonymousCart), anonymousCartModel.Id));
            }

            anonymousCart = mapper.Map(anonymousCartModel, anonymousCart);
            anonymousCartRepository.Update(anonymousCart);
            await unitOfWork.CommitAsync();
        }

        /// <summary>
        /// Delete anonymous cart
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="id"></param>
        /// <param name="identityCode"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int websiteId, int id, string identityCode)
        {
            var anonymousCartRepository = unitOfWork.GetRepository<AnonymousCart>();
            var anonymousCart = await anonymousCartRepository.FindByAsync(x => x.Id == id &&
                                   x.IdentityCode == identityCode && x.WebsiteId == websiteId);
            if (anonymousCart == null)
            {
                throw new NotFoundException(string.Format(
                   MessageResponse.NotFoundError, nameof(AnonymousCart), id));
            }

            anonymousCart.DeletedDate = DateTime.UtcNow;
            anonymousCartRepository.Update(anonymousCart);
            await unitOfWork.CommitAsync();
        }

        /// <summary>
        /// Empty anonymous cart
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="identityCode"></param>
        /// <returns></returns>
        public async Task EmptyAsync(int websiteId, string identityCode)
        {
            var anonymousCartRepository = unitOfWork.GetRepository<AnonymousCart>();
            var anonymousCarts = (await anonymousCartRepository.GetAllAsync(x => !x.DeletedDate.HasValue &&
                                   x.WebsiteId == websiteId && x.IdentityCode == identityCode)).ToList();
            foreach (var item in anonymousCarts)
            {
                item.DeletedDate = DateTime.UtcNow;
                anonymousCartRepository.Update(item);
            }
            await unitOfWork.CommitAsync();
        }
    }
}
