using Asoode.Core.Contracts.General;
using Asoode.Core.ViewModels.General.Search;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asoode.Core.Contracts.Logging;
using Asoode.Core.Extensions;
using Asoode.Core.Helpers;
using Asoode.Core.Primitives;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.ProjectManagement;
using Asoode.Core.ViewModels.Storage;
using Asoode.Data.Contexts;
using Asoode.Data.Models;
using Asoode.Data.Models.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Business.General
{
    internal class SearchBiz : ISearchBiz
    {
        private readonly IServiceProvider _serviceProvider;

        public SearchBiz(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<OperationResult<SearchResultViewModel>> Query(string search, Guid userId)
        {
            try
            {
                search = (search ?? "").Trim().ConvertDigitsToLatin();
                
                if (string.IsNullOrEmpty(search) || search.Length < 3) return OperationResult<SearchResultViewModel>
                    .Success(new SearchResultViewModel());
                using (var unit = _serviceProvider.GetService<ProjectManagementDbContext>())
                {
                    var allGroups = await (
                        from grp in unit.Groups
                        join member in unit.GroupMembers on grp.Id equals member.GroupId
                        where member.UserId == userId && !grp.ArchivedAt.HasValue && !member.DeletedAt.HasValue
                        select new { Group = grp, Member = member }
                    ).AsNoTracking().ToArrayAsync();
                    
                    var allGroupIds = allGroups.Select(i => i.Group.Id).Distinct().ToArray();

                    var allProjects = await (
                        from proj in unit.Projects
                        join member in unit.ProjectMembers on proj.Id equals member.ProjectId
                        where 
                            !proj.ArchivedAt.HasValue && 
                            !proj.DeletedAt.HasValue && 
                            (member.RecordId == userId || allGroupIds.Contains(member.RecordId))
                        select new { Project = proj, Member = member }
                    ).AsNoTracking().ToArrayAsync();
                    
                    var allProjectIds = allProjects.Select(i => i.Project.Id).Distinct().ToArray();

                    var allPackages = await (
                        from pkg in unit.WorkPackages
                        join member in unit.WorkPackageMembers on pkg.Id equals member.PackageId
                        where 
                            !pkg.ArchivedAt.HasValue && 
                            !pkg.DeletedAt.HasValue && 
                            (member.RecordId == userId || allGroupIds.Contains(member.RecordId))
                        select new { Package = pkg, Member = member }
                    ).AsNoTracking().ToArrayAsync();
                    
                    var allPackageIds = allPackages.Select(i => i.Package.Id).Distinct().ToArray();

                    var filteredGroups = allGroups.Where(i => 
                        (i.Group.Address ?? string.Empty).Contains(search) ||
                        (i.Group.Description ?? string.Empty).Contains(search) ||
                        (i.Group.Email ?? string.Empty).Contains(search) ||
                        (i.Group.Fax ?? string.Empty).Contains(search) ||
                        (i.Group.Tel ?? string.Empty).Contains(search) ||
                        (i.Group.Title ?? string.Empty).Contains(search) ||
                        (i.Group.Website ?? string.Empty).Contains(search) ||
                        (i.Group.BrandTitle ?? string.Empty).Contains(search) ||
                        (i.Group.PostalCode ?? string.Empty).Contains(search) ||
                        (i.Group.ResponsibleName ?? string.Empty).Contains(search) ||
                        (i.Group.ResponsibleNumber ?? string.Empty).Contains(search) ||
                        (i.Group.SubTitle ?? string.Empty).Contains(search) ||
                        (i.Group.SupervisorName ?? string.Empty).Contains(search) ||
                        (i.Group.SupervisorNumber ?? string.Empty).Contains(search)
                    ).Select(i => i.Group.ToViewModel()).ToArray();

                    var filteredProjects = allProjects.Where(i =>
                        (i.Project.Description ?? string.Empty).Contains(search) ||
                        (i.Project.Title ?? string.Empty).Contains(search)
                    ).Select(i => i.Project.ToViewModel()).ToArray();

                    var everyOne = allGroups.Select(i => i.Member.UserId)
                        .Concat(allProjects.Where(i => !i.Member.IsGroup).Select(i => i.Member.RecordId))
                        .Distinct().ToArray();

                    var filteredMembers = (await unit.Users
                        .Where(u =>
                            everyOne.Contains(u.Id) && (
                                u.FirstName.Contains(search) ||
                                u.Bio.Contains(search) ||
                                u.Email.Contains(search) ||
                                u.Phone.Contains(search) ||
                                u.Username.Contains(search) ||
                                u.LastName.Contains(search)
                            )
                        )
                        .AsNoTracking()
                        .ToArrayAsync()).Select(i => i.ToViewModel()).ToArray();
                    
                    var allTasks = await (
                        from task in unit.WorkPackageTasks
                        join list in unit.WorkPackageLists on task.ListId equals list.Id
                        where allPackageIds.Contains(task.PackageId) && (
                                task.Description.Contains(search) ||
                                task.Title.Contains(search)
                        )
                        select new { Task = task, List = list.Title }
                    ).AsNoTracking().ToArrayAsync();

                    var allTaskIds = allTasks.Select(i => i.Task.Id).ToArray();
                    
                    var allLabels = await (
                        from label in unit.WorkPackageLabels
                        join taskLabel in unit.WorkPackageTaskLabels on label.Id equals taskLabel.LabelId
                        where allTaskIds.Contains(taskLabel.TaskId)
                        select new { Label = label, TaskLabel = taskLabel }
                    ).AsNoTracking().ToArrayAsync();
                    
                    var allMembers = await (
                        from user in unit.Users
                        join member in unit.WorkPackageTaskMember on user.Id equals member.RecordId
                        where !member.IsGroup && allTaskIds.Contains(member.TaskId)
                        select new { User = user, TaskMember = member }
                    ).AsNoTracking().ToArrayAsync();

                    var filteredTasks = allTasks.Select(t =>
                    {
                        return new SearchTaskViewModel
                        {
                            Description = t.Task.Description,
                            Id = t.Task.Id,
                            State = t.Task.State,
                            Title = t.Task.Title,
                            ArchivedAt = t.Task.ArchivedAt,
                            CreatedAt = t.Task.CreatedAt,
                            ProjectId = t.Task.ProjectId,
                            UpdatedAt = t.Task.UpdatedAt,
                            WorkPackageId = t.Task.PackageId,
                            Labels = allLabels.Where(m => m.TaskLabel.TaskId == t.Task.Id)
                                .Select(i => new TaskLabelViewModel
                                {
                                    Color = i.Label.Color,
                                    Dark = i.Label.DarkColor,
                                    Title = i.Label.Title
                                }).ToArray(),
                            Members = allMembers.Where(m => m.TaskMember.TaskId == t.Task.Id)
                                .Select(i => i.User.ToViewModel()).ToArray(),
                            List = t.List,
                            Project = allProjects.FirstOrDefault(p => p.Project.Id == t.Task.ProjectId)?.Project.Title ?? "---",
                            WorkPackage = allPackages.FirstOrDefault(p => p.Package.Id == t.Task.PackageId)?.Package.Title ?? "---",
                        };
                    }).ToArray();

                    var allFiles = await unit.Uploads.Where(i => 
                        i.UserId == userId && i.Section == UploadSection.Storage && (
                            i.Directory.Contains(search) ||
                            i.Name.Contains(search) ||
                            i.Path.Contains(search)
                        )
                    ).AsNoTracking().ToArrayAsync();

                    var filteredFiles = allFiles.Select(f => new ExplorerFileViewModel
                    {
                        Extension = f.Extension,
                        Name = f.Name,
                        Size = f.Size,
                        Url = f.Path,
                        CreatedAt = f.CreatedAt,
                        IsDocument = IOHelper.IsDocument(f.Extension),
                        IsImage = IOHelper.IsImage(f.Extension),
                        IsPdf = IOHelper.IsPdf(f.Extension),
                        IsPresentation = IOHelper.IsPresentation(f.Extension),
                        IsSpreadsheet = IOHelper.IsSpreadsheet(f.Extension),
                        IsArchive = IOHelper.IsArchive(f.Extension),
                        IsExecutable = IOHelper.IsExecutable(f.Extension),
                        IsCode = IOHelper.IsCode(f.Extension),
                        IsOther = IOHelper.IsOther(f.Extension),
                        ExtensionLessName = f.Name
                    }).ToArray();
                    
                    var result = new SearchResultViewModel
                    {
                        Groups = filteredGroups,
                        Projects = filteredProjects.Where(p => 
                            allPackages.Any(w => w.Package.ProjectId == p.Id)
                        ).ToArray(),
                        Members = filteredMembers,
                        Tasks = filteredTasks,
                        Storage = new SearchStorageViewModel
                        {
                            // Folders = new object[0],
                            Files = filteredFiles
                        },
                    };
                    return OperationResult<SearchResultViewModel>.Success(result);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<SearchResultViewModel>.Failed();
            }
        }
    }
}