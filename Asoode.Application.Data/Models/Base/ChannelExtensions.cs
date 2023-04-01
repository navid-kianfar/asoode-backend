namespace Asoode.Application.Data.Models.Base
{
    public static class ChannelExtensions
    {
        public static ChannelViewModel ToViewModel(this Channel channel)
        {
            return new ChannelViewModel
            {
                Id = channel.Id,
                Title = channel.Title,
                Type = channel.Type,
                ArchivedAt = channel.ArchivedAt,
                CreatedAt = channel.CreatedAt,
                RootId = channel.RootId,
                UpdatedAt = channel.UpdatedAt,
                UserId = channel.UserId
            };
        }

        public static ConversationViewModel ToViewModel(
            this Conversation conversation, 
            UploadViewModel upload = null)
        {
            return new ConversationViewModel
            {
                Id = conversation.Id,
                FromBot = !conversation.UserId.HasValue,
                Type = conversation.Type,
                CreatedAt = conversation.CreatedAt,
                UpdatedAt = conversation.UpdatedAt,
                UserId = conversation.UserId,
                Message = conversation.Message,
                Path = conversation.Path,
                ChannelId = conversation.ChannelId,
                ReplyId = conversation.ReplyId,
                UploadId = conversation.UploadId,
                Upload = upload
            };
        }
    }
}