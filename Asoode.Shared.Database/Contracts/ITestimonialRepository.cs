using Asoode.Shared.Abstraction.Dtos.Testimonial;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Shared.Database.Contracts;

public interface ITestimonialRepository
{
    Task<OperationResult<TestimonialDto[]>> Top5(string culture);
}