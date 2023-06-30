namespace Asoode.Shared.Abstraction.Dtos.General;

public record AutoCompleteFilterWithParams<T> : AutoCompleteFilter
{
    public T Params { get; set; }
}