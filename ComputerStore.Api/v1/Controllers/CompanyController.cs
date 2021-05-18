//-----------------------------------------------------------------------
// <copyright file="CategoryController.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Enums;
using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Company;
using ComputerStore.Structure.Models.Pagination;
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
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService companyService;

        public CompanyController(ICompanyService companyService)
        {
            this.companyService = companyService;
        }

        /// <summary>
        /// Get all company
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var companies = await companyService.GetAllAsync();
            return Ok(new ApiResponse<List<CompanyModel>>(companies));
        }

        /// <summary>
        /// Get company active
        /// </summary>
        [HttpGet("getByStatus")]
        public async Task<IActionResult> GetByStatus()
        {
            var companies = await companyService.GetByStatus();
            return Ok(new ApiResponse<List<CompanyModel>>(companies));
        }

        /// <summary>
        /// Get company by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var company=await companyService.GetByIdAsync(id);
            return Ok(new ApiResponse<CompanyModel>(company));
        }

        /// <summary>
        /// Search company
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody] SearchModel<CompanySearchModel> searchModel)
        {
            var companies = await companyService.SearchAsync(searchModel);
            return Ok(new ApiResponse<PaginationResponse<List<CompanyModel>>>(companies));
        }

        /// <summary>
        /// Create new company
        /// </summary>
        /// <param name="companyModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post(CompanyModel companyModel)
        {
            await companyService.CreateAsync(companyModel);
            return Ok(new ApiResponse<CompanyModel>());
        }

        /// <summary>
        /// Update company
        /// </summary>
        /// <param name="id"></param>
        /// <param name="companyModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] CompanyModel companyModel)
        {
            await companyService.UpdateAsync(id, companyModel);
            return Ok(new ApiResponse<CompanyModel>());
        }

        /// <summary>
        /// Change status of company
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPut("ChangeStatus/{id}")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] Status status)
        {
            await companyService.ChangeStatusCompanyAsync(id, (int)status);
            return Ok(new ApiResponse<CompanyModel>());
        }

        /// <summary>
        /// Check existed by name
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        [HttpGet("existedByName")]
        public async Task<IActionResult> ExistedByName([FromQuery(Name = "name")] string companyName)
        {
            var isExisted = await companyService.ExistedByName(companyName);
            return Ok(new ApiResponse<bool>(isExisted));
        }
    }
}