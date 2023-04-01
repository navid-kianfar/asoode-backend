using System.ComponentModel.DataAnnotations;

namespace Asoode.Application.Core.ViewModels.ProjectManagement;

public class VoteViewModel
{
    [Required] public bool Vote { get; set; }
}