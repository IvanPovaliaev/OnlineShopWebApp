using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OnlineShop.Application.Interfaces;
using OnlineShop.Domain.Interfaces;
using OnlineShop.Domain.Models;
using OnlineShop.Infrastructure.CommonDI;
using OnlineShop.Infrastructure.Data;
using OnlineShop.Infrastructure.Excel;
using OnlineShop.Infrastructure.Jwt;
using OnlineShop.Infrastructure.Redis;
using OnlineShop.WebAPI.Helpers;
using OnlineShop.WebAPI.Middleware;
using StackExchange.Redis;
using System.Text;

namespace OnlineShop.WebAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            var databaseProvider = builder.Configuration["DatabaseProvider"];
            var connection = builder.Configuration.GetConnectionString(databaseProvider!);
            var databaseTypes = builder.Configuration.GetSection("DatabaseTypes")
                                                     .Get<Dictionary<string, string>>();

            if (!databaseTypes.TryGetValue(databaseProvider!, out var databaseType))
            {
                throw new InvalidOperationException("Invalid database provider");
            }

            switch (databaseType.ToLower())
            {
                case "postgresql":
                    builder.Services.AddDbContext<DatabaseContext, PostgreSQLContext>(options => options.UseNpgsql(connection), ServiceLifetime.Scoped);
                    break;
                case "mssql":
                    builder.Services.AddDbContext<DatabaseContext, MsSQLContext>(options => options.UseSqlServer(connection), ServiceLifetime.Scoped);
                    break;
                case "mysql":
                    builder.Services.AddDbContext<DatabaseContext, MySQLContext>(options => options.UseMySql(connection, ServerVersion.AutoDetect(connection)), ServiceLifetime.Scoped);
                    break;
                default:
                    throw new InvalidOperationException("Invalid database type");
            }

            builder.Services.AddIdentity<User, Domain.Models.Role>()
                            .AddEntityFrameworkStores<DatabaseContext>()
                            .AddDefaultTokenProviders();

            builder.Services.AddControllers();

            var redisConfiguration = ConfigurationOptions.Parse(builder.Configuration.GetSection("Redis:ConnectionString").Value);
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect(redisConfiguration);
            });

            builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddCommonServices(builder.Configuration);

            builder.Services.AddTransient<IExcelService, ClosedXMLExcelService>();

            var jwtOptions = builder.Configuration.GetSection("JwtOptions");
            builder.Services.Configure<JwtOptions>(jwtOptions);
            builder.Services.AddTransient<JwtProvider>();

            builder.Services.AddScoped<IHostingEnvironmentService, HostingEnvironmentService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "OnlineShop.WebAPI",
                    Description = "OnlineShop ASP.NET Core Web API"
                });

                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                });

                swagger.UseInlineDefinitionsForEnums();
            });

            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                var jwtSettings = jwtOptions.Get<JwtOptions>();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings!.Issuer,
                    ValidAudience = jwtSettings!.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings!.SecretKey))
                };
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseMiddleware<JWTMiddleware>();
            app.UseAuthorization();

            app.MapControllers();

            using (var serviceScope = app.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<User>>();
                var roleManager = services.GetRequiredService<RoleManager<Domain.Models.Role>>();
                var configuration = services.GetRequiredService<IConfiguration>();

                var identityInitializer = new IdentityInitializer(configuration);
                await identityInitializer.InitializeAsync(userManager, roleManager);
            }

            app.Run();
        }
    }
}
