using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.Membership.Profile;

public record UserProfileUpdateDto
{
    public string Bio { get; set; }
    public string FirstName { get; set; }
    public string Initials { get; set; }
    public string LastName { get; set; }
    public string TimeZone { get; set; }
    public bool DarkMode { get; set; }
    public CalendarType Calendar { get; set; }
}