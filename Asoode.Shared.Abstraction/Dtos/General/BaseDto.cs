namespace Asoode.Shared.Abstraction.Dtos.General;

public record BaseDto
{
    public DateTime CreatedAt { get; set; }
    public Guid Id { get; set; }
    public DateTime? UpdatedAt { get; set; }
}