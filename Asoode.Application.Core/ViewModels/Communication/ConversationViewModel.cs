using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.General;
using Asoode.Application.Core.ViewModels.Storage;

namespace Asoode.Application.Core.ViewModels.Communication;

public class ConversationViewModel : BaseViewModel
{
    public Guid ChannelId { get; set; }
    public string Message { get; set; }
    public string Path { get; set; }
    public Guid? ReplyId { get; set; }
    public Guid? UploadId { get; set; }
    public ConversationType Type { get; set; }
    public Guid? UserId { get; set; }
    public bool FromBot { get; set; }
    public UploadViewModel Upload { get; set; }
}