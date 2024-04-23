using Microsoft.AspNetCore.SignalR;

public class CurrencyHub : Hub
{
    public async Task SendCurrencyUpdate(CurrencyResponse currencyResponse)
    {
        await Clients.All.SendAsync("ReceiveCurrencyUpdate", currencyResponse);
    }
}
