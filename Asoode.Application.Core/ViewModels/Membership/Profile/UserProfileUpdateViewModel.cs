using Asoode.Application.Core.Primitives.Enums;

namespace Asoode.Application.Core.ViewModels.Membership.Profile;

public class UserProfileUpdateViewModel
{
    public string Bio { get; set; }
    public string FirstName { get; set; }
    public string Initials { get; set; }
    public string LastName { get; set; }
    public string TimeZone { get; set; }
    public bool DarkMode { get; set; }
    public CalendarType Calendar { get; set; }
}