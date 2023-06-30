namespace Asoode.Shared.Abstraction.Dtos.Communication;

public record ChannelRepositoryDto
{
    public ChannelDto[] Directs { get; set; }
}