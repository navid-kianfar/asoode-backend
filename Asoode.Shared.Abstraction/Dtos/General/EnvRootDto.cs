namespace Asoode.Shared.Abstraction.Dtos.General;

internal record EnvRootDto
{
    public Dictionary<string, EnvApplicationDto> profiles { get; set; }
}