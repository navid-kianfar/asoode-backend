using Asoode.Core.ViewModels.General;

namespace Asoode.Core.ViewModels.Admin;

public class MarketerViewModel : BaseViewModel
{
    public string Code { get; set; }
    public string Description { get; set; }
    public bool Enabled { get; set; }
    public int? Fixed { get; set; }
    public int? Percent { get; set; }
    public string Title { get; set; }
    public int Index { get; set; }
}