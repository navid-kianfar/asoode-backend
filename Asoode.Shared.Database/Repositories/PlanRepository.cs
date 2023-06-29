using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.Plan;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contexts;
using Asoode.Shared.Database.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Asoode.Shared.Database.Repositories;

internal class PlanRepository : IPlanRepository
{
    private readonly ILoggerService _loggerService;
    private readonly WebsiteContext _context;

    public PlanRepository(ILoggerService loggerService, WebsiteContext context)
    {
        _loggerService = loggerService;
        _context = context;
    }
    public async Task<OperationResult<PlanDto[]>> List()
    {
        try
        {
            var plans = await _context.Plans
                .Where(i => !i.DeletedAt.HasValue && i.Enabled)
                .OrderBy(i => i.Type)
                .AsNoTracking()
                .ToArrayAsync();
            var result = plans.Select(p => p.ToDto()).ToArray();
            return OperationResult<PlanDto[]>.Success(result);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "PlanRepository.List", e);
            return OperationResult<PlanDto[]>.Failed();
        }
    }
}