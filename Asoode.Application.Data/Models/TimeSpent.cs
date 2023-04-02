using System.ComponentModel.DataAnnotations;
using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models
{
    public class TimeSpent : BaseEntity
    {
        public DateTime? BeginAt { get; set; }
        public DateTime? EndAt { get; set; }
        public Guid? TaskId { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? TimeOffId { get; set; }
        public TimeSpentType Type { get; set; }
        [Required] public Guid UserId { get; set; }
    }
}