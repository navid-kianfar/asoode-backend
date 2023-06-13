using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Asoode.Core.Contracts.General;
using Asoode.Core.Helpers;
using Asoode.Core.Primitives;
using Asoode.Core.ViewModels.General;
using Asoode.Core.ViewModels.Payment;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Business.General;

internal class SmsBiz : ISmsBiz
{
    private readonly IServiceProvider _serviceProvider;

    public SmsBiz(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<OperationResult<bool>> Send(string mobile, string message)
    {
        var op = OperationResult<bool>.Success(true);
        return Task.FromResult(op);
    }

    public async Task<string> ChangeNumber(string code)
    {
        var content = await LoadTemplate("change-phone");
        return content.Trim()
            .Replace("{token}", code);
    }

    public async Task<string> Forget(string token)
    {
        var content = await LoadTemplate("forget");
        return content.Trim().Replace("{token}", token);
    }

    public async Task<string> LoadTemplate(string name)
    {
        var root = _serviceProvider.GetService<IServerInfo>().SmsRootPath;
        var lang = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
        var combined = Path.Combine(root, $"{name}.{lang}.txt");
        var destination = File.Exists(combined) ? combined : Path.Combine(root, $"{name}.en.txt");
        return await IOHelper.ReadText(destination);
    }

    public async Task<string> Register(string code)
    {
        var content = await LoadTemplate("register");
        return content.Trim()
            .Replace("{token}", code);
    }

    public async Task<OperationResult<bool>> OrderCreated(string planTitle, MemberInfoViewModel user,
        OrderViewModel order)
    {
        var content = await LoadTemplate("order-created");
        content = content.Trim()
            .Replace("{fullName}", user.FullName)
            .Replace("{amount}", order.PaymentAmount.ToString("#,##0"))
            .Replace("{date}", DateHelper.Format(order.CreatedAt))
            .Replace("{id}", order.Id.ToString());

        return await Send(user.Phone, content);
    }

    public async Task<OperationResult<bool>> OrderPaid(MemberInfoViewModel user, OrderViewModel order)
    {
        var content = await LoadTemplate("order-paid");
        content = content.Trim()
            .Replace("{fullName}", user.FullName)
            .Replace("{amount}", order.PaymentAmount.ToString("#,##0"))
            .Replace("{date}", DateHelper.Format(order.CreatedAt));

        return await Send(user.Phone, content);
    }
}