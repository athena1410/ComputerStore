//-----------------------------------------------------------------------
// <copyright file="ApiResponse.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using ComputerStore.Structure.Enums;

namespace ComputerStore.Structure.Models
{
    public class ApiResponse<T>
    {
        public StatusCode StatusCode { get; set; }
        public string ResultMessage { get; set; }
        public T Data { get; set; }
        public ApiResponse()
        {
            StatusCode = StatusCode.Ok;
        }

        public ApiResponse(string message) : this()
        {
            ResultMessage = message;
        }

        public ApiResponse(StatusCode statusCode, string message)
        {
            StatusCode = statusCode;
            ResultMessage = message;
        }

        public ApiResponse(T data) : this()
        {
            Data = data;
        }

        public ApiResponse(StatusCode statusCode, T data, string message)
        {
            StatusCode = statusCode;
            Data = data;
            ResultMessage = message;
        }
    }
}
