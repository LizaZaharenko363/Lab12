using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class CurrencyService
{
    private const string BaseUrl = "https://openexchangerates.org/api/latest.json";
    private readonly string _appId;

    public CurrencyService(string appId)
    {
        _appId = appId;
    }

    public async Task<CurrencyResponse> GetLatestRatesAsync()
    {
        using (var httpClient = new HttpClient())
        {
            var requestUrl = $"{BaseUrl}?app_id={_appId}";
            Console.WriteLine(requestUrl);
            var response = await httpClient.GetAsync($"{BaseUrl}?app_id={_appId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var currencyResponse = JsonConvert.DeserializeObject<CurrencyResponse>(content);
                return currencyResponse;
            }
            else
            {
                throw new Exception("Failed to retrieve currency rates");
            }
        }
    }
}

public class CurrencyResponse
{
    public string Base { get; set; }
    public long Timestamp { get; set; } 
    public IDictionary<string, decimal> Rates { get; set; }
}



