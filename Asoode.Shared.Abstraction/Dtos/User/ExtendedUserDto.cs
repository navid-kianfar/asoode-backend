using Asoode.Shared.Abstraction.Dtos.Plan;
using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.User;

public record ExtendedUserDto
{
    public bool PhoneConfirmed { get; set; }
    public bool EmailConfirmed { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string TimeZone { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string Email { get; set; }
    public CalendarType Calendar { get; set; }
    public string Bio { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Username { get; set; }
    public UserType Type { get; set; }
    public string Phone { get; set; }
    public Guid Id { get; set; }
    public string Avatar { get; set; }
    public int Index { get; set; }
    public UserPlanInfoDto Plan { get; set; } = new();
}