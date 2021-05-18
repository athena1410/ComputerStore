//-----------------------------------------------------------------------
// <copyright file="SecondAuthorize.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using ComputerStore.Structure.Enums;
using ComputerStore.Structure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;
using static ComputerStore.Structure.Constants.Constants;
using Role = ComputerStore.Structure.Enums.Role;

namespace ComputerStore.Api.Attribute
{
    public class SecondAuthorize : ActionFilterAttribute
    {
        /// <summary>
        /// Check user has permission on site
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var apiKey = filterContext.HttpContext.Request.Headers["website-id"].FirstOrDefault();
            var tokenApiKey = filterContext.HttpContext.User?.Claims?.SingleOrDefault(c => c.Type == ClaimTypes.Uri)?.Value;
            var role = filterContext.HttpContext.User?.Claims?.SingleOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            //If api key sent from header not mapping with api key in token
            //Meaning they don't have permission on this site
            //Should return forbidden error
            if (role != nameof(Role.SuperAdmin) && apiKey != tokenApiKey)
            {
                filterContext.Result = new OkObjectResult(new ApiResponse<object>(StatusCode.Forbidden, MessageResponse.ForbiddenError));
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
