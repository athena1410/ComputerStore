//-----------------------------------------------------------------------
// <copyright file="ValidateWebsite.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using ComputerStore.Domain.Interfaces;
using ComputerStore.Structure.Enums;
using ComputerStore.Structure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;
using static ComputerStore.Structure.Constants.Constants;

namespace ComputerStore.Api.Attribute
{
    /// <summary>
    /// Action filter attribute to validate website existed and active
    /// </summary>
    public class ValidateWebsite : ActionFilterAttribute
    {
        /// <summary>
        /// Return bad request if not valid
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var apiKey = filterContext.HttpContext.Request.Headers["website-id"].FirstOrDefault();
            var tokenRole = filterContext.HttpContext.User?.Claims?.SingleOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            // Return if data invalid
            if (!int.TryParse(apiKey, out var websiteId) &&
                (tokenRole == null || !tokenRole.Equals(nameof(Role.SuperAdmin))))
            {
                filterContext.Result = new OkObjectResult(new ApiResponse<object>(StatusCode.BadRequest, MessageResponse.WebsiteNotValid));
                return;
            }

            var websiteService = (IWebsiteService)filterContext.HttpContext.RequestServices.GetService(typeof(IWebsiteService));
            var website = websiteService.GetByIdAsync(websiteId)
                                .ConfigureAwait(false).GetAwaiter().GetResult();
            // Return if website is de-active
            if (website?.Status == (int)Status.DEACTIVATE &&
                (tokenRole == null || !tokenRole.Equals(nameof(Role.SuperAdmin))))
            {
                filterContext.Result = new OkObjectResult(new ApiResponse<object>(StatusCode.BadRequest, MessageResponse.WebsiteNotValid));
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
