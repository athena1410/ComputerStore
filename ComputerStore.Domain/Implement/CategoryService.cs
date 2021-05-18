//-----------------------------------------------------------------------
// <copyright file="CategoryService.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>BinhHTV</author>
//-----------------------------------------------------------------------

using AutoMapper;
using ComputerStore.BoundedContext.Entities;
using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Constants;
using ComputerStore.Structure.Exceptions;
using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Category;
using ComputerStore.Structure.Models.Pagination;
using ComputerStore.UnitOfWork.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ComputerStore.Domain.Implement
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        /// <summary>
        /// Get all parent categories
        /// </summary>
        /// <param name="websiteId">site id</param>
        /// <returns>response list category</returns>
        public async Task<List<CategoryModel>> GetAllAsync(int websiteId, int status)
        {
            var categoryRepository = this.unitOfWork.GetRepository<Category>();
            var categories = await categoryRepository.GetAllAsync(x => x.WebsiteId == websiteId && x.Status == status);
            var categoryModels = mapper.Map<List<CategoryModel>>(categories);

            List<CategoryModel> result = new List<CategoryModel>();
            foreach (var category in categoryModels.Where(x=>x.ParentId==null))
            {
                category.Categories = categoryModels.Where(x => x.ParentId == category.Id).ToList();
                result.Add(category);
            }
            return result;
        }

        /// <summary>
        /// Get category by id
        /// </summary>
        /// <param name="websiteId">site id</param>
        /// <param name="categoryId">category id</param>
        /// <returns>response category</returns>
        public async Task<CategoryModel> GetByIdAsync(int websiteId, int categoryId)
        {
            var categoryRepository = this.unitOfWork.GetRepository<Category>();
            var category = await categoryRepository.FindByAsync(x => x.Id == categoryId && x.WebsiteId == websiteId);

            if (category == null)
            {
                throw new NotFoundException(
                    string.Format(Constants.MessageResponse.NotFoundError,
                        nameof(Category), categoryId));
            }

            return mapper.Map<CategoryModel>(category);
        }

        /// <summary>
        /// Search categories
        /// </summary>
        /// <param name="websiteId">site id</param>
        /// <param name="searchModel">category model search</param>
        /// <returns>response list category</returns>
        public async Task<PaginationResponse<List<CategoryModel>>> SearchAsync(int websiteId, SearchModel<CategorySearchModel> searchModel)
        {
            var categoryRepository = this.unitOfWork.GetRepository<Category>();

            //Extract search model generate category search model and paging context
            var (categorySearchModel, pagingContext) = searchModel.Extract();

            Expression<Func<Category, bool>> predicate = x =>
            (searchModel.Data.Name == null || x.Name.ToLower().Contains(categorySearchModel.Name.ToLower()) &&
                x.WebsiteId == websiteId);

            var categories = await categoryRepository.GetAllAsync(predicate, pagingContext);
            var totalRecord = await categoryRepository.CountAsync(predicate);

            return new PaginationResponse<List<CategoryModel>>(
                mapper.Map<List<CategoryModel>>(categories), totalRecord, searchModel.NumberPerPage);
        }

         /// Create new Category
        /// </summary>
        /// <param name="websiteId">site id</param>
        /// <param name="categoryModel">category model</param>
        /// <returns>response create new category</returns>
        public async Task CreateAsync(int websiteId, CategoryModel categoryModel)
        {

            var categoryRepository = this.unitOfWork.GetRepository<Category>();

            //check exists category name
            var existedCategory = await ExistedByName(websiteId, categoryModel.Name);
            if (existedCategory)
            {
                throw new ValidationException(Constants.MessageResponse.CategoryNameExisted);
            }

            //check condition when create category is children of subcategory
            CheckCreateUpdateToChildrenSubCategory(websiteId, categoryModel);

            var category = mapper.Map<CategoryModel, Category>(categoryModel);
            category.WebsiteId = websiteId;
            category.CreatedDate = DateTime.UtcNow;
            categoryRepository.Add(category);
            await this.unitOfWork.CommitAsync();
        }

        /// <summary>
        /// Update existed category
        /// </summary>
        /// <param name="websiteId">site id</param>
        /// <param name="categoryId">category id</param>
        /// <param name="categoryModel">category model</param>
        /// <returns>response update category</returns>
        public async Task UpdateAsync(int websiteId, int categoryId, CategoryModel categoryModel)
        {
            var categoryRepository = unitOfWork.GetRepository<Category>();

            var category = await categoryRepository.FindByAsync(x=>x.Id == categoryId && x.WebsiteId == websiteId);
            if (category == null)
            {
                throw new NotFoundException(
                    string.Format(Constants.MessageResponse.NotFoundError,
                        nameof(Category), categoryId.ToString()));
            }

            //check status parent category if this category is sub category
            //if parent category is deactivate => sub category don't update/change sattus
            CheckAccessActionSubCategory(websiteId, category.ParentId);

            //check exists category name in db
            if (categoryModel.Name.Trim() != category.Name.Trim())
            {
                var existedCategory = await ExistedByName(websiteId, categoryModel.Name);
                if (existedCategory)
                {
                    throw new ValidationException(Constants.MessageResponse.CategoryNameExisted);
                }
            }

            //check exists product in children category when children category update to root catgory
            if (categoryModel.ParentId == null && category.ParentId != null)
            {
                var productRepository = unitOfWork.GetRepository<Product>();
                var existedProduct = await productRepository.ExistsAsync(p => p.CategoryId == category.Id && p.WebsiteId == websiteId);
                if (existedProduct)
                {
                    throw new ValidationException(Constants.MessageResponse.ExistedProductInCategory);
                }
            }
            else
            {
                //if update parent/sub category to children of subcategory
                CheckCreateUpdateToChildrenSubCategory(websiteId, categoryModel);

                //if update parent category to children category
                if (category.ParentId != categoryModel.ParentId)
                {
                    //check exists children category in of root category
                    var existsChildrenCategory = await GetChildrenAsync(websiteId, categoryId);
                    if (existsChildrenCategory.Any())
                    {
                        throw new ValidationException(Constants.MessageResponse.ExistsChildrenCatgory);
                    }
                }
            }

            mapper.Map(categoryModel, category);
            category.UpdatedDate = DateTime.UtcNow;
            categoryRepository.Update(category);
            await unitOfWork.CommitAsync();
        }

        /// <summary>
        /// change status activate - deactivate
        /// </summary>
        /// <param name="websiteId">site id</param>
        /// <param name="categoryId">category id</param>
        /// <param name="status">new status</param>
        /// <returns>status change</returns>
        public async Task ChangeStatusAsync(int websiteId, int categoryId, int status)
        {
            var categoryRepository = unitOfWork.GetRepository<Category>();

            var category = await categoryRepository.FindByAsync(x=>x.Id == categoryId && x.WebsiteId == websiteId);
            if (category == null)
            {
                throw new NotFoundException(
                    string.Format(Constants.MessageResponse.NotFoundError, nameof(Category), categoryId.ToString()));
            }

            //check status parent category if this category is sub category
            //if parent category is deactivate => sub category don't update/change sattus
            CheckAccessActionSubCategory(websiteId, category.ParentId);

            category.Status = status;
            category.UpdatedDate = DateTime.UtcNow;
            categoryRepository.Update(category);
            await unitOfWork.CommitAsync();
        }

        /// <summary>
        /// Get all sub-categories
        /// </summary>
        /// <param name="websiteId">site id</param>
        /// <param name="parentId">patient id</param>
        /// <returns>list category</returns>
        public async Task<List<CategoryModel>> GetChildrenAsync(int websiteId, int parentId)
        {
            var categoryRepository = this.unitOfWork.GetRepository<Category>();

            var categories = await categoryRepository.GetAllAsync(x => x.ParentId == parentId && x.WebsiteId == websiteId);
            return mapper.Map<List<CategoryModel>>(categories);
        }

        /// <summary>
        /// check exists category by name
        /// </summary>
        /// <param name="websiteId">site id</param>
        /// <param name="categoryName">category name</param>
        /// <returns>response exists name category</returns>
        public async Task<bool> ExistedByName(int websiteId, string categoryName)
        {
            var categoryRepository = unitOfWork.GetRepository<Category>();
            return await categoryRepository.ExistsAsync(x => x.WebsiteId == websiteId && x.Name.Equals(categoryName.Trim()));
        }

        /// <summary>
        /// check access action with sub category
        /// if parent category is deactivate => sub category don't update/change sattus
        /// </summary>
        /// <param name="websiteId">website id</param>
        /// <param name="parentId">parent id</param>
        private void CheckAccessActionSubCategory(int websiteId, int? parentId) {
            //check status parent category if this category is sub category
            if (parentId != null)
            {
                //get parent category
                var parentCategory = GetByIdAsync(websiteId, parentId.Value);
                if (parentCategory?.Result != null)
                {
                    //check valid data to update / change status. if parent category is deactivate => sub category don't update/change sattus
                    var isValid = parentCategory.Result.Status == (int)Structure.Enums.Status.ACTIVE;
                    if (!isValid)
                    {
                        throw new ValidationException(Constants.MessageResponse.CannotActionSubCategory);
                    }
                }
            }
        }

        /// <summary>
        /// check condition when category want to update/create to children of subcategory
        /// </summary>
        /// <param name="websiteId">website id</param>
        /// <param name="parentId">parent id</param>
        private void CheckCreateUpdateToChildrenSubCategory(int websiteId, CategoryModel categoryModel)
        {
            //if update parent/sub category to children of subcategory
            if (categoryModel.ParentId != null)
            {
                var tempParentCategory = GetByIdAsync(websiteId, categoryModel.ParentId.Value)?.Result;
                var isParentCategory = tempParentCategory != null && tempParentCategory.ParentId == null;
                if (!isParentCategory)
                {
                    throw new ValidationException(
                        string.Format(Constants.MessageResponse.NotChildrentOfSubCategory, categoryModel.Name.ToString()));
                }
            }
        }
    }
}
