//-----------------------------------------------------------------------
// <copyright file="CategoryController.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using ComputerStore.BoundedContext.Data;
using ComputerStore.Structure.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ComputerStore.Api.Configuration
{
    public static class EntityFrameworkConfiguration
    {
        /// <summary>
        /// Configures the service.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        public static void ConfigureService(IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString(AppSettings.DefaultConnection);

            // Entity framework configuration
            services.AddDbContext<ComputerStoreContext>(options =>
               options.UseSqlServer(connectionString, optionsBuilder => GetMigrationInformation(optionsBuilder)));

            services.AddScoped<IDbContext, ComputerStoreContext>();
        }

        /// <summary>
        ///  Configures the assembly where migrations are maintained for this context.
        /// </summary>
        /// <typeparam name="TBuilder"></typeparam>
        /// <typeparam name="TExtension"></typeparam>
        /// <param name="builder"></param>
        /// <returns>Migrations configured builder</returns>
        public static TBuilder GetMigrationInformation<TBuilder, TExtension>(RelationalDbContextOptionsBuilder<TBuilder, TExtension> builder)
             where TBuilder : RelationalDbContextOptionsBuilder<TBuilder, TExtension>
            where TExtension : RelationalOptionsExtension, new()
        {

            return builder.MigrationsAssembly(typeof(ComputerStoreContext).Assembly.GetName().Name);
        }
    }
}
