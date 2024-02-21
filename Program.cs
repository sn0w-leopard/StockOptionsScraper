using Microsoft.OpenApi.Models;
using StockOptionsScraper.Interfaces;
using StockOptionsScraper.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace StockOptionsScraper;
public class Program
{
    public static void Main(String[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //docker
        builder.WebHost.UseKestrel()
            .UseUrls($"http://+:{Environment.GetEnvironmentVariable("PORT") ?? "80"}");

        // Add services
        ConfigureWebHost(builder);
        ConfigureServices(builder.Services);
        ConfigureLogging(builder);

        var app = builder.Build();

        // Configure HTTP pipeline
        app.UseRouting();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        //middleware
        app.UseExceptionHandler("/error");
        app.UseSwagger();
        app.UseSwaggerUI();
        //app.UseAuthentication();

        app.Run();
    }

    private static void ConfigureLogging(WebApplicationBuilder builder)
    {
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();
    }

    private static void ConfigureWebHost(WebApplicationBuilder builder)
    {
        builder.Configuration.AddJsonFile("appsettings.json");
        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen();
        builder.Logging.AddConsole();
    }
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IScrapingService, ScrapingService>();
        /*         services.AddAuthentication(options => {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(); */
    }
}