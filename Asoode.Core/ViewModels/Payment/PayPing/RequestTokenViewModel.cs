﻿namespace Asoode.Core.ViewModels.Payment.PayPing;

public class RequestTokenViewModel
{
    public string client_id { get; set; }
    public string client_secret { get; set; }
    public string grant_type { get; set; }
    public string Username { get; set; }
    public string password { get; set; }
    public string scope { get; set; }
}