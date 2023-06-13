using System.ComponentModel.DataAnnotations;

namespace Asoode.Core.ViewModels.Communication;

public class ChatViewModel
{
    [Required] public string Message { get; set; }
}