using System.ComponentModel.DataAnnotations;

namespace Asoode.Application.Core.ViewModels.Membership.Authentication;

public class ChangePhoneViewModel
{
    [Required] public string Phone { get; set; }
}