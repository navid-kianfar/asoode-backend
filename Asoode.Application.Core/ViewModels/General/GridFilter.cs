namespace Asoode.Application.Core.ViewModels.General;

public class GridFilter
{
    public GridFilter()
    {
        PageSize = 20;
    }

    public int Page { get; set; }
    public int PageSize { get; set; }
}