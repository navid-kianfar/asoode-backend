using System;
using System.Collections.Generic;

namespace Asoode.Core.ViewModels.ProjectManagement;

public class TreeViewModel
{
    public Dictionary<Guid, TreeReportViewModel> Tree { get; set; }
}

public class ProjectProgressViewModel
{
    public DateTime Date { get; set; }
    public int Created { get; set; }
    public int Done { get; set; }
    public int Blocked { get; set; }
}