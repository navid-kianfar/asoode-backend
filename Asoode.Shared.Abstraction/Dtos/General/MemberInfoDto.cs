using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.General;

public record MemberInfoDto
{
    public string Avatar { get; set; }
    public string Bio { get; set; }
    public string FullName { get; set; }
    public Guid Id { get; set; }
    public string Initials { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string Phone { get; set; }
    public string TimeZone { get; set; }
    public CalendarType Calendar { get; set; }
    public bool DarkMode { get; set; }
}