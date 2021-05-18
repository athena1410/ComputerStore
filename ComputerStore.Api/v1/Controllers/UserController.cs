using ComputerStore.Api.Attribute;
using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Constants;
using ComputerStore.Structure.Enums;
using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Pagination;
using ComputerStore.Structure.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComputerStore.Api.v1.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        /// <summary>
        /// Create user 
        /// </summary>
        /// <param name="userModel"></param>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Post(UserModel userModel)
        {
            //Prevent Administrator create user on other website
            if (TokenRole != null && TokenRole.Equals(nameof(Role.Administrator)) &&
                    this.TokenWebsiteId != this.WebsiteId)
                return Ok(new ApiResponse<UserModel>(
                    Structure.Enums.StatusCode.Forbidden, Constants.MessageResponse.ForbiddenError));

            Role role;
            int websiteId;
            //If token role is super admin => create administrator user else create normal user
            if (TokenRole != null && TokenRole.Equals(nameof(Role.SuperAdmin)))
            {
                role = Role.Administrator;
                websiteId = userModel.WebsiteId;
            }
            else
            {
                role = Role.User;
                websiteId = this.WebsiteId;
            }
            await userService.CreateAsync(websiteId, role, userModel);
            return Ok(new ApiResponse<UserModel>());
        }

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userModel"></param>
        /// <returns></returns>
        [Authorize(Roles = "SuperAdmin, Administrator")]
        [SecondAuthorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UserUpdateModel userModel)
        {
            var websiteId = TokenRole.Equals(nameof(Role.SuperAdmin))
                            ? userModel.WebsiteId
                            : this.WebsiteId;
            await userService.UpdateAsync(websiteId, id, userModel);
            return Ok(new ApiResponse<UserUpdateModel>());
        }

        /// <summary>
        /// Get user detail
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "SuperAdmin, Administrator, User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            //Prevent user get information of other user
            if (TokenRole.Equals(nameof(Role.User)) && this.UserId != id)
            {
                return Ok(new ApiResponse<UserModel>(
                    Structure.Enums.StatusCode.Forbidden, Constants.MessageResponse.ForbiddenError));
            }

            var websiteId = TokenRole.Equals(nameof(Role.SuperAdmin)) ? (int?)null : this.WebsiteId;
            var user = await userService.GetByIdAsync(websiteId, id);
            return Ok(new ApiResponse<UserModel>(user));
        }

        /// <summary>
        /// Search user
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Authorize(Roles = "SuperAdmin, Administrator")]
        [SecondAuthorize]
        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody] SearchModel<UserSearchModel> searchModel)
        {
            var websiteId = TokenRole.Equals(nameof(Role.SuperAdmin)) ? (int?)null : this.WebsiteId;
            var users = await userService.SearchAsync(websiteId, searchModel);
            return Ok(new ApiResponse<PaginationResponse<List<UserModel>>>(users));
        }

        /// <summary>
        /// Change status of user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [Authorize(Roles = "SuperAdmin, Administrator")]
        [SecondAuthorize]
        [HttpPut("ChangeStatus/{id}")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] Status status)
        {
            var websiteId = TokenRole.Equals(nameof(Role.SuperAdmin)) ? (int?)null : this.WebsiteId;
            await userService.ChangeStatusAsync(websiteId, id, (int)status);
            return Ok(new ApiResponse<UserModel>());
        }


        /// <summary>
        /// Check existed user
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("CheckEmailExisted")]
        public async Task<IActionResult> CheckExisted([FromQuery(Name = "websiteId")] int websiteId,
            [FromQuery(Name = "email")] string email)
        {
            var user = await userService.GetByEmailAndSite(email, websiteId);
            return Ok(new ApiResponse<bool>(user != null));
        }

        /// <summary>
        /// User update profile
        /// </summary>
        /// <param name="userUpdateModel"></param>
        /// <returns></returns>
        [Authorize(Roles = "User, Administrator")]
        [SecondAuthorize(Order = 2)]
        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(UserUpdateModel userUpdateModel)
        {
            await userService.UpdateAsync(this.WebsiteId, this.UserId, userUpdateModel);
            return Ok(new ApiResponse<UserUpdateModel>());
        }

        /// <summary>
        /// User update password
        /// </summary>
        /// <param name="userUpdatePasswordModel"></param>
        /// <returns></returns>
        [Authorize(Roles = "User, Administrator")]
        [SecondAuthorize(Order = 2)]
        [HttpPut("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword(UserUpdatePasswordModel userUpdatePasswordModel)
        {
            await userService.UpdatePassword(this.WebsiteId, this.UserId, userUpdatePasswordModel);
            return Ok(new ApiResponse<UserUpdatePasswordModel>());
        }
    }
}