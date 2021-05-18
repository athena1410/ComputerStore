using ComputerStore.Structure.Models.Product;
using ComputerStore.Structure.Models.ProductImage;
using System.Threading.Tasks;

namespace ComputerStore.Domain.Interfaces
{
   public interface IProductImageService
   {
      Task ImportAsync(int websiteId, ProductModel model);

      Task DeleteAsync(int websiteId, int imageId, ProductModel model);
   }
}
