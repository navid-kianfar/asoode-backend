using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos;

public record SortOrderDto
{
    public SortType ListsSort { get; set; }
    public SortType TasksSort { get; set; }
    public SortType SubTasksSort { get; set; }
    public SortType AttachmentsSort { get; set; }
}