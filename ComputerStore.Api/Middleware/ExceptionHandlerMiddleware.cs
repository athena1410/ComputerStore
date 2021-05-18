//-----------------------------------------------------------------------
// <copyright file="ExceptionHandlerMiddleware.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using ComputerStore.Structure.Enums;
using ComputerStore.Structure.Exceptions;
using ComputerStore.Structure.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ComputerStore.Api.Middleware
{
    /// <summary>
    /// Central error/exception handler Middleware
    /// </summary>
    public class ExceptionHandlerMiddleware
    {
        private const string JsonContentType = "application/json";
        private readonly RequestDelegate _request;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandlerMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next.</param>
        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            this._request = next;
        }

        /// <summary>
        /// Invokes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task Invoke(HttpContext context) => this.InvokeAsync(context);

        private async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await this._request(context);
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.Message);
                var httpStatusCode = ConfigureExceptionTypes(exception);

                // set http status code and content type
                context.Response.StatusCode = httpStatusCode != (int)StatusCode.NotFound
                                                ? (int)StatusCode.Ok : httpStatusCode;
                context.Response.ContentType = JsonContentType;

                var jsonSerializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                // writes / returns error model to the response
                await context.Response.WriteAsync(
                    JsonConvert.SerializeObject(new ApiResponse<Exception>
                    {
                        StatusCode = (StatusCode)httpStatusCode,
                        ResultMessage = exception.Message,
                    }, jsonSerializerSettings));
            }
        }

        /// <summary>
        /// Configure/maps exception to the proper HTTP error Type
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        private static int ConfigureExceptionTypes(Exception exception)
        {
            var httpStatusCode = exception switch
            {
                var _ when exception is ValidationException => (int)StatusCode.BadRequest,
                var _ when exception is NotFoundException => (int)StatusCode.NotFound,
                _ => (int)StatusCode.InternalServerError,
            };
            return httpStatusCode;
        }
    }
}
