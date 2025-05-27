
using LearnArchitecture.API.Common;
using LearnArchitecture.API.Common.Middleware;
using LearnArchitecture.Core.Helper.Attributes;
using LearnArchitecture.Data.Context;
using LearnArchitecture.Data.IRepository;
using LearnArchitecture.Data.Repository;
using LearnArchitecture.Services.IServices;
using LearnArchitecture.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace LearnArchitecture.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Logger Service configuration
            Log.Logger = new LoggerConfiguration()
                        .WriteTo.Console()
                        .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
                        .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<LearnArchitectureDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DbContext"));
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            //Swagger UI for API authentication
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "Bearer Authenticatipon with JWT Token",
                    Type = SecuritySchemeType.Http,
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });

                options.MapType<DateOnly>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "date"
                });
                options.MapType<TimeOnly>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "time"
                });
            });

            builder.Services.AddMemoryCache();

            // Use Redis cache
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("Redis");
                options.InstanceName = "MyApp_";
            });

            //Validate Token 
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                     ValidAudience = builder.Configuration["JWT:ValidAudience"],
                     IssuerSigningKey = new SymmetricSecurityKey(
                         Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]))
                 };
             });

            //Dependency Injector
            Injector.APIInitializer(builder.Services);
            builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
            builder.Services.AddSwaggerGen();
            builder.Host.UseSerilog();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            //CORS POLICY
            app.UseCors(builder =>
            {
                builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            }); 
             
            app.UseHttpsRedirection();
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseAuthentication();
            app.UseMiddleware<JwtClaimsMiddleware>();
            app.UseAuthorization();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                 Path.Combine(Directory.GetCurrentDirectory(), "Images")),
                RequestPath = "/Images"
            });
            app.MapControllers();

            app.Run();
        }
    }
}
