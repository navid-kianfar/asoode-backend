using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.General;
using Asoode.Application.Core.ViewModels.Membership.Profile;

namespace Asoode.Application.Data.Models.Base
{
    public static class UserExtensions
    {
        public static string GenerateTempName(this User user)
        {
            var random = DateTime.UtcNow.ToShortTimeString().Replace(":", "");
            if (!string.IsNullOrEmpty(user.LastName)) return $"{user.LastName}_{random}";
            if (!string.IsNullOrEmpty(user.FirstName)) return $"{user.FirstName}_{random}";
            if (!string.IsNullOrEmpty(user.Email))
            {
                var email = user.Email.Split("@").First();
                return $"{email}_{random}";
            }

            return $"asoode_{random}";
        }

        public static UserProfileViewModel ToProfileViewModel(this User user)
        {
            return new UserProfileViewModel
            {
                Avatar = user.Avatar,
                DarkMode = user.DarkMode,
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                UserType = user.Type,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                Initials = user.Initials,
                Phone = user.Phone,
                TimeZone = user.TimeZone,
                Calendar = user.Calendar,
                Bio = user.Bio,
                EmailConfirmed = user.LastEmailConfirmed.HasValue,
                PhoneConfirmed = user.LastPhoneConfirmed.HasValue
            };
        }

        public static MemberInfoViewModel ToViewModel(this User user, bool full = false)
        {
            return new MemberInfoViewModel
            {
                DarkMode = user.DarkMode,
                Id = user.Id,
                Bio = user.Bio,
                Avatar = user.Avatar,
                Initials = user.Initials,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Calendar = full ? user.Calendar : CalendarType.Default,
                Phone = full ? user.Phone : "",
                TimeZone = full ? user.TimeZone : ""
            };
        }
    }
}