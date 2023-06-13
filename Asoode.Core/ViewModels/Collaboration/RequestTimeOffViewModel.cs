using System;

namespace Asoode.Core.ViewModels.Collaboration;

public class RequestTimeOffViewModel
{
    public DateTime BeginAt { get; set; }
    public string Description { get; set; }
    public DateTime EndAt { get; set; }
    public bool IsHourly { get; set; }
}