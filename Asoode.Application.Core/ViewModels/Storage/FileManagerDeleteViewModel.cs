using System.ComponentModel.DataAnnotations;

namespace Asoode.Application.Core.ViewModels.Storage;

public class FileManagerDeleteViewModel
{
    [Required] public string[] Paths { get; set; }
}