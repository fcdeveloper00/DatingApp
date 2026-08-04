using API.Data;
using API.Extensions;
using API.Interfaces;
using API.Middleware;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();


//IEndpointConventionBuilder;
//IEndpointRouteBuilder;
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(
    cfg =>
           cfg.AllowAnyHeader().
           AllowAnyMethod().
           WithOrigins("https://localhost:4200", "http://localhost:4200")
);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var serviceProvider = scope.ServiceProvider;

try
{
    var context = serviceProvider.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();
    await Seed.SeedUsers(context);

}
catch (Exception ex)
{
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex,"Error while applying migration to the database");
}

app.Run();

/*var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};*/

/*record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

*/

//int number1 = 100_000;
//Console.WriteLine(number1);


//ISomeTask<Vehicle> someTask = new SomeTask<BMW>();
//var output = someTask.DoTask();
//Console.WriteLine(output.GetType());

//public interface ISomeTask<out T> where T : new()
//{
//    T DoTask();
//}

//public class SomeTask<T> : ISomeTask<T> where T : new()
//{
//    public T DoTask()
//    {
//        T t = new T();
//        Console.WriteLine("The Task is performed.");
//        return t;
//        //throw new NotImplementedException();
//    }
//}

//public class Vehicle
//{

//}

//public class Car : Vehicle
//{

//}

//public class BMW : Car
//{

//}

//public class Truck { }