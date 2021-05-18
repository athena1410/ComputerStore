using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Company;
using ComputerStore.Structure.Models.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComputerStore.Domain.Interfaces
{
    public interface ICompanyService
    {
        /// <summary>
        /// Create new Company
        /// </summary>
        /// <param name="companyModel"></param>
        /// <returns></returns>
        Task CreateAsync(CompanyModel companyModel);
        /// <summary>
        /// Update existed company
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="companyModel"></param>
        /// <returns></returns>
        Task UpdateAsync(int companyId, CompanyModel companyModel);
        /// <summary>
        /// Get all companies
        /// </summary>
        /// <returns></returns>
        Task<List<CompanyModel>> GetAllAsync();
        /// <summary>
        /// Get company active
        /// </summary>
        /// <returns></returns>
        Task<List<CompanyModel>> GetByStatus();
        /// <summary>
        /// Search company
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        Task<PaginationResponse<List<CompanyModel>>> SearchAsync(SearchModel<CompanySearchModel> searchModel);
        /// <summary>
        /// Get Company by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<CompanyModel> GetByIdAsync(int id);

        /// <summary>
        /// Change status activate - deactivate
        /// </summary>
        /// <param name="companyId">company id</param>
        /// <param name="status">status</param>
        Task ChangeStatusCompanyAsync(int companyId, int status);

        /// <summary>
        /// Check existed company by name
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        Task<bool> ExistedByName(string companyName);
    }
}
