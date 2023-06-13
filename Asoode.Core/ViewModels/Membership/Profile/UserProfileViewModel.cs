using System;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.Membership.Plan;

namespace Asoode.Core.ViewModels.Membership.Profile;

public class UserProfileViewModel
{
    public Guid? WorkingGroupId { get; set; }
    public Guid? WorkingProjectId { get; set; }
    public Guid? WorkingTaskId { get; set; }
    public DateTime? WorkingGroupFrom { get; set; }
    public DateTime? WorkingTaskFrom { get; set; }
    public Guid? WorkingPackageId { get; set; }

    public string Email { get; set; }
    public string FirstName { get; set; }
    public string FullName { get; set; }
    public Guid Id { get; set; }
    public string Initials { get; set; }
    public string LastName { get; set; }
    public string Phone { get; set; }
    public string Username { get; set; }
    public UserType UserType { get; set; }
    public string TimeZone { get; set; }
    public CalendarType Calendar { get; set; }
    public string Bio { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool PhoneConfirmed { get; set; }
    public UserPlanInfoViewModel Plan { get; set; }
    public bool DarkMode { get; set; }
    public string Avatar { get; set; }
}