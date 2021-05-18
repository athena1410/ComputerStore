using ComputerStore.BoundedContext.Entities;
using ComputerStore.Structure.Enums;
using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Pagination;
using ComputerStore.Structure.Models.Website;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ComputerStore.Domain.Interfaces
{
    public interface IWebsiteService
    {
        /// <summary>
        /// Get website by website id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<WebsiteModel> GetByIdAsync(int id);
        /// <summary>
        /// Get all websites
        /// </summary>
        /// <returns></returns>
        Task<List<WebsiteModel>> GetAllAsync(Status status);
        /// <summary>
        /// Search website
        /// </summary>
        /// <param name="website"></param>
        /// <returns></returns>
        Task<PaginationResponse<List<WebsiteModel>>> SearchAsync(SearchModel<WebsiteSearchModel> searchModel);
        /// <summary>
        /// Create new Website
        /// </summary>
        /// <param name="websiteModel"></param>
        /// <returns></returns>
        Task<int> CreateAsync(WebsiteModel websiteModel);
        /// <summary>
        /// Update existed website
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="websiteModel"></param>
        /// <returns></returns>
        Task UpdateAsync(int websiteId, UpdateWebsiteModel websiteModel);
        /// <summary>
        /// Change status activate - deactivate
        /// </summary>
        /// <param name="websiteId">website id</param>
        /// <param name="status">status</param>
        Task ChangeStatusAsync(int websiteId, int status);
        /// <summary>
        /// Get website image url
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<string> GetLogoUrl(int id);
    }
}
