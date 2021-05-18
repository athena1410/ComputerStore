//-----------------------------------------------------------------------
// <copyright file="CategoryController.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using AutoMapper;
using ComputerStore.BoundedContext.Entities;
using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Constants;
using ComputerStore.Structure.Enums;
using ComputerStore.Structure.Exceptions;
using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Company;
using ComputerStore.Structure.Models.Pagination;
using ComputerStore.UnitOfWork.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ComputerStore.Domain.Implement
{
    public class CompanyService : ICompanyService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public CompanyService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        /// <summary>
        /// Get all companies
        /// </summary>
        /// <returns></returns>
        public async Task<List<CompanyModel>> GetAllAsync()
        {
            var companyRepository = unitOfWork.GetRepository<Company>();
            var companies = await companyRepository.GetAllAsync();
            return mapper.Map<List<CompanyModel>>(companies);
        }

        /// <summary>
        /// Get company active
        /// </summary>
        /// <returns></returns>
        public async Task<List<CompanyModel>> GetByStatus()
        {
            var companyRepository = unitOfWork.GetRepository<Company>();

            Expression<Func<Company, bool>> predicate = x => x.Status == (int)Status.ACTIVE;

            var companies = await companyRepository.GetAllAsync(predicate);
            return mapper.Map<List<CompanyModel>>(companies);
        }

        /// <summary>
        /// Search company
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public async Task<PaginationResponse<List<CompanyModel>>> SearchAsync(SearchModel<CompanySearchModel> searchModel)
        {
            var companyRepository = unitOfWork.GetRepository<Company>();

            //Extract search model generate company search model and paging context
            var (companySearchModel, pagingContext) = searchModel.Extract();

            Expression<Func<Company, bool>> predicate = x =>
            (companySearchModel.Name == null || x.Name.ToLower().Contains(companySearchModel.Name.ToLower())) &&
            (companySearchModel.Address == null || x.Address.ToLower().Contains(companySearchModel.Address.ToLower())) &&
            (companySearchModel.Phone == null || x.Phone.Contains(companySearchModel.Phone)) &&
            (companySearchModel.Status == null || x.Status == companySearchModel.Status);

            var companies = await companyRepository.GetAllAsync(predicate, pagingContext);
            var totalRecord = await companyRepository.CountAsync(predicate);
            return new PaginationResponse<List<CompanyModel>>(
                mapper.Map<List<CompanyModel>>(companies), totalRecord, searchModel.NumberPerPage);
        }

        /// <summary>
        /// Create new Company
        /// </summary>
        /// <param name="companyModel"></param>
        /// <returns></returns>
        public async Task CreateAsync(CompanyModel companyModel)
        {
            var companyRepository = unitOfWork.GetRepository<Company>();
            var existedCompany = await companyRepository.ExistsAsync(c => c.Name.Equals(companyModel.Name));

            if (existedCompany)
            {
                throw new ValidationException(Constants.MessageResponse.CompanyNameExisted);
            }

            var company = mapper.Map<CompanyModel, Company>(companyModel);
            company.CreatedDate = DateTime.UtcNow;
            companyRepository.Add(company);
            await unitOfWork.CommitAsync();
        }

        /// <summary>
        /// Update existed company
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="companyModel"></param>
        /// <returns></returns>
        public async Task UpdateAsync(int companyId, CompanyModel companyModel)
        {
            var companyRepository = unitOfWork.GetRepository<Company>();
            
            var company = await companyRepository.GetAsync(companyId, x => x.Website);
            if (company == null)
            {
                throw new NotFoundException(
                    string.Format(Constants.MessageResponse.NotFoundError,
                        nameof(Company), companyId.ToString()));
            }

            if (company.Name.ToLower() != companyModel.Name.ToLower())
            {
                var existedCompany = await companyRepository.ExistsAsync(c => c.Name.Equals(companyModel.Name));

                if (existedCompany)
                {
                    throw new ValidationException(Constants.MessageResponse.CompanyNameExisted);
                }
            }

            // If company status changed and this has website
            // Update status of related website
            if (company.Status != companyModel.Status &&
                company.Website != null)
            {
                company.Website.Status = companyModel.Status;
                company.Website.UpdatedDate = DateTime.UtcNow;
            }

            mapper.Map(companyModel, company);

            company.UpdatedDate = DateTime.UtcNow;
            companyRepository.Update(company);
            await unitOfWork.CommitAsync();
        }

        /// <summary>
        /// change status activate - deactivate
        /// </summary>
        /// <param name="companyId">company id</param>
        /// <param name="status">status</param>
        public async Task ChangeStatusCompanyAsync(int companyId, int status)
        {
            var companyRepository = unitOfWork.GetRepository<Company>();

            var company = await companyRepository.GetAsync(companyId, x => x.Website);
            if (company == null)
            {
                throw new NotFoundException(
                    string.Format(Constants.MessageResponse.NotFoundError,
                        nameof(Company), companyId.ToString()));
            }

            //Update status of related website
            if (company.Website != null)
            {
                company.Website.Status = status;
                company.Website.UpdatedDate = DateTime.UtcNow;
            }
            
            company.Status = status;
            company.UpdatedDate = DateTime.UtcNow;
            companyRepository.Update(company);
            await unitOfWork.CommitAsync();
        }

        public async Task<CompanyModel> GetByIdAsync(int id)
        {
            var companyRepository = unitOfWork.GetRepository<Company>();
            var company = await companyRepository.GetAsync(id);
            return mapper.Map<CompanyModel>(company);
        }

        public async Task<bool> ExistedByName(string companyName)
        {
            var companyRepository = unitOfWork.GetRepository<Company>();
            return await companyRepository.ExistsAsync(x => x.Name.Equals(companyName.Trim()));    
        }
    }
}
