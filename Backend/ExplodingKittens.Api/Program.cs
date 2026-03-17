using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using System.Text;
using ExplodingKittens.Api.Models;
using ExplodingKittens.Api.Services;
using ExplodingKittens.Api.Services.Contracts;
using ExplodingKittens.Api.Util;
using ExplodingKittens.Bootstrapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace ExplodingKittens.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ///////////////////////////////////
        // Dependency injection container//
        ///////////////////////////////////

        builder.Services.AddSingleton(provider =>
            new KittensExceptionFilterAttribute(provider.GetRequiredService<ILogger<Program>>()));

        builder.Services.AddControllers(options =>
        {
            var onlyAuthenticatedUsersPolicy =
                new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build();
            options.Filters.Add(new AuthorizeFilter(onlyAuthenticatedUsersPolicy));
            options.Filters.AddService<KittensExceptionFilterAttribute>();
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });


        builder.Services.AddOpenApi();

        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

                document.Components.SecuritySchemes.Add("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "Enter your JWT token"
                });

                document.Security = [
                    new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                    }
                ];

                return Task.CompletedTask;
            });
        });

        IConfiguration configuration = builder.Configuration;
        var tokenSettings = new TokenSettings();
        configuration.Bind("Token", tokenSettings);

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = tokenSettings.Issuer,
                    ValidAudience = tokenSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Key)),
                };
            });

        builder.Services.AddAuthorization();

        builder.Services.AddScoped<ITokenFactory>(_ => new JwtTokenFactory(tokenSettings));
        builder.Services.AddScoped<IModelMapper, ModelMapper>();
        builder.Services.AddCore(configuration);
        builder.Services.AddInfrastructure(configuration);

        var app = builder.Build();

        //////////////////////////////////////////////
        //Create database (if it does not exist yet)//
        //////////////////////////////////////////////
        app.EnsureDatabaseIsCreated();

        ////////////////////////
        // Middleware pipeline//
        ////////////////////////

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "v1");
            });
        }

        app.UseCors(policyName: "AllowAll");

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}