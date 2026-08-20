using API.Data;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers;

public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        if (context.HttpContext.User.Identity?.IsAuthenticated is not true)
            return;

        int userId = resultContext.HttpContext.User.GetUserIdClaim();

        var uow = resultContext.HttpContext.RequestServices.GetRequiredService<IUnitOfWork<IUserRepository>>();

        var user = await uow.Repository.GetUserByIdAsync(userId);

        if (user == null) return;

        user.LastActive = DateTime.UtcNow;

        await uow.CompleteAsync();
    }
}
