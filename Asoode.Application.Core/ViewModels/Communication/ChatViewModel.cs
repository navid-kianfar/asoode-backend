using System.ComponentModel.DataAnnotations;

namespace Asoode.Application.Core.ViewModels.Communication;

public class ChatViewModel
{
    [Required] public string Message { get; set; }
}