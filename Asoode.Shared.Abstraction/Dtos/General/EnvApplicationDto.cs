namespace Asoode.Shared.Abstraction.Dtos.General;

internal record EnvApplicationDto
{
    public string commandName { get; set; }
    public bool dotnetRunMessages { get; set; }
    public bool launchBrowser { get; set; }
    public string applicationUrl { get; set; }
    public Dictionary<string, string> environmentVariables { get; set; }
}