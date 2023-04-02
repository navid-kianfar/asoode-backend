using Asoode.Application.Core.Primitives.Enums;
using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models.Junctions
{
    public class UploadAccess : BaseEntity
    {
        public Guid UploadId { get; set; }
        public Guid UserId { get; set; }
        public AccessType Access { get; set; }
    }
}