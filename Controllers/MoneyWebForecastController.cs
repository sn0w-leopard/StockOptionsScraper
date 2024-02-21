using Microsoft.AspNetCore.Mvc;
using StockOptionsScraper.Interfaces;
using StockOptionsScraper.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace StockOptionsScraper.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoneyWebForecastController
{
    private readonly ILogger<MoneyWebForecastController> _logger;
    private readonly IScrapingService _scrapingService;

    public MoneyWebForecastController(ILogger<MoneyWebForecastController> logger, IScrapingService scrapingService)
    {
        _scrapingService = scrapingService;
        _logger = logger;
    }

    [HttpGet("Forecast{companyCode}")]
    public async Task<MoneyWebForecast> GetForecast(string companyCode) 
    {
        var forecast = await _scrapingService.GetForecastAsync(companyCode);
        return forecast;
    }

    [HttpGet("Companies")]
    public async Task<List<MoneyWebCompany>> GetCompanies() 
    {
        var companies = await _scrapingService.GetCompaniesAsync();
        return companies;
    }

    [HttpGet("ForecastList")]
    public async Task<List<MoneyWebForecast>> GetForecastList() 
    {
        var companies = await _scrapingService.GetCompaniesAsync();
        var forecasts = await _scrapingService.GetMoneyWebForecastList(companies);
        return forecasts;
    }
}
