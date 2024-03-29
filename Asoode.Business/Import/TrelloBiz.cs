using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asoode.Business.ProjectManagement;
using Asoode.Core.Contracts.General;
using Asoode.Core.Contracts.Import;
using Asoode.Core.Contracts.Logging;
using Asoode.Core.Contracts.ProjectManagement;
using Asoode.Core.Primitives;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.Collaboration;
using Asoode.Core.ViewModels.Import.Trello;
using Asoode.Core.ViewModels.ProjectManagement;
using Asoode.Core.ViewModels.Storage;
using Asoode.Data.Contexts;
using Asoode.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Business.Import;

public class TrelloBiz : ITrelloBiz
{
    private readonly IProjectBiz _projectBiz;
    private readonly IServiceProvider _serviceProvider;

    public TrelloBiz(IServiceProvider serviceProvider, IProjectBiz projectBiz)
    {
        _serviceProvider = serviceProvider;
        _projectBiz = projectBiz;
    }

    public async Task<OperationResult<ProjectPrepareViewModel>> Import(
        StorageItemDto file, TrelloMapedDataViewModel data, Guid userId)
    {
        try
        {
            #region Read File

            if (file == null) return OperationResult<ProjectPrepareViewModel>.Rejected();
            var result = new StringBuilder();
            using (var reader = new StreamReader(file.Stream))
            {
                while (reader.Peek() >= 0) result.AppendLine(await reader.ReadLineAsync());
            }

            var jsonBiz = _serviceProvider.GetService<IJsonBiz>();
            var json = result.ToString().Trim();
            var export = jsonBiz.Deserialize<TrelloExport>(json);
            if (export == null) return OperationResult<ProjectPrepareViewModel>.Failed();

            #endregion

            var validation = _serviceProvider.GetService<IValidateBiz>();
            foreach (var key in data.MapData.Keys.ToArray())
                if (validation.IsMobile(data.MapData[key]))
                    data.MapData[key] = validation.PrefixMobileNumber(data.MapData[key]);

            User user;
            User[] users;
            var now = DateTime.UtcNow;
            var mappedUserIds = new Dictionary<string, Guid>();
            var mappedColorIds = new Dictionary<string, Guid>();
            var mappedListsIds = new Dictionary<string, Guid>();
            var labels = new List<WorkPackageLabelViewModel>();
            var package = new WorkPackageViewModel
                { Description = export.Desc, Title = export.Name, Id = Guid.NewGuid() };
            var allComments = export.Actions.Where(a => a.Type == "commentCard").ToArray();

            #region Users

            using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
            {
                user = await unit.FindUser(userId);
                var identifier = data.MapData.Values.Distinct().ToArray();
                users = await unit.Users
                    .Where(i => identifier.Contains(i.Email) || identifier.Contains(i.Phone))
                    .AsNoTracking()
                    .ToArrayAsync();
            }

            foreach (var pair in data.MapData)
            {
                var id = (users.SingleOrDefault(u =>
                    u.Email == pair.Value || u.Phone == pair.Value) ?? user).Id;
                mappedUserIds.Add(pair.Key, id);
            }

            package.Members = mappedUserIds.Values.Distinct().Select(i => new WorkPackageMemberViewModel
            {
                Id = Guid.NewGuid(),
                Access = i == userId ? AccessType.Admin : AccessType.Editor,
                RecordId = i
            }).ToArray();

            #endregion

            #region Labels

            foreach (var label in export.Labels)
            {
                var lbl = new WorkPackageLabelViewModel
                {
                    Id = Guid.NewGuid(),
                    Color = label.Color,
                    Title = label.Name
                };
                labels.Add(lbl);
                mappedColorIds.Add(label.Id, lbl.Id);
            }

            package.Labels = labels.ToArray();

            #endregion

            #region List

            package.Lists = export.Lists?.Select((l, i) =>
            {
                var listId = Guid.NewGuid();
                mappedListsIds.Add(l.Id, listId);
                return new WorkPackageListViewModel
                {
                    Id = listId,
                    Order = i + 1,
                    Title = l.Name,
                    ArchivedAt = l.Closed ? now : null,
                    CreatedAt = now
                };
            }).ToArray();

            #endregion

            #region Task

            package.Tasks = export.Cards?.OrderByDescending(c => c.DateLastActivity).Select(c =>
            {
                var taskId = Guid.NewGuid();
                var createdAt = c.Due ?? DateTime.UtcNow; // CHECK LATER;
                DateTime? archivedAt = null;
                if (c.Closed) archivedAt = c.Due;

                return new WorkPackageTaskViewModel
                {
                    Id = taskId,
                    ListId = mappedListsIds[c.IdList],
                    PackageId = package.Id,
                    ProjectId = package.ProjectId,
                    Title = c.Name,
                    Description = c.Desc,
                    DueAt = c.Due,
                    State = c.DueComplete ? WorkPackageTaskState.Done : WorkPackageTaskState.ToDo,
                    DoneAt = c.Due, // CHECK LATER
                    CreatedAt = createdAt,
                    ArchivedAt = archivedAt, // CHECK LATER
                    GeoLocation = c.Coordinates?.ToString(),
                    Labels = c.IdLabels.Select(l => new WorkPackageTaskLabelViewModel
                    {
                        Id = Guid.NewGuid(),
                        LabelId = mappedColorIds[l],
                        PackageId = package.Id,
                        TaskId = taskId
                    }).ToArray(),
                    Members = c.IdMembers.Select(i => new WorkPackageTaskMemberViewModel
                    {
                        Id = Guid.NewGuid(),
                        PackageId = package.Id,
                        RecordId = mappedUserIds[i],
                        TaskId = taskId
                    }).ToArray(),
                    Attachments = c.Attachments?.Select(a => new WorkPackageTaskAttachmentViewModel
                    {
                        Id = Guid.NewGuid(),
                        TaskId = taskId,
                        UserId = mappedUserIds.ContainsKey(a.IdMember) ? mappedUserIds[a.IdMember] : userId,
                        Path = a.Url,
                        CreatedAt = a.Date,
                        Description = null,
                        Title = a.Name,
                        Type = WorkPackageTaskAttachmentType.Link,
                        PackageId = package.Id,
                        ProjectId = package.ProjectId
                    }).ToArray(),
                    SubTasks = export.Checklists?
                        .Where(cl => cl.IdCard == c.Id)
                        .SelectMany(i => i.CheckItems)
                        .Select(ci =>
                        {
                            var checkListItemId = Guid.NewGuid();
                            var isDone = ci.State != "incomplete";
                            DateTime? doneAt = null;
                            if (isDone) doneAt = DateTime.UtcNow;
                            return new WorkPackageTaskViewModel
                            {
                                ParentId = taskId,
                                Id = checkListItemId,
                                Title = ci.Name,
                                State = isDone ? WorkPackageTaskState.ToDo : WorkPackageTaskState.Done,
                                DoneAt = doneAt,
                                UserId = userId,
                                CreatedAt = DateTime.UtcNow,
                                ListId = mappedListsIds[c.IdList],
                                PackageId = package.Id,
                                ProjectId = package.ProjectId,
                                ArchivedAt = archivedAt
                            };
                        }).ToArray(),
                    Comments = allComments.Where(k => k.Data.Card.Id == c.Id)
                        .Select(k => new WorkPackageTaskCommentViewModel
                        {
                            Id = Guid.NewGuid(),
                            Message = k.Data.Text,
                            UserId = mappedUserIds[k.IdMemberCreator],
                            CreatedAt = k.Date,
                            PackageId = package.Id,
                            TaskId = taskId
                        }).ToArray()
                };
            }).ToArray();

            #endregion

            return await _projectBiz.Import(userId, new ImportViewModel
            {
                Description = export.Desc,
                Title = export.Name,
                Packages = new[] { package },
                Members = package.Members.Select(m => new InviteViewModel
                {
                    Access = m.Access,
                    Id = m.RecordId.ToString()
                }).ToArray()
            });
        }
        catch (Exception ex)
        {
            await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
            return OperationResult<ProjectPrepareViewModel>.Failed();
        }
    }
}