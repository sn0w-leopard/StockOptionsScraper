using Abot2.Crawler;
using Abot2.Poco;
using HtmlAgilityPack;
using StockOptionsScraper.Interfaces;
using StockOptionsScraper.Models;
using System.Text.Json;

namespace StockOptionsScraper.Services
{
    public class AbotScrapingService : IScrapingService
    {
        private readonly PoliteWebCrawler _crawler;
        private readonly HttpClient _httpClient;

        public AbotScrapingService()
        {
            var config = new CrawlConfiguration
            {
                MinCrawlDelayPerDomainMilliSeconds = 1000,
                MaxConcurrentThreads = 10,
                UserAgentString = "StockOptionsScraper Bot"
            };
            _crawler = new PoliteWebCrawler(config);
            _httpClient = new HttpClient();
        }

        public async Task<MoneyWebForecast> GetForecastAsync(string companyCode)
        {
            var url = $"https://www.moneyweb.co.za/tools-and-data/click-a-company/{companyCode}/";
            var web = new HtmlWeb();
            var htmlDoc = await web.LoadFromWebAsync(url);

            var htmlTableName = "m1010 table table-condensed text-right dataTable no-footer";
            var finTable = htmlDoc.DocumentNode.Descendants("table")
                .FirstOrDefault(t => t.GetAttributeValue("class", "")
                    .Contains(htmlTableName));
            var companyName = htmlDoc.DocumentNode.Descendants("span")
                .FirstOrDefault(span => span.HasClass("cac_companyname"));

            if (finTable != null)
            {
                var forecast = new MoneyWebForecast
                {
                    CompanyCode = companyCode,
                    CompanyName = companyName?.InnerText
                };

                var rows = finTable.Descendants("tr");
                foreach (var row in rows)
                {
                    var labelCell = row.Descendants("td").First();
                    var valueCell = row.Descendants("td").Skip(1).First();

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
                    }
                }
                return forecast;
            }
            throw new Exception("Could not find table with financial data");
        }

        public async Task<MoneyWebBalanceSheet> GetBalanceSheetAsync(string companyCode)
        {
            var url = $"https://www.moneyweb.co.za/tools-and-data/click-a-company/{companyCode}";
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(url);

            var balanceSheet = new MoneyWebBalanceSheet { CompanyCode = companyCode };
            var tables = doc.DocumentNode.SelectNodes("//table");
            
            var balanceSheetTable = tables?.FirstOrDefault(t => 
                t.PreviousSibling?.PreviousSibling?
                .SelectNodes(".//li")?
                .Any(li => li.InnerText.Contains("BALANCE SHEET")) ?? false);

            if (balanceSheetTable != null)
            {
                var headers = balanceSheetTable.SelectNodes(".//thead//h6")?.ToList();
                if (headers?.Count >= 3)
                {
                    balanceSheet.FirstPeriodName = headers[1].InnerText.Trim();
                    balanceSheet.SecondPeriodName = headers[2].InnerText.Trim();
                }

                var rows = balanceSheetTable.SelectNodes(".//tr[.//a[contains(@id, 'display_balancesheet')]]");
                if (rows != null)
                {
                    foreach (var row in rows)
                    {
                        var cells = row.SelectNodes(".//td")?.ToList();
                        if (cells != null && cells.Count >= 3)
                        {
                            var title = cells[0].SelectSingleNode(".//a").GetAttributeValue("title", "");
                            var firstPeriodValue = cells[1].InnerText.Trim();
                            var secondPeriodValue = cells[2].InnerText.Trim();

                            switch (title)
                            {
                                case "Fixed Assets":
                                    balanceSheet.FixedAssetsFirstPeriod = firstPeriodValue;
                                    balanceSheet.FixedAssetsSecondPeriod = secondPeriodValue;
                                    break;
                                case "Total Current Assets":
                                    balanceSheet.TotalCurrentAssetsFirstPeriod = firstPeriodValue;
                                    balanceSheet.TotalCurrentAssetsSecondPeriod = secondPeriodValue;
                                    break;
                                case "Ordinary Shareholders' Interest":
                                    balanceSheet.OrdinaryShareholdersInterestFirstPeriod = firstPeriodValue;
                                    balanceSheet.OrdinaryShareholdersInterestSecondPeriod = secondPeriodValue;
                                    break;
                                case "Minority Interest":
                                    balanceSheet.MinorityInterestFirstPeriod = firstPeriodValue;
                                    balanceSheet.MinorityInterestSecondPeriod = secondPeriodValue;
                                    break;
                                case "Total Long-Term Liabilities":
                                    balanceSheet.TotalLongTermLiabilitiesFirstPeriod = firstPeriodValue;
                                    balanceSheet.TotalLongTermLiabilitiesSecondPeriod = secondPeriodValue;
                                    break;
                                case "Total Current Liabilities":
                                    balanceSheet.TotalCurrentLiabilitiesFirstPeriod = firstPeriodValue;
                                    balanceSheet.TotalCurrentLiabilitiesSecondPeriod = secondPeriodValue;
                                    break;
                            }
                        }
                    }
                }
            }
            return balanceSheet;
        }

        public async Task<List<MoneyWebCompany>> GetCompaniesAsync()
        {
            var url = "https://www.moneyweb.co.za/wp-admin/admin-ajax.php";
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("action", "load_clickacompany_dropdown"),
                new KeyValuePair<string, string>("dropdownID", "goto-companynews"),
                new KeyValuePair<string, string>("dropdownType", "search-shares")
            });

            var response = await _httpClient.PostAsync(url, formContent);
            var json = await response.Content.ReadAsStringAsync();
            var companies = JsonSerializer.Deserialize<List<MoneyWebCompany>>(json);

            if (companies == null)
            {
                throw new Exception("Failed to deserialize company list");
            }
            return companies;
        }

        public async Task<List<MoneyWebForecast>> GetMoneyWebForecastList(List<MoneyWebCompany> companies)
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

        public async Task<List<MoneyWebBalanceSheet>> GetMoneyWebBalanceSheetList(List<MoneyWebCompany> companies)
        {
            var balanceSheets = new List<MoneyWebBalanceSheet>();
            foreach (var company in companies)
            {
                if (!String.IsNullOrEmpty(company.value))
                {
                    var balanceSheet = await GetBalanceSheetAsync(company.value);
                    balanceSheets.Add(balanceSheet);
                }
            }
            return balanceSheets;
        }
    }
}
