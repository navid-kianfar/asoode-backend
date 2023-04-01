namespace Asoode.Application.Core.ViewModels.General;

public class ContactListViewModel : ContactViewModel
{
    public bool Seen { get; set; }
    public int Index { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid Id { get; set; }
}