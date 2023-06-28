using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos;

public class TitleDto
{
    [Required] public string Title { get; set; }
}