namespace Asoode.Shared.Abstraction.Dtos;

public record PaginatedResponse<T>
{
    public T[] Items { get; set; } = Array.Empty<T>();
    public int Page { get; set; }
    public double PageSize { get; set; }
    public double TotalItems { get; set; }

    public double TotalPages => Math.Ceiling(TotalItems / PageSize);
}