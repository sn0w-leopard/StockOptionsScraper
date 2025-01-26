using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using StockOptionsScraper.Interfaces;
using StockOptionsScraper.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace StockOptionsScraper.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoneyWebController : ControllerBase
{
    private readonly ILogger<MoneyWebController> _logger;
    private readonly IScrapingService _scrapingService;

    public MoneyWebController(ILogger<MoneyWebController> logger, IScrapingService scrapingService)
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

    [HttpGet("balanceSheet{companyCode}")]
    public async Task<MoneyWebBalanceSheet> GetBalanceSheetByCompanyCode(
    [Required]
    [StringLength(5)]
    string companyCode) 
    {
        _logger.LogInformation("Getting balance sheet for {CompanyCode}", companyCode);
        try
        {
            var forecast = await _scrapingService.GetBalanceSheetAsync(companyCode);

            _logger.LogInformation("Balance sheet returned for {CompanyCode}", companyCode);
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

    [HttpGet("balanceSheets")]
    public async Task<List<MoneyWebForecast>> GetBalanceSheetList() 
    {
        _logger.LogInformation("Getting companies' balance sheets from MoneyWeb");
        try
        {
            var companies = await _scrapingService.GetCompaniesAsync();
            var balanceSheets = await _scrapingService.GetMoneyWebForecastList(companies);
            _logger.LogInformation("Companies' balance sheets from MoneyWeb returned");
            return balanceSheets;
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
