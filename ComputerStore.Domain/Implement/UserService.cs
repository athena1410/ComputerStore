//-----------------------------------------------------------------------
// <copyright file="UserService.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using AutoMapper;
using ComputerStore.BoundedContext.Entities;
using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Constants;
using ComputerStore.Structure.Exceptions;
using ComputerStore.Structure.Helper;
using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Pagination;
using ComputerStore.Structure.Models.User;
using ComputerStore.UnitOfWork.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Role = ComputerStore.Structure.Enums.Role;

namespace ComputerStore.Domain.Implement
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        /// <summary>
        /// Create new user
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="role"></param>
        /// <param name="userModel"></param>
        /// <returns></returns>
        public async Task CreateAsync(int websiteId, Role role, UserModel userModel)
        {
            var userRepository = unitOfWork.GetRepository<User>();
            var user = await userRepository.FindByAsync(x => x.Email.Equals(userModel.Email) &&
                            x.WebsiteId == websiteId);

            if (user != null)
            {
                throw new ValidationException(
                    string.Format(Constants.MessageResponse.ExistedError, nameof(User), nameof(user.Email)));
            }

            user = mapper.Map<UserModel, User>(userModel);
            user.WebsiteId = websiteId;
            user.RoleId = (int)role;
            user.Password = Security.CreateHashPassword(userModel.Email, userModel.Password);
            user.CreatedDate = DateTime.UtcNow;
            userRepository.Add(user);
            await unitOfWork.CommitAsync();
        }

        /// <summary>
        /// Update user information
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="userId"></param>
        /// <param name="userModel"></param>
        /// <returns></returns>
        public async Task UpdateAsync(int websiteId, int userId, UserUpdateModel userModel)
        {
            var userRepository = unitOfWork.GetRepository<User>();
            var user = await userRepository.FindByAsync(x => x.Id == userId &&
                        x.WebsiteId == websiteId);

            if (user == null) 
            {
                throw new NotFoundException(string.Format(
                    Constants.MessageResponse.NotFoundError, nameof(User), nameof(userId)));
            }

            user = mapper.Map(userModel, user);
            if (userModel.Password != null && 
                userModel.Password.Length >= Constants.Utility.PasswordMinLength)
            {
                user.Password = Security.CreateHashPassword(userModel.Email, userModel.Password);
            }
            user.UpdatedDate = DateTime.UtcNow;
            userRepository.Update(user);
            await unitOfWork.CommitAsync();
        }

        /// <summary>
        /// Get all user by website
        /// </summary>
        /// <param name="websiteId"></param>
        /// <returns></returns>
        public async Task<List<UserModel>> GetAllAsync(int websiteId)
        {
            var users = await unitOfWork.GetRepository<User>()
                            .GetAllAsync(x => x.WebsiteId == websiteId);
            return mapper.Map<List<UserModel>>(users);
        }

        /// <summary>
        /// Get user or super admin by email and site
        /// </summary>
        /// <param name="email"></param>
        /// <param name="websiteId"></param>
        /// <returns></returns>
        public async Task<UserModel> GetByEmailAndSite(string email, int websiteId)
        {
            var userRepository = unitOfWork.GetRepository<User>();
            var user = await userRepository.FindByAsync(x => x.Email == email &&
                                                  x.WebsiteId == websiteId, nameof(BoundedContext.Entities.Role));
            return mapper.Map<User, UserModel>(user);
        }

        /// <summary>
        /// Get user of website by userId and websiteId 
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<UserModel> GetByIdAsync(int? websiteId, int userId)
        {
            var user = await unitOfWork.GetRepository<User>()
                        .FindByAsync(x => x.Id == userId &&
                                     (websiteId == null || x.WebsiteId == websiteId));
            return mapper.Map<UserModel>(user);
        }

        /// <summary>
        /// Search users
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public async Task<PaginationResponse<List<UserModel>>> SearchAsync(int? websiteId, SearchModel<UserSearchModel> searchModel)
        {
            var roleId = websiteId == null ? (int)Role.Administrator : (int)Role.User;
            var userRepository = unitOfWork.GetRepository<User>();
            //Extract search model generate user search model and paging context
            var (userSearchModel, pagingContext) = searchModel.Extract();

            Expression<Func<User, bool>> predicate = x =>
            (userSearchModel.DisplayName == null ||
            (x.LastName + " " + x.FirstName).Contains(userSearchModel.DisplayName.Trim().ToLower())) && 
            x.RoleId == roleId && (roleId == (int)Role.Administrator || x.WebsiteId == websiteId);

            var users = await userRepository.GetAllAsync(predicate, pagingContext);
            var totalRecord = await userRepository.CountAsync(predicate);
            return new PaginationResponse<List<UserModel>>(
                mapper.Map<List<UserModel>>(users), totalRecord, searchModel.NumberPerPage);
        }

        /// <summary>
        /// Change status of user
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="userId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task ChangeStatusAsync(int? websiteId, int userId, int status)
        {
            var userRepository = unitOfWork.GetRepository<User>();
            var user = await userRepository.FindByAsync(x => x.Id == userId &&
                                (websiteId == null || x.WebsiteId == websiteId));
            if (user == null)
            {
                throw new NotFoundException(string.Format(
                    Constants.MessageResponse.NotFoundError, nameof(User), nameof(userId)));
            }
            user.Status = status;
            user.UpdatedDate = DateTime.UtcNow;
            userRepository.Update(user);
            await unitOfWork.CommitAsync();
        }

        /// <summary>
        /// User update password
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="userId"></param>
        /// <param name="userUpdatePasswordModel"></param>
        /// <returns></returns>
        public async Task UpdatePassword(int websiteId, int userId, UserUpdatePasswordModel userUpdatePasswordModel)
        {
            if (userUpdatePasswordModel.NewPassword == null ||
                userUpdatePasswordModel.NewPassword.Length < Constants.Utility.PasswordMinLength)
            {
                throw new ValidationException(Constants.MessageResponse.NewPasswordNotMeetExpected);
            }

            var userRepository = unitOfWork.GetRepository<User>();
            var user = await userRepository.FindByAsync(x => x.Id == userId &&
                        x.WebsiteId == websiteId);
            if (user == null)
            {
                throw new NotFoundException(string.Format(
                    Constants.MessageResponse.NotFoundError, nameof(User), nameof(userUpdatePasswordModel.Id)));
            }

            var isValid = Security.ValidatePassword(user.Email, userUpdatePasswordModel.OldPassword, user.Password);
            if (!isValid)
            {
                throw new ValidationException(string.Format(Constants.MessageResponse.OldPasswordIncorrect));
            }

            user.Password = Security.CreateHashPassword(user.Email, userUpdatePasswordModel.NewPassword);
            user.UpdatedDate = DateTime.UtcNow;
            userRepository.Update(user);
            await unitOfWork.CommitAsync();
        }
    }
}
