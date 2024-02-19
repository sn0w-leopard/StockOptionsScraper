using StockOptionsScraper.Models;
namespace StockOptionsScraper.Interfaces;

public interface IScrapingService
{
  public Task<MoneyWebForecast> GetForecastAsync(string companyCode);
}
