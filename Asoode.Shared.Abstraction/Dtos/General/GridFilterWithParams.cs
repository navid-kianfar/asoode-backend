namespace Asoode.Shared.Abstraction.Dtos.General;

public record GridFilterWithParams<T> : GridFilter
{
    public T Params { get; set; }
}