using AutoMapper;
using ComputerStore.BoundedContext.Entities;
using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Constants;
using ComputerStore.Structure.Exceptions;
using ComputerStore.Structure.Helper;
using ComputerStore.Structure.Models.Product;
using ComputerStore.UnitOfWork.Interfaces;
using System.Threading.Tasks;

namespace ComputerStore.Domain.Implement
{
   public class ProductImageService : IProductImageService
   {
      private readonly IUnitOfWork unitOfWork;
      private readonly IMapper mapper;

      public ProductImageService(IUnitOfWork unitOfWork, IMapper mapper)
      {
         this.unitOfWork = unitOfWork;
         this.mapper = mapper;
      }

      public async Task DeleteAsync(int websiteId, int imageId, ProductModel productModel)
      {
         var repository = this.unitOfWork.GetRepository<ProductImage>();
         var repositoryProduct = this.unitOfWork.GetRepository<Product>();

         var isProductExist = await repositoryProduct.ExistsAsync(x => x.WebsiteId == websiteId && x.Id == productModel.Id);

         if (!isProductExist)
         {
            throw new NotFoundException(string.Format(Constants.MessageResponse.NotFoundError, nameof(Product), productModel.Id));
         }

         var image = await repository.GetAsync(imageId);

         if (image == null || image.ProductId != productModel.Id)
         {
            throw new NotFoundException(string.Format(Constants.MessageResponse.NotFoundError, nameof(ProductImage), imageId));
         }

         repository.Delete(image);
         await this.unitOfWork.CommitAsync();
      }

      public async Task ImportAsync(int websiteId, ProductModel productModel)
      {
         var repository = this.unitOfWork.GetRepository<Product>();

         var product = await repository.FindByAsync(x => x.Id == productModel.Id && x.WebsiteId == websiteId, "ProductImage");
         if (product == null)
         {
            throw new NotFoundException(
                 string.Format(Constants.MessageResponse.NotFoundError, nameof(Product), productModel.Id));
         }

         var productFolder = FileHelper.CreateProductFolder(product.ProductCode);

         // Copy image from Temp to Products folder and create ProductImage model			
         if (productModel.PathImages != null)
         {
            foreach (var path in productModel.PathImages)
            {
               var filePath = FileHelper.MoveFile(productFolder, path);
               if (string.IsNullOrEmpty(filePath))
               {
                  continue;
               }
               product.ProductImage.Add(new ProductImage()
               {
                  ImageUrl = filePath.Replace("\\", "/"),
                  CreatedDate = System.DateTime.Now
               });
            }
         }

         repository.Update(product);
         await this.unitOfWork.CommitAsync();
      }
   }
}
