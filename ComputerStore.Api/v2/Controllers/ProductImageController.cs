using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ComputerStore.Api.v2.Controllers
{
    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
   //[Authorize(Roles = nameof(Role.Administrator))]
   [AllowAnonymous]
   public class ProductImageController : BaseController
   {
      private readonly IProductImageService productImageService;

      public ProductImageController(IProductImageService productImageService)
      {
         this.productImageService = productImageService;
      }

      [HttpPost]
      public async Task<IActionResult> Post(ProductModel productModel)
      {
         await productImageService.ImportAsync(this.WebsiteId, productModel);
         return Ok(new ApiResponse<ProductModel>());
      }

      /// <summary>
      /// Remove Product Image
      /// </summary>
      [HttpPut("{id}")]
      public async Task<IActionResult> Delete(int id, ProductModel productModel)
      {
         await productImageService.DeleteAsync(this.WebsiteId, id, productModel);
         return Ok(new ApiResponse<ProductModel>());
      }
   }
}