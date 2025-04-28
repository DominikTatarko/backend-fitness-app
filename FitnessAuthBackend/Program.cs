using FitnessAuthBackend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using FitnessAuthBackend.Data;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddLogging(builder => builder.AddConsole());

        // Configure database and identity
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            // Configure password policy
            options.Password.RequireDigit = true; // Require at least one numeric digit
            options.Password.RequiredLength = 6; // Minimum password length
            options.Password.RequireNonAlphanumeric = false; // Require at least one special character
            options.Password.RequireUppercase = true; // Enable uppercase character requirement
            options.Password.RequireLowercase = true; // Require at least one lowercase character
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // Configure Swagger
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Description = "Enter 'Bearer' followed by your JWT token in the text input below.\nExample: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
            });

            options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
        });

        // Configure JWT Authentication
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"])),
                NameClaimType = ClaimTypes.NameIdentifier // Explicitly map NameIdentifier to the user ID
            };
        });

        // Configure CORS policy
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()     // Allow any origin
                      .AllowAnyMethod()     // Allow any HTTP method
                      .AllowAnyHeader();    // Allow any header
            });
        });

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.WebHost.UseUrls("http://*:1337");
        var app = builder.Build();

        // Seed database with initial data
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            DataSeeder.SeedFitnessPrograms(context).Wait();
            DataSeeder.SeedExercises(context).Wait();
            DataSeeder.SeedProgramWorkouts(context).Wait();
            DataSeeder.SeedFood(context).Wait();
            DataSeeder.SeedKroky(context).Wait();


        }

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Enable HTTPS redirection (optional but recommended in production)
        //app.UseHttpsRedirection();

        app.UseRouting();

        // Enable CORS
        app.UseCors("AllowAll");

        // Add authentication and authorization middleware
        app.UseAuthentication();
        app.UseAuthorization();

        // Map controllers
        app.MapControllers();

        app.Run();
    }
}
