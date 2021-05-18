using ComputerStore.BoundedContext.Entities;
using ComputerStore.Structure.Models.Authentication;
using System.Threading.Tasks;

namespace ComputerStore.Domain.Interfaces
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Process authentice to generate token
        /// </summary>
        /// <param name="model"></param>
        /// <param name="websiteId"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, int? websiteId, string ipAddress);
        /// <summary>
        /// Receive refresh token to generate new access token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        Task<AuthenticateResponse> RefreshToken(string token, string ipAddress);
        /// <summary>
        /// Revoke refresh token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        Task<bool> RevokeToken(string token, string ipAddress);
        /// <summary>
        /// Generate new token
        /// </summary>
        /// <param name="user"></param>
        /// <param name="website"></param>
        /// <returns></returns>
        string GenerateJwtToken(User user, Website website);
        /// <summary>
        /// Generate new refresh token
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        RefreshToken GenerateRefreshToken(string ipAddress);
    }
}
