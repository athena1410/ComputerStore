//-----------------------------------------------------------------------
// <copyright file="CartController.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using ComputerStore.Api.Attribute;
using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ComputerStore.Api.v1.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    
    public class CartController : BaseController
    {
        private readonly ICartService cartService;
        public CartController(ICartService cartService)
        {
            this.cartService = cartService;
        }

        /// <summary>
        /// Get shopping cart
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "User, Administrator, SuperAdmin")]
        [SecondAuthorize(Order = 2)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var shoppingCart = await cartService.GetAsync(this.UserId);
            return Ok(new ApiResponse<ShoppingCartModel>(shoppingCart));
        }

        /// <summary>
        /// Create new cart
        /// </summary>
        /// <param name="cartModel"></param>
        /// <returns></returns>
        [Authorize(Roles = "User")]
        [SecondAuthorize(Order = 2)]
        [HttpPost]
        public async Task<IActionResult> Post(CartCreateModel cartModel)
        {
            await this.cartService.CreateAsync(this.WebsiteId, this.UserId, cartModel);
            return Ok(new ApiResponse<CartCreateModel>());
        }

        /// <summary>
        /// Update existed cart
        /// </summary>
        /// <param name="cartModel"></param>
        /// <returns></returns>
        [Authorize(Roles = "User")]
        [SecondAuthorize(Order = 2)]
        [HttpPut]
        public async Task<IActionResult> Put(CartModel cartModel)
        {
            await this.cartService.UpdateAsync(this.UserId, cartModel);
            return Ok(new ApiResponse<CartModel>());
        }

        /// <summary>
        /// Merge anonymous cart to user's cart
        /// </summary>
        /// <param name="identityCode"></param>
        /// <returns></returns>
        [Authorize(Roles = "User")]
        [SecondAuthorize(Order = 2)]
        [HttpPost("merge-cart")]
        public async Task<IActionResult> Merge([FromBody] string identityCode)
        {
            await this.cartService.MergeAsync(this.WebsiteId, this.UserId, identityCode);
            return Ok(new ApiResponse<CartModel>());
        }

        /// <summary>
        /// Delete cart
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "User")]
        [SecondAuthorize(Order = 2)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await this.cartService.DeleteAsync(this.UserId, id);
            return Ok(new ApiResponse<CartModel>());
        }

        /// <summary>
        /// Empty cart
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "User")]
        [SecondAuthorize(Order = 2)]
        [HttpDelete]
        public async Task<IActionResult> Empty()
        {
            await this.cartService.EmptyAsync(this.UserId);
            return Ok(new ApiResponse<CartModel>());
        }
    }
}
