using Asoode.Core.ViewModels.Collaboration;
using Asoode.Core.ViewModels.ProjectManagement;

namespace Asoode.Core.ViewModels.General.Search;

public class SearchResultViewModel
{
    public SearchResultViewModel()
    {
        Members = new MemberInfoViewModel[0];
        Groups = new GroupViewModel[0];
        Projects = new ProjectViewModel[0];
        Tasks = new SearchTaskViewModel[0];
        Storage = new SearchStorageViewModel();
    }

    public MemberInfoViewModel[] Members { get; set; }
    public GroupViewModel[] Groups { get; set; }
    public ProjectViewModel[] Projects { get; set; }
    public SearchStorageViewModel Storage { get; set; }
    public SearchTaskViewModel[] Tasks { get; set; }
}