using Asoode.Shared.Abstraction.Dtos.Communication;
using Asoode.Shared.Abstraction.Dtos.Storage;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Application.Abstraction.Contracts;

public interface IMessengerService
{
    Task<OperationResult<ChannelRepositoryDto>> Channels(Guid userId);
    Task<OperationResult<ConversationDto[]>> ChannelMessages(Guid userId, Guid id);
    Task<OperationResult<bool>> SendMessage(Guid userId, Guid id, ChatDto model);
    Task<OperationResult<UploadResultDto>> AddAttachment(Guid userId, Guid id, StorageItemDto file);
}