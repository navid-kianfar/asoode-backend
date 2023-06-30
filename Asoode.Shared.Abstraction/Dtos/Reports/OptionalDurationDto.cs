namespace Asoode.Shared.Abstraction.Dtos.Reports;

public record OptionalDurationDto
{
    public DateTime Begin { get; set; }
    public DateTime? End { get; set; }
}