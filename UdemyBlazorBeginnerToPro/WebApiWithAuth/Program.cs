using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApiWithAuth.Data;

namespace WebApiWithAuth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // Add database
            builder.Services.AddDbContext<ApplicationDbContext>(opts =>
            {
                var connectionString =
                    builder.Configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException(
                        "Connection string 'DefaultConnection' not configured."
                    );

                opts.UseNpgsql(connectionString);

                if (builder.Environment.IsDevelopment())
                {
                    opts.EnableSensitiveDataLogging().EnableDetailedErrors();
                }
            });

            // Add auth
            builder.Services.AddAuthorization();

            builder
                .Services.AddIdentityApiEndpoints<IdentityUser>(opts =>
                    opts.SignIn.RequireConfirmedAccount = false
                )
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddCors(opts =>
                opts.AddDefaultPolicy(p => p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin())
            );

            var app = builder.Build();

            app.UseCors();
            app.MapIdentityApi<IdentityUser>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
