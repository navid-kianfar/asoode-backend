using System;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.General;

namespace Asoode.Core.ViewModels.Storage;

public class StoreViewModel
{
    public string Path { get; set; }
    public Guid UserId { get; set; }
    public UploadSection Section { get; set; }
    public Guid PlanId { get; set; }
    public Guid RecordId { get; set; }
    public StorageItemDto FormFile { get; set; }
    public SelectableItem<Guid>[] Subs { get; set; }
}