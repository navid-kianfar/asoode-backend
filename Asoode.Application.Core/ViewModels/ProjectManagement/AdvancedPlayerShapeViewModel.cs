using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Core.ViewModels.ProjectManagement;

public class AdvancedPlayerShapeViewModel : BaseViewModel
{
    public Guid AttachmentId { get; set; }
    public int StartFrame { get; set; }
    public int EndFrame { get; set; }
}