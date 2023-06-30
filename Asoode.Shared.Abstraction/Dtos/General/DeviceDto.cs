namespace Asoode.Shared.Abstraction.Dtos.General;

public record DeviceDto : BaseDto
{
    public string Title { get; set; }
    public string Os { get; set; }
    public bool Enabled { get; set; }
}