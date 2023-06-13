using System;
using System.Net;
using System.Net.Mail;

namespace Asoode.Console;

internal class Program
{
    private static readonly string GmailAccount = "asoode.work@gmail.com";
    private static readonly string GmailPassword = "rwhaoeetblnkvtgf";
    private static string SmtpHost = "smtp.gmail.com";
    private static int SmtpPort = 587;

    private static void Main(string[] args)
    {
        try
        {
            var ToEmails =
                "nvd.kianfar@gmail.com"; // this is the addresses to send the email to; can be the same Gmail account or another email address; separate multiple emails with commas

            var mail = new MailMessage(GmailAccount, ToEmails);
            mail.Subject = "test gmail send";
            mail.IsBodyHtml = true;
            mail.Body = "<!doctype html><html lang=en-us><body><h1>Hello world</h1></body></html>";

            var smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(GmailAccount, GmailPassword);
            smtp.Send(mail);
        }
        catch (Exception ex)
        {
            System.Console.WriteLine(ex.ToString());
        }
    }
}