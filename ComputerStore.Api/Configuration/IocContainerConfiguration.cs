//-----------------------------------------------------------------------
// <copyright file="CategoryController.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using ComputerStore.Api.Attribute;
using ComputerStore.BoundedContext.Data.DataSeed;
using ComputerStore.Domain.Implement;
using ComputerStore.Domain.Interfaces;
using ComputerStore.UnitOfWork.Implement;
using ComputerStore.UnitOfWork.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ComputerStore.Api.Configuration
{
   public static class IocContainerConfiguration
   {
      /// <summary>
      /// Configures dependency injection 
      /// </summary>
      /// <param name="services">The services.</param>
      public static void ConfigureService(IServiceCollection services)
      {
         services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
         services.AddTransient<IUnitOfWork, UnitOfWork.Implement.UnitOfWork>();
         services.AddTransient<IDataSeeder, DataSeeder>();
         services.AddTransient<IAuthenticationService, AuthenticationService>();
         services.AddTransient<ICompanyService, CompanyService>();
         services.AddTransient<IUserService, UserService>();
         services.AddTransient<IWebsiteService, WebsiteService>();
         services.AddTransient<ICategoryService, CategoryService>();
         services.AddTransient<IProductService, ProductService>();
         services.AddTransient<IOrderService, OrderService>();
         services.AddTransient<IProductImageService, ProductImageService>();
         services.AddTransient<ICartService, CartService>();
         services.AddTransient<IAnonymousCartService, AnonymousCartService>();
         services.AddScoped(typeof(ValidateWebsite));
         services.AddScoped(typeof(SecondAuthorize));
      }
   }
}
