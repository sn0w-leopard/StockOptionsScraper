using Microsoft.AspNetCore.Mvc;
using StockOptionsScraper.Models;
using StockOptionsScraper.Services;
using StockOptionsScraper.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace StockOptionsScraper.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvestingController : ControllerBase
{
    private readonly ILogger<InvestingController> _logger;
    private readonly InvestingScrapingService _investingService;
    private readonly IScrapingService _moneyWebService;

    public InvestingController(
        ILogger<InvestingController> logger,
        InvestingScrapingService investingService,
        IScrapingService moneyWebService)
    {
        _logger = logger;
        _investingService = investingService;
        _moneyWebService = moneyWebService;
    }

    [HttpGet("mappings")]
    public async Task<ActionResult<List<CompanyMapping>>> GetMappings()
    {
        var companies = await _moneyWebService.GetCompaniesAsync();
        var mappings = new List<CompanyMapping>();

        foreach (var company in companies)
        {
            var mapping = await _investingService.GetInvestingMapping(company.value.ToUpper(), company.label.ToUpper());
            mappings.Add(mapping);
            //await Task.Delay(1000); // Rate limiting
        }

        return Ok(mappings);
    }

    [HttpGet("dividends/{moneyWebCode}")]
    public async Task<ActionResult<InvestingDividendData>> GetDividends(string moneyWebCode)
    {
        var companies = await _moneyWebService.GetCompaniesAsync();
        var company = companies.FirstOrDefault(c => c.value.ToUpper() == moneyWebCode.ToUpper());
        
        if (company == null)
            return NotFound($"Company with code {moneyWebCode} not found");

        var mapping = await _investingService.GetInvestingMapping(company.value, company.label);
        if (!mapping.HasMatch)
            return NotFound($"No Investing.com match found for {company.label}");

        var dividendData = await _investingService.GetDividendData(mapping);
        return Ok(dividendData);
    }
}
