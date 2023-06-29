using Asoode.Shared.Abstraction.Dtos.Testimonial;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Website.Abstraction.Contracts;

public interface ITestimonailService
{
    Task<OperationResult<TestimonialDto[]>> Top5(string culture);
}