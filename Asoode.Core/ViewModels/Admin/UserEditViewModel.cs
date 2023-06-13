using System.ComponentModel.DataAnnotations;
using Asoode.Core.Primitives.Enums;

namespace Asoode.Core.ViewModels.Admin;

public class UserEditViewModel
{
    [Required] public string FirstName { get; set; }
    [Required] public string LastName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public UserType Type { get; set; }
}