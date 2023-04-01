namespace Asoode.Application.Core.ViewModels.Payment.PayPing;

public class PKCEViewModel
{
    public string Code { get; set; }
    public string CodeVerifier { get; set; }
    public string CodeChallenge { get; set; }
}