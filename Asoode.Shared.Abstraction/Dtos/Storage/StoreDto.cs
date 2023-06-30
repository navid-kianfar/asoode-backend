using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.Storage;

public record StoreDto
{
    public string Path { get; set; }
    public Guid UserId { get; set; }
    public UploadSection Section { get; set; }
    public Guid PlanId { get; set; }
    public Guid RecordId { get; set; }
    public StorageItemDto FormFile { get; set; }
    public SelectableItem<Guid>[] Subs { get; set; }
}