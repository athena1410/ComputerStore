using AutoMapper;
using ComputerStore.BoundedContext.Entities;
using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Constants;
using ComputerStore.Structure.Enums;
using ComputerStore.Structure.Exceptions;
using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Pagination;
using ComputerStore.Structure.Models.Website;
using ComputerStore.UnitOfWork.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ComputerStore.Domain.Implement
{
    public class WebsiteService : IWebsiteService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public WebsiteService(
            IUnitOfWork unitOfWork,
            IMapper mapper
        )
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        /// <summary>
        /// Get all websites
        /// </summary>
        /// <returns></returns>
        public async Task<List<WebsiteModel>> GetAllAsync(Status status)
        {
            var websiteRepository = unitOfWork.GetRepository<Website>();
            var websites = await websiteRepository.GetAllAsync(x => x.Status == (int)status);
            return mapper.Map<List<WebsiteModel>>(websites);
        }

        /// <summary>
        /// Get website by website id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<WebsiteModel> GetByIdAsync(int id)
        {
            var websiteRepository = unitOfWork.GetRepository<Website>();
            var website = await websiteRepository.GetAsync(id);
            return mapper.Map<Website, WebsiteModel>(website);
        }

        /// <summary>
        /// Search website
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public async Task<PaginationResponse<List<WebsiteModel>>> SearchAsync(SearchModel<WebsiteSearchModel> searchModel)
        {
            var websiteRepository = unitOfWork.GetRepository<Website>();

            //Extract search model generate website search model and paging context
            var (websiteSearchModel, pagingContext) = searchModel.Extract();

            Expression<Func<Website, bool>> predicate = x =>
            (websiteSearchModel.Name == null || x.Name.ToLower().Contains(websiteSearchModel.Name.ToLower()));

            var websites = await websiteRepository.GetAllAsync(predicate, pagingContext, new[] {"Company"});
            var totalRecord = await websiteRepository.CountAsync(predicate);
            return new PaginationResponse<List<WebsiteModel>>(
                mapper.Map<List<WebsiteModel>>(websites), totalRecord, searchModel.NumberPerPage);
        }

        /// <summary>
        /// Create new Website
        /// </summary>
        /// <param name="websiteModel"></param>
        /// <returns></returns>
        public async Task<int> CreateAsync(WebsiteModel websiteModel)
        {
            var websiteRepository = unitOfWork.GetRepository<Website>();
            var companyRepository = unitOfWork.GetRepository<Company>();
            var oldPath = "";
            if (!string.IsNullOrEmpty(websiteModel.LogoUrl))
            {
                oldPath = Path.Combine(Environment.CurrentDirectory, websiteModel.LogoUrl);
                var logoUrl = $"Assets\\Images\\{websiteModel.UrlPath}_{DateTime.Now:yyMMddHHmmss}{Path.GetExtension(oldPath)}";
                string newPath = Path.Combine(Environment.CurrentDirectory, logoUrl);
                File.Move(oldPath, newPath);
                websiteModel.LogoUrl = logoUrl;
            }

            var existedName = await websiteRepository.ExistsAsync(c => c.Name.Equals(websiteModel.Name));
            if (existedName)
            {
                if (!string.IsNullOrEmpty(websiteModel.LogoUrl)) File.Delete(oldPath);
                throw new ValidationException(Constants.MessageResponse.WebsiteNameExisted);
            }

            var existedUrlPath = await websiteRepository.ExistsAsync(c => c.UrlPath.Equals(websiteModel.UrlPath));
            if (existedUrlPath)
            {
                if (!string.IsNullOrEmpty(websiteModel.LogoUrl)) File.Delete(oldPath);
                throw new ValidationException(Constants.MessageResponse.WebsiteUrlPathExisted);
            }

            var company = await companyRepository.FindByAsync(o => o.Id == websiteModel.CompanyId, nameof(Website));
            if (company == null)
            {
                if (!string.IsNullOrEmpty(websiteModel.LogoUrl)) File.Delete(oldPath);
                throw new NotFoundException( string.Format(Constants.MessageResponse.NotFoundError,
                        nameof(Company), websiteModel.CompanyId.ToString()));
            }
            if (company.Website != null)
            {
                if (!string.IsNullOrEmpty(websiteModel.LogoUrl)) File.Delete(oldPath);
                throw new ValidationException(Constants.MessageResponse.UniqueWebsitePerCompanyError);
            }
           
            var website = mapper.Map<WebsiteModel, Website>(websiteModel);
            website.CreatedDate = DateTime.UtcNow;
            website.SecretKey = Guid.NewGuid().ToString();
            websiteRepository.Add(website);
            await unitOfWork.CommitAsync();

            return website.Id;
        }

        /// <summary>
        /// Update existed website
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="websiteModel"></param>
        /// <returns></returns>
        public async Task UpdateAsync(int websiteId, UpdateWebsiteModel websiteModel)
        {
            var websiteRepository = unitOfWork.GetRepository<Website>();
            var companyRepository = unitOfWork.GetRepository<Company>();
            var oldPath = "";
            if (websiteModel.IsNewImage)
            {
                oldPath = Path.Combine(Environment.CurrentDirectory, websiteModel.LogoUrl);
                var logoUrl = $"Assets\\Images\\{websiteModel.UrlPath}_{DateTime.Now:yyMMddHHmmss}{Path.GetExtension(oldPath)}";
                string newPath = Path.Combine(Environment.CurrentDirectory, logoUrl);
                File.Move(oldPath, newPath);
                websiteModel.LogoUrl = logoUrl;
            }
          
            var website = await websiteRepository.GetAsync(websiteId);
            if (website == null)
            {
                if (websiteModel.IsNewImage) File.Delete(oldPath);
                throw new NotFoundException(
                    string.Format(Constants.MessageResponse.NotFoundError,
                        nameof(Website), websiteId.ToString()));
            }
            if (website.Name.ToLower() != websiteModel.Name.ToLower())
            {
                var isExistedName = await websiteRepository.ExistsAsync(c => c.Name.Equals(websiteModel.Name));

                if (isExistedName)
                {
                    if (websiteModel.IsNewImage) File.Delete(oldPath);
                    throw new ValidationException(Constants.MessageResponse.WebsiteNameExisted);
                }
            }
            if (website.UrlPath != websiteModel.UrlPath)
            {
                var isExistedUrlPath = await websiteRepository.ExistsAsync(c => c.UrlPath.Equals(websiteModel.UrlPath));

                if (isExistedUrlPath)
                {
                    if (websiteModel.IsNewImage) File.Delete(oldPath);
                    throw new ValidationException(Constants.MessageResponse.WebsiteUrlPathExisted);
                }
            }

            var company = await companyRepository.GetAsync(websiteModel.CompanyId);
            if (company == null)
            {
                if (websiteModel.IsNewImage) File.Delete(oldPath);
                throw new NotFoundException(string.Format(Constants.MessageResponse.NotFoundError,
                        nameof(Company), websiteModel.CompanyId.ToString()));
            }
            if (company != null && websiteModel.CompanyId != website.CompanyId)
            {
                if (websiteModel.IsNewImage) File.Delete(oldPath);
                throw new ValidationException(Constants.MessageResponse.UniqueWebsitePerCompanyError);
            }

            mapper.Map(websiteModel, website);
            website.UpdatedDate = DateTime.UtcNow;
            websiteRepository.Update(website);
            await unitOfWork.CommitAsync();
        }

        /// <summary>
        /// change status activate - deactivate
        /// </summary>
        /// <param name="websiteId">website id</param>
        /// <param name="status">status</param>
        public async Task ChangeStatusAsync(int websiteId, int status)
        {
            var websiteRepository = unitOfWork.GetRepository<Website>();

            var website = await websiteRepository.GetAsync(websiteId);
            if (website == null)
            {
                throw new NotFoundException(
                    string.Format(Constants.MessageResponse.NotFoundError,
                        nameof(Website), websiteId.ToString()));
            }
            website.Status = status;
            website.UpdatedDate = DateTime.UtcNow;
            websiteRepository.Update(website);
            await unitOfWork.CommitAsync();
        }

        /// <summary>
        /// Get website image url
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> GetLogoUrl(int id)
        {
            var websiteRepository = unitOfWork.GetRepository<Website>();
            var website = await websiteRepository.GetAsync(id);
            return website?.LogoUrl;
        }
    }
}
