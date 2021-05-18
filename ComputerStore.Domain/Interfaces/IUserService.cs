//-----------------------------------------------------------------------
// <copyright file="IUserService.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------
using ComputerStore.Structure.Enums;
using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Pagination;
using ComputerStore.Structure.Models.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComputerStore.Domain.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Get user or super admin by email and site
        /// </summary>
        /// <param name="email"></param>
        /// <param name="websiteId"></param>
        /// <returns></returns>
        Task<UserModel> GetByEmailAndSite(string email, int websiteId);

        /// <summary>
        /// Create new user
        /// </summary>
        /// <param name="userModel"></param>
        /// <param name="websiteId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        Task CreateAsync(int websiteId, Role role, UserModel userModel);

        /// <summary>
        /// Update user information
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userModel"></param>
        /// <param name="websiteId"></param>
        /// <returns></returns>
        Task UpdateAsync(int websiteId, int userId, UserUpdateModel userModel);

        /// <summary>
        /// Get user by website id
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<UserModel> GetByIdAsync(int? websiteId, int id);
        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns></returns>
        Task<List<UserModel>> GetAllAsync(int websiteId);
        /// <summary>
        /// Search user
        /// </summary>
        /// <param name="websiteId"></param>
        /// <returns></returns>
        Task<PaginationResponse<List<UserModel>>> SearchAsync(int? websiteId, SearchModel<UserSearchModel> searchModel);
        /// <summary>
        /// Change status activate - deactivate
        /// </summary>
        Task ChangeStatusAsync(int? websiteId, int userId, int status);

        /// <summary>
        /// User update password
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="userId"></param>
        /// <param name="userUpdatePasswordModel"></param>
        /// <returns></returns>
        Task UpdatePassword(int websiteId, int userId, UserUpdatePasswordModel userUpdatePasswordModel);
    }
}
