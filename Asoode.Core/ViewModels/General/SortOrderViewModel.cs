using Asoode.Core.Primitives.Enums;

namespace Asoode.Core.ViewModels.General;

public class SortOrderViewModel
{
    public SortType ListsSort { get; set; }
    public SortType TasksSort { get; set; }
    public SortType SubTasksSort { get; set; }
    public SortType AttachmentsSort { get; set; }
}