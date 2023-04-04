using System.Text;
using Asoode.Application.Core.Contracts.General;
using Asoode.Application.Core.Contracts.Logging;
using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.General;
using Asoode.Application.Core.ViewModels.Logging;
using Asoode.Application.Data.Contexts;
using Asoode.Application.Data.Models;
using Asoode.Application.Data.Models.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Application.Business.Logging
{
    internal class ActivityBiz : IActivityBiz
    {
        #region Properties

        private readonly ITranslateBiz _translateBiz;
        private readonly IServiceProvider _serviceProvider;
        private readonly IJsonService _jsonService;
        private readonly string QueueServer;
        private readonly string QueueUsername;
        private readonly string QueuePassword;
        private readonly string PushQueueName;
        private readonly string SocketQueueName;
        private readonly string Domain;

        #endregion

        public ActivityBiz(
            ITranslateBiz translateBiz,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IJsonService jsonService
        )
        {
            _translateBiz = translateBiz;
            _serviceProvider = serviceProvider;
            _jsonService = jsonService;
            QueueServer = configuration["Setting:MessageQueue:Server"];
            QueueUsername = configuration["Setting:MessageQueue:Username"];
            QueuePassword = configuration["Setting:MessageQueue:Password"];
            PushQueueName = $"{configuration["Setting:MessageQueue:Prefix"]}-{configuration["Setting:I18n:Default"]}-push";
            SocketQueueName = $"{configuration["Setting:MessageQueue:Prefix"]}-{configuration["Setting:I18n:Default"]}-socket";
            Domain = configuration["Setting:Domain"];
        }

        public async Task Enqueue(ActivityLogViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<ActivityDbContext>())
                {
                    var baseUrl = $"https://panel.{Domain}";
                    var invited = Array.Empty<MemberInfoViewModel>();
                    var notification = new NotificationViewModel {Url = baseUrl, Type = model.Type};
                    var activity = new ActivityLog {Type = model.Type, UserId = model.UserId};

                    switch (model.Type)
                    {
                        #region Account

                        case ActivityType.AccountEdit:
                            activity.RecordId = model.UserId;
                            notification.Title = "PUSH_ACCOUNT_EDIT_TITLE";
                            notification.Description = GetDescription("PUSH_ACCOUNT_EDIT_DESCRIPTION",
                                new[] {model.User.FullName});
                            notification.Avatar = model.User.Avatar;
                            notification.Data = model.User;
                            notification.Users = await FindEveryOne(unit, model.UserId);
                            break;
                        case ActivityType.AccountDeviceAdd:
                            activity.RecordId = model.Device.Id;
                            notification.Title = "PUSH_ACCOUNT_DEVICE_ADD_TITLE";
                            notification.Description = GetDescription("PUSH_ACCOUNT_DEVICE_ADD_DESCRIPTION",
                                new[] {model.User.FullName, model.Device.Os});
                            notification.Data = model.Device;
                            notification.Users = new [] { model.UserId };
                            break;
                        case ActivityType.AccountDeviceEdit:
                            activity.RecordId = model.Device.Id;
                            notification.Title = "PUSH_ACCOUNT_DEVICE_EDIT_TITLE";
                            notification.Description = GetDescription("PUSH_ACCOUNT_DEVICE_EDIT_DESCRIPTION",
                                new[] {model.User.FullName, model.Device.Title});
                            notification.Data = model.Device;
                            notification.Users = new [] { model.UserId };
                            break;
                        case ActivityType.AccountDeviceRemove:
                            activity.RecordId = model.Device.Id;
                            notification.Title = "PUSH_ACCOUNT_DEVICE_REMOVE_TITLE";
                            notification.Description = GetDescription("PUSH_ACCOUNT_DEVICE_REMOVE_DESCRIPTION",
                                new[] {model.User.FullName, model.Device.Title});
                            notification.Data = model.Device.Id;
                            notification.Users = new [] { model.UserId };
                            break;

                        #endregion

                        #region Channel

                        case ActivityType.ChannelMessage:
                            activity.RecordId = model.Conversation.Id;
                            notification.Title =
                                GetDescription("PUSH_CHANNEL_TEXT_MESSAGE_TITLE", new[] {model.User.FullName});
                            notification.Description =
                                GetDescription("PUSH_CHANNEL_TEXT_MESSAGE_DESCRIPTION", new string[]
                                {
                                    model.User.FullName,
                                    await FindChannelTitle(unit, model.Conversation.ChannelId)
                                });
                            notification.Data = model.Conversation;
                            notification.Url = $"{baseUrl}/messenger/{model.Conversation.ChannelId}";
                            notification.Users =
                                await FindChannelMembersIds(unit, model.UserId, model.Conversation.ChannelId);
                            break;
                        case ActivityType.ChannelUpload:
                            activity.RecordId = model.Conversation.Id;
                            notification.Title =
                                GetDescription("PUSH_CHANNEL_UPLOAD_MESSAGE_TITLE", new[] {model.User.FullName});
                            notification.Description =
                                GetDescription("PUSH_CHANNEL_UPLOAD_MESSAGE_DESCRIPTION", new string[]
                                {
                                    model.User.FullName,
                                    await FindChannelTitle(unit, model.Conversation.ChannelId)
                                });

                            notification.Data = model.Conversation;
                            notification.Url = $"{baseUrl}/messenger/{model.Conversation.ChannelId}";
                            notification.Users =
                                await FindChannelMembersIds(unit, model.UserId, model.Conversation.ChannelId);
                            break;

                        #endregion

                        #region Group

                        case ActivityType.GroupAdd:
                            invited = (await unit.Users.AsNoTracking()
                                    .Where(u => model.UserIds.Contains(u.Id)).ToArrayAsync())
                                .Select(u => u.ToViewModel()).ToArray();

                            activity.RecordId = model.Group.Id;
                            notification.Title = "PUSH_GROUP_ADD_TITLE";
                            notification.Description = GetDescription("PUSH_GROUP_ADD_DESCRIPTION",
                                new[] {model.User.FullName, model.Group.Title});
                            notification.Url = $"{baseUrl}/group/{model.Group.Id}";
                            foreach (var member in model.GroupMembers)
                                member.Member = invited.SingleOrDefault(u => u.Id == member.UserId);
                            model.Group.Members = model.GroupMembers;
                            model.Group.Pending = model.Pendings;
                            notification.Data = model.Group;
                            notification.Users = model.UserIds;
                            break;
                        case ActivityType.GroupEdit:
                            activity.RecordId = model.Group.Id;
                            notification.Title = "PUSH_GROUP_EDIT_TITLE";
                            notification.Description =
                                GetDescription("PUSH_GROUP_EDIT_DESCRIPTION",
                                    new[] {model.User.FullName, model.Group.Title});
                            notification.Url = $"{baseUrl}/group/{model.Group.Id}";
                            notification.Data = model.Group;
                            notification.Users = await FindGroupMembersIds(unit, model.Group.Id);
                            break;
                        case ActivityType.GroupRemove:
                            activity.RecordId = model.Group.Id;
                            notification.Title = "PUSH_GROUP_REMOVE_TITLE";
                            notification.Description =
                                GetDescription("PUSH_GROUP_REMOVE_DESCRIPTION",
                                    new[] {model.Group.Title, model.User.FullName});
                            notification.Data = model.Group.Id;
                            notification.Users = await FindGroupMembersIds(unit, model.Group.Id);
                            break;
                        case ActivityType.GroupArchive:
                            activity.RecordId = model.Group.Id;
                            notification.Title = "PUSH_GROUP_ARCHIVE_TITLE";
                            notification.Description =
                                GetDescription("PUSH_GROUP_ARCHIVE_DESCRIPTION",
                                    new[] {model.Group.Title, model.User.FullName});
                            notification.Data = model.Group.Id;
                            notification.Users = await FindGroupMembersIds(unit, model.Group.Id);
                            break;
                        case ActivityType.GroupRestore:
                            activity.RecordId = model.Group.Id;
                            notification.Title = "PUSH_GROUP_RESTORE_TITLE";
                            notification.Description =
                                GetDescription("PUSH_GROUP_RESTORE_DESCRIPTION",
                                    new[] {model.Group.Title, model.User.FullName});
                            notification.Data = model.Group;
                            notification.Users = model.Group.Members.Select(i => i.UserId).ToArray();
                            break;
                        case ActivityType.GroupMemberPermission:
                            notification.Title = "PUSH_GROUP_MEMBER_PERMISSION_TITLE";
                            if (model.GroupMember != null)
                            {
                                var users = await unit.Users
                                    .Where(i => i.Id == model.GroupMember.UserId || i.Id == model.UserId)
                                    .Select(u => new {u.Id, u.FullName}).ToArrayAsync();

                                notification.Description = GetDescription("PUSH_GROUP_MEMBER_PERMISSION_DESCRIPTION",
                                    new[]
                                    {
                                        users.Single(u => u.Id == model.UserId).FullName,
                                        users.Single(u => u.Id == model.GroupMember.UserId).FullName,
                                        await FindGroupTitle(unit, model.GroupMember.GroupId)
                                    });

                                activity.RecordId = model.GroupMember.GroupId;
                                notification.Url = $"{baseUrl}/group/{model.GroupMember.GroupId}";
                                notification.Data = model.GroupMember;
                                notification.Users = await FindGroupMembersIds(unit, model.GroupMember.GroupId);
                            }
                            else
                            {
                                activity.RecordId = model.Pending.RecordId;
                                notification.Description =
                                    GetDescription("PUSH_GROUP_MEMBER_PERMISSION_DESCRIPTION", new string[]
                                    {
                                        model.User.FullName,
                                        model.Pending.Identifier,
                                        await FindGroupTitle(unit, model.Pending.RecordId)
                                    });
                                notification.Url = $"{baseUrl}/group/{model.Pending.RecordId}";
                                notification.Data = model.Pending;
                                notification.Users = await FindGroupMembersIds(unit, model.Pending.RecordId);
                            }

                            break;
                        case ActivityType.GroupMemberRemove:
                            notification.Title = "PUSH_GROUP_MEMBER_REMOVE_TITLE";
                            if (model.GroupMember != null)
                            {
                                var users = await unit.Users
                                    .Where(i => i.Id == model.GroupMember.UserId || i.Id == model.UserId)
                                    .Select(u => new {u.Id, u.FullName}).ToArrayAsync();
                                notification.Description =
                                    GetDescription("PUSH_GROUP_MEMBER_REMOVE_DESCRIPTION", new[]
                                    {
                                        users.Single(u => u.Id == model.GroupMember.UserId).FullName,
                                        await FindGroupTitle(unit, model.GroupMember.GroupId),
                                        users.Single(u => u.Id == model.UserId).FullName
                                    });
                                activity.RecordId = model.GroupMember.GroupId;
                                notification.Url = $"{baseUrl}/group/{model.GroupMember.GroupId}";
                                notification.Data = model.GroupMember;
                                notification.Users = await FindGroupMembersIds(unit, model.GroupMember.GroupId);
                            }
                            else
                            {
                                activity.RecordId = model.Pending.RecordId;
                                notification.Description =
                                    GetDescription("PUSH_GROUP_MEMBER_REMOVE_DESCRIPTION", new[]
                                    {
                                        model.Pending.Identifier,
                                        await FindGroupTitle(unit, model.Pending.RecordId),
                                        await FindUserFullName(unit, model.UserId)
                                    });
                                notification.Url = $"{baseUrl}/group/{model.Pending.RecordId}";
                                notification.Data = model.Pending;
                                notification.Users = await FindGroupMembersIds(unit, model.Pending.RecordId);
                            }

                            break;
                        case ActivityType.GroupMemberAdd:
                            // invited = (await unit.Users.AsNoTracking()
                            //         .Where(u => model.UserIds.Contains(u.Id)).ToArrayAsync())
                            //     .Select(u => u.ToViewModel()).ToArray();
                            activity.RecordId = model.Group.Id;
                            notification.Title = "PUSH_GROUP_MEMBER_ADD_TITLE";
                            notification.Description =
                                GetDescription("PUSH_GROUP_MEMBER_ADD_DESCRIPTION", new string[]
                                {
                                    (model.Pendings.Length + model.GroupMembers.Length).ToString(),
                                    model.Group.Title,
                                    model.User.FullName
                                });
                            notification.Url = $"{baseUrl}/group/{model.Group.Id}";
                            notification.Data = new
                            {
                                model.Pending,
                                Members = model.GroupMembers,
                                GroupId = model.Group.Id
                            };
                            notification.Users = await FindGroupMembersIds(unit, model.Group.Id);

                            break;

                        case ActivityType.GroupWorkEntry:
                            notification.Title = !model.WorkEntry.EndAt.HasValue ? 
                                "PUSH_GROUP_ENTRY_TITLE": 
                                "PUSH_GROUP_EXIT_TITLE";
                            
                            notification.Description =
                                GetDescription(
                                    !model.WorkEntry.EndAt.HasValue ? 
                                        "PUSH_GROUP_ENTRY_DESCRIPTION": 
                                        "PUSH_GROUP_EXIT_DESCRIPTION",
                                    new[] {model.User.FullName, model.Group.Title});
                            
                            activity.RecordId = model.Group.Id;
                            notification.Url = $"{baseUrl}/group/{model.Group.Id}";
                            notification.Data = model.WorkEntry;
                            notification.Users = await FindGroupMembersIds(unit, model.Group.Id);

                            break;
                        case ActivityType.GroupTimeOffAdd:

                            // notification.Title = "PUSH_GROUP_TIME_OFF_ADD_TITLE";
                            // notification.Description = "PUSH_GROUP_TIME_OFF_ADD_DESCRIPTION";

                            break;
                        case ActivityType.GroupTimeOffResponse:

                            // notification.Title = "PUSH_GROUP_TIME_OFF_RESPONSE_TITLE";
                            // notification.Description = "PUSH_GROUP_TIME_OFF_RESPONSE_DESCRIPTION";

                            break;

                        #endregion

                        #region Project

                        case ActivityType.ProjectArchive:
                            activity.RecordId = model.Project.Id;
                            notification.Title = "PUSH_PROJECT_ARCHIVE_TITLE";
                            notification.Description = GetDescription("PUSH_PROJECT_ARCHIVE_DESCRIPTION",
                                new[]
                                {
                                    await FindUserFullName(unit, model.UserId), 
                                    model.Project.Title
                                });
                            notification.Data = model.Project;
                            notification.Users = await FindProjectMembersIds(unit, model.UserId, model.Project.Id);
                            break;
                        case ActivityType.ProjectAdd:
                            var projectId = model.Project.Complex
                                ? model.Project.Id
                                : model.Project.WorkPackages.First().Id;

                            activity.RecordId = projectId;
                            notification.Title = "PUSH_PROJECT_ADD_TITLE";
                            notification.Description = GetDescription("PUSH_PROJECT_ADD_DESCRIPTION",
                                new[] {model.User.FullName, model.Project.Title});
                            notification.Data = model.Project;
                            notification.Url = model.Project.Complex
                                ? $"{baseUrl}/project/{projectId}"
                                : $"{baseUrl}/work-package/{projectId}";
                            notification.Users = await FindProjectMembersIds(unit, model.UserId, model.Project.Id);
                            break;
                        case ActivityType.ProjectSubAdd:
                            activity.RecordId = model.SubProject.ProjectId;
                            notification.Title = "PUSH_PROJECT_SUB_ADD_TITLE";
                            notification.Description = GetDescription("PUSH_PROJECT_SUB_ADD_DESCRIPTION",
                                new[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.SubProject.Title
                                });
                            notification.Data = model.SubProject;
                            notification.Url = $"{baseUrl}/project/{model.SubProject.ProjectId}";
                            notification.Users =
                                await FindProjectMembersIds(unit, model.UserId, model.SubProject.ProjectId);
                            break;
                        case ActivityType.ProjectSubEdit:
                            notification.Title = "PUSH_PROJECT_SUB_EDIT_TITLE";
                            activity.RecordId = model.SubProject.ProjectId;
                            notification.Description = GetDescription("PUSH_PROJECT_SUB_EDIT_DESCRIPTION",
                                new[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.SubProject.Title
                                });
                            notification.Data = model.SubProject;
                            notification.Url = $"{baseUrl}/project/{model.SubProject.ProjectId}";
                            notification.Users =
                                await FindProjectMembersIds(unit, model.UserId, model.SubProject.ProjectId);
                            break;
                        case ActivityType.ProjectSubRemove:
                            activity.RecordId = model.SubProject.ProjectId;
                            notification.Title = "PUSH_PROJECT_SUB_REMOVE_TITLE";
                            notification.Description = GetDescription("PUSH_PROJECT_SUB_REMOVE_DESCRIPTION",
                                new[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.SubProject.Title
                                });
                            notification.Data = model.SubProject;
                            notification.Url = $"{baseUrl}/project/{model.SubProject.ProjectId}";
                            notification.Users =
                                await FindProjectMembersIds(unit, model.UserId, model.SubProject.ProjectId);
                            break;
                        case ActivityType.ProjectSeasonAdd:
                            activity.RecordId = model.Season.ProjectId;
                            notification.Description = GetDescription("PUSH_PROJECT_SEASON_ADD_DESCRIPTION",
                                new[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.Season.Title
                                });
                            notification.Title = "PUSH_PROJECT_SEASON_ADD_TITLE";
                            notification.Data = model.Season;
                            notification.Url = $"{baseUrl}/project/{model.Season.ProjectId}";
                            notification.Users =
                                await FindProjectMembersIds(unit, model.UserId, model.Season.ProjectId);

                            break;
                        case ActivityType.ProjectSeasonEdit:
                            activity.RecordId = model.Season.ProjectId;
                            notification.Description = GetDescription("PUSH_PROJECT_SEASON_EDIT_DESCRIPTION",
                                new[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.Season.Title
                                });
                            notification.Title = "PUSH_PROJECT_SEASON_EDIT_TITLE";
                            notification.Data = model.Season;
                            notification.Url = $"{baseUrl}/project/{model.Season.ProjectId}";
                            notification.Users =
                                await FindProjectMembersIds(unit, model.UserId, model.Season.ProjectId);
                            break;
                        case ActivityType.ProjectSeasonRemove:
                            activity.RecordId = model.Season.ProjectId;
                            notification.Description = GetDescription("PUSH_PROJECT_SEASON_REMOVE_DESCRIPTION",
                                new[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.Season.Title
                                });
                            notification.Title = "PUSH_PROJECT_SEASON_REMOVE_TITLE";
                            notification.Data = model.Season;
                            notification.Url = $"{baseUrl}/project/{model.Season.ProjectId}";
                            notification.Users =
                                await FindProjectMembersIds(unit, model.UserId, model.Season.ProjectId);

                            break;
                        case ActivityType.ProjectEdit:
                            activity.RecordId = model.Project.Id;
                            notification.Description = GetDescription("PUSH_PROJECT_EDIT_DESCRIPTION",
                                new[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.Project.Title
                                });
                            notification.Title = "PUSH_PROJECT_EDIT_TITLE";
                            notification.Data = model.Project;
                            notification.Url = $"{baseUrl}/project/{model.Project.Id}";
                            notification.Users = await FindProjectMembersIds(unit, model.UserId, model.Project.Id);

                            break;
                        case ActivityType.ProjectRemove:

                            notification.Title = "PUSH_PROJECT_REMOVE_TITLE";
                            activity.RecordId = model.Project.Id;
                            notification.Description = GetDescription("PUSH_PROJECT_REMOVE_DESCRIPTION",
                                new[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.Project.Title
                                });
                            notification.Data = model.Project;
                            notification.Users = model.UserIds;
                            break;

                        case ActivityType.ProjectMemberAdd:
                            activity.RecordId = model.Project.Id;
                            notification.Description = GetDescription("PUSH_PROJECT_MEMBER_ADD_DESCRIPTION",
                                new[]
                                {
                                    (model.Pendings.Length + model.ProjectMembers.Length).ToString(),
                                    model.Project.Title,
                                    model.User.FullName
                                });

                            notification.Title = "PUSH_PROJECT_MEMBER_ADD_TITLE";
                            notification.Data = new
                            {
                                model.Project,
                                Pending = model.Pendings,
                                Members = model.ProjectMembers
                            };
                            notification.Url = $"{baseUrl}/project/{model.Project.Id}";
                            notification.Users = await FindProjectMembersIds(unit, model.UserId, model.Project.Id);
                            break;
                        case ActivityType.ProjectMemberRemove:
                            notification.Title = "PUSH_PROJECT_MEMBER_REMOVE_TITLE";
                            if (model.ProjectMember != null)
                            {
                                activity.RecordId = model.ProjectMember.ProjectId;
                                notification.Description =
                                    GetDescription("PUSH_PROJECT_MEMBER_REMOVE_DESCRIPTION", new string[]
                                    {
                                        await FindProjectMemberName(unit, model.ProjectMember.RecordId),
                                        await FindProjectTitle(unit, model.ProjectMember.ProjectId),
                                        await FindUserFullName(unit, model.UserId)
                                    });
                                notification.Url = $"{baseUrl}/project/{model.ProjectMember.ProjectId}";
                                notification.Data = model.ProjectMember;
                                notification.Users = await FindProjectMembersIds(unit, model.UserId,
                                    model.ProjectMember.ProjectId);
                            }
                            else
                            {
                                activity.RecordId = model.Pending.RecordId;
                                notification.Description =
                                    GetDescription("PUSH_PROJECT_MEMBER_REMOVE_DESCRIPTION", new string[]
                                    {
                                        model.Pending.Identifier,
                                        await FindProjectTitle(unit, model.Pending.RecordId),
                                        await FindUserFullName(unit, model.UserId)
                                    });
                                notification.Url = $"{baseUrl}/project/{model.Pending.RecordId}";
                                notification.Data = model.Pending;
                                notification.Users =
                                    await FindProjectMembersIds(unit, model.UserId, model.Pending.RecordId);
                            }

                            break;
                        case ActivityType.ProjectMemberPermission:
                            notification.Title = "PUSH_PROJECT_MEMBER_PERMISSION_TITLE";
                            if (model.ProjectMember != null)
                            {
                                activity.RecordId = model.ProjectMember.ProjectId;
                                notification.Description =
                                    GetDescription("PUSH_PROJECT_MEMBER_PERMISSION_DESCRIPTION", new[]
                                    {
                                        await FindUserFullName(unit, model.UserId),
                                        await FindProjectMemberName(unit, model.ProjectMember.RecordId),
                                        await FindProjectTitle(unit, model.ProjectMember.ProjectId),
                                    });

                                notification.Data = model.ProjectMember;
                                notification.Url = $"{baseUrl}/project/{model.ProjectMember.ProjectId}";
                                notification.Users = await FindProjectMembersIds(unit, model.UserId,
                                    model.ProjectMember.ProjectId);
                            }
                            else
                            {
                                activity.RecordId = model.Pending.RecordId;
                                notification.Description =
                                    GetDescription("PUSH_PROJECT_MEMBER_PERMISSION_DESCRIPTION", new string[]
                                    {
                                        await FindUserFullName(unit, model.UserId),
                                        model.Pending.Identifier,
                                        await FindProjectTitle(unit, model.Pending.RecordId),
                                    });
                                notification.Url = $"{baseUrl}/project/{model.Pending.RecordId}";
                                notification.Data = model.Pending;
                                notification.Users =
                                    await FindProjectMembersIds(unit, model.UserId, model.Pending.RecordId);
                            }

                            break;

                        #endregion

                        #region WorkPackage

                        case ActivityType.WorkPackageUserSetting:
                            activity.RecordId = model.PackageUserSetting.PackageId;
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_USER_SETTING_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    await FindPackageTitle(unit, model.PackageUserSetting.PackageId)
                                });
                            notification.Title = "PUSH_WORK_PACKAGE_USER_SETTING_TITLE";
                            notification.Data = model.PackageUserSetting;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageUserSetting.PackageId}";
                            notification.Users = new[] {model.UserId};
                            break;
                        case ActivityType.WorkPackageFavorite:
                            activity.RecordId = model.WorkPackage.Id;
                            notification.Title = "PUSH_WORK_PACKAGE_FAVORITE_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_FAVORITE_DESCRIPTION", new string[]
                                {
                                    model.User.FullName,
                                    model.WorkPackage.Title
                                });
                            notification.Data = model.WorkPackage.Id;
                            notification.Users = new[] {model.UserId};
                            break;
                        case ActivityType.WorkPackageArchive:
                            activity.RecordId = model.WorkPackage.Id;
                            notification.Title = "PUSH_WORK_PACKAGE_ARCHIVE_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_ARCHIVE_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.WorkPackage.Title
                                });
                            notification.Data = model.WorkPackage.Id;
                            notification.Users = await FindPackageMembersIds(unit, model.UserId, model.WorkPackage.Id);
                            break;
                        case ActivityType.WorkPackageRemove:
                            activity.RecordId = model.WorkPackage.Id;
                            notification.Title = "PUSH_WORK_PACKAGE_REMOVE_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_REMOVE_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.WorkPackage.Title
                                });
                            notification.Data = model.WorkPackage.Id;
                            notification.Users = model.UserIds;
                            break;
                        case ActivityType.WorkPackageLabelRemove:
                            activity.RecordId = model.PackageLabel.PackageId;
                            notification.Title = "PUSH_WORK_PACKAGE_LABEL_REMOVE_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_LABEL_REMOVE_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.PackageLabel.Title
                                });
                            notification.Data = model.PackageLabel;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageLabel.PackageId}";
                            notification.Users =
                                await FindPackageMembersIds(unit, model.UserId, model.PackageLabel.PackageId);
                            break;
                        case ActivityType.WorkPackageLabelRename:
                            activity.RecordId = model.PackageLabel.PackageId;
                            notification.Title = "PUSH_WORK_PACKAGE_LABEL_RENAME_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_LABEL_RENAME_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.PackageLabel.Title
                                });
                            notification.Data = model.PackageLabel;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageLabel.PackageId}";
                            notification.Users =
                                await FindPackageMembersIds(unit, model.UserId, model.PackageLabel.PackageId);
                            break;
                        case ActivityType.WorkPackageLabelAdd:
                            activity.RecordId = model.PackageLabel.PackageId;
                            notification.Title = "PUSH_WORK_PACKAGE_LABEL_ADD_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_LABEL_ADD_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.PackageLabel.Title
                                });
                            notification.Data = model.PackageLabel;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageLabel.PackageId}";
                            notification.Users =
                                await FindPackageMembersIds(unit, model.UserId, model.PackageLabel.PackageId);
                            break;
                        case ActivityType.WorkPackageAdd:
                            activity.RecordId = model.WorkPackage.Id;
                            notification.Title = "PUSH_WORK_PACKAGE_ADD_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_ADD_DESCRIPTION", new string[]
                                {
                                    model.User.FullName,
                                    model.WorkPackage.Title
                                });
                            notification.Data = model.WorkPackage;
                            notification.Url = $"{baseUrl}/work-package/{model.WorkPackage.Id}";
                            notification.Users = await FindPackageMembersIds(unit, model.UserId, model.WorkPackage.Id);
                            break;
                        case ActivityType.WorkPackageEdit:
                            activity.RecordId = model.WorkPackage.Id;
                            notification.Title = "PUSH_WORK_PACKAGE_EDIT_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_EDIT_DESCRIPTION", new string[]
                                {
                                    model.User.FullName,
                                    model.WorkPackage.Title
                                });
                            notification.Data = model.WorkPackage;
                            notification.Url = $"{baseUrl}/work-package/{model.WorkPackage.Id}";
                            notification.Users = await FindPackageMembersIds(unit, model.UserId, model.WorkPackage.Id);
                            break;
                        case ActivityType.WorkPackageRestore:
                            activity.RecordId = model.WorkPackage.Id;
                            notification.Title = "PUSH_WORK_PACKAGE_RESTORE_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_RESTORE_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.WorkPackage.Title
                                });
                            notification.Data = model.WorkPackage.Id;
                            notification.Url = $"{baseUrl}/work-package/{model.WorkPackage.Id}";
                            notification.Users = await FindPackageMembersIds(unit, model.UserId, model.WorkPackage.Id);
                            break;
                        case ActivityType.WorkPackageObjectiveAdd:
                            activity.RecordId = model.Objective.PackageId;
                            notification.Title = "PUSH_WORK_PACKAGE_OBJECTIVE_ADD_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_OBJECTIVE_ADD_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.Objective.Title
                                });
                            notification.Data = model.Objective;
                            notification.Url = $"{baseUrl}/work-package/{model.Objective.PackageId}";
                            notification.Users =
                                await FindPackageMembersIds(unit, model.UserId, model.Objective.PackageId);
                            break;
                        case ActivityType.WorkPackageObjectiveEdit:
                            activity.RecordId = model.Objective.PackageId;
                            notification.Title = "PUSH_WORK_PACKAGE_OBJECTIVE_EDIT_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_OBJECTIVE_EDIT_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.Objective.Title
                                });
                            notification.Data = model.Objective;
                            notification.Url = $"{baseUrl}/work-package/{model.Objective.PackageId}";
                            notification.Users =
                                await FindPackageMembersIds(unit, model.UserId, model.Objective.PackageId);
                            break;
                        case ActivityType.WorkPackageObjectiveRemove:
                            activity.RecordId = model.Objective.PackageId;
                            notification.Title = "PUSH_WORK_PACKAGE_OBJECTIVE_REMOVE_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_OBJECTIVE_REMOVE_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.Objective.Title
                                });
                            notification.Data = model.Objective;
                            notification.Url = $"{baseUrl}/work-package/{model.Objective.PackageId}";
                            notification.Users =
                                await FindPackageMembersIds(unit, model.UserId, model.Objective.PackageId);
                            break;
                        case ActivityType.WorkPackageListAdd:
                            activity.RecordId = model.PackageList.PackageId;
                            notification.Title = "PUSH_WORK_PACKAGE_LIST_ADD_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_LIST_ADD_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.PackageList.Title
                                });
                            notification.Data = model.PackageList;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageList.PackageId}";
                            notification.Users =
                                await FindPackageMembersIds(unit, model.UserId, model.PackageList.PackageId);
                            break;
                        case ActivityType.WorkPackageListEdit:
                            activity.RecordId = model.PackageList.PackageId;
                            notification.Title = "PUSH_WORK_PACKAGE_LIST_EDIT_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_LIST_EDIT_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.PackageList.Title
                                });
                            notification.Data = model.PackageList;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageList.PackageId}";
                            notification.Users =
                                await FindPackageMembersIds(unit, model.UserId, model.PackageList.PackageId);
                            break;
                        case ActivityType.WorkPackageListOrder:
                            activity.RecordId = model.PackageList.PackageId;
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_LIST_ORDER_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.PackageList.Title
                                });
                            notification.Title = "PUSH_WORK_PACKAGE_LIST_ORDER_TITLE";
                            notification.Data = model.PackageList;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageList.PackageId}";
                            notification.Users =
                                await FindPackageMembersIds(unit, model.UserId, model.PackageList.PackageId);
                            break;
                        case ActivityType.WorkPackageSetting:
                            activity.RecordId = model.WorkPackage.Id;
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_SETTING_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.WorkPackage.Title
                                });
                            notification.Title = "PUSH_WORK_PACKAGE_SETTING_TITLE";
                            notification.Data = model.WorkPackage;
                            notification.Url = $"{baseUrl}/work-package/{model.WorkPackage.Id}";
                            notification.Users = await FindPackageMembersIds(unit, model.UserId, model.WorkPackage.Id);
                            break;
                        case ActivityType.WorkPackageMemberAdd:
                            activity.RecordId = model.WorkPackage.Id;
                            notification.Title = "PUSH_WORK_PACKAGE_MEMBER_ADD_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_MEMBER_ADD_DESCRIPTION", new string[]
                                {
                                    (model.WorkPackage.Pending.Length + model.WorkPackage.Members.Length).ToString(),
                                    model.WorkPackage.Title,
                                    model.User.FullName
                                });
                            notification.Data = model.WorkPackage;
                            notification.Url = $"{baseUrl}/work-package/{model.WorkPackage.Id}";
                            notification.Users = await FindPackageMembersIds(unit, model.UserId, model.WorkPackage.Id);
                            break;
                        case ActivityType.WorkPackageMemberPermission:
                            notification.Title = "PUSH_WORK_PACKAGE_MEMBER_PERMISSION_TITLE";
                            if (model.WorkPackageMember != null)
                            {
                                activity.RecordId = model.WorkPackageMember.PackageId;
                                notification.Description =
                                    GetDescription("PUSH_WORK_PACKAGE_MEMBER_PERMISSION_DESCRIPTION", new string[]
                                    {
                                        await FindUserFullName(unit, model.UserId),
                                        await FindProjectMemberName(unit, model.WorkPackageMember.RecordId),
                                        await FindPackageTitle(unit, model.WorkPackageMember.PackageId)
                                    });

                                notification.Data = model.WorkPackageMember;
                                notification.Url = $"{baseUrl}/work-package/{model.WorkPackageMember.PackageId}";
                                notification.Users = await FindPackageMembersIds(unit, model.UserId,
                                    model.WorkPackageMember.PackageId);
                            }
                            else
                            {
                                activity.RecordId = model.Pending.RecordId;
                                notification.Description =
                                    GetDescription("PUSH_WORK_PACKAGE_MEMBER_PERMISSION_DESCRIPTION", new string[]
                                    {
                                        await FindUserFullName(unit, model.UserId),
                                        model.Pending.Identifier,
                                        await FindPackageTitle(unit, model.Pending.RecordId)
                                    });
                                notification.Url = $"{baseUrl}/work-package/{model.Pending.RecordId}";
                                notification.Data = model.Pending;
                                notification.Users =
                                    await FindPackageMembersIds(unit, model.UserId, model.Pending.RecordId);
                            }

                            break;
                        case ActivityType.WorkPackageMemberRemove:
                            notification.Title = "PUSH_WORK_PACKAGE_MEMBER_REMOVE_TITLE";
                            if (model.WorkPackageMember != null)
                            {
                                activity.RecordId = model.WorkPackageMember.PackageId;
                                notification.Description =
                                    GetDescription("PUSH_WORK_PACKAGE_MEMBER_REMOVE_DESCRIPTION", new string[]
                                    {
                                        await FindUserFullName(unit, model.UserId),
                                        await FindProjectMemberName(unit, model.WorkPackageMember.RecordId),
                                        await FindPackageTitle(unit, model.WorkPackageMember.PackageId)
                                    });

                                notification.Data = model.WorkPackageMember;
                                notification.Url = $"{baseUrl}/work-package/{model.WorkPackageMember.PackageId}";
                                notification.Users = await FindPackageMembersIds(unit, model.UserId,
                                    model.WorkPackageMember.PackageId);
                            }
                            else
                            {
                                activity.RecordId = model.Pending.RecordId;
                                notification.Description =
                                    GetDescription("PUSH_WORK_PACKAGE_MEMBER_REMOVE_DESCRIPTION", new string[]
                                    {
                                        await FindUserFullName(unit, model.UserId),
                                        model.Pending.Identifier,
                                        await FindPackageTitle(unit, model.Pending.RecordId)
                                    });
                                notification.Url = $"{baseUrl}/work-package/{model.Pending.RecordId}";
                                notification.Data = model.Pending;
                                notification.Users =
                                    await FindPackageMembersIds(unit, model.UserId, model.Pending.RecordId);
                            }

                            break;

                        case ActivityType.WorkPackageUpgrade:
                            activity.RecordId = model.WorkPackage.Id;
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_UPGRADE_DESCRIPTION", new string[]
                                {
                                    model.User.FullName,
                                    model.WorkPackage.Title
                                });
                            notification.Title = "PUSH_WORK_PACKAGE_UPGRADE_TITLE";
                            notification.Data = model.WorkPackage;
                            notification.Url = $"{baseUrl}/project/{model.WorkPackage.ProjectId}";
                            notification.Users = await FindPackageMembersIds(unit, model.UserId, model.WorkPackage.Id);
                            break;
                        case ActivityType.WorkPackageMerge:
                            activity.RecordId = model.WorkPackage.Id;
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_MERGE_DESCRIPTION", new string[]
                                {
                                    model.User.FullName,
                                    model.WorkPackage.Title,
                                    model.WorkPackage2.Title
                                });
                            notification.Title = "PUSH_WORK_PACKAGE_MERGE_TITLE";
                            notification.Data = model.WorkPackage;
                            notification.Url = $"{baseUrl}/work-package/{model.WorkPackage2.Id}";
                            notification.Users = await FindPackageMembersIds(unit, model.UserId, model.WorkPackage.Id);
                            break;
                        case ActivityType.WorkPackageConnect:
                            activity.RecordId = model.WorkPackage.Id;
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_CONNECT_DESCRIPTION", new string[]
                                {
                                    model.User.FullName,
                                    model.WorkPackage.Title,
                                    model.Project.Title
                                });
                            notification.Title = "PUSH_WORK_PACKAGE_CONNECT_TITLE";
                            notification.Data = model.WorkPackage;
                            notification.Url = $"{baseUrl}/project/{model.Project.Id}";
                            notification.Users = await FindPackageMembersIds(unit, model.UserId, model.WorkPackage.Id);
                            break;
                        
                        case ActivityType.WorkPackageListArchive:
                            activity.RecordId = model.PackageList.PackageId;
                            notification.Title = "PUSH_WORK_PACKAGE_LIST_ARCHIVE_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_LIST_ARCHIVE_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.PackageList.Title
                                });
                            notification.Data = model.PackageList;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageList.PackageId}";
                            notification.Users =
                                await FindPackageMembersIds(unit, model.UserId, model.PackageList.PackageId);
                            break;
                        case ActivityType.WorkPackageListTasksArchive:
                            activity.RecordId = model.PackageList.PackageId;
                            notification.Title = "PUSH_WORK_PACKAGE_LIST_ARCHIVE_TASKS_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_LIST_ARCHIVE_TASKS_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.PackageList.Title
                                });
                            notification.Data = model.PackageList;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageList.PackageId}";
                            notification.Users =
                                await FindPackageMembersIds(unit, model.UserId, model.PackageList.PackageId);
                            break;
                        case ActivityType.WorkPackageListTasksDelete:
                            activity.RecordId = model.PackageList.PackageId;
                            notification.Title = "PUSH_WORK_PACKAGE_LIST_CLEAR_TASKS_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_LIST_CLEAR_TASKS_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.PackageList.Title
                                });
                            notification.Data = model.PackageList;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageList.PackageId}";
                            notification.Users =
                                await FindPackageMembersIds(unit, model.UserId, model.PackageList.PackageId);
                            break;
                        case ActivityType.WorkPackageListClone:
                            activity.RecordId = model.PackageList.PackageId;
                            notification.Title = "PUSH_WORK_PACKAGE_LIST_CLONE_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_LIST_CLONE_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.PackageList.Title
                                });
                            notification.Data = model.PackageList;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageList.PackageId}";
                            notification.Users =
                                await FindPackageMembersIds(unit, model.UserId, model.PackageList.PackageId);
                            break;
                        case ActivityType.WorkPackageListPermission:
                            break;
                        case ActivityType.WorkPackageListMove:
                            break;
                        case ActivityType.WorkPackageListCopy:
                            break;
                        case ActivityType.WorkPackageListRemove:
                            break;
                        case ActivityType.WorkPackageListRestore:
                            break;
                        case ActivityType.WorkPackageListSetting:
                            break;
                        case ActivityType.WorkPackageListSort:
                            break;

                        #endregion

                        #region Task

                        case ActivityType.WorkPackageTaskVote:
                            notification.Title = "PUSH_WORK_PACKAGE_TASK_VOTE_TITLE";
                            activity.RecordId = model.PackageTask.Id;
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_TASK_VOTE_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.PackageTask.Title
                                });
                            notification.Data = model.PackageTaskVote;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageTaskVote.PackageId}";
                            notification.Users = await FindTaskMembersIds(
                                unit, model.UserId, model.PackageTask.PackageId, model.PackageTask.Id);
                            break;
                        case ActivityType.WorkPackageTaskVoteReset:
                            activity.RecordId = model.PackageTask.Id;
                            notification.Title = "PUSH_WORK_PACKAGE_TASK_VOTE_RESET_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_TASK_VOTE_RESET_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.PackageTask.Title
                                });
                            notification.Data = model.PackageTask.Id;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageTask.PackageId}";
                            notification.Users = await FindTaskMembersIds(
                                unit, model.UserId, model.PackageTask.PackageId, model.PackageTask.Id);
                            break;
                        case ActivityType.WorkPackageTaskTime:
                            activity.RecordId = model.PackageTask.Id;
                            notification.Title = "PUSH_WORK_PACKAGE_TASK_TIME_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_TASK_TIME_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.PackageTask.Title
                                });
                            notification.Data = model.PackageTaskTime;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageTask.PackageId}";
                            notification.Users = await FindTaskMembersIds(
                                unit, model.UserId, model.PackageTask.PackageId, model.PackageTask.Id);
                            break;
                        case ActivityType.WorkPackageTaskWatch:
                            activity.RecordId = model.PackageTask.Id;
                            notification.Title = "PUSH_WORK_PACKAGE_TASK_WATCH_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_TASK_WATCH_DESCRIPTION", new string[]
                                {
                                    model.User.FullName,
                                    model.PackageTask.Title
                                });
                            notification.Data = model.PackageTaskInteraction;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageTask.PackageId}";
                            notification.Users = await FindTaskMembersIds(
                                unit, model.UserId, model.PackageTask.PackageId, model.PackageTask.Id);
                            break;
                        case ActivityType.WorkPackageTaskAdd:
                            activity.RecordId = model.PackageTask.Id;
                            notification.Title = "PUSH_WORK_PACKAGE_TASK_ADD_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_TASK_ADD_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.PackageTask.Title
                                });
                            notification.Data = model.PackageTask;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageTask.PackageId}";
                            notification.Users = await FindTaskMembersIds(
                                unit, model.UserId, model.PackageTask.PackageId, model.PackageTask.Id);
                            break;
                        case ActivityType.WorkPackageTaskBulkAdd:
                            activity.RecordId = model.WorkPackage.Id;
                            notification.Title = "PUSH_WORK_PACKAGE_TASK_BULK_ADD_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_TASK_BULK_ADD_DESCRIPTION", new string[]
                                {
                                    model.User.FullName,
                                    model.WorkPackage.Title,
                                    model.PackageTasks.Length.ToString()
                                });
                            notification.Data = model.PackageTasks;
                            notification.Url = $"{baseUrl}/work-package/{model.WorkPackage.Id}";
                            notification.Users = new[] {model.UserId};
                            break;
                        case ActivityType.WorkPackageTaskArchive:
                            activity.RecordId = model.PackageTask.Id;
                            notification.Title = "PUSH_WORK_PACKAGE_TASK_ARCHIVE_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_TASK_ARCHIVE_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.PackageTask.Title
                                });
                            notification.Data = model.PackageTask;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageTask.PackageId}";
                            notification.Users = await FindTaskMembersIds(
                                unit, model.UserId, model.PackageTask.PackageId, model.PackageTask.Id);
                            break;

                        case ActivityType.WorkPackageTaskEdit:
                            activity.RecordId = model.PackageTask.Id;
                            notification.Title = "PUSH_WORK_PACKAGE_TASK_EDIT_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_TASK_EDIT_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.PackageTask.Title
                                });
                            notification.Data = model.PackageTask;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageTask.PackageId}";
                            notification.Users = await FindTaskMembersIds(
                                unit, model.UserId, model.PackageTask.PackageId, model.PackageTask.Id);
                            break;

                        case ActivityType.WorkPackageTaskDone:
                            activity.RecordId = model.PackageTask.Id;
                            notification.Title = "PUSH_WORK_PACKAGE_TASK_DONE_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_TASK_DONE_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.PackageTask.Title
                                });
                            notification.Data = model.PackageTask;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageTask.PackageId}";
                            notification.Users = await FindTaskMembersIds(
                                unit, model.UserId, model.PackageTask.PackageId, model.PackageTask.Id);
                            break;

                        case ActivityType.WorkPackageTaskUnBlock:
                            activity.RecordId = model.PackageTask.Id;
                            notification.Title = "PUSH_WORK_PACKAGE_TASK_UNBLOCK_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_TASK_UNBLOCK_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.PackageTask.Title
                                });
                            notification.Data = model.PackageTask;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageTask.PackageId}";
                            notification.Users = await FindTaskMembersIds(
                                unit, model.UserId, model.PackageTask.PackageId, model.PackageTask.Id);
                            break;
                        
                        case ActivityType.WorkPackageTaskBlocked:
                        case ActivityType.WorkPackageTaskBlocker:
                        case ActivityType.WorkPackageTaskPaused:
                            activity.RecordId = model.PackageTask.Id;
                            notification.Title = "PUSH_WORK_PACKAGE_TASK_STOP_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_TASK_STOP_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.PackageTask.Title
                                });
                            notification.Data = model.PackageTask;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageTask.PackageId}";
                            notification.Users = await FindTaskMembersIds(
                                unit, model.UserId, model.PackageTask.PackageId, model.PackageTask.Id);
                            break;
                        
                        case ActivityType.WorkPackageTaskComment:
                            activity.RecordId = model.PackageTaskComment.TaskId;
                            notification.Title = "PUSH_WORK_PACKAGE_TASK_COMMENT_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_TASK_COMMENT_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.PackageTask.Title
                                });
                            notification.Data = model.PackageTaskComment;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageTaskComment.PackageId}";
                            notification.Users = await FindTaskMembersIds(
                                unit, model.UserId, model.PackageTask.PackageId, model.PackageTaskComment.TaskId);
                            break;
                        case ActivityType.WorkPackageTaskReposition:
                            activity.RecordId = model.PackageTask.Id;
                            notification.Title = "PUSH_WORK_PACKAGE_TASK_REPOSITION_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_TASK_REPOSITION_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.PackageTask.Title
                                });
                            notification.Data = model.PackageTask;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageTask.PackageId}";
                            notification.Users = await FindTaskMembersIds(
                                unit, model.UserId, model.PackageTask.PackageId, model.PackageTask.Id);
                            break;
                        case ActivityType.WorkPackageTaskMove:
                            activity.RecordId = model.PackageTask.Id;
                            notification.Title = "PUSH_WORK_PACKAGE_TASK_MOVE_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_TASK_MOVE_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.PackageTask.Title
                                });
                            notification.Data = model.PackageTask;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageTask.PackageId}";
                            notification.Users = await FindTaskMembersIds(
                                unit, model.UserId, model.PackageTask.PackageId, model.PackageTask.Id);
                            break;
                        case ActivityType.WorkPackageTaskLabelAdd:
                            activity.RecordId = model.PackageTaskLabel.TaskId;
                            notification.Title = "PUSH_WORK_PACKAGE_TASK_LABEL_ADD_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_TASK_LABEL_ADD_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.PackageLabel.Title,
                                    model.PackageTask.Title
                                });
                            notification.Data = model.PackageTaskLabel;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageTaskLabel.PackageId}";
                            notification.Users = await FindTaskMembersIds(
                                unit, model.UserId, model.PackageTask.PackageId, model.PackageTaskLabel.TaskId);
                            break;
                        case ActivityType.WorkPackageTaskLabelRemove:
                            activity.RecordId = model.PackageTaskLabel.TaskId;
                            notification.Title = "PUSH_WORK_PACKAGE_TASK_LABEL_REMOVE_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_TASK_LABEL_REMOVE_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.PackageLabel.Title,
                                    model.PackageTask.Title
                                });
                            notification.Data = model.PackageTaskLabel;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageTaskLabel.PackageId}";
                            notification.Users = await FindTaskMembersIds(
                                unit, model.UserId, model.PackageTask.PackageId, model.PackageTaskLabel.TaskId);
                            break;
                        case ActivityType.WorkPackageTaskAttachmentAdd:
                            activity.RecordId = model.PackageTaskAttachment.TaskId;
                            notification.Title = "PUSH_WORK_PACKAGE_TASK_ATTACHMENT_ADD_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_TASK_ATTACHMENT_ADD_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.PackageTaskAttachment.Title,
                                    model.PackageTask.Title
                                });
                            notification.Data = model.PackageTaskAttachment;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageTaskAttachment.PackageId}";
                            notification.Users = await FindTaskMembersIds(
                                unit, model.UserId, model.PackageTask.PackageId, model.PackageTaskAttachment.TaskId);
                            break;
                        case ActivityType.WorkPackageTaskAttachmentBulkAdd:
                            activity.RecordId = model.PackageTask.Id;
                            notification.Title = "PUSH_WORK_PACKAGE_TASK_ATTACHMENT_BULK_ADD_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_TASK_ATTACHMENT_BULK_ADD_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.Affected.ToString("#,##0"),
                                    model.PackageTask.Title
                                });
                            notification.Data = model.PackageTask;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageTask.PackageId}";
                            notification.Users = await FindTaskMembersIds(
                                unit, model.UserId, model.PackageTask.PackageId, model.PackageTask.Id);
                            break;
                        case ActivityType.WorkPackageTaskAttachmentRename:
                            activity.RecordId = model.PackageTaskAttachment.TaskId;
                            notification.Title = "PUSH_WORK_PACKAGE_TASK_ATTACHMENT_RENAME_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_TASK_ATTACHMENT_RENAME_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.PackageTaskAttachment.Title,
                                    model.PackageTask.Title
                                });
                            notification.Data = model.PackageTaskAttachment;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageTaskAttachment.PackageId}";
                            notification.Users = await FindTaskMembersIds(
                                unit, model.UserId, model.PackageTask.PackageId, model.PackageTaskAttachment.TaskId);
                            break;
                        case ActivityType.WorkPackageTaskAttachmentRemove:
                            activity.RecordId = model.PackageTaskAttachment.TaskId;
                            notification.Title = "PUSH_WORK_PACKAGE_TASK_ATTACHMENT_REMOVE_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_TASK_ATTACHMENT_REMOVE_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.PackageTaskAttachment.Title
                                });
                            notification.Data = model.PackageTaskAttachment;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageTaskAttachment.PackageId}";
                            notification.Users = await FindTaskMembersIds(
                                unit, model.UserId, model.PackageTaskAttachment.PackageId,
                                model.PackageTaskAttachment.TaskId);
                            break;
                        case ActivityType.WorkPackageTaskAttachmentCover:
                            activity.RecordId = model.PackageTaskAttachment.TaskId;
                            notification.Title = "PUSH_WORK_PACKAGE_TASK_ATTACHMENT_COVER_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_TASK_ATTACHMENT_COVER_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    model.PackageTaskAttachment.Title
                                });
                            notification.Data = model.PackageTaskAttachment;
                            notification.Url = $"{baseUrl}/work-package/{model.PackageTaskAttachment.PackageId}";
                            notification.Users = await FindTaskMembersIds(
                                unit, model.UserId, model.PackageTaskAttachment.PackageId,
                                model.PackageTaskAttachment.TaskId);
                            break;
                        case ActivityType.WorkPackageTaskMemberAdd:
                            activity.RecordId = model.WorkPackageTaskMember.TaskId;
                            notification.Title = "PUSH_WORK_PACKAGE_TASK_MEMBER_ADD_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_TASK_MEMBER_ADD_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    await FindProjectMemberName(unit, model.WorkPackageTaskMember.RecordId),
                                    model.PackageTask.Title
                                });
                            notification.Data = model.WorkPackageTaskMember;
                            notification.Url = $"{baseUrl}/work-package/{model.WorkPackageTaskMember.PackageId}";
                            notification.Users = await FindTaskMembersIds(
                                unit, model.UserId, model.WorkPackageTaskMember.PackageId,
                                model.WorkPackageTaskMember.TaskId);
                            break;
                        case ActivityType.WorkPackageTaskMemberRemove:
                            activity.RecordId = model.WorkPackageTaskMember.TaskId;
                            notification.Title = "PUSH_WORK_PACKAGE_TASK_MEMBER_REMOVE_TITLE";
                            notification.Description =
                                GetDescription("PUSH_WORK_PACKAGE_TASK_MEMBER_REMOVE_DESCRIPTION", new string[]
                                {
                                    await FindUserFullName(unit, model.UserId),
                                    await FindProjectMemberName(unit, model.WorkPackageTaskMember.RecordId),
                                    model.PackageTask.Title
                                });
                            notification.Data = model.WorkPackageTaskMember;
                            notification.Url = $"{baseUrl}/work-package/{model.WorkPackageTaskMember.PackageId}";
                            notification.Users = await FindTaskMembersIds(
                                unit, model.UserId, model.WorkPackageTaskMember.PackageId,
                                model.WorkPackageTaskMember.TaskId);
                            break;

                        case ActivityType.WorkPackageTaskRemove:
                            break;
                        case ActivityType.WorkPackageTaskRestore:
                            break;
                        case ActivityType.WorkPackageTaskView:
                            break;

                        #endregion
                    }

                    #region Broadcast & Save Changes

                    // If socket not implemented, skip save to database
                    if (string.IsNullOrEmpty(notification.Title)) return;
                    notification.Title = _translateBiz.Get(notification.Title);
                    activity.Description = notification.Description;

                    var allWebPushes = (await unit.WebPushes
                        .Where(u => u.UserId != model.UserId && notification.Users.Contains(u.UserId) && u.Enabled)
                        .AsNoTracking()
                        .ToArrayAsync()).Select(i => i.ToViewModel()).ToArray();

                    switch (model.Type)
                    {
                        case ActivityType.WorkPackageFavorite:
                        case ActivityType.WorkPackageUserSetting:
                        case ActivityType.AccountEdit:
                        case ActivityType.WorkPackageTaskWatch:
                            notification.PushUsers = Array.Empty<PushNotificationViewModel>();
                            break;
                        default:
                            notification.PushUsers = allWebPushes.Where(a =>
                                notification.Users.Contains(a.UserId)).ToArray();
                            break;
                    }

                    var notifications = notification.Users.Select(i => new UserNotification
                    {
                        Description = notification.Description,
                        Title = notification.Title,
                        UserId = i,
                        ActivityUserId = model.UserId
                    });
                    await QueueNotifications(notification);
                    await unit.Activities.AddAsync(activity);
                    await unit.UserNotifications.AddRangeAsync(notifications);
                    await unit.SaveChangesAsync();

                    #endregion
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            }
        }

        #region Private Methods

        private async Task QueueNotifications(NotificationViewModel notification)
        {
            var socketData = new
            {
                notification.Url,
                notification.Title,
                notification.Avatar,
                notification.Description,
                notification.UserId,
                CreatedAt = DateTime.UtcNow,
                Data = new
                {
                    Type = notification.Type,
                    Data = notification.Data,
                    Users = notification.Users
                }
            };
            var pushData = new
            {
                notification.Url,
                notification.Title,
                notification.Avatar,
                notification.Description,
                notification.UserId,
                CreatedAt = DateTime.UtcNow,
                Data = new
                {
                    Type = notification.Type,
                    Data = notification.Data,
                    Users = notification.PushUsers
                }
            };
            var pushSerialized = _jsonService.Serialize(pushData);
            var socketSerialized = _jsonService.Serialize(socketData);
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = QueueServer,
                    UserName = QueueUsername,
                    Password = QueuePassword
                };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    if (notification.Users.Any())
                    {
                        channel.QueueDeclare(
                            queue: SocketQueueName,
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null
                        );
                        channel.BasicPublish(
                            exchange: "",
                            routingKey: SocketQueueName,
                            basicProperties: null,
                            body: Encoding.UTF8.GetBytes(socketSerialized)
                        );
                    }

                    if (notification.PushUsers.Any())
                    {
                        channel.QueueDeclare(
                            queue: PushQueueName,
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null
                        );
                        channel.BasicPublish(
                            exchange: "",
                            routingKey: PushQueueName,
                            basicProperties: null,
                            body: Encoding.UTF8.GetBytes(pushSerialized)
                        );
                    }
                }
            }
            catch (Exception e)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(e);
            }
        }

        private async Task<string> FindUserFullName(ActivityDbContext unit, Guid userId)
        {
            return await unit.Users.Where(i => i.Id == userId)
                .Select(i => i.FullName)
                .SingleOrDefaultAsync();
        }

        private async Task<string> FindGroupTitle(ActivityDbContext unit, Guid groupId)
        {
            return await unit.Groups.Where(i => i.Id == groupId)
                .Select(i => i.Title)
                .SingleOrDefaultAsync();
        }

        private async Task<string> FindChannelTitle(ActivityDbContext unit, Guid channelId)
        {
            return await unit.Channels.Where(i => i.Id == channelId)
                .Select(i => i.Title)
                .SingleOrDefaultAsync();
        }

        private async Task<string> FindPackageTitle(ActivityDbContext unit, Guid packageId)
        {
            return await unit.WorkPackages.Where(i => i.Id == packageId)
                .Select(i => i.Title)
                .SingleOrDefaultAsync();
        }

        private async Task<string> FindProjectMemberName(ActivityDbContext unit, Guid recordId)
        {
            var title = await FindGroupTitle(unit, recordId);
            if (string.IsNullOrEmpty(title)) return await FindUserFullName(unit, recordId);
            return title;
        }

        private async Task<string> FindProjectTitle(ActivityDbContext unit, Guid projectId)
        {
            return await unit.Projects.Where(i => i.Id == projectId)
                .Select(i => i.Title)
                .SingleOrDefaultAsync();
        }

        private async Task<Guid[]> FindGroupMembersIds(ActivityDbContext unit, Guid groupId)
        {
            return await unit.GroupMembers.Where(i => i.GroupId == groupId)
                .Select(i => i.UserId)
                .ToArrayAsync();
        }

        private async Task<Guid[]> FindChannelMembersIds(ActivityDbContext unit, Guid userId, Guid channelId)
        {
            var channel = await unit.Channels.AsNoTracking().SingleOrDefaultAsync(i => i.Id == channelId);
            if (channel == null) return Array.Empty<Guid>();
            return channel.Type switch
            {
                ChannelType.Project => await FindProjectMembersIds(unit, userId, channelId),
                ChannelType.Group => await FindGroupMembersIds(unit, channelId),
                ChannelType.WorkPackage => await FindPackageMembersIds(unit, userId, channelId),
                _ => Array.Empty<Guid>()
            };
        }

        private Task<Guid[]> FindTaskMembersIds(ActivityDbContext unit, Guid userId, Guid packageId, Guid taskId)
        {
            return FindTaskMembersIds(unit, userId, packageId, new[] {taskId});
        }
        
        private async Task<Guid[]> FindTaskMembersIds(ActivityDbContext unit, Guid userId, Guid packageId, Guid[] taskIds)
        {
            var visibility = await unit.WorkPackages
                .Where(i => i.Id == packageId)
                .Select(i => i.TaskVisibility)
                .SingleOrDefaultAsync();

            if (visibility == WorkPackageTaskVisibility.Normal)
                return await FindPackageMembersIds(unit, userId, packageId);

            var packageMembers = await unit.WorkPackageMembers
                .Where(i =>
                    i.PackageId == packageId &&
                    i.Access != AccessType.Editor &&
                    i.Access != AccessType.Visitor
                )
                .AsNoTracking()
                .ToArrayAsync();

            var packageUserIds = packageMembers
                .Where(i => !i.IsGroup)
                .Select(i => i.RecordId)
                .ToArray();

            var packageGroupIds = packageMembers
                .Where(i => i.IsGroup)
                .Select(i => i.RecordId)
                .ToArray();

            var packageGroupMembers = await unit.GroupMembers
                .Where(i => packageGroupIds.Contains(i.GroupId))
                .Select(i => i.UserId)
                .ToArrayAsync();

            var taskMembers = await unit.WorkPackageTaskMember
                .Where(i => taskIds.Contains(i.TaskId))
                .AsNoTracking()
                .ToArrayAsync();

            var taskUserIds = taskMembers
                .Where(i => !i.IsGroup)
                .Select(i => i.RecordId)
                .ToArray();

            var taskGroupIds = taskMembers
                .Where(i => i.IsGroup)
                .Select(i => i.RecordId)
                .ToArray();

            var taskGroupMembers = await unit.GroupMembers
                .Where(i => taskGroupIds.Contains(i.GroupId))
                .Select(i => i.UserId)
                .ToArrayAsync();

            return taskUserIds
                .Concat(taskGroupMembers)
                .Concat(packageUserIds)
                .Concat(packageGroupMembers)
                .Distinct().ToArray();
        }

        private async Task<Guid[]> FindEveryOne(ActivityDbContext unit, Guid userId)
        {
            var allGroups = await unit.GroupMembers.Where(i => i.UserId == userId)
                .Select(i => i.GroupId)
                .ToArrayAsync();

            var allProjects = await unit.ProjectMembers
                .Where(i => i.RecordId == userId || allGroups.Contains(i.RecordId))
                .Select(i => i.ProjectId).ToArrayAsync();

            var allGroupMembers = await unit.GroupMembers
                .Where(i => allGroups.Contains(i.GroupId))
                .Select(i => i.UserId)
                .Distinct().ToArrayAsync();

            var allProjectMembers = await unit.ProjectMembers
                .Where(i => allProjects.Contains(i.ProjectId) && !i.IsGroup)
                .Select(i => i.RecordId)
                .Distinct().ToArrayAsync();
            return allGroupMembers.Concat(allProjectMembers).Distinct().ToArray();
        }

        private async Task<Guid[]> FindProjectMembersIds(ActivityDbContext unit, Guid userId, Guid projectId)
        {
            var allGroups = await unit.GroupMembers.Where(i => i.UserId == userId)
                .Select(i => i.GroupId)
                .ToArrayAsync();

            var projectMembers = await unit.ProjectMembers
                .Where(i => i.ProjectId == projectId && i.RecordId == userId || allGroups.Contains(i.ProjectId))
                .Select(i => new {i.RecordId, i.IsGroup})
                .Distinct().ToArrayAsync();

            var userIds = projectMembers.Where(i => !i.IsGroup)
                .Select(i => i.RecordId).ToArray();

            var groupIds = projectMembers.Where(i => i.IsGroup)
                .Select(i => i.RecordId).ToArray();

            var groupMembers = await unit.GroupMembers
                .Where(i => groupIds.Contains(i.GroupId))
                .Select(i => i.UserId)
                .ToArrayAsync();

            return userIds.Concat(groupMembers).Distinct().ToArray();
        }

        private async Task<Guid[]> FindPackageMembersIds(ActivityDbContext unit, Guid userId, Guid packageId)
        {
            // var allGroups = await unit.GroupMembers.Where(i => i.UserId == userId)
            //     .Select(i => i.GroupId)
            //     .ToArrayAsync();

            var packageMembers = await unit.WorkPackageMembers
                .Where(i => i.PackageId == packageId)
                .Select(i => new {i.RecordId, i.IsGroup})
                .Distinct().ToArrayAsync();

            var userIds = packageMembers.Where(i => !i.IsGroup)
                .Select(i => i.RecordId).ToArray();

            var groupIds = packageMembers.Where(i => i.IsGroup)
                .Select(i => i.RecordId).ToArray();

            var groupMembers = await unit.GroupMembers
                .Where(i => groupIds.Contains(i.GroupId))
                .Select(i => i.UserId)
                .ToArrayAsync();

            return userIds.Concat(groupMembers).Distinct().ToArray();
        }

        private string GetDescription(string key, string[] inputs) => string.Format(_translateBiz.Get(key), inputs);

        #endregion
    }
}