using System;
using Asoode.Core.ViewModels.General;

namespace Asoode.Core.ViewModels.Admin;

public class PostViewModel : BaseViewModel
{
    public string Key { get; set; }
    public int Index { get; set; }
    public Guid BlogId { get; set; }
    public Guid? CategoryId { get; set; }
    public string Culture { get; set; }
    public string Keywords { get; set; }
    public string Description { get; set; }
    public string Title { get; set; }
    public string NormalizedTitle { get; set; }
    public string Summary { get; set; }
    public string Text { get; set; }
    public string ThumbImage { get; set; }
    public string MediumImage { get; set; }
    public string LargeImage { get; set; }
    public int Priority { get; set; }
    public string EmbedCode { get; set; }
}