//-----------------------------------------------------------------------
// <copyright file="AuthenticationController.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Models;
using ComputerStore.Structure.Models.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using static ComputerStore.Structure.Constants.Constants;

namespace ComputerStore.Api.v1.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService authenticationService;
        private string IpAddress =>
            Request.Headers.ContainsKey("X-Forwarded-For")
                ? (string)Request.Headers["X-Forwarded-For"]
                : HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        /// <summary>
        /// Authenticate for Admin and User
        /// </summary>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest model)
        {
            var currentWebsiteId = HttpContext.Request.Headers["website-id"].FirstOrDefault();
            int? websiteId = null;
            if (int.TryParse(currentWebsiteId, out var id))
            {
                websiteId = id;
            }

            var response = await authenticationService.Authenticate(model, websiteId, IpAddress);

            if (response == null)
                return Ok(new ApiResponse<AuthenticateResponse>(Structure.Enums.StatusCode.BadRequest,
                    MessageResponse.LoginFailed));

            return Ok(new ApiResponse<AuthenticateResponse>(response));
        }

        /// <summary>
        /// Receive refresh token to generate new access token
        /// </summary>
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            var response = await authenticationService.RefreshToken(refreshTokenRequest.Token, IpAddress);

            if (response == null)
                return Unauthorized(new { message = "Invalid token" });

            return Ok(new ApiResponse<AuthenticateResponse>(response));
        }

        /// <summary>
        /// Revoke refresh token
        /// </summary>
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest revokeTokenRequest)
        {
            var response = await authenticationService.RevokeToken(revokeTokenRequest.Token, IpAddress);

            if (!response)
                return Ok(new ApiResponse<AuthenticateResponse>(Structure.Enums.StatusCode.NotFound,
                    string.Format(MessageResponse.NotFoundError, "Token", revokeTokenRequest.Token)));

            return Ok(new ApiResponse<bool>("Token revoked!"));
        }
    }
}