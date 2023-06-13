using System;
using System.Threading.Tasks;
using Asoode.Core.Primitives;
using Asoode.Core.ViewModels.Communication;
using Asoode.Core.ViewModels.Storage;

namespace Asoode.Core.Contracts.Communication;

public interface IMessengerBiz
{
    Task<OperationResult<ChannelRepositoryViewModel>> Channels(Guid userId);
    Task<OperationResult<ConversationViewModel[]>> ChannelMessages(Guid userId, Guid id);
    Task<OperationResult<bool>> SendMessage(Guid userId, Guid id, ChatViewModel model);
    Task<OperationResult<UploadResultViewModel>> AddAttachment(Guid userId, Guid id, StorageItemDto file);
}