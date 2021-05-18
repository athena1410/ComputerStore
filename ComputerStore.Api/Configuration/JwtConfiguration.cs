//-----------------------------------------------------------------------
// <copyright file="JwtConfiguration.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Constants;
using ComputerStore.Structure.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputerStore.Api.Configuration
{
    public static class JwtConfiguration
    {
        /// <summary>
        /// Configure jwt authentication
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        public static void ConfigureService(IServiceCollection services, IConfiguration configuration)
        {
            var jwtConfigurationSection = configuration.GetSection(AppSettings.JwtConfiguration);
            services.Configure<JwtSettings>(jwtConfigurationSection);
            var jwtKey = jwtConfigurationSection.Get<JwtSettings>().SecretKey;

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
                    {
                        string secretKey;
                        if (string.IsNullOrEmpty(kid))
                        {
                            secretKey = jwtKey;
                        }
                        else
                        {
                            var websiteService = services.BuildServiceProvider().GetService<IWebsiteService>();
                            var website = websiteService.GetByIdAsync(Convert.ToInt32(kid))
                                .ConfigureAwait(false).GetAwaiter().GetResult();
                            secretKey = website?.SecretKey;
                        }
                        
                        var keys = new List<SecurityKey>();
                        if (secretKey == null) return keys;
                        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                        keys.Add(signingKey);
                        return keys;
                    },
                    // set clock skew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                };
            });
        }
    }
}
