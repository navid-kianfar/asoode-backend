using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Core.ViewModels.Storage;

public class StoreViewModel
{
    public Guid UserId { get; set; }
    public UploadSection Section { get; set; }
    public Guid RecordId { get; set; }
    public UploadedFileViewModel File { get; set; }
    public string FilePath { get; set; }
    public SelectableItem<Guid>[] Subs { get; set; }
    public string Path { get; set; }
}