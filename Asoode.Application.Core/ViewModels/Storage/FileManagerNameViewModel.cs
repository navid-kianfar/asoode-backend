using System.ComponentModel.DataAnnotations;

namespace Asoode.Application.Core.ViewModels.Storage;

public class FileManagerNameViewModel : FileManagerViewModel
{
    [Required] public string Name { get; set; }
}