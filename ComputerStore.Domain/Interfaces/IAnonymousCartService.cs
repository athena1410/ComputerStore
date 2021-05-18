//-----------------------------------------------------------------------
// <copyright file="IAnonymousCartService.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using ComputerStore.Structure.Models.Cart;
using System.Threading.Tasks;

namespace ComputerStore.Domain.Interfaces
{
    public interface IAnonymousCartService
    {
        /// <summary>
        /// Get shopping cart
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="identityCode"></param>
        /// <returns></returns>
        Task<ShoppingCartModel> GetAsync(int websiteId, string identityCode);
        /// <summary>
        /// Create anonymous cart
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="anonymousCartModel"></param>
        /// <returns></returns>
        Task CreateAsync(int websiteId, AnonymousCartCreateModel anonymousCartModel);
        /// <summary>
        /// Update anonymous cart
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="anonymousCartModel"></param>
        /// <returns></returns>
        Task UpdateAsync(int websiteId, AnonymousCartModel anonymousCartModel);
        /// <summary>
        /// Delete anonymous cart
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="cartId"></param>
        /// <param name="identityCode"></param>
        /// <returns></returns>
        Task DeleteAsync(int websiteId, int cartId, string identityCode);
        /// <summary>
        /// Empty anonymoust cart
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="identityCode"></param>
        /// <returns></returns>
        Task EmptyAsync(int websiteId, string identityCode);
    }
}
