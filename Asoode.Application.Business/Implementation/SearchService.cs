using Asoode.Application.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.General.Search;
using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Asoode.Shared.Abstraction.Dtos.Storage;
using Asoode.Shared.Abstraction.Extensions;
using Asoode.Shared.Abstraction.Helpers;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contracts;

namespace Asoode.Application.Business.Implementation;

internal class SearchService : ISearchService
{
    private readonly ILoggerService _loggerService;
    private readonly ISearchRepository _repository;

    public SearchService(ILoggerService loggerService, ISearchRepository repository)
    {
        _loggerService = loggerService;
        _repository = repository;
    }

    public async Task<OperationResult<SearchResultDto>> Query(string search, Guid userId)
    {
        try
        {
            search = (search ?? "").Trim().ConvertDigitsToLatin();

            if (string.IsNullOrEmpty(search) || search.Length < 3)
                return OperationResult<SearchResultDto>.Success(new SearchResultDto());

            var allGroups = await _repository.GetAllGroups(userId);
            var allGroupIds = allGroups.Select(i => i.Group.Id).Distinct().ToArray();

            var allProjects = await _repository.GetAllProjects(userId, allGroupIds);
            var allProjectIds = allProjects.Select(i => i.Project.Id).Distinct().ToArray();

            var allPackages = await _repository.GetAllWorkPackages(userId, allGroupIds, allProjectIds);
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
            ).Select(i => i.Group).ToArray();

            var filteredProjects = allProjects.Where(i =>
                (i.Project.Description ?? string.Empty).Contains(search) ||
                (i.Project.Title ?? string.Empty).Contains(search)
            ).Select(i => i.Project).ToArray();

            var everyOne = allGroups.Select(i => i.Member.UserId)
                .Concat(allProjects.Where(i => !i.Member.IsGroup).Select(i => i.Member.RecordId))
                .Distinct().ToArray();

            var filteredMembers = await _repository.GetAllUsers(everyOne, search);

            var allTasks = await _repository.GetAllTasks(allPackageIds, search);
            var allTaskIds = allTasks.Select(i => i.Task.Id).ToArray();

            var allLabels = await _repository.GetAllLabeledTasks(allTaskIds);

            var allMembers = await _repository.GetAllTaskMembers(allTaskIds);
            var filteredTasks = allTasks.Select(t =>
            {
                return new SearchTaskDto
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
                        .Select(i => new TaskLabelDto
                        {
                            Color = i.Label.Color,
                            Dark = i.Label.DarkColor,
                            Title = i.Label.Title
                        }).ToArray(),
                    Members = allMembers.Where(m => m.Member.TaskId == t.Task.Id)
                        .Select(i => i.User).ToArray(),
                    List = t.List,
                    Project = allProjects.FirstOrDefault(p => p.Project.Id == t.Task.ProjectId)?.Project.Title ??
                              "---",
                    WorkPackage =
                        allPackages.FirstOrDefault(p => p.Package.Id == t.Task.PackageId)?.Package.Title ?? "---"
                };
            }).ToArray();

            var allFiles = await _repository.GetAllUploads(userId, search);
            var filteredFiles = allFiles.Select(f => new ExplorerFileDto
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

            var result = new SearchResultDto
            {
                Members = filteredMembers,
                Groups = filteredGroups,
                Tasks = filteredTasks,
                Projects = filteredProjects.Where(p =>
                    allPackages.Any(w => w.Package.ProjectId == p.Id)
                ).ToArray(),
                Storage = new SearchStorageDto
                {
                    Folders = Array.Empty<ExplorerFolderDto>(),
                    Files = filteredFiles
                }
            };
            return OperationResult<SearchResultDto>.Success(result);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "SearchService.Query", e);
            return OperationResult<SearchResultDto>.Failed();
        }
    }
}