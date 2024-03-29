﻿using System;
using System.ComponentModel.DataAnnotations;
using Asoode.Data.Models.Base;

namespace Asoode.Data.Models.Junctions;

public class WorkPackageRelatedTask : BaseEntity
{
    [Required] public Guid PackageId { get; set; }
    [Required] public Guid ProjectId { get; set; }
    [Required] public Guid TaskId { get; set; }
    public Guid? SubProjectId { get; set; }

    public Guid? RecordPackageId { get; set; }
    public Guid? RecordProjectId { get; set; }
    public Guid? RecordSubProjectId { get; set; }
    public Guid? RecordTaskId { get; set; }

    public bool IsDependency { get; set; }
}