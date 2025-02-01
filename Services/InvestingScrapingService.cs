using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using StockOptionsScraper.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockOptionsScraper.Services;

public class InvestingScrapingService
{
    private readonly HtmlWeb _web;
    private readonly ILogger<InvestingScrapingService> _logger;

    public InvestingScrapingService(ILogger<InvestingScrapingService> logger)
    {
        _web = new HtmlWeb();
        _logger = logger;
    }

    public async Task<CompanyMapping> GetInvestingMapping(string moneyWebCode, string companyLabel)
    {

        _logger.LogInformation($"Searching for {companyLabel} ({moneyWebCode})");

        var cleanCompanyName = companyLabel.Split('-').Last().Trim();
        _logger.LogInformation($"Clean company name: {cleanCompanyName}");
        
        var searchUrl = $"https://www.investing.com/search/?q={Uri.EscapeDataString(cleanCompanyName)}";
        _logger.LogInformation($"Search URL: {searchUrl}");


        var web = new HtmlWeb();
        var doc = await _web.LoadFromWebAsync(searchUrl);

        _logger.LogInformation("Search response received");

        var searchResults = doc.DocumentNode.SelectNodes("//div[contains(@class, 'js-inner-all-results-quotes-wrapper')]");
        _logger.LogInformation($"Found {searchResults?.Count ?? 0} result containers");

        if (searchResults != null)
        {
            var jseListing = searchResults.SelectNodes(".//a")
                ?.FirstOrDefault(x => x.InnerText.Contains("JSE") || 
                                    x.InnerText.Contains("Johannesburg"));

            if (jseListing != null)
            {
                _logger.LogInformation($"Found JSE listing: {jseListing.InnerText}");
                
                return new CompanyMapping
                {
                    MoneyWebCode = moneyWebCode,
                    MoneyWebName = cleanCompanyName,
                    InvestingSymbol = jseListing.SelectSingleNode(".//span[@class='second']")?.InnerText,
                    InvestingName = jseListing.SelectSingleNode(".//span[@class='third']")?.InnerText,
                    InvestingUrl = jseListing.GetAttributeValue("href", ""),
                    HasMatch = true
                };
            }
        }

        _logger.LogWarning($"No JSE listing found for {cleanCompanyName}");

        return new CompanyMapping
        {
            MoneyWebCode = moneyWebCode,
            MoneyWebName = cleanCompanyName,
            HasMatch = false
        };
    }

    public async Task<InvestingDividendData> GetDividendData(CompanyMapping mapping)
    {
        if (!mapping.HasMatch) return null;

        var dividendUrl = $"https://za.investing.com{mapping.InvestingUrl}-dividends";
        var doc = await _web.LoadFromWebAsync(dividendUrl);

        var dividendData = new InvestingDividendData
        {
            CompanyCode = mapping.MoneyWebCode,
            CompanyName = mapping.MoneyWebName,
            DividendHistory = new List<DividendHistory>()
        };

        var rows = doc.DocumentNode.SelectNodes("//table[@id='dividendsHistoryData']//tr");
        if (rows != null)
        {
            foreach (var row in rows.Skip(1)) // Skip header row
            {
                var cells = row.SelectNodes(".//td");
                if (cells?.Count >= 4)
                {
                    dividendData.DividendHistory.Add(new DividendHistory
                    {
                        ExDate = DateTime.Parse(cells[0].InnerText.Trim()),
                        Amount = decimal.Parse(cells[1].InnerText.Trim().Replace("ZAc", "")),
                        Type = cells[2].InnerText.Trim(),
                        PaymentDate = DateTime.Parse(cells[3].InnerText.Trim())
                    });
                }
            }
        }

        return dividendData;
    }
}
