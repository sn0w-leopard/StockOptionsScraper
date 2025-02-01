using System;

namespace StockOptionsScraper.Models;

public class CompanyMapping
{
    public string? MoneyWebCode { get; set; }
    public string? MoneyWebName { get; set; }
    public string? InvestingSymbol { get; set; }
    public string? InvestingName { get; set; }
    public string? InvestingUrl { get; set; }
    public bool HasMatch { get; set; }
}


