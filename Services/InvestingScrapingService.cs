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
        var searchUrl = $"https://za.investing.com/search/?q={Uri.EscapeDataString(companyLabel)}";
        var doc = await _web.LoadFromWebAsync(searchUrl);

        var resultsContainer = doc.DocumentNode.SelectSingleNode("//div[@class='js-inner-all-results-quotes-wrapper newResultsContainer quatesTable']");
        
        if (resultsContainer != null)
        {
            var resultLink = resultsContainer.SelectSingleNode(".//a[@class='js-inner-all-results-quote-item row']");
            if (resultLink != null)
            {
                return new CompanyMapping
                {
                    MoneyWebCode = moneyWebCode,
                    MoneyWebName = companyLabel,
                    InvestingSymbol = resultLink.SelectSingleNode(".//span[@class='second']")?.InnerText.Trim(),
                    InvestingName = resultLink.SelectSingleNode(".//span[@class='third']")?.InnerText.Trim(),
                    InvestingUrl = resultLink.GetAttributeValue("href", ""),
                    HasMatch = true
                };
            }
        }

        return new CompanyMapping
        {
            MoneyWebCode = moneyWebCode,
            MoneyWebName = companyLabel,
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
