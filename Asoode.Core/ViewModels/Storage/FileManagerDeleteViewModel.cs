using System.ComponentModel.DataAnnotations;

namespace Asoode.Core.ViewModels.Storage;

public class FileManagerDeleteViewModel
{
    [Required] public string[] Paths { get; set; }
}