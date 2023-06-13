using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.General;

namespace Asoode.Core.ViewModels.Admin;

public class BlogViewModel : BaseViewModel
{
    public int Index { get; set; }

    public BlogType Type { get; set; }
    public string Culture { get; set; }
    public string Keywords { get; set; }
    public string Description { get; set; }
    public string Title { get; set; }
}