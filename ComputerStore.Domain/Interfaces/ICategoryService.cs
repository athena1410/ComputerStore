//-----------------------------------------------------------------------
// <copyright file="ICategoryService.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>BinhHTV</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Category;
using ComputerStore.Structure.Models.Pagination;

namespace ComputerStore.Domain.Interfaces
{
    public interface ICategoryService
    {
        /// <summary>
        /// Get all categories
        /// </summary>
        /// <param name="websiteId">site id</param>
        /// <returns>response list category</returns>
        Task<List<CategoryModel>> GetAllAsync(int websiteId,int status);

        /// <summary>
        /// Get category by id
        /// </summary>
        /// <param name="websiteId">site id</param>
        /// <param name="categoryId">category id</param>
        /// <returns>response category</returns>
        Task<CategoryModel> GetByIdAsync(int websiteId, int categoryId);

        /// <summary>
        /// Get all sub-categories
        /// </summary>
        /// <param name="websiteId">site id</param>
        /// <param name="parentId">patient id</param>
        /// <returns>list category</returns>
        Task<List<CategoryModel>> GetChildrenAsync(int websiteId, int parentId);

        /// <summary>
        /// Search categories by name
        /// </summary>
        /// <param name="websiteId">site id</param>
        /// <param name="categorySearch">category model search</param>
        /// <returns>response list category</returns>
        Task<PaginationResponse<List<CategoryModel>>> SearchAsync(int websiteId, SearchModel<CategorySearchModel> categorySearch);

        /// <summary>
        /// Create new Category
        /// </summary>
        /// <param name="websiteId">site id</param>
        /// <param name="categoryModel">category model</param>
        /// <returns>response create new category</returns>
        Task CreateAsync(int websiteId, CategoryModel categoryModel);

        /// <summary>
        /// Update existed category
        /// </summary>
        /// <param name="websiteId">site id</param>
        /// <param name="categoryId">category id</param>
        /// <param name="categoryModel">category model</param>
        /// <returns>response update category</returns>
        Task UpdateAsync(int websiteId, int categoryId, CategoryModel categoryModel);

        /// <summary>
        /// change status activate - deactivate
        /// </summary>
        /// <param name="websiteId">site id</param>
        /// <param name="categoryId">category id</param>
        /// <param name="status">new status</param>
        /// <returns>status change</returns>
        Task ChangeStatusAsync(int websiteId, int categoryId, int status);

        /// <summary>
        /// check exists category name in database
        /// </summary>
        /// <param name="websiteId">site id</param>
        /// <param name="companyName">category name</param>
        /// <returns></returns>
        Task<bool> ExistedByName(int websiteid, string companyName);
    }
}
