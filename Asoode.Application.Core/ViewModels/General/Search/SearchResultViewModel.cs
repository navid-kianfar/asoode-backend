using Asoode.Application.Core.ViewModels.Collaboration;
using Asoode.Application.Core.ViewModels.ProjectManagement;

namespace Asoode.Application.Core.ViewModels.General.Search;

public class SearchResultViewModel
{
    public SearchResultViewModel()
    {
        Members = Array.Empty<MemberInfoViewModel>();
        Groups = Array.Empty<GroupViewModel>();
        Projects = Array.Empty<ProjectViewModel>();
        Tasks = Array.Empty<SearchTaskViewModel>();
        Storage = new SearchStorageViewModel();
    }

    public MemberInfoViewModel[] Members { get; set; }
    public GroupViewModel[] Groups { get; set; }
    public ProjectViewModel[] Projects { get; set; }
    public SearchStorageViewModel Storage { get; set; }
    public SearchTaskViewModel[] Tasks { get; set; }
}