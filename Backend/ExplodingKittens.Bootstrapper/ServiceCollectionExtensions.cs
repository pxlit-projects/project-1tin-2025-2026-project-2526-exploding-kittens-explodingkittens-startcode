using ExplodingKittens.Core.ActionAggregate;
using ExplodingKittens.Core.ActionAggregate.Contracts;
using ExplodingKittens.Core.CardAggregate;
using ExplodingKittens.Core.CardAggregate.Contracts;
using ExplodingKittens.Core.GameAggregate;
using ExplodingKittens.Core.GameAggregate.Contracts;
using ExplodingKittens.Core.PlayerAggregate;
using ExplodingKittens.Core.PlayerAggregate.Contracts;
using ExplodingKittens.Core.TableAggregate;
using ExplodingKittens.Core.TableAggregate.Contracts;
using ExplodingKittens.Core.UserAggregate;
using ExplodingKittens.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExplodingKittens.Bootstrapper;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<KittensDbContext>(options =>
        {
            string connectionString = configuration.GetConnectionString("KittensDbConnection")!;
            options.UseSqlServer(connectionString).EnableSensitiveDataLogging();
        });

        services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 8;
                options.Lockout.AllowedForNewUsers = true;

                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredLength = 5;

                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<KittensDbContext>()
            .AddDefaultTokenProviders();

        services.AddSingleton<ITableRepository, InMemoryTableRepository>();
        services.AddSingleton<IGameRepository, InMemoryGameRepository>();
    }

    public static void AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITableManager, TableManager>();
        services.AddScoped<ITableFactory, TableFactory>();
        services.AddScoped<IGameFactory, GameFactory>();
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<IActionFactory, ActionFactory>();
        services.AddScoped<ICardDeckFactory, CardDeckFactory>();
        services.AddScoped<IGamePlayStrategy, GamePlayStrategy>();
    }
}

