﻿using System.ComponentModel.DataAnnotations;
using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models
{
    public class WorkPackageList : BaseEntity
    {
        [Required] public Guid PackageId { get; set; }
        [Required] [MaxLength(250)] public string Title { get; set; }

        [MaxLength(100)] public string Color { get; set; }
        public bool DarkColor { get; set; }
        public WorkPackageTaskState? Kanban { get; set; }
        public bool Restricted { get; set; }
        public int Order { get; set; }
        public DateTime? ArchivedAt { get; set; }
    }
}