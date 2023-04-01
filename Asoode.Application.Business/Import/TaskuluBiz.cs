using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asoode.Business.ProjectManagement;
using Asoode.Core.Contracts.General;
using Asoode.Core.Contracts.Import;
using Asoode.Core.Contracts.Logging;
using Asoode.Core.Contracts.ProjectManagement;
using Asoode.Core.Contracts.Storage;
using Asoode.Core.Helpers;
using Asoode.Core.Primitives;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.Collaboration;
using Asoode.Core.ViewModels.General;
using Asoode.Core.ViewModels.Import.Taskulu;
using Asoode.Core.ViewModels.ProjectManagement;
using Asoode.Core.ViewModels.Storage;
using Asoode.Data.Contexts;
using Asoode.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Business.Import
{
    public class TaskuluBiz : ITaskuluBiz
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IProjectBiz _projectBiz;

        public TaskuluBiz(IServiceProvider serviceProvider, IProjectBiz projectBiz)
        {
            _serviceProvider = serviceProvider;
            _projectBiz = projectBiz;
        }

        public async Task<OperationResult<ProjectPrepareViewModel>> Import(IFormFile file, Guid userId)
        {
            try
            {
                var extension = Path.GetExtension(file.FileName);
                if (extension.ToLower() != ".zip") return OperationResult<ProjectPrepareViewModel>.Rejected();

                var serverInfo = _serviceProvider.GetService<IServerInfo>();
                var zipPath = $"{serverInfo.ContentRootPath}/tmp/{Guid.NewGuid()}/taskulu.zip";
                var extractDirectory = Path.GetDirectoryName(zipPath);
                Directory.CreateDirectory(extractDirectory);

                Taskulu export;

                using (var stream = new FileStream(zipPath, FileMode.Create)) await file.CopyToAsync(stream);
                using (var zip = ZipFile.OpenRead(zipPath))
                {
                    var nameNoEtx = Path.GetFileNameWithoutExtension(file.FileName);
                    var entryKey = $"{nameNoEtx}/data.json";
                    var jsonEntry = zip.GetEntry(entryKey);
                    if (jsonEntry == null)
                    {
                        System.IO.File.Delete(zipPath);
                        System.IO.Directory.Delete(extractDirectory, true);
                        return OperationResult<ProjectPrepareViewModel>.Rejected();
                    }

                    var result = new StringBuilder();
                    using (var reader = new StreamReader(jsonEntry.Open()))
                    {
                        while (reader.Peek() >= 0)
                            result.AppendLine(await reader.ReadLineAsync());
                    }

                    var jsonBiz = _serviceProvider.GetService<IJsonBiz>();
                    var json = result.ToString().Trim();
                    export = jsonBiz.Deserialize<Taskulu>(json);
                    if (export == null)
                    {
                        System.IO.File.Delete(zipPath);
                        return OperationResult<ProjectPrepareViewModel>.Failed();
                    }

                    var rootDirectory = Path.GetDirectoryName(zipPath);
                    extractDirectory = Path.Combine(rootDirectory, nameNoEtx);
                    if (zip.Entries.Count > 1)
                    {
                        if (Directory.Exists(rootDirectory)) Directory.Delete(rootDirectory, true);
                        zip.ExtractToDirectory(rootDirectory);
                        System.IO.File.Delete(Path.Combine(extractDirectory, "data.json"));
                    }

                    System.IO.File.Delete(zipPath);
                }

                var now = DateTime.UtcNow;
                var workPackages = new List<WorkPackageViewModel>();
                var groups = new List<GroupViewModel>();
                var inviteList = new Dictionary<string, string>();
                var listMap = new Dictionary<string, Guid>();
                var usersMap = new Dictionary<string, Guid>();
                var sheetsMap = new Dictionary<string, Guid>();
                double totalAttachmentSize = 0;

                export.Users = export.Users.Where(u => u.Username != "taskulu").ToList();

                // Data.Models.User user;
                Data.Models.User[] users;

                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var uploadService = _serviceProvider.GetService<IUploadProvider>();
                    // user = await unit.FindUser(userId);
                    var allEmails = export.Users.Select(i => i.Email.ToLower()).ToArray();

                    users = await unit.Users
                        .Where(u => allEmails.Contains(u.Email))
                        .AsNoTracking()
                        .ToArrayAsync();

                    var planId = await unit.UserPlanInfo
                        .Where(i => i.UserId == userId)
                        .OrderByDescending(i => i.CreatedAt)
                        .Select(i => i.Id)
                        .FirstAsync();

                    foreach (var usr in export.Users)
                    {
                        var found = users.SingleOrDefault(i =>
                            i.Email == usr.Email);
                        if (found != null) usersMap.Add(usr.Id, found.Id);
                        else inviteList.Add(usr.Id, usr.Email);
                    }

                    var allTeamViewers = export.Teams.Single(t => t.Name == "Viewer").UserIds;
                    var allTeamAdmins = export.Teams.Single(t => t.Name == "Admin").UserIds;
                    var existingViewerMembers = allTeamViewers.Where(i => usersMap.ContainsKey(i)).ToArray();
                    var existingAdminMembers = allTeamAdmins.Where(i => usersMap.ContainsKey(i)).ToArray();
                    var pendingViewerMembers = allTeamViewers.Except(existingViewerMembers).ToArray();
                    var pendingAdminMembers = allTeamAdmins.Except(existingAdminMembers).ToArray();

                    var rootId = Guid.NewGuid();
                    var rootGroup = new GroupViewModel
                    {
                        Id = rootId,
                        RootId = rootId,
                        Title = export.Title,
                        UserId = userId,
                        Description = export.Type,
                        Type = GroupType.Team,
                        CreatedAt = export.CreatedAt,
                        Members = existingViewerMembers.Select(u => new GroupMemberViewModel
                        {
                            Access = AccessType.Visitor,
                            GroupId = rootId,
                            UserId = usersMap[u],
                            Id = Guid.NewGuid()
                        }).Concat(existingAdminMembers.Select(u => new GroupMemberViewModel
                        {
                            Access = AccessType.Admin,
                            GroupId = rootId,
                            UserId = usersMap[u],
                            Id = Guid.NewGuid()
                        })).ToArray(),
                        Pending = pendingViewerMembers.Select(u => new PendingInvitationViewModel
                        {
                            Identifier = inviteList[u],
                            Access = AccessType.Visitor,
                            Id = Guid.NewGuid(),
                            RecordId = rootId
                        }).Concat(pendingAdminMembers.Select(u => new PendingInvitationViewModel
                        {
                            Identifier = inviteList[u],
                            Access = AccessType.Admin,
                            Id = Guid.NewGuid(),
                            RecordId = rootId
                        })).ToArray(),
                    };
                    groups.Add(rootGroup);

                    foreach (var sheet in export.Sheets)
                    {
                        var packageId = Guid.NewGuid();
                        var tasks = new List<WorkPackageTaskViewModel>();
                        var attachmentsMap = new Dictionary<string, Guid>();
                        var lists = sheet.Lists.Select(sheetList =>
                        {
                            var listId = Guid.NewGuid();
                            listMap.Add(sheetList.Id, listId);
                            int counter = 0;
                            var mapped = sheetList.Tasks
                                .OrderByDescending(i => i.CreatedAt)
                                .Select(listTask =>
                                {
                                    var cardId = Guid.NewGuid();
                                    DateTime? archivedAt = null;
                                    if (listTask.Archived) archivedAt = DateTime.Now;

                                    WorkPackageTaskState taskState;
                                    switch (listTask.Section.ToLower())
                                    {
                                        case "doing":
                                            taskState = WorkPackageTaskState.InProgress;
                                            break;
                                        case "todo":
                                            taskState = WorkPackageTaskState.ToDo;
                                            break;
                                        default:
                                            taskState = WorkPackageTaskState.Done;
                                            break;
                                    }

                                    var subTasks = listTask.CheckLists.SelectMany(i => i.Items)
                                        .Select(cl => new WorkPackageTaskViewModel
                                        {
                                            Id = Guid.NewGuid(),
                                            ParentId = cardId,
                                            UserId = userId,
                                            PackageId = packageId,
                                            Title = cl.Title,
                                            Description = cl.Type,
                                            CreatedAt = cl.CreatedAt,
                                            State = cl.CompletedAt.HasValue
                                                ? WorkPackageTaskState.Done
                                                : WorkPackageTaskState.ToDo,
                                            DoneAt = cl.CompletedAt
                                        }).ToArray();

                                    var attachments = listTask.Attachments.Select(att =>
                                    {
                                        var attachmentUser = usersMap.ContainsKey(att.CreatorUserId)
                                            ? usersMap[att.CreatorUserId]
                                            : userId;
                                        var attachmentId = Guid.NewGuid();
                                        attachmentsMap.Add(att.Id, attachmentId);

                                        Guid? uploadId = null;
                                        string path;
                                        WorkPackageTaskAttachmentType type;
                                        if (string.IsNullOrEmpty(att.File.Url))
                                        {
                                            OperationResult<UploadViewModel> uploadOp;
                                            var source = Path.Combine(extractDirectory, "Attachments", att.Id,
                                                att.File.Name);
                                            using (var stream = System.IO.File.OpenRead(source))
                                            {
                                                uploadOp = uploadService.Upload(new StoreViewModel
                                                {
                                                    FormFile = new FormFile(stream, 0, stream.Length, null,
                                                        Path.GetFileName(stream.Name))
                                                    {
                                                        Headers = new HeaderDictionary(),
                                                        ContentType = att.File.MimeType
                                                    },
                                                    Section = UploadSection.WorkPackage,
                                                    PlanId = planId,
                                                    RecordId = cardId,
                                                    UserId = userId
                                                }).GetAwaiter().GetResult();
                                            }

                                            if (uploadOp.Status != OperationResultStatus.Success)
                                            {
                                                // TODO: handle error
                                                throw new Exception(uploadOp.Exception.ToString());
                                            }

                                            uploadId = Guid.NewGuid();
                                            uploadOp.Data.Id = uploadId.Value;
                                            uploadOp.Data.CreatedAt = att.CreatedAt;
                                            totalAttachmentSize += uploadOp.Data.Size;
                                            path = uploadOp.Data.Path;
                                            type = WorkPackageTaskAttachmentType.Upload;
                                            unit.Uploads.Add(new Upload
                                            {
                                                Directory = uploadOp.Data.Directory,
                                                Extension = uploadOp.Data.Extension,
                                                Name = uploadOp.Data.Name,
                                                Path = uploadOp.Data.Path,
                                                Section = uploadOp.Data.Section,
                                                Size = uploadOp.Data.Size,
                                                RecordId = uploadOp.Data.RecordId,
                                                ThumbnailPath = uploadOp.Data.ThumbnailPath,
                                                UserId = uploadOp.Data.UserId,
                                                Type = uploadOp.Data.Type,
                                                Id = uploadOp.Data.Id,
                                                Public = false,
                                            });
                                        }
                                        else
                                        {
                                            type = WorkPackageTaskAttachmentType.Link;
                                            path = att.File.Url;
                                        }

                                        return new WorkPackageTaskAttachmentViewModel
                                        {
                                            UploadId = uploadId,
                                            Path = path,
                                            Type = type,
                                            Id = attachmentId,
                                            CreatedAt = att.CreatedAt,
                                            IsCover = att.Id == listTask.CoverAttachmentId,
                                            TaskId = cardId,
                                            UserId = attachmentUser,
                                            Description = att.Type,
                                            PackageId = packageId,
                                            Title = att.File.Name,
                                        };
                                    }).ToArray();

                                    Guid? attachmentCoverImageId = null;
                                    if (!string.IsNullOrEmpty(listTask.CoverAttachmentId))
                                        attachmentCoverImageId = attachmentsMap[listTask.CoverAttachmentId];

                                    return new WorkPackageTaskViewModel
                                    {
                                        Id = cardId,
                                        UserId = userId,
                                        PackageId = packageId,
                                        Title = listTask.Title,
                                        Description = listTask.Description,
                                        CreatedAt = listTask.CreatedAt,
                                        BeginAt = listTask.StartTime,
                                        ArchivedAt = archivedAt,
                                        EndAt = listTask.Deadline,
                                        State = taskState,
                                        ListId = listId,
                                        Order = ++counter,
                                        SubTasks = subTasks,
                                        CoverId = attachmentCoverImageId,
                                        Attachments = attachments,
                                        Comments = listTask.Comments
                                            .Select(c => new WorkPackageTaskCommentViewModel
                                            {
                                                UserId = usersMap[c.CreatorUserId],
                                                Message = c.Content,
                                                CreatedAt = c.CreatedAt,
                                                TaskId = cardId,
                                                PackageId = packageId,
                                                Id = Guid.NewGuid()
                                            }).ToArray(),
                                        Members = listTask.AssignedUserIds.Where(k => usersMap.ContainsKey(k))
                                            .Select(u => new WorkPackageTaskMemberViewModel
                                            {
                                                Id = Guid.NewGuid(),
                                                CreatedAt = now,
                                                PackageId = packageId,
                                                RecordId = usersMap[u],
                                                TaskId = cardId
                                            }).ToArray(),
                                        // Labels = labelMap[task.ColorLabel]
                                    };
                                }).ToArray();
                            tasks.AddRange(mapped);
                            return new WorkPackageListViewModel
                            {
                                Id = listId,
                                Title = sheetList.Title,
                                CreatedAt = sheetList.CreatedAt,
                                ArchivedAt = sheetList.Archived ? now : (DateTime?) null,
                                PackageId = packageId,
                            };
                        }).ToArray();
                        var package = new WorkPackageViewModel
                        {
                            Description = sheet.Type,
                            Id = packageId,
                            Title = sheet.Title,
                            CreatedAt = sheet.CreatedAt,
                            TaskVisibility = WorkPackageTaskVisibility.Normal,
                            UserId = userId,
                            Labels = new WorkPackageLabelViewModel[0],
                            Members = rootGroup.Members.Select(m => new WorkPackageMemberViewModel
                            {
                                Access = m.Access,
                                Id = Guid.NewGuid(),
                                CreatedAt = sheet.CreatedAt,
                                PackageId = packageId,
                                RecordId = m.UserId
                            }).ToArray(),
                            Pending = rootGroup.Pending.Select(p => new PendingInvitationViewModel
                            {
                                Access = p.Access,
                                Id = Guid.NewGuid(),
                                CreatedAt = sheet.CreatedAt,
                                RecordId = packageId,
                                Identifier = p.Identifier
                            }).ToArray(),
                            Lists = lists,
                            Tasks = tasks.ToArray(),
                        };
                        sheetsMap.Add(sheet.Id, packageId);
                        workPackages.Add(package);
                    }
                }

                return await _projectBiz.Import(userId, new ImportViewModel
                {
                    Description = export.Type,
                    Title = export.Title,
                    Packages = workPackages.ToArray(),
                    Members = workPackages.SelectMany(i => i.Members)
                        .Select(m =>
                            new InviteViewModel {Access = m.Access, Id = m.RecordId.ToString()}).ToArray(),
                    Teams = groups.ToArray(),
                    TotalAttachmentSize = totalAttachmentSize
                });
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<ProjectPrepareViewModel>.Failed();
            }
        }
    }
}