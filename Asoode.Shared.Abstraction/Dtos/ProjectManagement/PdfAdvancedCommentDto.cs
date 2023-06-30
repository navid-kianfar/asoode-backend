namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record PdfAdvancedCommentDto
{
    public Stream Stream { get; set; }
    public string FileName { get; set; }
}