using System;

namespace StockOptionsScraper.Models;

public class InvestingDividendData
{
    public string? CompanyCode { get; set; }
    public string? CompanyName { get; set; }
    public List<DividendHistory> DividendHistory { get; set; } = new();
}

public class DividendHistory
{
    public DateTime ExDate { get; set; }
    public decimal Amount { get; set; }
    public string? Type { get; set; }
    public DateTime PaymentDate { get; set; }
}
