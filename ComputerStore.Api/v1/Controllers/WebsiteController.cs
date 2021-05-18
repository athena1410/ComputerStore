using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Enums;
using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Pagination;
using ComputerStore.Structure.Models.Website;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComputerStore.Api.v1.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = nameof(Role.SuperAdmin))]
    public class WebsiteController : ControllerBase
    {
        private readonly IWebsiteService websiteService;

        public WebsiteController(IWebsiteService websiteService)
        {
            this.websiteService = websiteService;
        }

        /// <summary>
        /// Search websites by search model
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody] SearchModel<WebsiteSearchModel> searchModel)
        {
            var companies = await websiteService.SearchAsync(searchModel);
            return Ok(new ApiResponse<PaginationResponse<List<WebsiteModel>>>(companies));
        }

        /// <summary>
        /// Create new website
        /// </summary>
        /// <param name="websiteModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post(WebsiteModel websiteModel)
        {
            var websiteId = await websiteService.CreateAsync(websiteModel);
            return Ok(new ApiResponse<int>(websiteId));
        }

        /// <summary>
        /// Update website information
        /// </summary>
        /// <param name="id"></param>
        /// <param name="websiteModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateWebsiteModel websiteModel)
        {
            await websiteService.UpdateAsync(id, websiteModel);
            return Ok(new ApiResponse<UpdateWebsiteModel>());
        }

        /// <summary>
        /// Change status of website
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPut("ChangeStatus/{id}")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] Status status)
        {
            await websiteService.ChangeStatusAsync(id, (int)status);
            return Ok(new ApiResponse<WebsiteModel>());
        }

        /// <summary>
        /// Get website by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var website = await websiteService.GetByIdAsync(id);
            return Ok(new ApiResponse<WebsiteModel>(website));
        }

        /// <summary>
        /// Get all websites
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] Status status)
        {
            var websites = await websiteService.GetAllAsync(status);
            return Ok(new ApiResponse<List<WebsiteModel>>(websites));
        }

        /// <summary>
        /// Get website image url
        /// </summary>
        [AllowAnonymous]
        [HttpGet("GetLogoUrl/{id}")]
        public async Task<IActionResult> GetLogoUrl(int id)
        {
            var websites = await websiteService.GetLogoUrl(id);
            return Ok(new ApiResponse<string>(Structure.Enums.StatusCode.Ok, websites, "Success"));
        }
    }
}
