
using FIS_API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

namespace FIS_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

			var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

            if (allowedOrigins == null)
				allowedOrigins = [];

            builder.Services.AddCors(options =>
			{
				options.AddPolicy("CustomCors",
					builder =>
				{
					builder.WithOrigins(allowedOrigins)
						.AllowAnyMethod()
						.AllowAnyHeader();
				});
			});

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(option =>
			{
				option.SwaggerDoc("v1",
					new OpenApiInfo { Title = "RWA Web API", Version = "v1" });

				option.AddSecurityDefinition("Bearer",
					new OpenApiSecurityScheme
					{
						In = ParameterLocation.Header,
						Description = "Please enter valid JWT",
						Name = "Authorization",
						Type = SecuritySchemeType.Http,
						BearerFormat = "JWT",
						Scheme = "Bearer"
					});

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
				new List<string>()
			}
					});
			});

			// Configure JWT security services
			var secureKey = builder.Configuration["JWT:SecureKey"];
			builder.Services
				.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(o => {
					var Key = Encoding.UTF8.GetBytes(secureKey);
					o.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = false,
						ValidateAudience = false,
						IssuerSigningKey = new SymmetricSecurityKey(Key)
					};
				});

			builder.Services.AddDbContext<FirefighterDbContext>(options => {
				options.UseSqlServer("name=ConnectionStrings:ffDB");
			});

			builder.Services.AddControllersWithViews() // Circular references are dead, long live monitored circular references!
				.AddJsonOptions(options =>
					options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

			var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("CustomCors");

            // Use authentication / authorization middleware
            app.UseAuthentication();
			app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
