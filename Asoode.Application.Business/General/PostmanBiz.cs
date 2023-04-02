using Asoode.Application.Core.Contracts.General;
using Asoode.Application.Core.Primitives;
using Asoode.Application.Core.ViewModels.ProjectManagement;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Application.Business.General
{
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
    }
}