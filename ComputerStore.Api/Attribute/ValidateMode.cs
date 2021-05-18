//-----------------------------------------------------------------------
// <copyright file="ValidateModel.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using ComputerStore.Structure.Enums;
using ComputerStore.Structure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace ComputerStore.Api.Attribute
{
    public class ValidateModel : ActionFilterAttribute
    {
        /// <summary>
        /// Return bad request if not valid
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var message = string.Join(" | ", context.ModelState.Values
                   .SelectMany(v => v.Errors)
                   .Select(e => e.ErrorMessage));

                context.Result = new OkObjectResult(new ApiResponse<object>(StatusCode.BadRequest, message));
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
