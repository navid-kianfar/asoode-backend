namespace Asoode.Core.ViewModels.General;

public class AutoCompleteFilterWithParams<T> : AutoCompleteFilter
{
    public T Params { get; set; }
}