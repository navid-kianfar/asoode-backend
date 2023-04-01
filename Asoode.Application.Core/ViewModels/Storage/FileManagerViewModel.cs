using System.ComponentModel.DataAnnotations;

namespace Asoode.Application.Core.ViewModels.Storage;

public class FileManagerViewModel
{
    [Required] public string Path { get; set; }
}