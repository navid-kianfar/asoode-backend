namespace Asoode.Shared.Abstraction.Dtos.Postman;

public record SendSmsDTO
{
    public string To { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}