using System;
using System.Threading.Tasks;
using Asoode.Core.Primitives;
using Asoode.Core.ViewModels.Admin;
using Asoode.Core.ViewModels.General;

namespace Asoode.Core.Contracts.Admin;

public interface IContactBiz
{
    Task<OperationResult<bool>> Contact(ContactViewModel model);
    Task<OperationResult<GridResult<ContactListViewModel>>> List(Guid userId, GridFilterWithParams<GridQuery> model);
    Task<OperationResult<bool>> Delete(Guid userId, Guid id);
    Task<OperationResult<bool>> Reply(Guid userId, Guid id, ContactReplyViewModel model);
    Task<OperationResult<ContactReplyViewModel[]>> Replies(Guid userId, Guid id);
}