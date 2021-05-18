//-----------------------------------------------------------------------
// <copyright file="AnonymousCartController.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using ComputerStore.Api.Attribute;
using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Cart;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerStore.Api.v2.Controllers
{
    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ValidateModel]
    [ValidateWebsite]
    public class AnonymousCartController : ControllerBase
    {
        private readonly IAnonymousCartService anonymousCartService;
        private int WebsiteId => Convert.ToInt32(HttpContext.Request.Headers["website-id"].FirstOrDefault());

        public AnonymousCartController(IAnonymousCartService anonymousCartService)
        {
            this.anonymousCartService = anonymousCartService;
        }

        /// <summary>
        /// Get shopping cart 
        /// </summary>
        /// <param name="identityCode"></param>
        /// <returns></returns>
        [HttpGet("{identityCode}")]
        public async Task<IActionResult> Get(string identityCode)
        {
            var shoppingCart = await anonymousCartService.GetAsync(this.WebsiteId, identityCode);
            return Ok(new ApiResponse<ShoppingCartModel>(shoppingCart));
        }

        /// <summary>
        /// Create new anonymous cart
        /// </summary>
        /// <param name="anonymousCartModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post(AnonymousCartCreateModel anonymousCartModel)
        {
            await this.anonymousCartService.CreateAsync(this.WebsiteId, anonymousCartModel);
            return Ok(new ApiResponse<AnonymousCartModel>());
        }

        /// <summary>
        /// Update existed anonymous cart
        /// </summary>
        /// <param name="anonymousCartModel"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Put(AnonymousCartModel anonymousCartModel)
        {
            await this.anonymousCartService.UpdateAsync(this.WebsiteId, anonymousCartModel);
            return Ok(new ApiResponse<AnonymousCartModel>());
        }

        /// <summary>
        /// Delete anonymous cart
        /// </summary>
        /// <param name="id"></param>
        /// <param name="identityCode"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromBody] string identityCode)
        {
            await this.anonymousCartService.DeleteAsync(this.WebsiteId, id, identityCode);
            return Ok(new ApiResponse<AnonymousCartModel>());
        }

        /// <summary>
        /// Empty anonymous cart
        /// </summary>
        /// <param name="identityCode"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Empty([FromBody] string identityCode)
        {
            await this.anonymousCartService.EmptyAsync(this.WebsiteId, identityCode);
            return Ok(new ApiResponse<AnonymousCartModel>());
        }
    }
}
