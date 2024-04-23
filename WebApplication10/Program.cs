using Newtonsoft.Json;
using System;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("openExchangeRates", client =>
{
    client.BaseAddress = new Uri("https://openexchangerates.org/");
});

builder.Services.AddSignalR();

var app = builder.Build();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<CurrencyHub>("/currencyHub");
    endpoints.MapGet("/currency", async context =>
    {
        var httpClientFactory = context.RequestServices.GetRequiredService<IHttpClientFactory>();
        var client = httpClientFactory.CreateClient("openExchangeRates");
        var response = await client.GetAsync($"api/latest.json?app_id=3913694cd3cf4cb59263b9d44de60a69");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);

            var currencyResponse = JsonConvert.DeserializeObject<CurrencyResponse>(content);

            // Prepare a dictionary to hold the required currency rates
            var selectedRates = new Dictionary<string, decimal>
            {
                {"USD", currencyResponse.Rates["USD"]}, // Base currency
                {"UAH", currencyResponse.Rates["UAH"]},
                {"EUR", currencyResponse.Rates["EUR"]},
                {"JPY", currencyResponse.Rates["JPY"]}
                // Add more currencies as needed
            };

            // Prepare the response content
            var responseContent = $"Base: {currencyResponse.Base}\n";
            foreach (var rate in selectedRates)
            {
                responseContent += $"{rate.Key}: {rate.Value}\n";
            }

            await context.Response.WriteAsync(responseContent);
        }
        else
        {
            context.Response.StatusCode = (int)response.StatusCode;
            await context.Response.WriteAsync("Failed to retrieve currency rates");
        }
    });
});

app.Run();
