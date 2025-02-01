using System;

namespace StockOptionsScraper.Models;

public class MoneyWebBalanceSheet
{
    public string CompanyCode { get; set; }
    public string CompanyName { get; set; }
    
    // Period Names
    public string FirstPeriodName { get; set; }  // e.g. "Jun 24 F"
    public string SecondPeriodName { get; set; } // e.g. "Dec 23 I"
    
    // Balance Sheet Data - First Period
    public string? FixedAssetsFirstPeriod { get; set; }
    public string? TotalCurrentAssetsFirstPeriod { get; set; }
    public string? OrdinaryShareholdersInterestFirstPeriod { get; set; }
    public string? MinorityInterestFirstPeriod { get; set; }
    public string? TotalLongTermLiabilitiesFirstPeriod { get; set; }
    public string? TotalCurrentLiabilitiesFirstPeriod { get; set; }

    // Balance Sheet Data - Second Period
    public string? FixedAssetsSecondPeriod { get; set; }
    public string? TotalCurrentAssetsSecondPeriod { get; set; }
    public string? OrdinaryShareholdersInterestSecondPeriod { get; set; }
    public string? MinorityInterestSecondPeriod { get; set; }
    public string? TotalLongTermLiabilitiesSecondPeriod { get; set; }
    public string? TotalCurrentLiabilitiesSecondPeriod { get; set; }

    public DateTime ScrapedDate { get; set; } = DateTime.UtcNow;
}
