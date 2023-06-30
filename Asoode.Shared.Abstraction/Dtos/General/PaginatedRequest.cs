namespace Asoode.Shared.Abstraction.Dtos.General;

public record PaginatedRequest
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? Query { get; set; }
}