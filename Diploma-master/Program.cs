using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Diploma.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Diploma.Extensions;
using Diploma.Services.Authorization;
using Diploma.Services;
using Neo4j.Driver;

namespace Diploma;

internal class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.AddApiAuthentication(configuration);

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(
                        "https://avet.netlify.app",
                        "http://localhost:5173"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        services.AddDbContext<ShopDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString(nameof(ShopDbContext)))
        );

        services.AddScoped<Neo4jSyncService>();
        services.AddSingleton<IDriver>(provider =>
        {
            var config = provider.GetRequiredService<IConfiguration>();
            return GraphDatabase.Driver(
                config["Neo4j:Uri"],
                AuthTokens.Basic(config["Neo4j:User"], config["Neo4j:Password"])
            );
        });
        services.AddScoped<RecommendationService>();

        services.AddScoped<CartService>();
        services.AddScoped<CategoriesService>();
        services.AddScoped<OrdersService>();
        services.AddScoped<ProductsService>();
        services.AddScoped<ReviewsService>();
        services.AddScoped<SellersService>();
        services.AddScoped<UsersService>();
        services.AddScoped<WishListsService>();

        services.AddScoped<MyJwtProvider>();
        services.AddScoped<MyPasswordHasher>();

        services.AddControllers();
        services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }


        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
                    