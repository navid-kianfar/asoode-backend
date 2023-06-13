using System;

namespace Asoode.Core.ViewModels.General;

public class BaseViewModel
{
    public DateTime CreatedAt { get; set; }
    public Guid Id { get; set; }
    public DateTime? UpdatedAt { get; set; }
}