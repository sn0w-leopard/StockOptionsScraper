using StockOptionsScraper.Models;
namespace StockOptionsScraper.Interfaces;

public interface IScrapingService
{
  public Task<MoneyWebForecast> GetForecastAsync(string companyCode);
  public Task<MoneyWebBalanceSheet> GetBalanceSheetAsync(string companyCode);
  public Task<List<MoneyWebCompany>> GetCompaniesAsync();
  public Task<List<MoneyWebForecast>> GetMoneyWebForecastList(List<MoneyWebCompany> companies);
  public Task<List<MoneyWebBalanceSheet>> GetMoneyWebBalanceSheetList(List<MoneyWebCompany> companies);
}
