using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Asoode.Application.Data.Models.Base;

namespace Asoode.Application.Data.Models
{
    public class BlogPost : BaseEntity
    {
        public int Index { get; set; }
        public Guid BlogId { get; set; }
        public Guid? CategoryId { get; set; }
        [Required][MaxLength(2)]public string Culture { get; set; }
        [Required][MaxLength(1000)]public string Keywords { get; set; }
        [Required][MaxLength(2000)]public string Description { get; set; }
        [Required][MaxLength(1000)]public string Title { get; set; }
        [Required][MaxLength(1000)]public string NormalizedTitle { get; set; }
        [Required][MaxLength(2000)]public string Summary { get; set; }
        [MaxLength(1000)]public string EmbedCode { get; set; }
        [Required][MaxLength(10000)]public string Text { get; set; }
        [MaxLength(500)]public string ThumbImage { get; set; }
        [MaxLength(500)]public string MediumImage { get; set; }
        [MaxLength(500)]public string LargeImage { get; set; }
        [MaxLength(20)]public string Key { get; set; }
        [DefaultValue(0)]public int Priority { get; set; }
    }
}