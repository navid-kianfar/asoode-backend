namespace Asoode.Shared.Abstraction.Dtos;

public class GridFilterWithParams<T> : GridFilter
{
    public T Params { get; set; }
}