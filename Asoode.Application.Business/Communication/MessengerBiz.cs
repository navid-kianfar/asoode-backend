using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asoode.Core.Contracts.Communication;
using Asoode.Core.Contracts.Logging;
using Asoode.Core.Contracts.Storage;
using Asoode.Core.Primitives;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.Communication;
using Asoode.Core.ViewModels.Logging;
using Asoode.Core.ViewModels.Storage;
using Asoode.Data.Contexts;
using Asoode.Data.Models;
using Asoode.Data.Models.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Z.EntityFramework.Plus;

namespace Asoode.Business.Communication
{
    internal class MessengerBiz : IMessengerBiz
    {
        private readonly IServiceProvider _serviceProvider;

        public MessengerBiz(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<OperationResult<ChannelRepositoryViewModel>> Channels(Guid userId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var user = await unit.Users.AsNoTracking().SingleOrDefaultAsync(i => i.Id == userId);
                    if (user == null || user.Blocked || user.IsLocked || user.DeletedAt.HasValue)
                        return OperationResult<ChannelRepositoryViewModel>.NotFound();
                    var bot = await unit.Channels.SingleAsync(i => i.Id == userId);
                    var result = new ChannelRepositoryViewModel {Directs = new[] {bot.ToViewModel()}};
                    return OperationResult<ChannelRepositoryViewModel>.Success(result);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<ChannelRepositoryViewModel>.Failed();
            }
        }

        public async Task<OperationResult<ConversationViewModel[]>> ChannelMessages(Guid userId, Guid id)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var user = await unit.Users.AsNoTracking().SingleOrDefaultAsync(i => i.Id == userId);
                    if (user == null || user.Blocked || user.IsLocked || user.DeletedAt.HasValue)
                        return OperationResult<ConversationViewModel[]>.NotFound();

                    var channel = await unit.Channels.AsNoTracking().SingleOrDefaultAsync(c => c.Id == id);
                    if (channel == null) return OperationResult<ConversationViewModel[]>.NotFound();

                    if (channel.Type == ChannelType.Group)
                    {
                        var groupAccess = await unit.GroupMembers
                            .Where(g => g.GroupId == id && g.UserId == userId)
                            .AsNoTracking()
                            .SingleOrDefaultAsync();

                        if (groupAccess == null) return OperationResult<ConversationViewModel[]>.Rejected();
                    }
                    else if (channel.Type == ChannelType.Project)
                    {
                        var projectMembers = await unit.ProjectMembers
                            .Where(p => p.ProjectId == id)
                            .AsNoTracking()
                            .ToArrayAsync();
                        var directAccess = projectMembers.SingleOrDefault(m => m.RecordId == userId);
                        if (directAccess == null)
                        {
                            var groupIds = await unit.GroupMembers
                                .Where(g => g.UserId == userId)
                                .Select(i => i.GroupId)
                                .ToArrayAsync();
                            var groupAccess = projectMembers
                                .Any(m => m.IsGroup && groupIds.Contains(m.RecordId));
                            if (!groupAccess) return OperationResult<ConversationViewModel[]>.Rejected();
                        }
                    }
                    else if (channel.Type == ChannelType.WorkPackage)
                    {
                        var projectMembers = await unit.WorkPackageMembers
                            .Where(p => p.PackageId == id)
                            .AsNoTracking()
                            .ToArrayAsync();
                        var directAccess = projectMembers.SingleOrDefault(m => m.RecordId == userId);
                        if (directAccess == null)
                        {
                            var groupIds = await unit.GroupMembers
                                .Where(g => g.UserId == userId)
                                .Select(i => i.GroupId)
                                .ToArrayAsync();
                            var groupAccess = projectMembers
                                .Any(m => m.IsGroup && groupIds.Contains(m.RecordId));
                            if (!groupAccess) return OperationResult<ConversationViewModel[]>.Rejected();
                        }
                    }

