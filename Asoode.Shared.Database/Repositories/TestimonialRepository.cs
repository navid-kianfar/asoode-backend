using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.Testimonial;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contexts;
using Asoode.Shared.Database.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Asoode.Shared.Database.Repositories;

internal class TestimonialRepository : ITestimonialRepository
{
    private readonly WebsiteContext _context;
    private readonly ILoggerService _loggerService;

    public TestimonialRepository(ILoggerService loggerService, WebsiteContext context)
    {
        _loggerService = loggerService;
        _context = context;
    }

    public async Task<OperationResult<TestimonialDto[]>> Top5(string culture)
    {
        try
        {
            var testimonials = await (
                    from testimonial in _context.Testimonials
                    join usr in _context.Users on testimonial.UserId equals usr.Id
                    where testimonial.Approved && testimonial.Culture == culture
                    orderby testimonial.CreatedAt descending
                    select new TestimonialDto
                    {
                        Avatar = usr.Avatar,
                        Bio = usr.Bio,
                        FullName = usr.FullName,
                        Approved = testimonial.Approved,
                        Culture = testimonial.Culture,
                        Id = testimonial.Id,
                        Message = testimonial.Message,
                        Rate = testimonial.Rate,
                        CreatedAt = testimonial.CreatedAt,
                        UpdatedAt = testimonial.UpdatedAt,
                        UserId = testimonial.UserId
                    }
                )
                .Skip(0)
                .Take(5)
                .ToArrayAsync();

            return OperationResult<TestimonialDto[]>.Success(testimonials);
        }
        catch (Exception ex)
        {
            await _loggerService.Error(ex.Message, "TestimonialRepository.Top5", ex);
            return OperationResult<TestimonialDto[]>.Failed();
        }
    }
}