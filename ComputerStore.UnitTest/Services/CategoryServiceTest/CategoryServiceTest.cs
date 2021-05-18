//-----------------------------------------------------------------------
// <copyright file="CategoryServiceTest.cs" category="Young">
//     Company copyright tag.
// </copyright>
// <author>BinhHTV</author>
//-----------------------------------------------------------------------

using AutoMapper;
using ComputerStore.Api.Mappings;
using ComputerStore.BoundedContext.Entities;
using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Constants;
using ComputerStore.Structure.Exceptions;
using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Category;
using ComputerStore.Structure.Models.Pagination;
using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ComputerStore.UnitTest.Services.CategoryServiceTest
{
    [TestFixture]
    public class CategoryServiceTest
    {
        private ICategoryService categoryService;
        private List<Product> products;
        private List<Category> categories;
        private PagingContext pagingContext;
        private Mapper mapper;

        [SetUp]
        public void SetUp()
        {
            var mapperConfiguration = new MapperConfiguration(new MappingProfile());
            mapper = new Mapper(mapperConfiguration);

            #region Seed data for unit test
            //Build List category
            categories = new List<Category>()
            {
                new Category()
                {
                    Id = 1,
                    WebsiteId = 1,
                    ParentId = null,
                    Name = "Category Test 1",
                    TemplateMetaData = "{\"CPU\":\"8BG\", \"RAM\":\"500GB\"}",
                    TemplateSpecificData = "{\"Color\":\"Black\"}",
                    Status = 0
                },
                new Category()
                {
                    Id = 2,
                    WebsiteId = 1,
                    ParentId = 1,
                    Name = "Category 1 Sub 1",
                    TemplateMetaData = "{\"CPU\":\"8BG\", \"RAM\":\"500GB\"}",
                    TemplateSpecificData = "{\"Color\":\"Black\"}",
                     Status = 0
                },
                new Category()
                {
                    Id = 3,
                    WebsiteId = 1,
                    ParentId = 1,
                    Name = "Category 1 Sub 2",
                    TemplateMetaData = "{\"CPU\":\"8BG\", \"RAM\":\"500GB\"}",
                    TemplateSpecificData = "{\"Color\":\"Black\"}",
                     Status = 0
                },
                new Category()
                {
                    Id = 4,
                    WebsiteId = 1,
                    ParentId = null,
                    Name = "Category Test 4",
                    TemplateMetaData = "{\"CPU\":\"8BG\", \"RAM\":\"500GB\"}",
                    TemplateSpecificData = "{\"Color\":\"Black\"}",
                    Status = 0
                },
            };

            //build list product
            products = new List<Product>()
            {
                new Product()
                {
                    Id = 1,
                    WebsiteId = 1,
                    CategoryId = 2,
                    Name = "Product Sub Category 1",
                    ProductCode = "Product01"
                },
                new Product()
                {
                    Id = 1,
                    WebsiteId = 1,
                    CategoryId = 2,
                    Name = "Product Sub Category 2",
                    ProductCode = "Product02"
                }
            };

            //build pagingcontext
            pagingContext = new PagingContext() { 
                NumberPerPage = 2,
                PageNumber = 1, 
                SortColums = "Id",
                SortDirection = "asc"
            };
            #endregion

            //Build category service with unit of work
            categoryService = new CategoryServiceBuilder()
                .WithRepositoryMock(categories, products, pagingContext)
                .WithUnitOfWorkSetup()
                .Build();
        }

        #region Test GetAllAsync
        /// <summary>
        /// Get all parent categogy include sub category with status = 0 (activate)
        /// </summary>
        [Test]
        public void TestGetAllAsync_WithValidData_StatusActivate()
        {
            const int websiteId = 1;
            const int status = (int)Structure.Enums.Status.ACTIVE;
            var result = categoryService.GetAllAsync(websiteId, status);

            Assert.IsNotEmpty(result.Result);
            Assert.AreEqual(result.Result.Count, 2);

            Assert.AreEqual(result.Result[0].Id, 1);
            Assert.AreEqual(result.Result[0].WebsiteId, websiteId);
            Assert.IsNull(result.Result[0].ParentId);
            Assert.AreEqual(result.Result[0].Name, "Category Test 1");
            Assert.AreEqual(result.Result[0].TemplateMetaData, "{\"CPU\":\"8BG\", \"RAM\":\"500GB\"}");
            Assert.AreEqual(result.Result[0].TemplateSpecificData, "{\"Color\":\"Black\"}");
            Assert.AreEqual(result.Result[0].Status, 0);
            Assert.AreEqual(result.Result[0].Categories.Count, 2);
            Assert.AreEqual(result.Result[0].Categories[0].Id, 2);
            Assert.AreEqual(result.Result[0].Categories[0].Name, "Category 1 Sub 1");
            Assert.AreEqual(result.Result[0].Categories[1].Id, 3);
            Assert.AreEqual(result.Result[0].Categories[1].Name, "Category 1 Sub 2");

            Assert.AreEqual(result.Result[1].Id, 4);
            Assert.AreEqual(result.Result[1].WebsiteId, websiteId);
            Assert.IsNull(result.Result[1].ParentId);
            Assert.AreEqual(result.Result[1].Name, "Category Test 4");
            Assert.AreEqual(result.Result[1].TemplateMetaData, "{\"CPU\":\"8BG\", \"RAM\":\"500GB\"}");
            Assert.AreEqual(result.Result[1].TemplateSpecificData, "{\"Color\":\"Black\"}");
            Assert.AreEqual(result.Result[1].Status, 0);
            Assert.IsEmpty(result.Result[1].Categories);
        }

        /// <summary>
        /// Get all parent categogy include sub category with status = 1 (deactivate)
        /// </summary>
        [Test]
        public void TestGetAllAsync_WithValidData_StatusDeactivate()
        {
            const int websiteId = 1;
            const int status = (int)Structure.Enums.Status.DEACTIVATE;
            var result = categoryService.GetAllAsync(websiteId, status);
            Assert.IsEmpty(result.Result);
        }
        #endregion

        #region Test GetByIdAsync
        /// <summary>
        /// get category by id when category.website diffirent website parameter
        /// => throw not found exception
        /// </summary>
        [Test]
        public void TestGetByIdAsync_WithInvalidData_DiffirentWebsiteId_ShouldThrowNotFoundException()
        {
            const int categoryId = 1;
            const int websiteId = 2;
            var ex = Assert.ThrowsAsync<NotFoundException>(() => categoryService.GetByIdAsync(websiteId, categoryId));
            Assert.AreEqual(string.Format(Constants.MessageResponse.NotFoundError,
                        nameof(Category), categoryId), ex.Message);
        }

        /// <summary>
        /// get category by id when category.website diffirent website parameter
        /// => throw exception access denied
        /// </summary>
        [Test]
        public void TestGetByIdAsync_WithInvalidData_NotExistsCategory_ShouldThrowNotFoundException()
        {
            const int categoryId = 10;
            const int websiteId = 1;
            var ex = Assert.ThrowsAsync<NotFoundException>(() => categoryService.GetByIdAsync(websiteId, categoryId));
            Assert.AreEqual(
                string.Format(Constants.MessageResponse.NotFoundError,
                        nameof(Category), categoryId), ex.Message);
        }

        /// <summary>
        /// get category by id with valid data
        /// </summary>
        [Test]
        public void TestGetByIdAsync_WithValidData()
        {
            const int categoryId = 1;
            const int websiteId = 1;
            var categoryExpect = new Category()
            {
                Id = 1,
                WebsiteId = 1,
                ParentId = null,
                Name = "Category Test 1",
                TemplateMetaData = "{\"CPU\":\"8BG\", \"RAM\":\"500GB\"}",
                TemplateSpecificData = "{\"Color\":\"Black\"}",
                Status = 0
            };
            var category = categoryService.GetByIdAsync(websiteId, categoryId);
            Assert.IsNotNull(category);
            Assert.AreEqual(category.Result.Id, categoryExpect.Id);
            Assert.AreEqual(category.Result.ParentId, categoryExpect.ParentId);
            Assert.AreEqual(category.Result.Name, categoryExpect.Name);
            Assert.AreEqual(category.Result.TemplateMetaData, categoryExpect.TemplateMetaData);
            Assert.AreEqual(category.Result.TemplateSpecificData, categoryExpect.TemplateSpecificData);
            Assert.AreEqual(category.Result.Status, categoryExpect.Status);
        }
        #endregion

        #region Test SearchAsync
        /// <summary>
        /// Search category by name, exists category name, pageSize = 1, numberPerPage = 2
        /// </summary>
        [Test]
        public void TestSearchAsync_ValidData_ExistsCategoryByName_Page1PerPage2()
        {
            const int websiteId = 1;
            var searchModel = new SearchModel<CategorySearchModel>()
            {
                Data = new CategorySearchModel()
                {
                    Name = "Category"
                }
            };
            var result = categoryService.SearchAsync(websiteId, searchModel);
            var listCategory = result.Result.Results;
            Assert.IsNotEmpty(listCategory);
            Assert.AreEqual(listCategory.Count, 2);
            Assert.AreEqual(listCategory[0].Id, 1);
            Assert.AreEqual(listCategory[0].WebsiteId, 1);
            Assert.IsNull(listCategory[0].ParentId);
            Assert.AreEqual(listCategory[0].Name, "Category Test 1");
            Assert.AreEqual(listCategory[0].TemplateMetaData, "{\"CPU\":\"8BG\", \"RAM\":\"500GB\"}");
            Assert.AreEqual(listCategory[0].TemplateSpecificData, "{\"Color\":\"Black\"}");
            Assert.AreEqual(listCategory[0].Status, 0);

            Assert.AreEqual(listCategory[1].Id, 2);
            Assert.AreEqual(listCategory[1].WebsiteId, 1);
            Assert.AreEqual(listCategory[1].ParentId, 1);
            Assert.AreEqual(listCategory[1].Name, "Category 1 Sub 1");
            Assert.AreEqual(listCategory[1].TemplateMetaData, "{\"CPU\":\"8BG\", \"RAM\":\"500GB\"}");
            Assert.AreEqual(listCategory[1].TemplateSpecificData, "{\"Color\":\"Black\"}");
            Assert.AreEqual(listCategory[1].Status, 0);
        }

        /// <summary>
        /// Search category by name, not exists category name
        /// </summary>
        [Test]
        public void TestSearchAsync_ValidData_NotExistsCategoryByName_Page1PerPage2()
        {
            const int websiteId = 1;

            var searchModel = new SearchModel<CategorySearchModel>()
            {
                Data = new CategorySearchModel()
                {
                    Name = "Category Test Null"
                },
            };
            var result = categoryService.SearchAsync(websiteId, searchModel);
            var listCategory = result.Result.Results;
            Assert.IsEmpty(listCategory);
        }
        #endregion

        #region Test CreateAsync
        /// <summary>
        /// create category when exists category name database, case parent category name
        /// => throw exception invalid operation
        /// </summary>
        [Test]
        public void TestCreateCategoryAsync_WithInvalidData_SameParentCategoryName_ShouldThrowValidationException()
        {
            const int websiteId = 1;
            var categoryModel = new CategoryModel()
            {
                WebsiteId = 1,
                ParentId = null,
                Name = "Category Test 1",
                TemplateMetaData = "{\"CPU\":\"8BG\", \"RAM\":\"1TB\"}",
                TemplateSpecificData = "{\"Color\":\"White\"}",
                Status = 0
            };
            var ex = Assert.ThrowsAsync<ValidationException>(() => categoryService.CreateAsync(websiteId, categoryModel));
            Assert.AreEqual(Constants.MessageResponse.CategoryNameExisted, ex.Message);
        }

        /// <summary>
        /// create category when exists category name database, case sub category name
        /// => throw exception invalid operation
        /// </summary>
        [Test]
        public void TestCreateCategoryAsync_WithInvalidData_SameSubCategoryName_ShouldThrowValidationException()
        {
            const int websiteId = 1;
            var categoryModel = new CategoryModel()
            {
                WebsiteId = 1,
                ParentId = 1,
                Name = "Category 1 Sub 1",
                TemplateMetaData = "{\"CPU\":\"8BG\", \"RAM\":\"1TB\"}",
                TemplateSpecificData = "{\"Color\":\"White\"}",
                Status = 0
            };
            var ex = Assert.ThrowsAsync<ValidationException>(() => categoryService.CreateAsync(websiteId, categoryModel));
            Assert.AreEqual(Constants.MessageResponse.CategoryNameExisted, ex.Message);
        }

        /// <summary>
        /// create category is children of subcategory
        /// => throw exception invalid operation
        /// </summary>
        [Test]
        public void TestCreateCategoryAsync_WithInvalidData_ChildrenSubCategory_ShouldThrowValidationException()
        {
            const int websiteId = 1;
            var categoryModel = new CategoryModel()
            {
                WebsiteId = 1,
                ParentId = 2,
                Name = "Category 1 Sub 1 Level 1",
                TemplateMetaData = "{\"CPU\":\"8BG\", \"RAM\":\"1TB\"}",
                TemplateSpecificData = "{\"Color\":\"White\"}",
                Status = 0
            };
            var ex = Assert.ThrowsAsync<ValidationException>(() => categoryService.CreateAsync(websiteId, categoryModel));
            Assert.AreEqual(string.Format(
                Constants.MessageResponse.NotChildrentOfSubCategory, categoryModel.Name.ToString()),
                ex.Message);
        }

        /// <summary>
        /// create category when exists category name database, case parent category name
        /// </summary>
        [Test]
        public void TestCreateCategoryAsync_WithValidData_ParentCategory_ShouldNotThrowAnyException()
        {
            const int websiteId = 1;
            var categoryModel = new CategoryModel()
            {
                WebsiteId = 1,
                ParentId = null,
                Name = "Category Test 2",
                TemplateMetaData = "{\"CPU\":\"8BG\", \"RAM\":\"1TB\"}",
                TemplateSpecificData = "{\"Color\":\"White\"}"
            };
            Assert.DoesNotThrowAsync(() => categoryService.CreateAsync(websiteId, categoryModel));
        }

        /// <summary>
        /// create category when exists category name database, case sub category name
        /// </summary>
        [Test]
        public void TestCreateCategoryAsync_WithValidData_SubCategory_ShouldNotThrowAnyException()
        {
            const int websiteId = 1;
            var categoryModel = new CategoryModel()
            {
                WebsiteId = 1,
                ParentId = 1,
                Name = "Category 1 Sub 3",
                TemplateMetaData = "{\"CPU\":\"8BG\", \"RAM\":\"1TB\"}",
                TemplateSpecificData = "{\"Color\":\"White\"}"
            };
            Assert.DoesNotThrowAsync(() => categoryService.CreateAsync(websiteId, categoryModel));
        }
        #endregion

        #region Test UpdateAsync
        /// <summary>
        /// update category when categoryModel.website diffirent website parameter
        /// => throw exception access denied
        /// </summary>
        [Test]
        public void TestUpdateCategoryAsync_WithInvalidData_DiffirentWebsiteId_ShouldThrowNotFoundException()
        {
            const int websiteId = 2;
            const int categoryId = 1;
            var categoryModel = new CategoryModel()
            {
                Id = 1,
                WebsiteId = 1,
                ParentId = null,
                Name = "Category Test 1",
                TemplateMetaData = "{\"CPU\":\"8BG\", \"RAM\":\"1TB\", \"Test\":\"Test\"}",
                TemplateSpecificData = "{\"Color\":\"White\"}"
            };

            var ex = Assert.ThrowsAsync<NotFoundException>(() => categoryService.UpdateAsync(websiteId, categoryId, categoryModel));
            Assert.AreEqual(
                string.Format(Constants.MessageResponse.NotFoundError, nameof(Category), categoryId),
                ex.Message);
        }

        /// <summary>
        /// update category when category not exists
        /// => throw exception not found
        /// </summary>
        [Test]
        public void TestUpdateCategoryAsync_WithInvalidData_CategoryNotExists_ShouldThrowNotFoundException()
        {
            const int websiteId = 1;
            const int categoryId = 10;
            var categoryModel = new CategoryModel()
            {
                Id = 1,
                WebsiteId = 1,
                ParentId = null,
                Name = "Category Test 1",
                TemplateMetaData = "{\"CPU\":\"8BG\", \"RAM\":\"1TB\", \"Test\":\"Test\"}",
                TemplateSpecificData = "{\"Color\":\"White\"}"
            };

            var ex = Assert.ThrowsAsync<NotFoundException>(() => categoryService.UpdateAsync(websiteId, categoryId, categoryModel));
            Assert.AreEqual(string.Format(Constants.MessageResponse.NotFoundError,
                        nameof(Category), categoryId.ToString()), ex.Message);
        }

        /// <summary>
        /// update category when category exists but category.websiteId diffirence website parameter
        /// => throw exception not found
        /// </summary>
        [Test]
        public void TestUpdateCategoryAsync_WithInvalidData_CategoryExists_DiffirentWebsiteId_ShouldThrowNotFoundException()
        {
            const int websiteId = 2;
            const int categoryId = 1;
            var categoryModel = new CategoryModel()
            {
                Id = 1,
                WebsiteId = 2,
                ParentId = null,
                Name = "Category Test 1",
                TemplateMetaData = "{\"CPU\":\"8BG\", \"RAM\":\"1TB\", \"Test\":\"Test\"}",
                TemplateSpecificData = "{\"Color\":\"White\"}"
            };

            var ex = Assert.ThrowsAsync<NotFoundException>(() => categoryService.UpdateAsync(websiteId, categoryId, categoryModel));
            Assert.AreEqual(string.Format(Constants.MessageResponse.NotFoundError,
                        nameof(Category), categoryId.ToString()), ex.Message);
        }

        /// <summary>
        /// update category when same category name
        /// => throw exception invalid operation
        /// </summary>
        [Test]
        public void TestUpdateCategoryAsync_WithInvalidData_SameNameCategory_ShouldThrowValidationException()
        {
            const int websiteId = 1;
            const int categoryId = 1;
            var categoryModel = new CategoryModel()
            {
                Id = 1,
                WebsiteId = 1,
                ParentId = null,
                Name = "Category 1 Sub 1",
                TemplateMetaData = "{\"CPU\":\"8BG\", \"RAM\":\"1TB\", \"Test\":\"Test\"}",
                TemplateSpecificData = "{\"Color\":\"White\"}"
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => categoryService.UpdateAsync(websiteId, categoryId, categoryModel));
            Assert.AreEqual(Constants.MessageResponse.CategoryNameExisted, ex.Message);
        }

        /// <summary>
        /// update sub category (exists product) to parent category
        /// => throw exception invalid operation
        /// </summary>
        [Test]
        public void TestUpdateCategoryAsync_ParentCategory_WithInvalidData_ExistsProduct_ShouldThrowValidationException()
        {
            const int websiteId = 1;
            const int categoryId = 2;
            var categoryModel = new CategoryModel()
            {
                Id = 2,
                WebsiteId = 1,
                ParentId = null,
                Name = "Category Test 2",
                TemplateMetaData = "{\"CPU\":\"8BG\", \"RAM\":\"1TB\", \"Test\":\"Test\"}",
                TemplateSpecificData = "{\"Color\":\"White\"}"
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => categoryService.UpdateAsync(websiteId, categoryId, categoryModel));
            Assert.AreEqual(Constants.MessageResponse.ExistedProductInCategory, ex.Message);
        }

        /// <summary>
        /// update parent category to children of subcategory
        /// => throw exception invalid operation
        /// </summary>
        [Test]
        public void TestUpdateCategoryAsync_WithInvalidData_ParentToChildrenSubCategory_ShouldThrowValidationException()
        {
            const int websiteId = 1;
            const int categoryId = 1;
            var categoryModel = new CategoryModel()
            {
                Id = 1,
                WebsiteId = 1,
                ParentId = 2,
                Name = "Category 1 Sub 1 Level 1",
                TemplateMetaData = "{\"CPU\":\"8BG\", \"RAM\":\"1TB\", \"Test\":\"Test\"}",
                TemplateSpecificData = "{\"Color\":\"White\"}"
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => categoryService.UpdateAsync(websiteId, categoryId, categoryModel));
            Assert.AreEqual(
                string.Format(Constants.MessageResponse.NotChildrentOfSubCategory, categoryModel.Name.ToString()), ex.Message);
        }

        /// <summary>
        /// update sub category to children of subcategory
        /// => throw exception invalid operation
        /// </summary>
        [Test]
        public void TestUpdateCategoryAsync_WithInvalidData_SubToChildrenSubCategory_ShouldThrowValidationException()
        {
            const int websiteId = 1;
            const int categoryId = 3;
            var categoryModel = new CategoryModel()
            {
                Id = 3,
                WebsiteId = 1,
                ParentId = 2,
                Name = "Category 1 Sub 1 Level 1",
                TemplateMetaData = "{\"CPU\":\"8BG\", \"RAM\":\"1TB\", \"Test\":\"Test\"}",
                TemplateSpecificData = "{\"Color\":\"White\"}"
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => categoryService.UpdateAsync(websiteId, categoryId, categoryModel));
            Assert.AreEqual(
                string.Format(Constants.MessageResponse.NotChildrentOfSubCategory, categoryModel.Name.ToString()), ex.Message);
        }

        /// <summary>
        /// update parent category (exists sub category) to sub category
        /// => throw exception invalid operation
        /// </summary>
        [Test]
        public void TestUpdateCategoryAsync_ParentCategory_WithInvalidData_ExistsSubCategory_ShouldThrowValidationException()
        {
            const int websiteId = 1;
            const int categoryId = 1;
            var categoryModel = new CategoryModel()
            {
                Id = 1,
                WebsiteId = 1,
                ParentId = 4,
                Name = "Category 4 Sub 1",
                TemplateMetaData = "{\"CPU\":\"8BG\", \"RAM\":\"1TB\", \"Test\":\"Test\"}",
                TemplateSpecificData = "{\"Color\":\"White\"}"
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => categoryService.UpdateAsync(websiteId, categoryId, categoryModel));
            Assert.AreEqual(Constants.MessageResponse.ExistsChildrenCatgory, ex.Message);
        }

        /// <summary>
        /// update parent category (without sub category) to sub category
        /// </summary>
        [Test]
        public void TestUpdateCategoryAsync_WithoutSubCategory_ParentCategoryToSubCategory_ShouldNotThrowException()
        {
            const int websiteId = 1;
            const int categoryId = 4;
            var categoryModel = new CategoryModel()
            {
                Id = 4,
                WebsiteId = 1,
                ParentId = 1,
                Name = "Category 1 Sub 3",
                TemplateMetaData = "{\"CPU\":\"8BG\", \"RAM\":\"1TB\"}",
                TemplateSpecificData = "{\"Color\":\"White\"}",
                Status = 0
            };

            var oldCategory = categories.SingleOrDefault(x => x.Id == categoryId);
            Assert.IsNotNull(oldCategory);
            Assert.AreEqual(oldCategory.Id, 4);
            Assert.AreEqual(oldCategory.WebsiteId, 1);
            Assert.IsNull(oldCategory.ParentId);
            Assert.AreEqual(oldCategory.Name, "Category Test 4");
            Assert.AreEqual(oldCategory.TemplateMetaData, "{\"CPU\":\"8BG\", \"RAM\":\"500GB\"}");
            Assert.AreEqual(oldCategory.TemplateSpecificData, "{\"Color\":\"Black\"}");
            Assert.AreEqual(oldCategory.Status, 0);
            Assert.IsNull(oldCategory.UpdatedDate);

            Assert.DoesNotThrowAsync(() => categoryService.UpdateAsync(websiteId, categoryId, categoryModel));

            var newCategory = categories.SingleOrDefault(x => x.Id == categoryId);
            Assert.AreEqual(newCategory.Id, 4);
            Assert.AreEqual(newCategory.ParentId, 1);
            Assert.AreEqual(newCategory.Name, "Category 1 Sub 3");
            Assert.AreEqual(newCategory.TemplateMetaData, "{\"CPU\":\"8BG\", \"RAM\":\"1TB\"}");
            Assert.AreEqual(newCategory.TemplateSpecificData, "{\"Color\":\"White\"}");
            Assert.AreEqual(newCategory.Status, 0);
            Assert.IsNotNull(newCategory.UpdatedDate);
        }

        /// <summary>
        /// update sub category (without product) to parent category
        /// </summary>
        [Test]
        public void TestUpdateCategoryAsync_WithoutProduct_SubCategoryToParentCategory_ShouldNotThrowException()
        {
            const int websiteId = 1;
            const int categoryId = 3;
            var categoryModel = new CategoryModel()
            {
                Id = 3,
                WebsiteId = 1,
                ParentId = null,
                Name = "Category Test 3",
                TemplateMetaData = "{\"CPU\":\"8BG\", \"RAM\":\"1TB\", \"Test\":\"Test\"}",
                TemplateSpecificData = "{\"Color\":\"White\"}",
                Status = 0
            };

            var oldCategory = categories.SingleOrDefault(x => x.Id == categoryId);
            Assert.IsNotNull(oldCategory);
            Assert.AreEqual(oldCategory.Id, 3);
            Assert.AreEqual(oldCategory.WebsiteId, 1);
            Assert.AreEqual(oldCategory.ParentId, 1);
            Assert.AreEqual(oldCategory.Name, "Category 1 Sub 2");
            Assert.AreEqual(oldCategory.TemplateMetaData, "{\"CPU\":\"8BG\", \"RAM\":\"500GB\"}");
            Assert.AreEqual(oldCategory.TemplateSpecificData, "{\"Color\":\"Black\"}");
            Assert.AreEqual(oldCategory.Status, 0);
            Assert.IsNull(oldCategory.UpdatedDate);

            Assert.DoesNotThrowAsync(() => categoryService.UpdateAsync(websiteId, categoryId, categoryModel));

            var newCategory = categories.SingleOrDefault(x => x.Id == categoryId);
            Assert.AreEqual(newCategory.Id, 3);
            Assert.IsNull(newCategory.ParentId);
            Assert.AreEqual(newCategory.Name, "Category Test 3");
            Assert.AreEqual(newCategory.TemplateMetaData, "{\"CPU\":\"8BG\", \"RAM\":\"1TB\", \"Test\":\"Test\"}");
            Assert.AreEqual(newCategory.TemplateSpecificData, "{\"Color\":\"White\"}");
            Assert.AreEqual(newCategory.Status, 0);
            Assert.IsNotNull(newCategory.UpdatedDate);
        }

        /// <summary>
        /// update category with valid data
        /// </summary>
        [Test]
        public void TestUpdateCategoryAsync_WithValidData_ShouldNotThrowException()
        {
            const int websiteId = 1;
            const int categoryId = 1;
            var categoryModel = new CategoryModel()
            {
                Id = 1,
                WebsiteId = 1,
                ParentId = null,
                Name = "Category Test 1",
                TemplateMetaData = "{\"CPU\":\"8BG\", \"RAM\":\"1TB\", \"Test\":\"Test\"}",
                TemplateSpecificData = "{\"Color\":\"White\"}",
                Status = 0
            };

            var oldCategory = categories.SingleOrDefault(x => x.Id == categoryId);
            Assert.IsNotNull(oldCategory);
            Assert.AreEqual(oldCategory.Id, 1);
            Assert.AreEqual(oldCategory.WebsiteId, 1);
            Assert.IsNull(oldCategory.ParentId);
            Assert.AreEqual(oldCategory.Name, "Category Test 1");
            Assert.AreEqual(oldCategory.TemplateMetaData, "{\"CPU\":\"8BG\", \"RAM\":\"500GB\"}");
            Assert.AreEqual(oldCategory.TemplateSpecificData, "{\"Color\":\"Black\"}");
            Assert.AreEqual(oldCategory.Status, 0);
            Assert.IsNull(oldCategory.UpdatedDate);

            Assert.DoesNotThrowAsync(() => categoryService.UpdateAsync(websiteId, categoryId, categoryModel));

            var newCategory = categories.SingleOrDefault(x => x.Id == categoryId);
            Assert.AreEqual(newCategory.Id, 1);
            Assert.IsNull(newCategory.ParentId);
            Assert.AreEqual(newCategory.Name, "Category Test 1");
            Assert.AreEqual(newCategory.TemplateMetaData, "{\"CPU\":\"8BG\", \"RAM\":\"1TB\", \"Test\":\"Test\"}");
            Assert.AreEqual(newCategory.TemplateSpecificData, "{\"Color\":\"White\"}");
            Assert.AreEqual(newCategory.Status, 0);
            Assert.IsNotNull(newCategory.UpdatedDate);
        }

        /// <summary>
        /// Update sub category with invalid data, parent category is deactivate
        /// => throw invalid operation exception
        /// </summary>
        [Test]
        public void TestUpdateAsync_SubCategory_WithInValidData_ParentCategoryIsDeactivate_ShouldThrowValidationException()
        {
            const int websiteId = 1;
            const int parentCategoryId = 1;
            const int statusParentCategory = (int)Structure.Enums.Status.DEACTIVATE;
            //parent category is deactivate
            categoryService.ChangeStatusAsync(websiteId, parentCategoryId, statusParentCategory);

            const int subCategoryId = 2;
            var subCategoryModel = new CategoryModel()
            {
                Id = 2,
                WebsiteId = 1,
                ParentId = 1,
                Name = "Category 1 Sub 1",
                TemplateMetaData = "{\"CPU\":\"8BG\", \"RAM\":\"1TB\", \"Test\":\"Test\"}",
                TemplateSpecificData = "{\"Color\":\"White\"}",
                Status = 0
            };

            var oldCategory = categories.SingleOrDefault(x => x.Id == subCategoryId);
            Assert.IsNotNull(oldCategory);
            Assert.AreEqual(oldCategory.Id, subCategoryId);
            Assert.AreEqual(oldCategory.ParentId, parentCategoryId);
            Assert.AreEqual(oldCategory.Name, "Category 1 Sub 1");
            Assert.AreEqual(oldCategory.TemplateMetaData, "{\"CPU\":\"8BG\", \"RAM\":\"500GB\"}");
            Assert.AreEqual(oldCategory.TemplateSpecificData, "{\"Color\":\"Black\"}");
            Assert.AreEqual(oldCategory.Status, 0);
            Assert.IsNull(oldCategory.UpdatedDate);

            var ex = Assert.ThrowsAsync<ValidationException>(() => categoryService.UpdateAsync(websiteId, subCategoryId, subCategoryModel));
            Assert.AreEqual(Constants.MessageResponse.CannotActionSubCategory, ex.Message);
        }
        #endregion

        #region Test ChangeStatusAsync
        /// <summary>
        /// Change status when category is null
        /// => throw not found exception
        /// </summary>
        [Test]
        public void TestChangeStatusAsync_WithInvalidData_CategoryNull_ShouldThrowNotFoundException()
        {
            const int websiteId = 1;
            const int categoryId = 10;
            const int status = (int)Structure.Enums.Status.ACTIVE;
            var ex = Assert.ThrowsAsync<NotFoundException>(() => categoryService.ChangeStatusAsync(websiteId, categoryId, status));
            Assert.AreEqual(
                    string.Format(
                        Constants.MessageResponse.NotFoundError, nameof(Category), categoryId.ToString()),
                    ex.Message);
        }

        /// <summary>
        /// Change status when category.websiteId diffirent parameter websiteId
        /// => throw not found exception
        /// </summary>
        [Test]
        public void TestChangeStatusAsync_WithInvalidData_DiffirentWebsiteId_ShouldThrowNotFoundException()
        {
            const int websiteId = 2;
            const int categoryId = 1;
            const int status = (int)Structure.Enums.Status.ACTIVE;
            var ex = Assert.ThrowsAsync<NotFoundException>(() => categoryService.ChangeStatusAsync(websiteId, categoryId, status));
            Assert.AreEqual(
                    string.Format(
                        Constants.MessageResponse.NotFoundError, nameof(Category), categoryId.ToString()),
                    ex.Message);
        }

        /// <summary>
        /// Change status with valid data, parent category
        /// </summary>
        [Test]
        public void TestChangeStatusAsync_WithValidData_ParentCategory()
        {
            const int websiteId = 1;
            const int categoryId = 4;
            const int status = (int)Structure.Enums.Status.DEACTIVATE;

            var oldCategory = categories.SingleOrDefault(x => x.Id == categoryId);
            Assert.IsNotNull(oldCategory);
            Assert.AreEqual(oldCategory.Id, 4);
            Assert.AreEqual(oldCategory.WebsiteId, 1);
            Assert.IsNull(oldCategory.ParentId);
            Assert.AreEqual(oldCategory.Name, "Category Test 4");
            Assert.AreEqual(oldCategory.TemplateMetaData, "{\"CPU\":\"8BG\", \"RAM\":\"500GB\"}");
            Assert.AreEqual(oldCategory.TemplateSpecificData, "{\"Color\":\"Black\"}");
            Assert.AreEqual(oldCategory.Status, 0);
            Assert.IsNull(oldCategory.UpdatedDate);

            Assert.DoesNotThrowAsync(() => categoryService.ChangeStatusAsync(websiteId, categoryId, status));

            var categoryChangedStatus = categories.SingleOrDefault(x=>x.Id == categoryId);
            Assert.AreEqual(categoryChangedStatus.Id, 4);
            Assert.AreEqual(oldCategory.WebsiteId, 1);
            Assert.IsNull(categoryChangedStatus.ParentId);
            Assert.AreEqual(categoryChangedStatus.Name, "Category Test 4");
            Assert.AreEqual(categoryChangedStatus.TemplateMetaData, "{\"CPU\":\"8BG\", \"RAM\":\"500GB\"}");
            Assert.AreEqual(categoryChangedStatus.TemplateSpecificData, "{\"Color\":\"Black\"}");
            Assert.AreEqual(categoryChangedStatus.Status, 1);
            Assert.IsNotNull(categoryChangedStatus.UpdatedDate);
        }

        /// <summary>
        /// Change status of sub category with valid data, parent category is activate
        /// </summary>
        [Test]
        public void TestChangeStatusAsync_SubCategory_WithValidData_ParentCategoryIsActivate()
        {
            const int websiteId = 1;
            const int categoryId = 2;
            const int status = (int)Structure.Enums.Status.DEACTIVATE;

            var oldCategory = categories.SingleOrDefault(x => x.Id == categoryId);
            Assert.IsNotNull(oldCategory);
            Assert.AreEqual(oldCategory.Id, 2);
            Assert.AreEqual(oldCategory.ParentId, 1);
            Assert.AreEqual(oldCategory.Name, "Category 1 Sub 1");
            Assert.AreEqual(oldCategory.TemplateMetaData, "{\"CPU\":\"8BG\", \"RAM\":\"500GB\"}");
            Assert.AreEqual(oldCategory.TemplateSpecificData, "{\"Color\":\"Black\"}");
            Assert.AreEqual(oldCategory.Status, 0);
            Assert.IsNull(oldCategory.UpdatedDate);

            Assert.DoesNotThrowAsync(() => categoryService.ChangeStatusAsync(websiteId, categoryId, status));

            var categoryChangedStatus = categories.SingleOrDefault(x => x.Id == categoryId);
            Assert.AreEqual(categoryChangedStatus.Id, 2);
            Assert.AreEqual(oldCategory.ParentId, 1);
            Assert.AreEqual(categoryChangedStatus.Name, "Category 1 Sub 1");
            Assert.AreEqual(categoryChangedStatus.TemplateMetaData, "{\"CPU\":\"8BG\", \"RAM\":\"500GB\"}");
            Assert.AreEqual(categoryChangedStatus.TemplateSpecificData, "{\"Color\":\"Black\"}");
            Assert.AreEqual(categoryChangedStatus.Status, 1);
            Assert.IsNotNull(categoryChangedStatus.UpdatedDate);

        }

        /// <summary>
        /// Change status of sub category with invalid data, parent category is deactivate
        /// => throw invalid operation exception
        /// </summary>
        [Test]
        public void TestChangeStatusAsync_SubCategory_WithInValidData_ParentCategoryIsDeactivate_ShouldThrowValidationException()
        {
            const int websiteId = 1;
            const int parentCategoryId = 1;
            const int statusParentCategory = (int)Structure.Enums.Status.DEACTIVATE;
            //parent category is deactivate
            categoryService.ChangeStatusAsync(websiteId, parentCategoryId, statusParentCategory);

            const int subCategoryId = 2;
            const int subCategoryStatus = (int)Structure.Enums.Status.DEACTIVATE;

            var oldCategory = categories.SingleOrDefault(x => x.Id == subCategoryId);
            Assert.IsNotNull(oldCategory);
            Assert.AreEqual(oldCategory.Id, subCategoryId);
            Assert.AreEqual(oldCategory.ParentId, parentCategoryId);
            Assert.AreEqual(oldCategory.Name, "Category 1 Sub 1");
            Assert.AreEqual(oldCategory.TemplateMetaData, "{\"CPU\":\"8BG\", \"RAM\":\"500GB\"}");
            Assert.AreEqual(oldCategory.TemplateSpecificData, "{\"Color\":\"Black\"}");
            Assert.AreEqual(oldCategory.Status, 0);
            Assert.IsNull(oldCategory.UpdatedDate);

            var ex = Assert.ThrowsAsync<ValidationException>(() => categoryService.ChangeStatusAsync(websiteId, subCategoryId, subCategoryStatus));
            Assert.AreEqual(Constants.MessageResponse.CannotActionSubCategory, ex.Message);
        }
        #endregion

        #region Test GetChildrenAsync
        /// <summary>
        /// get all sub category of parent category, exist sub category
        /// </summary>
        [Test]
        public void TestGetChildrenAsync_ExistsSubCategory() {
            const int websiteId = 1;
            const int parentId = 1;
            var result = categoryService.GetChildrenAsync(websiteId, parentId);
            Assert.IsNotEmpty(result.Result);
            Assert.AreEqual(result.Result.Count, 2);
            Assert.AreEqual(result.Result[0].Id, 2);
            Assert.AreEqual(result.Result[0].WebsiteId, websiteId);
            Assert.AreEqual(result.Result[0].ParentId, parentId);
            Assert.AreEqual(result.Result[0].Name, "Category 1 Sub 1");
            Assert.AreEqual(result.Result[0].TemplateMetaData, "{\"CPU\":\"8BG\", \"RAM\":\"500GB\"}");
            Assert.AreEqual(result.Result[0].TemplateSpecificData, "{\"Color\":\"Black\"}");
            Assert.AreEqual(result.Result[0].Status, 0);

            Assert.AreEqual(result.Result[1].Id, 3);
            Assert.AreEqual(result.Result[1].WebsiteId, websiteId);
            Assert.AreEqual(result.Result[1].ParentId, parentId);
            Assert.AreEqual(result.Result[1].Name, "Category 1 Sub 2");
            Assert.AreEqual(result.Result[1].TemplateMetaData, "{\"CPU\":\"8BG\", \"RAM\":\"500GB\"}");
            Assert.AreEqual(result.Result[1].TemplateSpecificData, "{\"Color\":\"Black\"}");
            Assert.AreEqual(result.Result[1].Status, 0);
        }

        /// <summary>
        ///  get all sub category of parent category, not exist sub category
        /// </summary>
        [Test]
        public void TestGetChildrenAsync_NotExistsSubCategory()
        {
            const int websiteId = 1;
            const int parentId = 4;
            var result = categoryService.GetChildrenAsync(websiteId, parentId);
            Assert.IsEmpty(result.Result);
        }

        #endregion

        #region Test ExistedByName
        /// <summary>
        /// Existed By Name Category with exists parent category name
        /// </summary>
        [Test]
        public void TestExistedByName_WithExistsParentCategoryName_ReturnTrue()
        {
            const int websiteId = 1;
            const string categoryName = "Category Test 1";
            var result = categoryService.ExistedByName(websiteId, categoryName);
            Assert.IsTrue(result.Result);
        }

        /// <summary>
        /// Existed By Name Category with exist sub category name
        /// </summary>
        [Test]
        public void TestExistedByName_WithExistsSubCategoryName_ReturnTrue()
        {
            const int websiteId = 1;
            const string categoryName = "Category 1 Sub 1";
            var result = categoryService.ExistedByName(websiteId, categoryName);
            Assert.IsTrue(result.Result);
        }

        /// <summary>
        /// Existed By Name Category with not exists category name
        /// </summary>
        [Test]
        public void TestExistedByName_WithNotExistsCategoryName_ReturnFalse()
        {
            const int websiteId = 1;
            const string categoryName = "Category Test 2";
            var result = categoryService.ExistedByName(websiteId, categoryName);
            Assert.IsFalse(result.Result);
        }
        #endregion
    }
}
