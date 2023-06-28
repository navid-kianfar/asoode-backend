namespace Asoode.Shared.Abstraction.Dtos;

internal class EnvRootDto
{
    public Dictionary<string, EnvApplicationDto> profiles { get; set; }
}

internal class EnvApplicationDto
{
    public string commandName { get; set; }
    public bool dotnetRunMessages { get; set; }
    public bool launchBrowser { get; set; }
    public string applicationUrl { get; set; }
    public Dictionary<string, string> environmentVariables { get; set; }
}