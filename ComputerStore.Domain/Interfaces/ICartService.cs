//-----------------------------------------------------------------------
// <copyright file="ICartService.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using ComputerStore.Structure.Models.Cart;
using System.Threading.Tasks;

namespace ComputerStore.Domain.Interfaces
{
    public interface ICartService
    {
        /// <summary>
        /// Get shopping cart model
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ShoppingCartModel> GetAsync(int userId);
        /// <summary>
        /// Create new cart
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="userId"></param>
        /// <param name="CartModel"></param>
        /// <returns></returns>
        Task CreateAsync(int websiteId, int userId, CartCreateModel CartModel);
        /// <summary>
        /// Update cart 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cartModel"></param>
        /// <returns></returns>
        Task UpdateAsync(int userId, CartModel cartModel);
        /// <summary>
        /// Delete cart
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cartId"></param>
        /// <returns></returns>
        Task DeleteAsync(int userId, int cartId);
        /// <summary>
        /// Empty cart of user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task EmptyAsync(int userId);
        /// <summary>
        /// Merge anonymous cart to user's cart
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="userId"></param>
        /// <param name="identityCode"></param>
        /// <returns></returns>
        Task MergeAsync(int websiteId, int userId, string identityCode);
    }
}
