using Asoode.Shared.Abstraction.Dtos.Testimonial;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contracts;
using Asoode.Website.Abstraction.Contracts;

namespace Asoode.Website.Business.Implementation;

internal class TestimonailService : ITestimonailService
{
    private readonly ITestimonialRepository _repository;

    public TestimonailService(ITestimonialRepository repository)
    {
        _repository = repository;
    }

    public Task<OperationResult<TestimonialDto[]>> Top5(string culture)
        => _repository.Top5(culture);
}