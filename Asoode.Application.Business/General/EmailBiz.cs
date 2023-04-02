using System.Net;
using System.Net.Mail;
using Asoode.Application.Core.Contracts.General;
using Asoode.Application.Core.Contracts.Logging;
using Asoode.Application.Core.Helpers;
using Asoode.Application.Core.Primitives;
using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.General;
using Asoode.Application.Core.ViewModels.Payment;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Application.Business.General
{
    internal class EmailBiz : IEmailBiz
    {
        private readonly IServiceProvider _serviceProvider;

        public EmailBiz(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Sender = $"postman@{configuration["Setting:Domain"]}";
            SmtpUsername = configuration["Setting:Email:Smtp:Username"];
            SmtpPassword = configuration["Setting:Email:Smtp:Password"];
            SmtpHost = configuration["Setting:Email:Smtp:Host"];
            SmtpPort = Convert.ToInt32(configuration["Setting:Email:Smtp:Port"]);
            SmtpSsl = Convert.ToBoolean(configuration["Setting:Email:Smtp:SSL"]);
        }

        private string Sender { get; set; }
        private string SmtpHost { get; set; }
        private string SmtpPassword { get; set; }
        private int SmtpPort { get; set; }
        private bool SmtpSsl { get; set; }
        private string SmtpUsername { get; set; }

        public async Task<string> LoadTemplate(string template)
        {
            var root = _serviceProvider.GetService<IServerInfo>().EmailsRootPath;
            var lang = _serviceProvider.GetService<IConfiguration>()["Setting:I18n:Default"];
            var combined = Path.Combine(root, $"{template}.{lang}.html");
            var destination = File.Exists(combined) ? combined : Path.Combine(root, $"{template}.en.html");
            return await IOHelper.ReadText(destination);
        }

        public async Task<OperationResult<bool>> Send(string subject, string to, string body, bool isHtml = true)
        {
            try
            {
                var msg = new MailMessage
                {
                    From = new MailAddress(Sender),
                    Subject = subject,
                    Body = body,
                    BodyEncoding = System.Text.Encoding.UTF8,
                    SubjectEncoding = System.Text.Encoding.UTF8,
                    IsBodyHtml = isHtml
                };
                msg.To.Add(to);
                var client = new SmtpClient
                {
                    UseDefaultCredentials = true,
                    Host = SmtpHost,
                    Port = SmtpPort,
                    EnableSsl = SmtpSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(SmtpUsername, SmtpPassword),
                    Timeout = 20000
                };
                await client.SendMailAsync(msg);
                client.Dispose();
                msg.Dispose();
                return OperationResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                // await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                // return OperationResult<bool>.Failed();
                // TODO: fix this
                await _serviceProvider.GetService<IErrorBiz>().LogException(new Exception("Sending Email Failed", ex));
                return OperationResult<bool>.Success(false);
            }
        }

        // public void BulkSend(string subject, Dictionary<string, string> messages, bool isHtml = true)
        // {
        //     foreach (var message in messages)
        //     {
        //         Task.Run(() => Send(subject, message.Key, message.Value, isHtml));
        //     }
        // }

        public async Task InviteGroup(string subject, string fullName, string[] noneMembers,
            Dictionary<string, string> members, Guid groupId, string groupTitle)
        {
            if (noneMembers.Length > 0)
            {
                var body = (await LoadTemplate("invite-asoode"))
                    .Replace("{{fullName}}", fullName);
                foreach (var member in noneMembers)
                    await Send(subject, member, body);
            }

            if (members.Keys.Count > 0)
            {
                var contentForMembers = await LoadTemplate("invite-team");
                foreach (var member in members)
                {
                    var body = contentForMembers
                        .Replace("{{fullName}}", fullName)
                        .Replace("{{groupName}}", groupTitle)
                        .Replace("{{groupId}}", groupId.ToString())
                        .Replace("{{userId}}", member.Key);
                    await Send(subject, member.Value, body);
                }
            }
        }

        public async Task InviteProject(string subject, string fullName, string[] noneMembers,
            Dictionary<string, string> members, string section, string id, string projectName)
        {
            if (noneMembers.Length > 0)
            {
                var body = (await LoadTemplate("invite-asoode"))
                    .Replace("{{fullName}}", fullName);
                foreach (var member in noneMembers)
                    await Send(subject, member, body);
            }

            if (members.Keys.Count > 0)
            {
                var contentForMembers = await LoadTemplate("invite-project");
                foreach (var member in members)
                {
                    var body = contentForMembers
                        .Replace("{{fullName}}", fullName)
                        .Replace("{{projectName}}", projectName)
                        .Replace("{{id}}", id)
                        .Replace("{{section}}", section)
                        .Replace("{{userId}}", member.Key);
                    await Send(subject, member.Value, body);
                }
            }
        }

        public async Task<OperationResult<bool>> OrderCreated(string subject, string planTitle,
            MemberInfoViewModel user, OrderViewModel order)
        {
            string fileName;
            switch (order.OrderType)
            {
                case OrderType.Patch:
                    fileName = "order-patch";
                    break;
                case OrderType.Change:
                    fileName = "order-change";
                    break;
                default:
                    fileName = "order-renew";
                    break;
            }
            
            var now = DateTime.UtcNow;
            var content = (await LoadTemplate(fileName)).Trim();

            var users = order.Users;
            var usersFee = users * order.AdditionalUserCost;
            
            var projects = order.Project;
            var projectsFee = projects * order.AdditionalProjectCost;
            
            var packages = order.WorkPackage;
            var packagesFee = packages * order.AdditionalWorkPackageCost;
            
            var simpleGroups = order.SimpleGroup;
            var simpleGroupsFee = simpleGroups * order.AdditionalSimpleGroupCost;
            
            var complexGroups = order.ComplexGroup;
            var complexGroupsFee = complexGroups * order.AdditionalComplexGroupCost;
            
            var space = (order.DiskSpace) / 1024 / 1024 / 1024;
            var spaceFee = space * order.AdditionalSpaceCost;
            
            string html = content
                .Replace("{{extraUsers}}", users.ToString("#,##0"))
                .Replace("{{extraUsersFee}}", usersFee.ToString("#,##0"))
                
                .Replace("{{extraSpace}}", space.ToString("#,##0"))
                .Replace("{{extraSpaceFee}}", spaceFee.ToString("#,##0"))
                
                .Replace("{{extraProject}}", projects.ToString("#,##0"))
                .Replace("{{extraProjectFee}}", projectsFee.ToString("#,##0"))
                
                .Replace("{{extraPackage}}", packages.ToString("#,##0"))
                .Replace("{{extraPackageFee}}", packagesFee.ToString("#,##0"))
                
                .Replace("{{extraSimpleGroup}}", simpleGroups.ToString("#,##0"))
                .Replace("{{extraSimpleGroupFee}}", simpleGroupsFee.ToString("#,##0"))
                
                .Replace("{{extraComplexGroup}}", complexGroups.ToString("#,##0"))
                .Replace("{{extraComplexGroupFee}}", complexGroupsFee.ToString("#,##0"))
                
                .Replace("{{totalSum}}", order.TotalAmount.ToString("#,##0"))
                .Replace("{{totalDiscount}}", order.AppliedDiscount?.ToString("#,##0"))
                .Replace("{{total}}", order.PaymentAmount.ToString("#,##0"))
                .Replace("{{valueAdded}}", order.ValueAdded.ToString("#,##0"))
                
                .Replace("{{fullName}}", user.FullName)
                .Replace("{{planName}}", planTitle)
                .Replace("{{days}}", order.Days.ToString("#,##0"))
                .Replace("{{planFee}}", order.PlanCost.ToString("#,##0"))
                .Replace("{{paymentDate}}", DateHelper.Format(now))
                .Replace("{{from}}", DateHelper.Format(order.CreatedAt))
                .Replace("{{to}}", DateHelper.Format(now.AddDays(order.Days)))
                .Replace("{{id}}", order.Id.ToString());
            
            if (string.IsNullOrEmpty(user.Email) || user.Email.Contains("@asoode.user"))
            {
                return OperationResult<bool>.Success(true);
            }
            return await Send(subject, user.Email, html);
        }

        public async Task<OperationResult<bool>> OrderPaid(string subject, MemberInfoViewModel user, OrderViewModel order)
        {
            var now = DateTime.UtcNow;
            var content = (await LoadTemplate("order-paid")).Trim();
            
            string html = content
                .Replace("{{totalSum}}", order.TotalAmount.ToString("#,##0"))
                .Replace("{{totalDiscount}}", order.AppliedDiscount?.ToString("#,##0"))
                .Replace("{{total}}", order.PaymentAmount.ToString("#,##0"))
                .Replace("{{valueAdded}}", order.ValueAdded.ToString("#,##0"))
                
                .Replace("{{fullName}}", user.FullName)
                .Replace("{{planName}}", "")
                .Replace("{{planFee}}", order.PlanCost.ToString("#,##0"))
                .Replace("{{paymentDate}}", DateHelper.Format(now))
                .Replace("{{from}}", DateHelper.Format(order.CreatedAt))
                .Replace("{{to}}", DateHelper.Format(now.AddDays(order.Days)))
                .Replace("{{id}}", order.Id.ToString());
            
            if (string.IsNullOrEmpty(user.Email) || user.Email.Contains("@asoode.user"))
            {
                return OperationResult<bool>.Success(true);
            }
            return await Send(subject, user.Email, html);
        }

        public async Task<OperationResult<bool>> Reply(string subject, string email, string message)
        {
            var content = (await LoadTemplate("contact-reply")).Trim()
                .Replace("{{replyMessage}}", message)
                .Replace("\r\n", "<br />");
            return await Send(subject, email, content);
        }

        public async Task<OperationResult<bool>> EmailWelcome(string subject, string userId, string email)
        {
            var content = (await LoadTemplate("welcome")).Trim()
                .Replace("{{userId}}", userId);
            return await Send(subject, email, content);
        }

        public async Task<OperationResult<bool>> Register(string subject, string userId, string email, string token)
        {
            var content = (await LoadTemplate("register")).Trim()
                .Replace("{{userId}}", userId)
                .Replace("{{code}}", token);
            return await Send(subject, email, content);
        }

        public async Task<OperationResult<bool>> ChangeEmail(string subject, string userId, string email, string token)
        {
            var content = (await LoadTemplate("change-email")).Trim()
                .Replace("{{userId}}", userId)
                .Replace("{{code}}", token);
            return await Send(subject, email, content);
        }

        public async Task<OperationResult<bool>> Forget(string subject, string userId, string email, string token)
        {
            var content = (await LoadTemplate("forget")).Trim()
                .Replace("{{userId}}", userId)
                .Replace("{{code}}", token);
            return await Send(subject, email, content);
        }
    }
}