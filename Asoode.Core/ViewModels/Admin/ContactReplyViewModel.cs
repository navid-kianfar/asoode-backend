using System;
using Asoode.Core.ViewModels.General;

namespace Asoode.Core.ViewModels.Admin;

public class ContactReplyViewModel : BaseViewModel
{
    public Guid ContactId { get; set; }
    public Guid UserId { get; set; }
    public string Message { get; set; }
}