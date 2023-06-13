using System.ComponentModel.DataAnnotations;

namespace Asoode.Core.ViewModels.ProjectManagement;

public class VoteViewModel
{
    [Required] public bool Vote { get; set; }
}