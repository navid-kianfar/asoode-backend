namespace Asoode.Application.Core.ViewModels.Payment.PayPing;

public class ResponseTokenViewModel
{
    public string access_token { get; set; }
    public long expires_in { get; set; }
    public string token_type { get; set; }
}