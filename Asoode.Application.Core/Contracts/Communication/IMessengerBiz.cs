using Asoode.Application.Core.Primitives;
using Asoode.Application.Core.ViewModels.Communication;
using Asoode.Application.Core.ViewModels.Storage;

namespace Asoode.Application.Core.Contracts.Communication;

public interface IMessengerBiz
{
    Task<OperationResult<ChannelRepositoryViewModel>> Channels(Guid userId);
    Task<OperationResult<ConversationViewModel[]>> ChannelMessages(Guid userId, Guid id);
    Task<OperationResult<bool>> SendMessage(Guid userId, Guid id, ChatViewModel model);
    Task<OperationResult<UploadResultViewModel>> AddAttachment(Guid userId, Guid id, UploadedFileViewModel file);
}