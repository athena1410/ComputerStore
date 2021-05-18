using AutoMapper;
using ComputerStore.Api.Attribute;
using ComputerStore.Api.Configuration;
using ComputerStore.Api.Middleware;
using ComputerStore.Structure.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.IO;
using System.Net;

namespace ComputerStore.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownProxies.Add(IPAddress.Parse("10.0.0.100"));
            });

            //Configure loging
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .WriteTo.Console()
                .CreateLogger();
            services.AddSingleton(Log.Logger);

            //Configure db context
            EntityFrameworkConfiguration.ConfigureService(services, Configuration);

            //Add auto mapper on start up
            services.AddAutoMapper(typeof(Startup));

            //Configure dependency injection
            IocContainerConfiguration.ConfigureService(services);

            //Configure jwt 
            JwtConfiguration.ConfigureService(services, Configuration);
            // Configure your policies
            services.AddAuthorization(options =>
            {
                options.AddPolicy(nameof(Role.SuperAdmin),
                  policy => policy.RequireClaim(nameof(Role.SuperAdmin)));

                options.AddPolicy(nameof(Role.Administrator),
                 policy => policy.RequireClaim(nameof(Role.Administrator)));

                options.AddPolicy(nameof(Role.User),
                 policy => policy.RequireClaim(nameof(Role.User)));
            });

            // Configure action filter attribute
            services.AddControllers(opt =>
            {
                opt.Filters.Add(typeof(ValidateModel));
            });

            // Configure enforce lowercase routing
            services.AddRouting(options => options.LowercaseUrls = true);

            SwaggerConfiguration.ConfigureService(services);

            // Config for upload file
            services.Configure<FormOptions>(o => {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });

            // Config Api Version
            services.AddApiVersioning();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            //app.UseHttpsRedirection();

            // Config for upload file
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Assets")),
                RequestPath = new PathString("/Assets")
            });

            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .SetIsOriginAllowed(origin => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();

            //Configure the Swagger API documentation
            SwaggerConfiguration.Configure(app);

            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
