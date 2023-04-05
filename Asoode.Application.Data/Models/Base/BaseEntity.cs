using System.ComponentModel.DataAnnotations;
using Asoode.Application.Core.Primitives;

namespace Asoode.Application.Data.Models.Base
{
    public abstract class BaseEntity
    {
        protected BaseEntity()
        {
            Id = IncrementalGuid.NewId();
            CreatedAt = DateTime.UtcNow;
        }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        [Key] public Guid Id { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}