using Asoode.Core.ViewModels.General;

namespace Asoode.Core.ViewModels.Admin;

public class ErrorViewModel : BaseViewModel
{
    public string Description { get; set; }
    public string ErrorBody { get; set; }
    public int Index { get; set; }
}