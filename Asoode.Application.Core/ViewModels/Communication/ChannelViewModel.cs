using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Core.ViewModels.Communication;

public class ChannelViewModel : BaseViewModel
{
    public DateTime? ArchivedAt { get; set; }
    public string Title { get; set; }
    public ChannelType Type { get; set; }
    public Guid UserId { get; set; }
    public Guid RootId { get; set; }
}