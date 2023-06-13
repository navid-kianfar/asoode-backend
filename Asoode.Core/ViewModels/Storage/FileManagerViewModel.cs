using System.ComponentModel.DataAnnotations;

namespace Asoode.Core.ViewModels.Storage;

public class FileManagerViewModel
{
    [Required] public string Path { get; set; }
}