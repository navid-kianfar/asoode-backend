using System.ComponentModel.DataAnnotations;
using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models
{
    public class Upload : BaseEntity
    {
        public Upload()
        {
            
        }
        public Upload(UploadViewModel model)
        {
            Directory = model.Directory;
            Extension = model.Extension;
            Id = model.Id;
            Name = model.Name;
            Path = model.Path;
            Public = model.Public;
            Section = model.Section;
            Size = model.Size;
            Type = model.Type;
            RecordId = model.RecordId;
            ThumbnailPath = model.ThumbnailPath;
        }

        [MaxLength(2000)] public string Directory { get; set; }
        [Required] [MaxLength(10)] public string Extension { get; set; }
        [Required] [MaxLength(200)] public string Name { get; set; }
        [Required] [MaxLength(500)] public string Path { get; set; }
        [MaxLength(500)] public string ThumbnailPath { get; set; }
        public bool Public { get; set; }
        public Guid RecordId { get; set; }
        public UploadSection Section { get; set; }
        public long Size { get; set; }
        public FileType Type { get; set; }
        public Guid UserId { get; set; }
    }
}