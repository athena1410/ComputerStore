//-----------------------------------------------------------------------
// <copyright file="ProductController.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>QuangNV25</author>
//-----------------------------------------------------------------------

using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Enums;
using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Pagination;
using ComputerStore.Structure.Models.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ComputerStore.Api.Attribute;

namespace ComputerStore.Api.v1.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]    
    [Authorize(Roles = "Administrator")]        
    public class ProductController : BaseController
    {
        private readonly IProductService productService;

        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }

        /// <summary>
        /// Get all products
        /// </summary>        
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await this.productService.GetAllAsync(this.WebsiteId);            
            return Ok(new ApiResponse<List<ProductModel>>(products));
        }

        /// <summary>
        /// Get product by id
        /// </summary>        
        [AllowAnonymous]
        [HttpGet("{id}")]        
        public async Task<IActionResult> Get(int id)
        {
            var product = await this.productService.GetByIdAsync(this.WebsiteId, id);
            return Ok(new ApiResponse<ProductModel>(product));
        }

        /// <summary>
        /// Search product
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody] SearchModel<ProductSearchModel> searchModel)
        {
            var products = await this.productService.SearchAsync(this.WebsiteId, searchModel);
            return Ok(new ApiResponse<PaginationResponse<List<ProductModel>>>(products));
        }

        /// <summary>
        /// Create new product
        /// </summary>        
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post(ProductModel productModel)
        {
            await this.productService.CreateAsync(this.WebsiteId, productModel);
            return Ok(new ApiResponse<ProductModel>());
        }

        /// <summary>
        /// Update product
        /// </summary>        
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Put(int id, [FromBody] ProductModel productModel)
        {
            await this.productService.UpdateAsync(this.WebsiteId, id, productModel);
            return Ok(new ApiResponse<ProductModel>());
        }

        /// <summary>
        /// Change status of product
        /// </summary>
        [SecondAuthorize]
        [HttpPut("ChangeStatus/{id}")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] Status status)
        {
            await this.productService.ChangeStatusAsync(this.WebsiteId, id, (int)status);
            return Ok(new ApiResponse<ProductModel>());
        }
        /// <summary>
        /// Get max and min price of all product
        /// </summary>
        /// <returns></returns>
        [HttpGet("threshold-price")]
        [AllowAnonymous]
        public async Task<IActionResult> GetThresholdPrice()
        {
            var thresholdPriceModel = await this.productService.GetThresholdPrice();
            return Ok(new ApiResponse<ThresholdPriceModel>(thresholdPriceModel));
        }
        /// <summary>
        /// Check product code duplicate
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        [HttpGet("existedByProductCode")]
        public async Task<IActionResult> ExistedByName([FromQuery(Name = "productCode")] string productCode)
        {
            var isExisted = await productService.ExistedByProductCode(productCode);
            return Ok(new ApiResponse<bool>(isExisted));
        }
    }
}