//-----------------------------------------------------------------------
// <copyright file="AuthenticationService.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using AutoMapper;
using ComputerStore.BoundedContext.Entities;
using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Enums;
using ComputerStore.Structure.Helper;
using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Authentication;
using ComputerStore.Structure.Models.User;
using ComputerStore.UnitOfWork.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static ComputerStore.Structure.Constants.Constants;
using Role = ComputerStore.Structure.Enums.Role;

namespace ComputerStore.Domain.Implement
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly JwtSettings jwtSettings;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public AuthenticationService(
            IUnitOfWork unitOfWork,
            IOptions<JwtSettings> option,
            IMapper mapper
        )
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            jwtSettings = option.Value;
        }

        /// <summary>
        /// Process authentice to generate token
        /// </summary>
        /// <param name="model"></param>
        /// <param name="websiteId"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, int? websiteId, string ipAddress)
        {
            var userRepository = unitOfWork.GetRepository<User>();
            var websiteRepository = unitOfWork.GetRepository<Website>();
            User user;
            Website website = null;

            if (websiteId.HasValue)
            {
                user = await userRepository.FindByAsync(x => x.Email == model.Email &&
                                x.WebsiteId == websiteId, nameof(RefreshToken));

                website = await websiteRepository.GetAsync(websiteId);
                if (website != null && website.Status == (int)Status.DEACTIVATE)
                {
                    throw new ValidationException(MessageResponse.WebsiteNotValid);
                }
            }
            else
            {
                user = await userRepository.FindByAsync(x => x.Email == model.Email &&
                                 x.RoleId == (int)Role.SuperAdmin && x.WebsiteId == null, nameof(RefreshToken));
            }

            // return null if user not found
            if (user == null) return null;

            // validate password
            var isValid = Security.ValidatePassword(user.Email, model.Password, user.Password);
            if (!isValid) return null;
            // authentication successful so generate jwt and refresh tokens
            var jwtToken = GenerateJwtToken(user, website);
            var refreshToken = GenerateRefreshToken(ipAddress);

            // save refresh token
            user.RefreshToken.Add(refreshToken);
            userRepository.Update(user);
            await unitOfWork.CommitAsync();
            var useModel = mapper.Map<User, UserModel>(user);
            return new AuthenticateResponse(useModel, jwtToken, refreshToken.Token);
        }

        /// <summary>
        /// Receive refresh token to generate new access token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public async Task<AuthenticateResponse> RefreshToken(string token, string ipAddress)
        {
            var userRepository = unitOfWork.GetRepository<User>();
            var websiteRepository = unitOfWork.GetRepository<Website>();
            var user = await userRepository.FindByAsync(x => 
                    x.RefreshToken.Any(r => r.Token.Equals(token)), nameof(RefreshToken));

            // return null if no user found with token
            if (user == null) return null;

            Website website = null;
            if (user.WebsiteId.HasValue)
            {
                website = await websiteRepository.GetAsync(user.WebsiteId);
                if (website != null && website.Status == (int)Status.DEACTIVATE)
                {
                    throw new ValidationException(MessageResponse.WebsiteNotValid);
                }
            }

            var refreshToken = user.RefreshToken.Single(x => x.Token == token);

            // return null if token is no longer active
            if (!refreshToken.IsActive) return null;

            // replace old refresh token with a new one and save
            var newRefreshToken = GenerateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            user.RefreshToken.Add(newRefreshToken);
            userRepository.Update(user);
            await unitOfWork.CommitAsync();

            // generate new jwt
            
            var jwtToken = GenerateJwtToken(user, website);
            var useModel = mapper.Map<User, UserModel>(user);
            return new AuthenticateResponse(useModel, jwtToken, newRefreshToken.Token);
        }

        /// <summary>
        /// Revoke refresh token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public async Task<bool> RevokeToken(string token, string ipAddress)
        {
            var userRepository = unitOfWork.GetRepository<User>();
            var user = await userRepository.FindByAsync(x => 
                    x.RefreshToken.Any(r => r.Token.Equals(token)), nameof(RefreshToken));

            // return false if no user found with token
            if (user == null) return false;

            var refreshToken = user.RefreshToken.Single(x => x.Token == token);

            // return false if token is not active
            if (!refreshToken.IsActive) return false;

            // revoke token and save
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            userRepository.Update(user);
            await unitOfWork.CommitAsync();
            return true;
        }

        /// <summary>
        /// Authentication successful so generate jwt token
        /// </summary>
        /// <param name="user"></param>
        /// <param name="website"></param>
        /// <returns></returns>
        public string GenerateJwtToken(User user, Website website)
        {
            int timeExpires;
            string secretKey;
            //If user is super admin get secret key from jwt setting else get from website
            if (user.WebsiteId == null)
            {
                secretKey = jwtSettings.SecretKey;
                timeExpires = jwtSettings.SuperAdminTimeExpires;
            }
            else
            {
                secretKey = website.SecretKey;
                timeExpires = user.RoleId == (int)Role.Administrator
                    ? jwtSettings.AdministratorTimeExpires 
                    : jwtSettings.UserTimeExpires;
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, ((Role)user.RoleId).ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), 
                    new Claim(ClaimTypes.Uri, user.WebsiteId?.ToString() ?? string.Empty)
                }),
                Expires = DateTime.UtcNow.AddMinutes(timeExpires),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)), 
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            token.Header.Add("kid", user.WebsiteId?.ToString());
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Generate new refresh token
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public RefreshToken GenerateRefreshToken(string ipAddress)
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[64];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(jwtSettings.RefreshTokenExpires),
                CreatedDate = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }
    }
}
