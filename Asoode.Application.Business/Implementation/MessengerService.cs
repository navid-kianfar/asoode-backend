using Asoode.Application.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.Communication;
using Asoode.Shared.Abstraction.Dtos.Storage;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Application.Business.Implementation;

internal class MessengerService : IMessengerService
{
    public Task<OperationResult<ChannelRepositoryDto>> Channels(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<ConversationDto[]>> ChannelMessages(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> SendMessage(Guid userId, Guid id, ChatDto model)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<UploadResultDto>> AddAttachment(Guid userId, Guid id, StorageItemDto file)
    {
        throw new NotImplementedException();
    }
}