using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Plan;
using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Admin.Abstraction.Dtos;

public record UserDto : BaseDto
{
    public int Index { get; set; }
    public string Avatar { get; set; }
    public string TimeZone { get; set; }
    public CalendarType Calendar { get; set; }
    public string Bio { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Phone { get; set; }
    public string Username { get; set; }
    public UserType Type { get; set; }

    public string FullName => $"{FirstName} {LastName}";
    public UserPlanInfoDto Plan { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool PhoneConfirmed { get; set; }
}