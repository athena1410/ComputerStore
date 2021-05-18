//-----------------------------------------------------------------------
// <copyright file="CategoryController.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using ComputerStore.Api.Attribute;

namespace ComputerStore.Api.v1.Controllers
{
    [ValidateModel]
    [ValidateWebsite(Order = 1)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BaseController : ControllerBase
    {
        protected int WebsiteId => Convert.ToInt32(HttpContext.Request.Headers["website-id"].FirstOrDefault());
        protected int TokenWebsiteId => Convert.ToInt32(HttpContext.User?.Claims?.SingleOrDefault(c => c.Type == ClaimTypes.Uri)?.Value);
        protected string TokenRole => HttpContext.User?.Claims?.SingleOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        protected int UserId => Convert.ToInt32(HttpContext.User?.Claims?.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
    }
}