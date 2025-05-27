using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Core.Helper
{
    public static class ResponseBuilder
    {
        public static ApiResponse<T> Success<T>(T data, string message = "Success", HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message,
                StatusCode = (int)statusCode
            };
        }
              
        public static ApiResponse<T> Fail<T>(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Data = default,
                Message = message,
                StatusCode = (int)statusCode
            };
        }
    }
}
