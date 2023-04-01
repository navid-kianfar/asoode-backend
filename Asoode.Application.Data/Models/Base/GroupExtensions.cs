using Asoode.Application.Data.Models.Junctions;

namespace Asoode.Application.Data.Models.Base
{
    public static class GroupExtensions
    {
        public static EntryLogViewModel ToViewModel(this WorkingTime time, string fullName = "")
        {
            var now = DateTime.UtcNow;
            var gap = ((time.EndAt ?? now) - time.BeginAt);
            return new EntryLogViewModel
            {
                Duration = $"{gap.Hours:00}:{gap.Minutes:00}:{gap.Seconds:00}",
                Id = time.Id,
                BeginAt = time.BeginAt,
                CreatedAt = time.CreatedAt,
                EndAt = time.EndAt,
                UpdatedAt = time.UpdatedAt,
                FullName = fullName,
                UserId = time.UserId,
                GroupId = time.GroupId
            };
        }
        
        public static GroupViewModel ToViewModel(
            this Group grp,
            GroupMemberViewModel[] members = null,
            UserPlanInfo info = null,
            PendingInvitationViewModel[] pending = null,
            int attachmentSize = 0
        )
        {
            return new GroupViewModel
            {
                AttachmentSize = attachmentSize,
                Members = members ?? new GroupMemberViewModel[0],
                Pending = pending ?? new PendingInvitationViewModel[0],
                Avatar = grp.Avatar,
                Address = grp.Address,
                Description = grp.Description,
                Email = grp.Email,
                Employees = grp.Employees,
                Fax = grp.Fax,
                Id = grp.Id,
                Offices = grp.Offices,
                Tel = grp.Tel,
                Title = grp.Title,
                Type = grp.Type,
                Website = grp.Website,
                BrandTitle = grp.BrandTitle,
                CreatedAt = grp.CreatedAt,
                ExpireAt = grp.ExpireAt,
                GeoLocation = grp.GeoLocation,
                NationalId = grp.NationalId,
                PostalCode = grp.PostalCode,
                RegisteredAt = grp.RegisteredAt,
                RegistrationId = grp.RegistrationId,
                ResponsibleName = grp.ResponsibleName,
                ResponsibleNumber = grp.ResponsibleNumber,
                SubTitle = grp.SubTitle,
                SupervisorName = grp.SupervisorNumber,
                SupervisorNumber = grp.SupervisorNumber,
                UpdatedAt = grp.UpdatedAt,
                UserId = grp.UserId,
                Premium = grp.Premium,
                Complex = grp.Complex,
                ParentId = grp.ParentId,
                Level = grp.Level,
                RootId = grp.RootId,
                ArchivedAt = grp.ArchivedAt,
                PlanType = info?.Type ?? PlanType.Free,
            };
        }

        public static TimeOffViewModel ToViewModel(this TimeOff timeOff)
        {
            return new TimeOffViewModel
            {
                Description = timeOff.Description,
                Id = timeOff.Id,
                Status = timeOff.Status,
                BeginAt = timeOff.BeginAt,
                CreatedAt = timeOff.CreatedAt,
                EndAt = timeOff.EndAt,
                GroupId = timeOff.GroupId,
                IsHourly = timeOff.IsHourly,
                ResponderId = timeOff.ResponderId,
                UpdatedAt = timeOff.UpdatedAt,
                UserId = timeOff.UserId
            };
        }
        public static ShiftViewModel ToViewModel(this Shift shift)
        {
            return new ShiftViewModel
            {
                Id = shift.Id,
                CreatedAt = shift.CreatedAt,
                UpdatedAt = shift.UpdatedAt,
                Description = shift.Description,
                End = $"{shift.End?.Hours:00}:{shift.End?.Minutes:00}",
                Start = $"{shift.Start?.Hours:00}:{shift.Start?.Minutes:00}",
                Float = $"{shift.Float?.Hours:00}:{shift.Float?.Minutes:00}",
                Type = shift.Type,
                Title = shift.Title,
                GroupId = shift.GroupId,
                PenaltyRate = shift.PenaltyRate,
                RestHours = shift.RestHours,
                RewardRate = shift.RewardRate,
                UserId = shift.UserId,
                WorkingHours = shift.WorkingHours,
                Saturday = shift.Saturday,
                Sunday = shift.Sunday,
                Monday = shift.Monday,
                Tuesday = shift.Tuesday,
                Wednesday = shift.Wednesday,
                Thursday = shift.Thursday,
                Friday = shift.Friday
            };
        }
        
        public static GroupMemberViewModel ToViewModel(this GroupMember member, User user = null)
        {
            return new GroupMemberViewModel
            {
                Access = member.Access,
                Id = member.Id,
                CreatedAt = member.CreatedAt,
                UpdatedAt = member.UpdatedAt,
                GroupId = member.GroupId,
                UserId = member.UserId,
                Member = user?.ToViewModel()
            };
        }

        public static PendingInvitationViewModel ToViewModel(this PendingInvitation invitation)
        {
            return new PendingInvitationViewModel
            {
                Access = invitation.Access,
                Id = invitation.Id,
                CreatedAt = invitation.CreatedAt,
                UpdatedAt = invitation.UpdatedAt,
                RecordId = invitation.RecordId,
                Identifier = invitation.Identifier
            };
        }
    }
}