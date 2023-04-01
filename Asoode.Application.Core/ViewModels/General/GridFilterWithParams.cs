namespace Asoode.Application.Core.ViewModels.General;

public class GridFilterWithParams<T> : GridFilter
{
    public T Params { get; set; }
}