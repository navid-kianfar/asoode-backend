namespace Asoode.Application.Data.Models.Base
{
    public static class ContactExtensions
    {
        public static ContactReplyViewModel ToViewModel(this ContactReply reply)
        {
            return new ContactReplyViewModel
            {
                Id = reply.Id,
                Message = reply.Message,
                ContactId = reply.ContactId,
                CreatedAt = reply.CreatedAt,
                UpdatedAt = reply.UpdatedAt,
                UserId = reply.UserId
            };
        }
        
        public static ContactListViewModel ToViewModel(this Contact contact, int index = 0)
        {
            return new ContactListViewModel
            {
                Email = contact.Email,
                Message = contact.Message,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Index = index,
                Seen = contact.Seen,
                CreatedAt = contact.CreatedAt,
                Id = contact.Id
            };
        }
    }
}