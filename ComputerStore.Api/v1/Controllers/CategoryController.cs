//-----------------------------------------------------------------------
// <copyright file="CategoryController.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>BinhHTV</author>
//-----------------------------------------------------------------------

using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Enums;
using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Category;
using ComputerStore.Structure.Models.Pagination;
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
    [Authorize(Roles = nameof(Role.Administrator))]
    public class CategoryController : BaseController
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        /// <returns>response list category</returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll(Status status)
        {
            var categories = await categoryService.GetAllAsync(this.WebsiteId,(int)status);
            return Ok(new ApiResponse<List<CategoryModel>>(categories));
        }

        /// <summary>
        /// get category by id
        /// </summary>
        /// <param name="id">catgory id</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var categories = await categoryService.GetByIdAsync(this.WebsiteId, id);
            return Ok(new ApiResponse<CategoryModel>(categories));
        }

        /// <summary>
        /// Search categories
        /// </summary>
        /// <param name="categorySearch">category model search</param>
        /// <returns>response list category</returns>
        [AllowAnonymous]
        [HttpPost("search")]
        public async Task<IActionResult> Search(SearchModel<CategorySearchModel> category)
        {
            var categories = await categoryService.SearchAsync(this.WebsiteId, category);
            return Ok(new ApiResponse<PaginationResponse<List<CategoryModel>>>(categories));
        }

        /// <summary>
        /// Create new Category
        /// </summary>
        /// <param name="categoryModel">category model</param>
        /// <returns>response create new category</returns>
        [SecondAuthorize(Order = 2)]
        [HttpPost]
        public async Task<IActionResult> Post(CategoryModel categoryModel)
        {
            await categoryService.CreateAsync(this.WebsiteId, categoryModel);
            return Ok(new ApiResponse<CategoryModel>());
        }

        /// <summary>
        /// Update existed category
        /// </summary>
        /// <param name="id">category id</param>
        /// <param name="categoryModel">category model</param>
        /// <returns>response update category</returns>
        [SecondAuthorize(Order = 2)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] CategoryModel categoryModel)
        {
            await categoryService.UpdateAsync(this.WebsiteId, id, categoryModel);
            return Ok(new ApiResponse<CategoryModel>());
        }

        /// <summary>
        /// change status activate - deactivate
        /// </summary>
        /// <param name="id">category id</param>
        /// <param name="status">new status</param>
        /// <returns>status change</returns>
        [SecondAuthorize(Order = 2)]
        [HttpPut("ChangeStatus/{id}")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] Status status)
        {
            await categoryService.ChangeStatusAsync(this.WebsiteId, id, (int)status);
            return Ok(new ApiResponse<CategoryModel>());
        }

        /// <summary>
        /// check exists category by name
        /// </summary>
        /// <param name="categoryName">category name</param>
        /// <returns>response exists name category</returns>
        [SecondAuthorize(Order = 2)]
        [HttpGet("existedByName")]
        public async Task<IActionResult> ExistedByName([FromQuery(Name = "name")] string categoryName)
        {
            var isExisted = await categoryService.ExistedByName(this.WebsiteId, categoryName);
            return Ok(new ApiResponse<bool>(isExisted));
        }
    }
}