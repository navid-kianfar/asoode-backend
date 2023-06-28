namespace Asoode.Shared.Abstraction.Dtos;

public record SendSmsDTO
{
    public string To { get; set; } = String.Empty;
    public string Message { get; set; } = String.Empty;
}