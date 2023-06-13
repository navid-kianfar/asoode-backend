using System.ComponentModel.DataAnnotations;

namespace Asoode.Core.ViewModels.Membership.Authentication;

public class ChangePhoneViewModel
{
    [Required] public string Phone { get; set; }
}