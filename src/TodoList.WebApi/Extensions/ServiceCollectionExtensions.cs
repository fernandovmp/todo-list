using System;
using System.Data;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TodoList.WebApi.Configurations;
using TodoList.WebApi.Repositories;
using TodoList.WebApi.Services;

namespace TodoList.WebApi.Extentsions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationCorsPolicy(this IServiceCollection services, IConfiguration configuration)
        {
            var corsOptions = new ApplicationCorsOptions();
            configuration.Bind("CorsOptions", corsOptions);
            return services
                .Configure<ApplicationCorsOptions>(configuration.GetSection("CorsOptions"))
                .AddCors(options =>
                {
                    options.AddPolicy(corsOptions.PolicyName, configurePolicy =>
                    {
                        configurePolicy
                            .AllowCredentials()
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .WithOrigins(corsOptions.AllowedOrigin);
                    });
                });
        }
        public static IServiceCollection AddSqlServerDatabase(this IServiceCollection services, string connectionString) =>
            services
                .AddTransient<IDbConnection>(sp => new SqlConnection(connectionString));

        public static IServiceCollection AddRepositories(this IServiceCollection services) =>
            services
                .AddScoped<ITodoItemRepository, TodoItemRepository>()
                .AddScoped<IUserRepository, UserRepository>();

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var tokenOptions = new JwtTokenOptions();
            configuration.Bind("JwtToken", tokenOptions);
            byte[] key = Encoding.ASCII.GetBytes(tokenOptions.Key);

            services
                .Configure<JwtTokenOptions>(configuration.GetSection("JwtToken"))
                .Configure<HasherOptions>(configuration.GetSection("PasswordHasher"))
                .AddScoped<IPasswordHasherService, PasswordHasherService>()
                .AddScoped<ITokenService, TokenService>()
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidAudience = tokenOptions.Audience,
                        ValidIssuer = tokenOptions.Issuer,
                        ClockSkew = TimeSpan.Zero
                    };
                });
            return services;
        }
    }
}
