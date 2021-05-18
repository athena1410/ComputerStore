//-----------------------------------------------------------------------
// <copyright file="ProductService.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>QuangNV25</author>
//-----------------------------------------------------------------------

using AutoMapper;
using ComputerStore.BoundedContext.Entities;
using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Constants;
using ComputerStore.Structure.Exceptions;
using ComputerStore.Structure.Helper;
using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Pagination;
using ComputerStore.Structure.Models.Product;
using ComputerStore.UnitOfWork.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ComputerStore.Domain.Implement
{
   public class ProductService : IProductService
   {
      private readonly IUnitOfWork unitOfWork;
      private readonly IMapper mapper;
      private readonly ICategoryService categoryService;

      public ProductService(IUnitOfWork unitOfWork, IMapper mapper, ICategoryService categoryService)
      {
         this.unitOfWork = unitOfWork;
         this.mapper = mapper;
         this.categoryService = categoryService;
      }

      /// <summary>
      /// Change status of product
      /// </summary>
      public async Task ChangeStatusAsync(int websiteId, int id, int status)
      {
         var repository = this.unitOfWork.GetRepository<Product>();

         var product = await repository.GetAsync(id);
         if (product == null || product.WebsiteId != websiteId)
         {
            throw new NotFoundException(string.Format(Constants.MessageResponse.NotFoundError, nameof(Product), id));
         }

         product.Status = status;
         product.UpdatedDate = DateTime.UtcNow;
         repository.Update(product);
         await this.unitOfWork.CommitAsync();
      }

      /// <summary>
      /// Create new product
      /// </summary>
      public async Task CreateAsync(int websiteId, ProductModel productModel)
      {
         await this.CheckCreateUpdateModel(websiteId, productModel);
         var isProductCodeExisted = await this.ExistedByProductCode(productModel.ProductCode);
         if (isProductCodeExisted)
         {
            throw new ValidationException(Constants.MessageResponse.ProductCodeExisted);
         }        

         var repository = this.unitOfWork.GetRepository<Product>();
         var product = this.mapper.Map<ProductModel, Product>(productModel);
         product.CreatedDate = DateTime.UtcNow;

         var productFolder = FileHelper.CreateProductFolder(product.ProductCode);

         // Copy image from Temp to Products folder and create ProductImage model			
         if (productModel.PathImages != null)
         {
            product.ProductImage = new List<ProductImage>();
            foreach (var path in productModel?.PathImages)
            {
               var filePath = FileHelper.MoveFile(productFolder, path);
               if (string.IsNullOrEmpty(filePath))
               {
                  continue;
               }

               product.ProductImage.Add(new ProductImage() { ImageUrl = filePath.Replace("\\", "/") });
            }
         }

         repository.Add(product);
         await this.unitOfWork.CommitAsync();
      }

      public async Task<bool> ExistedByProductCode(string productCode)
      {
         var productRepository = unitOfWork.GetRepository<Product>();
         return await productRepository.ExistsAsync(x => x.ProductCode.Equals(productCode.Trim()));
      }

      /// <summary>
      /// Get all products
      /// </summary>
      public async Task<List<ProductModel>> GetAllAsync(int websiteId)
      {
         var repository = this.unitOfWork.GetRepository<Product>();
         var products = await repository.GetAllAsync(x => x.WebsiteId == websiteId);
         return this.mapper.Map<List<ProductModel>>(products);
      }

      /// <summary>
      /// Get product by id
      /// </summary>
      public async Task<ProductModel> GetByIdAsync(int websiteId, int id)
      {
         var repository = this.unitOfWork.GetRepository<Product>();
         var product = await repository.FindByAsync(x => x.Id == id &&
              x.WebsiteId == websiteId, "ProductImage");

         if (product == null)
         {
            throw new NotFoundException(
                  string.Format(Constants.MessageResponse.NotFoundError, nameof(Product), id));
         }

         return this.mapper.Map<Product, ProductModel>(product);
      }

      /// <summary>
      /// Get threshold price of product
      /// </summary>
      /// <returns></returns>
      public async Task<ThresholdPriceModel> GetThresholdPrice()
      {
         var repository = this.unitOfWork.GetRepository<Product>();
         var max = await repository.GetMaxAsync(s => s.Price);
         var min = await repository.GetMinAsync(s => s.Price);
         return new ThresholdPriceModel()
         {
            MaxPrice = max,
            MinPrice = min
         };
      }

      /// <summary>
      /// Search product with paging
      /// </summary>
      /// <param name="websiteId"></param>
      /// <param name="searchModel"></param>
      /// <returns></returns>
      public async Task<PaginationResponse<List<ProductModel>>> SearchAsync(int websiteId, SearchModel<ProductSearchModel> searchModel)
      {
         var repository = this.unitOfWork.GetRepository<Product>();
         var queryString = searchModel.Data.Name?.ToUpper() ?? string.Empty;
         var (productSearchModel, pagingContext) = searchModel.Extract();
         Expression<Func<Product, bool>> predicate = x =>
                   x.WebsiteId == websiteId
                   &&
                   (x.Category.Name.ToUpper().Contains(queryString)
                   || x.Description.ToUpper().Contains(queryString)
                   || x.MetaData.ToUpper().Contains(queryString)
                   || x.Name.ToUpper().Contains(queryString)
                   || x.ProductCode.ToUpper().Contains(queryString)
                   || x.SpecificData.ToUpper().Contains(queryString)
                   )
                   &&
                   (productSearchModel.MinPrice == null || x.Price >= productSearchModel.MinPrice)
                   &&
                   (productSearchModel.MaxPrice == null || x.Price <= productSearchModel.MaxPrice)
                   &&
                   (productSearchModel.CategoryIds == null || productSearchModel.CategoryIds.Length == 0 || productSearchModel.CategoryIds.Contains(x.CategoryId));

         var includes = new[] { nameof(ProductImage) };
         var products = await repository.GetAllAsync(predicate, pagingContext, includes);
         var totalRecord = await repository.CountAsync(predicate);
         return new PaginationResponse<List<ProductModel>>(
              mapper.Map<List<ProductModel>>(products), totalRecord, searchModel.NumberPerPage);
      }

      /// <summary>
      /// Update product information
      /// </summary>
      public async Task UpdateAsync(int websiteId, int id, ProductModel productModel)
      {
         await this.CheckCreateUpdateModel(websiteId, productModel);
         var repository = this.unitOfWork.GetRepository<Product>();

         var product = await repository.FindByAsync(x => x.Id == id && x.WebsiteId == websiteId);

         if (product == null)
         {
            throw new NotFoundException(
                 string.Format(Constants.MessageResponse.NotFoundError, nameof(Product), id));
         }

         await ValidateProductCode(product, productModel.ProductCode.Trim());
         this.mapper.Map(productModel, product);

         product.Id = id;
         product.UpdatedDate = DateTime.UtcNow;
         repository.Update(product);
         await this.unitOfWork.CommitAsync();
      }

      private async Task ValidateProductCode(Product product, string newProductCode)
      {
         if (!product.ProductCode.Equals(newProductCode))
         {
            var repository = this.unitOfWork.GetRepository<Product>();
            var isProductCodeExisted = await repository.ExistsAsync(x => x.Id != product.Id && x.ProductCode.Equals(newProductCode));
            if (isProductCodeExisted)
            {
               throw new ValidationException(Constants.MessageResponse.ProductCodeExisted);
            }
         }
      }

      /// <summary>
      /// Validate product model
      /// </summary>
      /// <param name="websiteId"></param>
      /// <param name="productModel"></param>
      /// <returns></returns>
      private async Task CheckCreateUpdateModel(int websiteId, ProductModel productModel)
      {
         if (productModel.Discount < 0)
         {
            throw new ValidationException(Constants.MessageResponse.InvalidDiscount);
         }

         if (productModel.Price < 0)
         {
            throw new ValidationException(Constants.MessageResponse.InvalidPrice);
         }

         if (productModel.Quantity < 0)
         {
            throw new ValidationException(Constants.MessageResponse.InvalidQuantity);
         }

         if (productModel.Warranty < 0)
         {
            throw new ValidationException(Constants.MessageResponse.InvalidWaranty);
         }

         // Throw exception if not found Category
         await categoryService.GetByIdAsync(websiteId, productModel.CategoryId);

         // Get all children of category, if category has children then throw exception
         var subCategories = await this.categoryService.GetChildrenAsync(websiteId, productModel.CategoryId);
         if (subCategories.Any())
         {
            throw new ValidationException(Constants.MessageResponse.AddProductError);
         }
      }
   }
}
