using StockOptionsScraper.Interfaces;
using StockOptionsScraper.Services;

namespace StockOptionsScraper;
public class Program
{
    public static void Main(String[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services
        ConfigureWebHost(builder);
        ConfigureServices(builder.Services);

        var app = builder.Build();

        // Configure HTTP pipeline
        app.UseRouting();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        //middleware
        app.UseSwagger();
        app.UseSwaggerUI();

        app.Run();
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
    }
}


