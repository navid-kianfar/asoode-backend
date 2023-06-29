using Asoode.Shared.Abstraction.Types;
using Asoode.Website.Abstraction.Dtos.General;

namespace Asoode.Website.Abstraction.Contracts;

public interface ITestimonailService
{
    Task<OperationResult<TestimonialDto[]>> Top5(string culture);
}