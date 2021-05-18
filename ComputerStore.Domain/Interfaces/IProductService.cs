using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Pagination;
using ComputerStore.Structure.Models.Product;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComputerStore.Domain.Interfaces
{
    public interface IProductService
    {
        /// <summary>
        /// Get product by id
        /// </summary>
        Task<ProductModel> GetByIdAsync(int websiteId, int id);

        /// <summary>
        /// Get all products
        /// </summary>        
        Task<List<ProductModel>> GetAllAsync(int websiteId);

        /// <summary>
        /// Search product
        /// </summary>
        Task<PaginationResponse<List<ProductModel>>> SearchAsync(int websiteId, SearchModel<ProductSearchModel> searchModel);

        /// <summary>
        /// Create new product
        /// </summary>
        Task CreateAsync(int websiteId, ProductModel producProductModel);

        /// <summary>
        /// Update existed product
        /// </summary>
        Task UpdateAsync(int websiteId, int id, ProductModel producProductModel);

        /// <summary>
        /// Change status activate - deactivate
        /// </summary>
        Task ChangeStatusAsync(int websiteId, int id, int status);
        /// <summary>
        ///  Get Max Min price 
        /// </summary>
        /// <returns></returns>
        Task<ThresholdPriceModel> GetThresholdPrice();
        /// <summary>
        /// Check duplicate product code
        /// </summary>
        /// <param name="productCode"></param>
        /// <returns></returns>
        Task<bool> ExistedByProductCode(string productCode);
    }
}
