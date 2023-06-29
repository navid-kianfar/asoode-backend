using Asoode.Shared.Abstraction.Types;
using Asoode.Website.Abstraction.Contracts;
using Asoode.Website.Abstraction.Dtos.General;

namespace Asoode.Website.Business.Implementation;

internal class TestimonailService : ITestimonailService
{
    public Task<OperationResult<TestimonialDto[]>> Top5(string culture)
    {
        throw new NotImplementedException();
    }
}