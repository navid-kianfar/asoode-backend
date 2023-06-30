namespace Asoode.Shared.Abstraction.Dtos;

internal record EnvRootDto
{
    public Dictionary<string, EnvApplicationDto> profiles { get; set; }
}