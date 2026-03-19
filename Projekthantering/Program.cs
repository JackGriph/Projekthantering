using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Projekthantering.Data;
using Projekthantering.Services;
using Projekthantering.Repositories;
using System.Security.Claims;

namespace Projekthantering;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Database
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

        // JWT Authentication
        var jwtKey = builder.Configuration["Jwt:Key"]
            ?? "SuperSecretKeyThatIsAtLeast32CharactersLong!";
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "Projekthantering",
                    ValidAudience = builder.Configuration["Jwt:Audience"] ?? "Projekthantering",
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey))
                };
            });

        builder.Services.AddAuthorization();
        builder.Services.AddControllers();

        // Swagger/OpenAPI
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // CORS - tillåt Blazor Client
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowBlazor", policy =>
            {
                policy.WithOrigins("https://localhost:7147")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        // DI - Services och Repositories
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IBoardRepository, BoardRepository>();
        builder.Services.AddScoped<IBoardService, BoardService>();
        builder.Services.AddScoped<IListRepository, ListRepository>();
        builder.Services.AddScoped<IListService, ListService>();
        builder.Services.AddScoped<ICardRepository, CardRepository>();
        builder.Services.AddScoped<ICardService, CardService>();
        

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowBlazor");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