                    var messages = await (
                        from conversation in unit.Conversations
                        join upload in unit.Uploads on conversation.UploadId equals upload.Id into tmp
                        from upload in tmp.DefaultIfEmpty()
                        where conversation.ChannelId == id
                        orderby conversation.CreatedAt
                        select new { Conversation = conversation, Upload = upload }
                    ).AsNoTracking().ToArrayAsync();

                    var result = messages.Select(m => 
                        m.Conversation.ToViewModel(m.Upload?.ToViewModel())).ToArray();
                    return OperationResult<ConversationViewModel[]>.Success(result);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<ConversationViewModel[]>.Failed();
            }
        }

        public async Task<OperationResult<bool>> SendMessage(Guid userId, Guid id, ChatViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var user = await unit.Users.AsNoTracking().SingleOrDefaultAsync(i => i.Id == userId);
                    if (user == null || user.IsLocked || user.Blocked)
                        return OperationResult<bool>.Rejected();
                    
                    var channel = await unit.Channels.SingleOrDefaultAsync(i => i.Id == id);
                    if (channel == null) return OperationResult<bool>.Rejected();
                    if (channel.Type == ChannelType.Bot) return OperationResult<bool>.Rejected();

                    var groupIds = await unit.GroupMembers
                        .Where(i => i.UserId == userId && i.Access != AccessType.Visitor)
                        .Select(i => i.GroupId)
                        .ToArrayAsync();

                    if (channel.Type == ChannelType.Group && groupIds.All(i => i != id))
                        return OperationResult<bool>.Rejected();

                    if (channel.Type == ChannelType.Project)
                    {
                        var projectIds = await unit.ProjectMembers
                            .Where(i => (i.RecordId == userId || groupIds.Contains(i.RecordId))
                                        && i.Access != AccessType.Visitor)
                            .Select(i => i.ProjectId)
                            .ToArrayAsync();

                        if (projectIds.All(i => i != id)) return OperationResult<bool>.Rejected();
                    }

                    var conversation = new Conversation
                    {
                        Message = model.Message.Trim(),
                        Type = ConversationType.Text,
                        ChannelId = id,
                        UserId = userId
                    };
                    await unit.Conversations.AddAsync(conversation);
                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.ChannelMessage,
                        UserId = userId,
                        Conversation = conversation.ToViewModel(),
                        User = user.ToViewModel()
                    });
                    return OperationResult<bool>.Success(true);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }

        public async Task<OperationResult<UploadResultViewModel>> AddAttachment(Guid userId, Guid channelId, IFormFile file)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<CollaborationDbContext>())
                {
                    var channel = await unit.Channels.AsNoTracking().SingleOrDefaultAsync(i => i.Id == channelId);
                    if (channel == null) return OperationResult<UploadResultViewModel>.NotFound();

                    var user = await unit.Users.AsNoTracking().SingleOrDefaultAsync(i => i.Id == userId);
                    if (user == null || user.IsLocked || user.Blocked) return OperationResult<UploadResultViewModel>.Rejected();
                    
                    var groupIds = await unit.GroupMembers
                        .Where(i => i.UserId == userId)
                        .Select(i => i.GroupId)
                        .ToArrayAsync();
                    
                    bool hasAccess = true;
                    Guid planId = Guid.Empty;
                    WorkPackage workPackage;
                    Project project;
                    switch (channel.Type)
                    {
                        case ChannelType.WorkPackage:
                            workPackage = await unit.WorkPackages
                                .AsNoTracking()
                                .SingleOrDefaultAsync(i => i.Id == channelId);
                            if (workPackage.ArchivedAt.HasValue) return OperationResult<UploadResultViewModel>.Rejected();
                            project = await unit.Projects
                                .AsNoTracking()
                                .SingleOrDefaultAsync(i => i.Id == workPackage.ProjectId);
                            if (project.ArchivedAt.HasValue) return OperationResult<UploadResultViewModel>.Rejected();
                            planId = project.PlanInfoId;
                            hasAccess = await unit.WorkPackageMembers.AnyAsync(i
                                => i.PackageId == channelId &&
                                   (i.RecordId == userId || groupIds.Contains(i.RecordId)) &&
                                   i.Access != AccessType.Visitor
                            );
                            break;
                        case ChannelType.Project:
                            project = await unit.Projects
                                .AsNoTracking()
                                .SingleOrDefaultAsync(i => i.Id == channelId);
                            if (project.ArchivedAt.HasValue) return OperationResult<UploadResultViewModel>.Rejected();
                            planId = project.PlanInfoId;
                            hasAccess = await unit.ProjectMembers.AnyAsync(i
                                => i.ProjectId == channelId &&
                                   (i.RecordId == userId || groupIds.Contains(i.RecordId)) &&
                                   i.Access != AccessType.Visitor
                            );
                            break;
                        case ChannelType.Group:
                            var group = await unit.Groups
                                .AsNoTracking()
                                .SingleOrDefaultAsync(i => i.Id == channelId);
                            planId = group.PlanInfoId;
                            hasAccess = await unit.GroupMembers.AnyAsync(i
                                => i.GroupId == channelId &&
                                   i.UserId == userId &&
                                   i.Access != AccessType.Visitor
                            );
                            break;
                        case ChannelType.Direct:
                            break;
                        default:
                            break;
                    }
                    
                    if (!hasAccess) return OperationResult<UploadResultViewModel>.Rejected();

                    var plan = await unit.UserPlanInfo.Where(i => i.Id == planId).SingleOrDefaultAsync();
                    if (plan == null) return OperationResult<UploadResultViewModel>.Rejected();

                    if (plan.AttachmentSize < file.Length)
                    {
                        return OperationResult<UploadResultViewModel>.Success(new UploadResultViewModel
                        {
                            AttachmentSize = true
                        });
                    }

                    if ((plan.UsedSpace + file.Length) > plan.Space)
                    {
                        return OperationResult<UploadResultViewModel>.Success(new UploadResultViewModel
                        {
                            StorageSize = true
                        });
                    }

                    plan.UsedSpace += file.Length;
                    var conversationId = Guid.NewGuid();
                    var result = await _serviceProvider.GetService<IUploadProvider>().Upload(new StoreViewModel
                    {
                        FormFile = file,
                        Section = UploadSection.Messenger,
                        PlanId = plan.Id,
                        RecordId = conversationId,
                        UserId = userId
                    });
                    if (result.Status != OperationResultStatus.Success)
                        return OperationResult<UploadResultViewModel>.Rejected();
                    var upload = new Upload
                    {
                        Directory = result.Data.Directory,
                        Extension = result.Data.Extension,
                        Name = result.Data.Name,
                        Path = result.Data.Path,
                        Public = false,
                        Section = UploadSection.Messenger,
                        Size = result.Data.Size,
                        RecordId = result.Data.RecordId,
                        ThumbnailPath = result.Data.ThumbnailPath,
                        UserId = result.Data.UserId,
                        Type = result.Data.Type,
                    };
                    var conversation = new Conversation
                    {
                        Id = conversationId,
                        Message = null,
                        Path = upload.Path,
                        Type = ConversationType.Upload,
                        ChannelId = channelId,
                        UploadId = upload.Id,
                        UserId = userId
                    };
                    await unit.Conversations.AddAsync(conversation);
                    await unit.Uploads.AddAsync(upload);
                    await unit.SaveChangesAsync();
                    await _serviceProvider.GetService<IActivityBiz>().Enqueue(new ActivityLogViewModel
                    {
                        Type = ActivityType.ChannelUpload,
                        UserId = userId,
                        User = user.ToViewModel(),
                        Conversation = conversation.ToViewModel(upload.ToViewModel()),
                    });
                    return OperationResult<UploadResultViewModel>.Success();
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<UploadResultViewModel>.Failed();
            }
        }
    }
}