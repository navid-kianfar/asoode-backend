using Asoode.Shared.Abstraction.Dtos.Collaboration;

namespace Asoode.Shared.Abstraction.Dtos.General;

public record AccessDto
{
    public InviteDto[] Groups { get; set; }
    public InviteDto[] Members { get; set; }
}