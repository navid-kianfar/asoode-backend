namespace Asoode.Admin.Abstraction.Dtos;

public record BaseDto
{
    public DateTime CreatedAt { get; set; }
    public Guid Id { get; set; }
    public DateTime? UpdatedAt { get; set; }
}