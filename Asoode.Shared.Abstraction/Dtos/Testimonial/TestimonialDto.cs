using Asoode.Shared.Abstraction.Dtos.General;

namespace Asoode.Shared.Abstraction.Dtos.Testimonial;

public record TestimonialDto : BaseDto
{
    public Guid UserId { get; set; }
    public string Message { get; set; }
    public string Culture { get; set; }
    public bool Approved { get; set; }
    public int Rate { get; set; }

    public string FullName { get; set; }
    public string Bio { get; set; }
    public string Avatar { get; set; }
}