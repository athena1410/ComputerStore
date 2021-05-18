//-----------------------------------------------------------------------
// <copyright file="SwaggerConfiguration.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using ComputerStore.Structure.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace ComputerStore.Api.Configuration
{
    /// <summary>
    /// Swagger API documentation components start-up configuration
    /// </summary>
    public static class SwaggerConfiguration
    {
        private static IEnumerable<int> Versions => new[] {1, 2};

        /// <summary>
        /// Configures the service.
        /// </summary>
        /// <param name="services">The services.</param>
        public static void ConfigureService(IServiceCollection services)
        {
            // Swagger API documentation
            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<AuthorizationHeaderParameterOperationFilter>();

                foreach (var v in Versions)
                {
                    c.SwaggerDoc($"v{v}",
                    new OpenApiInfo
                    {
                        Title = $"Computer Store Api ver{v}",
                        Version = $"v{v}",
                        Description = "Dotnet core multi tenant application",
                    });
                }

                c.OperationFilter<RemoveVersionFromParameter>();

                c.DocumentFilter<ReplaceVersionWithExactValueInPath>();

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                        Reference = new OpenApiReference
                            {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
            });

            services.AddVersionedApiExplorer(o =>
            {
                o.GroupNameFormat = "'v'VVV";
                o.SubstituteApiVersionInUrl = true;
            });
        }

        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        public static void Configure(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                foreach (var v in Versions)
                {
                    c.SwaggerEndpoint($"/swagger/v{v}/swagger.json", $"Computer Store Api v{v}");
                }
                
                c.OAuthClientId(AppSettings.SwaggerClientId);
                c.OAuthAppName("Computer Store Api Swagger Ui");
            });
        }
    }

    public class AuthorizationHeaderParameterOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Adds an authorization header to the given operation in Swagger.
        /// </summary>
        /// <param name="operation">The Swashbuckle operation.</param>
        /// <param name="context">The Swashbuckle operation filter context.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!(context.ApiDescription.ActionDescriptor is ControllerActionDescriptor)) return;

            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "website-id",
                In = ParameterLocation.Header,
                Description = "Website Id",
                Required = false,
            });
        }
    }

    /// <summary>
    /// Remove version from parameter
    /// </summary>
    public class RemoveVersionFromParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var versionParameter = operation.Parameters.FirstOrDefault(p => p.Name == "version");
            operation.Parameters.Remove(versionParameter);
        }
    }

    /// <summary>
    /// Replace version with exactly value in path
    /// </summary>
    public class ReplaceVersionWithExactValueInPath : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = new OpenApiPaths();
            foreach (var path in swaggerDoc.Paths)
            {
                paths.Add(path.Key.Replace("v{version}", swaggerDoc.Info.Version), path.Value);
            }
            swaggerDoc.Paths = paths;
        }
    }
}
