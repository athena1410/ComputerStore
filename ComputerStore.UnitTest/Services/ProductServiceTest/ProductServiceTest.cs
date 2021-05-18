using AutoMapper;
using ComputerStore.Api.Mappings;
using ComputerStore.BoundedContext.Entities;
using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Constants;
using ComputerStore.Structure.Exceptions;
using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Pagination;
using ComputerStore.Structure.Models.Product;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace ComputerStore.UnitTest.Services.ProductServiceTest
{
   [TestFixture]
   public class ProductServiceTest
   {
      private Mapper mapper;
      private List<Product> products;
      private List<Category> categories;
      private PagingContext pagingContext;
      private IProductService productService;
      private ProductModel productModel;

      [SetUp]
      public void Setup()
      {
         var mapperConfiguration = new MapperConfiguration(new MappingProfile());
         mapper = new Mapper(mapperConfiguration);

         categories = new List<Category>()
            {
                new Category()
                {
                    Id = 1,
                    WebsiteId = 1,
                    ParentId = null,
                    Name = "Category has children",
                    TemplateMetaData = "{\"CPU\":\"8BG\", \"RAM\":\"500GB\"}",
                    TemplateSpecificData = "{\"Color\":\"Black\"}",
                    Status = 0
                },
                new Category()
                {
                    Id = 2,
                    WebsiteId = 1,
                    ParentId = null,
                    Name = "Category has'nt children",
                    TemplateMetaData = "{\"CPU\":\"8BG\", \"RAM\":\"500GB\"}",
                    TemplateSpecificData = "{\"Color\":\"Black\"}",
                     Status = 0
                },
                new Category()
                {
                    Id = 3,
                    WebsiteId = 1,
                    ParentId = 1,
                    Name = "SubCategory 1 (parent 1)",
                    TemplateMetaData = "{\"CPU\":\"8BG\", \"RAM\":\"500GB\"}",
                    TemplateSpecificData = "{\"Color\":\"Black\"}",
                     Status = 0
                },
                new Category()
                {
                    Id = 4,
                    WebsiteId = 1,
                    ParentId = 1,
                    Name = "SubCategory 2 (parent 1)",
                    TemplateMetaData = "{\"CPU\":\"8BG\", \"RAM\":\"500GB\"}",
                    TemplateSpecificData = "{\"Color\":\"Black\"}",
                    Status = 0
                },
            };

         products = new List<Product>()
            {
                new Product()
                {
                    WebsiteId = 1,
                    Id = 1,
                    Name = "Laptop Dell Inspiron",
                    ProductCode = "DELL001",
                    Description = "LAPTOP DELL",
                    CategoryId = 2,
                    Discount = 10,
                    Warranty = 12,
                    ViewCount = 0,
                    Price = 100,
                    Quantity = 100,
                    SpecificData = "{\"CPU\":\"8BG\", \"RAM\":\"500GB\"}",
                    MetaData = "{\"Color\":\"Red\", \"RAM\":\"500GB\"}",
                    Status = 0,
                    CreatedDate = DateTime.Now.Date,
                    UpdatedDate = DateTime.Now.Date,
                    ProductImage = new List<ProductImage>(),
                    Category = new Category()
                    {
                         Id = 1,
                        WebsiteId = 1,
                        ParentId = null,
                        Name = "Category Test 1",
                        TemplateMetaData = "{\"CPU\":\"8BG\", \"RAM\":\"500GB\"}",
                        TemplateSpecificData = "{\"Color\":\"Black\"}",
                        Status = 0
                    },
                },
                new Product()
                {
                    WebsiteId = 1,
                    Id = 2,
                    Name = "ASUS",
                    ProductCode = "ASUS001",
                    Description = "LAPTOP ASUS",
                    CategoryId = 3,
                    Discount = 10,
                    Warranty = 12,
                    ViewCount = 0,
                    Price = 200,
                    Quantity = 100,
                    SpecificData = "{\"CPU\":\"8BG\", \"RAM\":\"500GB\"}",
                    MetaData = "{\"Color\":\"Red\", \"RAM\":\"500GB\"}",
                    Status = 0,
                    CreatedDate = DateTime.Now.Date,
                    UpdatedDate = DateTime.Now.Date,
                    ProductImage = new List<ProductImage>(),
                    Category = new Category()
                    {
                         Id = 1,
                        WebsiteId = 1,
                        ParentId = null,
                        Name = "Category Test 1",
                        TemplateMetaData = "{\"CPU\":\"8BG\", \"RAM\":\"500GB\"}",
                        TemplateSpecificData = "{\"Color\":\"Black\"}",
                        Status = 0
                    },
                }
            };

         productModel = new ProductModel()
         {
            CategoryId = 2,            
            Id = 1,
            PathImages = new string[]
            {
               Path.Combine("Assets", "TmpImages", "1.jpg"),
               Path.Combine("Assets", "TmpImages", "2.jpg"),
               Path.Combine("Assets", "TmpImages", "3.jpg"),
            },
         };

         productService = new ProductServiceBuilder()
            .WithRepositoryMock(categories, products, pagingContext)
            .WithUnitOfWorkSetup()
            .WithCategoryService(categories)
            .Build();
      }

      #region ChangeStatus

      /// <summary>
      /// Change status when product is null
      /// => throw not found exception
      /// </summary>
      [Test]
      public void TestChangeStatusAsync_WithInvalidData_ProductNull_ShouldThrowNotFoundException()
      {
         const int websiteId = 1;
         const int productId = 100;
         const int status = (int)Structure.Enums.Status.ACTIVE;
         var ex = Assert.ThrowsAsync<NotFoundException>(() => productService.ChangeStatusAsync(websiteId, productId, status));
         Assert.AreEqual(
                 string.Format(Constants.MessageResponse.NotFoundError, nameof(Product), productId.ToString()), ex.Message);
      }

      /// <summary>
      /// Change status when webisteId of product is not matched with websiteId from Header
      /// => throw not found exception
      /// </summary>
      [Test]
      public void TestChangeStatusAsync_WithInvalidData_WebsiteIdNotCorrect_ShouldThrowNotFoundException()
      {
         const int websiteId = 100;
         const int productId = 1;
         const int status = (int)Structure.Enums.Status.ACTIVE;
         var ex = Assert.ThrowsAsync<NotFoundException>(() => productService.ChangeStatusAsync(websiteId, productId, status));
         Assert.AreEqual(
                 string.Format(Constants.MessageResponse.NotFoundError, nameof(Product), productId.ToString()), ex.Message);
      }

      /// <summary>
      /// Change status with valid data, change success
      /// </summary>
      [Test]
      public void TestChangeStatusAsync_WithValidData_DoesNotThrowException()
      {
         const int websiteId = 1;
         const int productId = 1;
         const int status = (int)Structure.Enums.Status.ACTIVE;
         Assert.DoesNotThrowAsync(() => productService.ChangeStatusAsync(websiteId, productId, status));
      }

      #endregion

      #region Create Product

      /// <summary>
      /// Create product with Category has children
      /// => throw ValidationException exception
      /// </summary>
      [Test]
      public void TestCreateAsync_WithInvalidData_CategoryHasChildren_ShouldThrowValidationException()
      {
         int websiteId = 1;
         var productModel = mapper.Map<ProductModel>(products.FirstOrDefault());
         productModel.CategoryId = 1;

         var ex = Assert.ThrowsAsync<ValidationException>(() => productService.CreateAsync(websiteId, productModel));
         Assert.AreEqual(Constants.MessageResponse.AddProductError, ex.Message);
      }

      /// <summary>
      /// Create product with valid data
      /// => Add product success
      /// </summary>
      [Test]
      public void TestCreateAsync_WithValidData_ShouldAddSuccess()
      {
         int websiteId = 1;
         var productModel = mapper.Map<ProductModel>(products.FirstOrDefault());
         productModel.Name = "Test";
         productModel.ProductCode = "Test";
         productModel.CategoryId = 2;
         productModel.PathImages = new string[0];

         Assert.DoesNotThrowAsync(() => productService.CreateAsync(websiteId, productModel));
      }

      #endregion

      #region GetAllAsync 

      /// <summary>
      /// Get all products
      /// => get all success
      /// </summary>
      [Test]
      public void GetAllAsync_WithValidData_ShouldSuccess()
      {
         var actualProducts = mapper.Map<List<ProductModel>>(this.products.Where(o => o.WebsiteId == 1));

         var products = productService.GetAllAsync(1).Result;

         Assert.IsNotNull(actualProducts);
         Assert.IsInstanceOf<List<ProductModel>>(actualProducts);
         var actualJson = JsonConvert.SerializeObject(actualProducts);
         var expectJson = JsonConvert.SerializeObject(products);
         Assert.AreEqual(expectJson, actualJson);
      }

      #endregion

      #region GetByIdAsync

      /// <summary>
      /// Get product by ID with valid data
      /// => get product success
      /// </summary>
      [Test]
      public void GetByIdAsync_WithValidData_ShouldSuccess()
      {
         var expected = mapper.Map<ProductModel>(products.Where(o => o.WebsiteId == 1 && o.Id == 1).FirstOrDefault());
         var actualProduct = productService.GetByIdAsync(1, 1).Result;

         Assert.IsNotNull(actualProduct);
         Assert.IsInstanceOf<ProductModel>(actualProduct);

         var actualJson = JsonConvert.SerializeObject(actualProduct);
         var expectJson = JsonConvert.SerializeObject(expected);
         Assert.AreEqual(expectJson, actualJson);
      }

      /// <summary>
      /// Get product with ID not found
      /// => Throw NotFoundException
      /// </summary>
      [Test]
      public void GetByIdAsync_WithNotFoundId_ShouldThrowNotFoundException()
      {
         var ex = Assert.ThrowsAsync<NotFoundException>(() => productService.GetByIdAsync(1, 100));
         Assert.AreEqual(ex.Message, string.Format(Constants.MessageResponse.NotFoundError, nameof(Product), 100));
      }

      #endregion

      #region GetThresholdPrice

      /// <summary>
      /// Get product by ID with valid data
      /// => get product success
      /// </summary>
      [Test]
      public void GetThresholdPrice_WithValidData_ShouldSuccess()
      {
         var min = products.Min(o => o.Price);
         var max = products.Max(o => o.Price);

         var thresholdPrice = productService.GetThresholdPrice().Result;
         Assert.AreEqual(thresholdPrice.MinPrice, min);
         Assert.AreEqual(thresholdPrice.MaxPrice, max);
      }

      #endregion

      #region UpdateAsync

      /// <summary>
      /// Update base information of product with valid data
      /// 
      /// </summary>
      [Test]
      public void UpdateAsync_UpdateBaseInfo_WithValidData_ShouldSuccess()
      {
         int websiteId = 1;
         var productModel = mapper.Map<ProductModel>(products.FirstOrDefault());
         productModel.CategoryId = 2;         

         Assert.DoesNotThrowAsync(() => productService.UpdateAsync(websiteId, 1, productModel));
      }

      /// <summary>
      /// Update base information of product with not found product
      /// => Throw NotFoundException
      /// </summary>
      [Test]
      public void UpdateAsync_UpdateBaseInfo_NotFoundProduct_ShouldThrowNotFoundException()
      {
         int websiteId = 1;
         int productId = 100;
         productModel.CategoryId = 2;

         var ex = Assert.ThrowsAsync<NotFoundException>(() => productService.UpdateAsync(websiteId, productId, productModel));
         Assert.AreEqual(ex.Message, string.Format(Constants.MessageResponse.NotFoundError, nameof(Product), productId));
      }

      #endregion

      #region SearchAsync		

      /// <summary>
      /// Search product with product name		
      /// </summary>
      [Test]
      public void SearchAsync_WithInputName_ShouldSuccess()
      {
         var expected = new PaginationResponse<List<ProductModel>>(
             new List<ProductModel>() { mapper.Map<ProductModel>(products.Last()) },
             1,
             10
         );

         var searchInput = new SearchModel<ProductSearchModel>()
         {
            Data = new ProductSearchModel() { Name = "ASUS" }
         };

         var actual = productService.SearchAsync(1, searchInput).Result;

         var actualJson = JsonConvert.SerializeObject(actual);
         var expectJson = JsonConvert.SerializeObject(expected);
         Assert.AreEqual(expectJson, actualJson);
      }

      /// <summary>
      /// Search product with product code		
      /// </summary>
      [Test]
      public void SearchAsync_WithInputProductCode_ShouldSuccess()
      {
         var expected = new PaginationResponse<List<ProductModel>>(
             new List<ProductModel>() { mapper.Map<ProductModel>(products.Last()) },
             1,
             10
         );

         var searchInput = new SearchModel<ProductSearchModel>()
         {
            Data = new ProductSearchModel() { Name = "ASUS001" }
         };

         var actual = productService.SearchAsync(1, searchInput).Result;

         var actualJson = JsonConvert.SerializeObject(actual);
         var expectJson = JsonConvert.SerializeObject(expected);
         Assert.AreEqual(expectJson, actualJson);
      }

      /// <summary>
      /// Search product with not found		
      /// </summary>
      [Test]
      public void SearchAsync_WithInputNotFound_ShouldSuccess()
      {
         var expected = new PaginationResponse<List<ProductModel>>(
             new List<ProductModel>(),
             0,
             10
         );

         var searchInput = new SearchModel<ProductSearchModel>()
         {
            Data = new ProductSearchModel() { Name = "ABCDEF" }
         };

         var actual = productService.SearchAsync(1, searchInput).Result;

         var actualJson = JsonConvert.SerializeObject(actual);
         var expectJson = JsonConvert.SerializeObject(expected);
         Assert.AreEqual(expectJson, actualJson);
      }

      // TH1: Name = Dell=> đưa ra dell
      // TH2: Name = code
      // TH2: Name = "XDFDFS" => Not found

      // TH3: MinPrice > 2 sản phẩm => Null
      // TH4: MinPrice < Sản phẩm => Ra thằng mong muốn
      // Th5: MaxPrice < 2 sản phẩm => Null
      // TH6: MaxPrice > 1 sản phẩm => ra 1
      // TH7: CateogryIds

      /// <summary>
      /// Search product with MinPrice	
      /// </summary>
      [Test]
      public void SearchAsync_WithMinPriceNotFound_ShouldSuccess()
      {
         var expected = new PaginationResponse<List<ProductModel>>(
             new List<ProductModel>(),
             0,
             10
         );

         var searchInput = new SearchModel<ProductSearchModel>()
         {
            Data = new ProductSearchModel() { MinPrice = 500 }
         };

         var actual = productService.SearchAsync(1, searchInput).Result;

         var actualJson = JsonConvert.SerializeObject(actual);
         var expectJson = JsonConvert.SerializeObject(expected);
         Assert.AreEqual(expectJson, actualJson);
      }

      /// <summary>
      /// Search product with MinPrice	
      /// </summary>
      [Test]
      public void SearchAsync_WithMinPriceValid_ShouldSuccess()
      {
         var expected = new PaginationResponse<List<ProductModel>>(
             new List<ProductModel>() { mapper.Map<ProductModel>(products.Last()) },
             1,
             10
         );

         var searchInput = new SearchModel<ProductSearchModel>()
         {
            Data = new ProductSearchModel() { MinPrice = 150 }
         };

         var actual = productService.SearchAsync(1, searchInput).Result;

         var actualJson = JsonConvert.SerializeObject(actual);
         var expectJson = JsonConvert.SerializeObject(expected);
         Assert.AreEqual(expectJson, actualJson);
      }

      /// <summary>
      /// Search product with MaxPrice	
      /// </summary>
      [Test]
      public void SearchAsync_WithMaxPriceNotFound_ShouldSuccess()
      {
         var expected = new PaginationResponse<List<ProductModel>>(
             new List<ProductModel>(),
             0,
             10
         );

         var searchInput = new SearchModel<ProductSearchModel>()
         {
            Data = new ProductSearchModel() { MaxPrice = 50 }
         };

         var actual = productService.SearchAsync(1, searchInput).Result;

         var actualJson = JsonConvert.SerializeObject(actual);
         var expectJson = JsonConvert.SerializeObject(expected);
         Assert.AreEqual(expectJson, actualJson);
      }

      /// <summary>
      /// Search product with MaxPrice
      /// </summary>
      [Test]
      public void SearchAsync_WithMaxPriceValid_ShouldSuccess()
      {
         var expected = new PaginationResponse<List<ProductModel>>(
             new List<ProductModel>() { mapper.Map<ProductModel>(products.First()) },
             1,
             10
         );

         var searchInput = new SearchModel<ProductSearchModel>()
         {
            Data = new ProductSearchModel() { MaxPrice = 150 }
         };

         var actual = productService.SearchAsync(1, searchInput).Result;

         var actualJson = JsonConvert.SerializeObject(actual);
         var expectJson = JsonConvert.SerializeObject(expected);
         Assert.AreEqual(expectJson, actualJson);
      }

      /// <summary>
      /// Search product by CategoryId
      /// </summary>
      public void SearchAsync_WithCateogoryId_ShouldSuccess()
      {
         var expected = new PaginationResponse<List<ProductModel>>(
             new List<ProductModel>() { mapper.Map<ProductModel>(products.First()) },
             1,
             10
         );

         var searchInput = new SearchModel<ProductSearchModel>()
         {
            Data = new ProductSearchModel() { CategoryIds = new int[] { 2 } }
         };

         var actual = productService.SearchAsync(1, searchInput).Result;

         var actualJson = JsonConvert.SerializeObject(actual);
         var expectJson = JsonConvert.SerializeObject(expected);
         Assert.AreEqual(expectJson, actualJson);
      }

      #endregion
   }
}
