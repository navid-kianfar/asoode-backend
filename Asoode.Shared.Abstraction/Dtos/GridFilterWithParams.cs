namespace Asoode.Shared.Abstraction.Dtos;

public record GridFilterWithParams<T> : GridFilter
{
    public T Params { get; set; }
}