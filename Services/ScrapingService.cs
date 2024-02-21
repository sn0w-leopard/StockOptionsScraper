using System.Text.Json;
using HtmlAgilityPack;
using StockOptionsScraper.Interfaces;
using StockOptionsScraper.Models;
namespace StockOptionsScraper.Services;

public class ScrapingService: IScrapingService
{
    public async Task<List<MoneyWebCompany>> GetCompaniesAsync()
    {
        try
        {
            var url = "https://www.moneyweb.co.za/wp-admin/admin-ajax.php";

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("action", "load_clickacompany_dropdown"),
                new KeyValuePair<string, string>("dropdownID", "goto-companynews"),
                new KeyValuePair<string, string>("dropdownType", "search-shares")
            });

            var httpClient = new HttpClient();

            var response = await httpClient.PostAsync(url, formContent);
            var json = await response.Content.ReadAsStringAsync();
            var companies = JsonSerializer.Deserialize<List<MoneyWebCompany>>(json);

            if (companies == null) 
            {
                throw new Exception("Failed to deserialize company list");
            }
            
            return companies;
        }
        catch (Exception ex)
        {
            throw new Exception("Error getting company list", ex); 
        }
    }


    public async Task<MoneyWebForecast> GetForecastAsync(string companyCode)
    {
        try
        {
            var url = $"https://www.moneyweb.co.za/tools-and-data/click-a-company/{companyCode}/";
            var htmlTableName = "m1010 table table-condensed text-right dataTable no-footer";

            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            // Find table by class name
            var finTable = htmlDoc.DocumentNode.Descendants("table")
                .FirstOrDefault(t => t.GetAttributeValue("class", "")
                    .Contains(htmlTableName));
            var companyName = htmlDoc.DocumentNode.Descendants("span")
                .FirstOrDefault(span => span.HasClass("cac_companyname"));

            if (finTable != null)
            {
                var forecast = new MoneyWebForecast();
                forecast.CompanyCode = companyCode;
                forecast.CompanyName = companyName?.InnerText;
                var rows = finTable.Descendants("tr");
                foreach (var row in rows)
                {
                    // Get label and value cells
                    var labelCell = row.Descendants("td").First();
                    var valueCell = row.Descendants("td").Skip(1).First();

                    // Check label text
                    switch (labelCell.InnerText.Trim())
                    {
                        case "Market cap:":
                            forecast.MarketCap = valueCell.InnerText;
                            break;
                        case "EPS - TTM:":
                            forecast.EPS_TTM = valueCell.InnerText;
                            break;
                        case "P/E ratio:":
                            forecast.PEratio = valueCell.InnerText;
                            break;
                        case "Forward P/E:":
                            forecast.ForwardPE = valueCell.InnerText;
                            break;
                        case "Dividend yield:":
                            forecast.DividendYield = valueCell.InnerText;
                            break;
                        case "Return on equity:":
                            forecast.ReturnOnEquity = valueCell.InnerText;
                            break;
                        case "Return on assets:":
                            forecast.ReturnOnAssets = valueCell.InnerText;
                            break;
                        case "Net asset value:":
                            forecast.NetAssetValue = valueCell.InnerText;
                            break;
                        case "Price/NAV:":
                            forecast.PriceNAV = valueCell.InnerText;
                            break;
                        case "Authorised shares:":
                            forecast.AuthorisedShares = valueCell.InnerText;
                            break;
                        case "Issued shares:":
                            forecast.IssuedShares = valueCell.InnerText;
                            break;
                        default:
                            break;
                    }
                }
                return forecast;
            }
            else 
            {
                throw new  Exception("Could not find table with financial data");
            }
        }    
        catch (Exception ex)
        {
            throw new Exception("Error occured while rtrieving forecasts", ex);
        }
    }

    public async Task<List<MoneyWebForecast>> GetMoneyWebForecastList(List<MoneyWebCompany> companies)
    {
        try
        {            
            var forecasts = new List<MoneyWebForecast>();

            foreach (var company in companies)
            {
                if (!String.IsNullOrEmpty(company.value))
                {
                    var forecast = await GetForecastAsync(company.value);
                    forecasts.Add(forecast); 
                }
            }

            return forecasts;
        }
        catch (Exception ex)
        {
            throw new Exception("Error occured while rtrieving forecasts", ex);
        }
    }
}
