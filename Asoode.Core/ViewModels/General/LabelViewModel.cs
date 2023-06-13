using System.ComponentModel.DataAnnotations;

namespace Asoode.Core.ViewModels.General;

public class LabelViewModel
{
    [MaxLength(100)] public string Title { get; set; }
    [MaxLength(15)] public string Color { get; set; }
}