using API.Errors;
using System.Net;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
//using static System.Console;
namespace API.Middleware;

public class ExceptionMiddleware(RequestDelegate next,
    ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
{

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {

            logger.LogError(ex, "Some Errors occurred.");
            

            var statusCode = context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            var mediaType = context.Response.ContentType = MediaTypeNames.Application.Json;

            var response = env.IsDevelopment()
                ? new ApiExceptions(context.Response.StatusCode, ex.Message, ex.StackTrace)
                : new ApiExceptions(context.Response.StatusCode, ex.Message, "internal server error");

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
            //Console.WriteLine();

            #region Approach-2
            /*
             var statusCode = context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
           //var mediaType = context.Response.ContentType = MediaTypeNames.Application.Json; // No need to this because of the use of WriteAsJsonAsync() method.
             var apiException = env.IsDevelopment()
                ? new ApiExceptions(context.Response.StatusCode, ex.Message, ex.StackTrace)
                : new ApiExceptions(context.Response.StatusCode, ex.Message, "internal server error");

            await context.Response.WriteAsJsonAsync(apiException,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            */
            #endregion
        }
    }


}
