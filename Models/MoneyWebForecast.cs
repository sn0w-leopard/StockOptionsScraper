namespace StockOptionsScraper.Models;

public class MoneyWebForecast
{
    public string? CompanyName { get; set; }
    public string? CompanyCode { get; set; }
    public string? MarketCap { get; set; }
    public string? EPS_TTM { get; set; }
    public string? PEratio { get; set; }
    public string? ForwardPE { get; set; }
    public string? DividendYield { get; set; }
    public string? ReturnOnEquity { get; set; }
    public string? ReturnOnAssets { get; set; }
    public string? NetAssetValue { get; set; }
    public string? PriceNAV { get; set; }
    public string? AuthorisedShares { get; set; }
    public string? IssuedShares { get; set; }
}