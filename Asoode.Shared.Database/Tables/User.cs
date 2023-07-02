using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Dtos.User;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

internal class User : BaseEntity
{
    public User()
    {
        Calendar = CalendarType.Default;
    }

    [MaxLength(500)] public string Avatar { get; set; }
    [MaxLength(100)] public string TimeZone { get; set; }
    public CalendarType Calendar { get; set; }
    [MaxLength(2000)] public string Bio { get; set; }
    [MaxLength(250)] public string Email { get; set; }
    [MaxLength(50)] public string FirstName { get; set; }
    [MaxLength(50)] public string LastName { get; set; }
    [MaxLength(2)] public string PersonalInitials { get; set; }
    [MaxLength(50)] public string Phone { get; set; }
    public bool SkipNotification { get; set; }
    public bool DarkMode { get; set; }
    public double Wallet { get; set; }
    [MaxLength(50)] public string Username { get; set; }

    public ExtendedUserDto ToExtendedDto(int index = 0)
    {
        return new ExtendedUserDto
        {
            Index = index,
            Avatar = Avatar,
            Id = Id,
            Phone = Phone,
            Type = Type,
            Username = Username,
            CreatedAt = CreatedAt,
            Bio = Bio,
            Calendar = Calendar,
            Email = Email,
            FirstName = FirstName,
            LastName = LastName,
            TimeZone = TimeZone,
            UpdatedAt = UpdatedAt,
            EmailConfirmed = LastEmailConfirmed.HasValue && !Email.EndsWith("@asoode.user"),
            PhoneConfirmed = LastPhoneConfirmed.HasValue
        };
    }

    public UserDto ToDto(int index = 0)
    {
        return new UserDto
        {
            Id = Id,
            Type = Type,
            Username = Username,
            CreatedAt = CreatedAt,
            Blocked = Blocked,
            IsLocked = IsLocked
        };
    }

    #region Membership

    public int Attempts { get; set; }
    public bool Blocked { get; set; }
    public DateTime? LastAttempt { get; set; }
    public DateTime? LastEmailConfirmed { get; set; }
    public DateTime? LastPhoneConfirmed { get; set; }
    public DateTime? LockedUntil { get; set; }
    public Guid? MarketerId { get; set; }
    [MaxLength(100)] public string PasswordHash { get; set; }
    public bool ReserveAccount { get; set; }
    [MaxLength(100)] public string Salt { get; set; }
    public UserType Type { get; set; }

    #endregion Membership

    #region NotMappedProperties

    [NotMapped] public string FullName => $"{FirstName} {LastName}";

    [NotMapped]
    public string Initials
    {
        get
        {
            if (!string.IsNullOrEmpty(PersonalInitials)) return PersonalInitials.Substring(0, 2);
            var initials = "";
            if (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
            {
                initials = $"{FirstName[0]}\u200C{LastName[0]}";
            }
            else
            {
                var parts = Username.Split('@')[0].Split('.');
                if (parts.Length > 1)
                {
                    initials = $"{parts[0][0]}\u200C{parts[1][0]}";
                }
                else
                {
                    parts = parts[0].Split('_');
                    initials = parts.Length > 1
                        ? $"{parts[0][0]}\u200C{parts[1][0]}"
                        : $"{parts[0].Substring(0, 1)}\u200C{parts[0].Substring(1, 1)}";
                }
            }

            return initials.ToUpper();
        }
    }

    [NotMapped] public bool IsLocked => LockedUntil.HasValue && LockedUntil.Value > DateTime.UtcNow;

    #endregion NotMappedProperties

    #region 3rd Party Identifications

    [MaxLength(50)] public string AsanaId { get; set; }
    [MaxLength(50)] public string SocialId { get; set; }
    [MaxLength(50)] public string TaskuluId { get; set; }
    [MaxLength(50)] public string TaskWorldId { get; set; }
    [MaxLength(50)] public string TrelloId { get; set; }

    #endregion 3rd Party Identifications

    public MemberInfoDto ToMemberInfoDto()
    {
        return new MemberInfoDto
        {
            Id = Id,
            Avatar = Avatar,
            Bio = Bio,
            Calendar = Calendar,
            Email = Email,
            Initials = Initials,
            Phone = Phone,
            Username = Username,
            FirstName = FirstName,
            DarkMode = DarkMode,
            LastName = LastName,
            TimeZone = TimeZone,
            FullName = FullName,
        };
    }
}