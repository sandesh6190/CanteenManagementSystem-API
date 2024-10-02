using Asp.Versioning;
using Canteen.Data;
using Canteen.Data.Repositories;
using Canteen.Data.Repositories.Interfaces;
using Canteen.Data.UnitOfWork;
using Canteen.Data.UnitOfWork.Interfaces;
using Canteen.Service;
using Canteen.Service.Interface;
using Lib.Authorization.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace Canteen.Api.Extensions;

public static class ServiceExtensions
{
    public static void JsonConfig(
        this IServiceCollection services,
        IConfiguration configuration
    ) { }

    public static void CoreConfigure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorizationLibrary<ApplicationDbContext>(configuration);

        services.AddRouting(options => options.LowercaseUrls = true);

        services.AddCors(policy =>
        {
            policy.AddPolicy(
                "CorsPolicy",
                opt => opt.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
            );
        });

        services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

        services.AddSwaggerGen();
        services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "NewsPortal.Api", Version = "v1" });
            option.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                }
            );
            option.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                }
            );
        });
    }

    public static void ConfigureDi(this IServiceCollection services)
    {
        services.AddServices();
        services.AddRepositories();
        services.AddMiscServices();
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUnitService, UnitService>();
        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IRedeemTypeService, RedeemTypeService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IRedeemService, RedeemService>();
        services.AddScoped<IUserBalanceService, UserBalanceService>();
        services.AddScoped<INotificationService, NotificationService>();
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        //register repositories
    }

    private static void AddMiscServices(this IServiceCollection services)
    {
        services
            .BuildServiceProvider()
            .CreateScope()
            .ServiceProvider.GetRequiredService<ApplicationDbContext>()
            .Database.Migrate();
    }
}