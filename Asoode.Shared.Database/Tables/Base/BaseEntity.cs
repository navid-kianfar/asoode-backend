using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Database.Tables.Base;

public abstract class BaseEntity
{
    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    [Key] public Guid Id { get; set; }
    public DateTime? UpdatedAt { get; set; }
}