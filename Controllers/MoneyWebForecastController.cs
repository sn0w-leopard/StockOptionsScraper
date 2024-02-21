using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using StockOptionsScraper.Interfaces;
using StockOptionsScraper.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace StockOptionsScraper.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoneyWebForecastController : ControllerBase
{
    private readonly ILogger<MoneyWebForecastController> _logger;
    private readonly IScrapingService _scrapingService;

    public MoneyWebForecastController(ILogger<MoneyWebForecastController> logger, IScrapingService scrapingService)
    {
        _scrapingService = scrapingService;
        _logger = logger;
    }

    [HttpGet("forecast{companyCode}")]
    public async Task<MoneyWebForecast> GetForecastByCompanyCode(
    [Required]
    [StringLength(5)]
    string companyCode) 
    {
        _logger.LogInformation("Getting forecast for {CompanyCode}", companyCode);
        try
        {
            var forecast = await _scrapingService.GetForecastAsync(companyCode);

            _logger.LogInformation("Forecast returned for {CompanyCode}", companyCode);
            return forecast;
        }
        catch (System.Exception)
        {           
            throw;
        }
    }

    [HttpGet("forecasts")]
    public async Task<List<MoneyWebForecast>> GetForecastList() 
    {
        _logger.LogInformation("Getting companies' forecasts from MoneyWeb");
        try
        {
            var companies = await _scrapingService.GetCompaniesAsync();
            var forecasts = await _scrapingService.GetMoneyWebForecastList(companies);
            _logger.LogInformation("Companies' forecasts from MoneyWeb returned");
            return forecasts;
        }
        catch (System.Exception)
        {           
            throw;
        }
    }

    [HttpGet("companies")]
    public async Task<List<MoneyWebCompany>> GetCompanies() 
    {
        _logger.LogInformation("Getting companies from MoneyWeb");
        try
        {
            var companies = await _scrapingService.GetCompaniesAsync();
            _logger.LogInformation("Companies from MoneyWeb returned");
            return companies;
        }
        catch (System.Exception)
        {           
            throw;
        }
    }
}
