using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ApplicationServicesExtensions
{
    public static  IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
        });
        services.AddCors();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserRepository,UserRepository>();
        services.AddScoped<ILikesRepository, LikesRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped(typeof (IUnitOfWork<>), typeof(UnitOfWork<>));
        services.AddScoped<IPhotoService,PhotoService>();
        services.AddScoped<LogUserActivity>();
        services.AddSingleton<PresenceTracker>();
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<AutoMapperProfiles>();
            cfg.AddMaps([typeof(AutoMapperProfiles).Assembly]);
        });

        services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
        services.AddSignalR();

        return services;

    }
}
