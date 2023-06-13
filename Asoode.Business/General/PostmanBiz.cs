using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asoode.Core.Contracts.General;
using Asoode.Core.Primitives;
using Asoode.Core.ViewModels.General;
using Asoode.Core.ViewModels.Payment;
using Asoode.Core.ViewModels.ProjectManagement;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Business.General;

internal class PostmanBiz : IPostmanBiz
{
    private readonly IServiceProvider _serviceProvider;

    public PostmanBiz(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<OperationResult<bool>> EmailChange(string userId, string email, string token)
    {
        var subject = _serviceProvider.GetService<ITranslateBiz>().Get("EMAILS_CHANGE_TITLE");
        return await _serviceProvider.GetService<IEmailBiz>().ChangeEmail(subject, userId, email, token);
    }

    public async Task<OperationResult<bool>> EmailConfirmAccount(string userId, string email, string token)
    {
        var subject = _serviceProvider.GetService<ITranslateBiz>().Get("EMAILS_REGISTER_TITLE");
        return await _serviceProvider.GetService<IEmailBiz>().Register(subject, userId, email, token);
    }

    public async Task<OperationResult<bool>> EmailForgetPassword(string userId, string email, string token)
    {
        var subject = _serviceProvider.GetService<ITranslateBiz>().Get("EMAILS_FORGET_PASSWORD_TITLE");
        return await _serviceProvider.GetService<IEmailBiz>().Forget(subject, userId, email, token);
    }

    public async Task InviteGroup(string fullName, string[] noneMembers, Dictionary<string, string> members,
        Guid groupId, string groupTitle)
    {
        var subject = _serviceProvider.GetService<ITranslateBiz>().Get("YOU_HAVE_BEEN_INVITED");
        await _serviceProvider.GetService<IEmailBiz>()
            .InviteGroup(subject, fullName, noneMembers, members, groupId, groupTitle);
    }

    public async Task<OperationResult<bool>> EmailWelcome(string userId, string email)
    {
        var subject = _serviceProvider.GetService<ITranslateBiz>().Get("EMAILS_WELCOME_TITLE");
        return await _serviceProvider.GetService<IEmailBiz>().EmailWelcome(subject, userId, email);
    }

    public async Task InviteProject(string fullName, string[] noneMembers, Dictionary<string, string> members,
        ProjectViewModel payload)
    {
        var subject = _serviceProvider.GetService<ITranslateBiz>().Get("YOU_HAVE_BEEN_INVITED");
        var projectName = payload.Title;
        var section = payload.Complex ? "project" : "work-package";
        var id = (payload.Complex ? payload.Id : payload.WorkPackages.First().Id).ToString();
        await _serviceProvider.GetService<IEmailBiz>()
            .InviteProject(subject, fullName, noneMembers, members, section, id, projectName);
    }

    public async Task InviteWorkPackage(string fullName, string[] noneMembers, Dictionary<string, string> members,
        WorkPackageViewModel payload)
    {
        var subject = _serviceProvider.GetService<ITranslateBiz>().Get("YOU_HAVE_BEEN_INVITED");
        var projectName = payload.Title;
        var id = payload.Id.ToString();
        await _serviceProvider.GetService<IEmailBiz>()
            .InviteProject(subject, fullName, noneMembers, members, "work-package", id, projectName);
    }

    public async Task OrderCreated(string planTitle, MemberInfoViewModel user, OrderViewModel order)
    {
        if (!string.IsNullOrEmpty(user.Phone))
            await _serviceProvider.GetService<ISmsBiz>()
                .OrderCreated(planTitle, user, order);
        var subject = _serviceProvider.GetService<ITranslateBiz>().Get("ORDER_CREATED");
        await _serviceProvider.GetService<IEmailBiz>()
            .OrderCreated(subject, planTitle, user, order);
    }

    public async Task OrderPaid(MemberInfoViewModel user, OrderViewModel order)
    {
        if (!string.IsNullOrEmpty(user.Phone))
            await _serviceProvider.GetService<ISmsBiz>()
                .OrderPaid(user, order);
        var subject = _serviceProvider.GetService<ITranslateBiz>().Get("ORDER_CREATED");
        await _serviceProvider.GetService<IEmailBiz>()
            .OrderPaid(subject, user, order);
    }

    public async Task<OperationResult<bool>> Reply(string email, string message)
    {
        var subject = _serviceProvider.GetService<ITranslateBiz>().Get("CONTACT_ANSWERED");
        return await _serviceProvider.GetService<IEmailBiz>()
            .Reply(subject, email, message);
    }

    public async Task<OperationResult<bool>> PhoneChange(string phone, string token)
    {
        var smsService = _serviceProvider.GetService<ISmsBiz>();
        var sms = await smsService.ChangeNumber(token);
        return await smsService.Send(phone, sms);
    }

    public async Task<OperationResult<bool>> PhoneConfirmAccount(string phone, string token)
    {
        var smsService = _serviceProvider.GetService<ISmsBiz>();
        var sms = await smsService.Register(token);
        return await smsService.Send(phone, sms);
    }

    public async Task<OperationResult<bool>> PhoneForgetPassword(string phone, string token)
    {
        var smsService = _serviceProvider.GetService<ISmsBiz>();
        var content = await smsService.Forget(token);
        return await smsService.Send(phone, content);
    }
}