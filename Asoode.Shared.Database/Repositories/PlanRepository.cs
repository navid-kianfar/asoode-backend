using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Dtos.Plan;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contexts;
using Asoode.Shared.Database.Contracts;
using Asoode.Shared.Database.Extensions;
using Asoode.Shared.Database.Tables;
using Microsoft.EntityFrameworkCore;

namespace Asoode.Shared.Database.Repositories;

internal class PlanRepository : IPlanRepository
{
    private readonly PremiumDbContext _context;
    private readonly ILoggerService _loggerService;

    public PlanRepository(ILoggerService loggerService, PremiumDbContext context)
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

    public async Task<OperationResult<SelectableItem<Guid>[]>> All(Guid userId)
    {
        try
        {
            var query = await _context.Plans.OrderBy(i => i.Order)
                .AsNoTracking()
                .ToArrayAsync();
            return OperationResult<SelectableItem<Guid>[]>.Success(
                query.Select(i => new SelectableItem<Guid>
                {
                    Text = i.Title,
                    Value = i.Id
                }).ToArray()
            );
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "PlanRepository.All", e);
            return OperationResult<SelectableItem<Guid>[]>.Failed();
        }
    }

    public async Task<OperationResult<bool>> EditUserPlan(Guid userId, Guid id, UserPlanInfoDto model)
    {
        try
        {
            var plan = await _context.UserPlanInfo
                .Where(i => i.UserId == id)
                .OrderByDescending(i => i.CreatedAt)
                .FirstAsync();

            plan.Days = model.Days;
            plan.Description = model.Description;
            plan.Enabled = model.Enabled;
            plan.Picture = model.Picture;
            plan.Project = model.Project;
            plan.Title = model.Title;
            plan.Type = model.Type;
            plan.Unit = model.Unit;
            plan.Users = model.Users;
            plan.AttachmentSize = model.AttachmentSize;
            plan.CanExtend = model.CanExtend;
            plan.ComplexGroup = model.ComplexGroup;
            plan.Space = model.Space;
            plan.FeatureBlocking = model.FeatureBlocking;
            plan.FeatureCalendar = model.FeatureCalendar;
            plan.FeatureChat = model.FeatureChat;
            plan.FeatureFiles = model.FeatureFiles;
            plan.FeatureKartabl = model.FeatureKartabl;
            plan.FeatureObjectives = model.FeatureObjectives;
            plan.FeaturePayments = model.FeaturePayments;
            plan.FeatureRelated = model.FeatureRelated;
            plan.FeatureReports = model.FeatureReports;
            plan.FeatureSeasons = model.FeatureSeasons;
            plan.FeatureShift = model.FeatureShift;
            plan.FeatureTree = model.FeatureTree;
            plan.FeatureVote = model.FeatureVote;
            plan.FeatureWbs = model.FeatureWbs;
            plan.OneTime = model.OneTime;
            plan.PlanCost = model.PlanCost;
            plan.SimpleGroup = model.SimpleGroup;
            plan.WorkPackage = model.WorkPackage;
            plan.AdditionalProjectCost = model.AdditionalProjectCost;
            plan.AdditionalSpaceCost = model.AdditionalSpaceCost;
            plan.AdditionalUserCost = model.AdditionalUserCost;
            plan.FeatureComplexGroup = model.FeatureComplexGroup;
            plan.FeatureCustomField = model.FeatureCustomField;
            plan.FeatureRoadMap = model.FeatureRoadMap;
            plan.FeatureSubTask = model.FeatureSubTask;
            plan.FeatureTimeOff = model.FeatureTimeOff;
            plan.FeatureTimeSpent = model.FeatureTimeSpent;
            plan.FeatureTimeValue = model.FeatureTimeValue;
            plan.AdditionalComplexGroupCost = model.AdditionalComplexGroupCost;
            plan.AdditionalSimpleGroupCost = model.AdditionalSimpleGroupCost;
            plan.AdditionalWorkPackageCost = model.AdditionalWorkPackageCost;
            plan.FeatureGroupTimeSpent = model.FeatureGroupTimeSpent;

            await _context.SaveChangesAsync();
            return OperationResult<bool>.Success(true);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "PlanRepository.EditUserPlan", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> Edit(Guid userId, Guid id, PlanDto model)
    {
        try
        {
            var plan = await _context.Plans.SingleAsync(i => i.Id == id);
            plan.Days = model.Days;
            plan.Description = model.Description;
            plan.Enabled = model.Enabled;
            plan.Order = model.Order;
            plan.Picture = model.Picture;
            plan.Project = model.Project;
            plan.Title = model.Title;
            plan.Type = model.Type;
            plan.Unit = model.Unit;
            plan.Users = model.Users;
            plan.AttachmentSize = model.AttachmentSize;
            plan.CanExtend = model.CanExtend;
            plan.ComplexGroup = model.ComplexGroup;
            plan.DiskSpace = model.DiskSpace;
            plan.FeatureBlocking = model.FeatureBlocking;
            plan.FeatureCalendar = model.FeatureCalendar;
            plan.FeatureChat = model.FeatureChat;
            plan.FeatureFiles = model.FeatureFiles;
            plan.FeatureKartabl = model.FeatureKartabl;
            plan.FeatureObjectives = model.FeatureObjectives;
            plan.FeaturePayments = model.FeaturePayments;
            plan.FeatureRelated = model.FeatureRelated;
            plan.FeatureReports = model.FeatureReports;
            plan.FeatureSeasons = model.FeatureSeasons;
            plan.FeatureShift = model.FeatureShift;
            plan.FeatureTree = model.FeatureTree;
            plan.FeatureVote = model.FeatureVote;
            plan.FeatureWbs = model.FeatureWbs;
            plan.OneTime = model.OneTime;
            plan.PlanCost = model.PlanCost;
            plan.SimpleGroup = model.SimpleGroup;
            plan.WorkPackage = model.WorkPackage;
            plan.AdditionalProjectCost = model.AdditionalProjectCost;
            plan.AdditionalSpaceCost = model.AdditionalSpaceCost;
            plan.AdditionalUserCost = model.AdditionalUserCost;
            plan.FeatureComplexGroup = model.FeatureComplexGroup;
            plan.FeatureCustomField = model.FeatureCustomField;
            plan.FeatureRoadMap = model.FeatureRoadMap;
            plan.FeatureSubTask = model.FeatureSubTask;
            plan.FeatureTimeOff = model.FeatureTimeOff;
            plan.FeatureTimeSpent = model.FeatureTimeSpent;
            plan.FeatureTimeValue = model.FeatureTimeValue;
            plan.AdditionalComplexGroupCost = model.AdditionalComplexGroupCost;
            plan.AdditionalSimpleGroupCost = model.AdditionalSimpleGroupCost;
            plan.AdditionalWorkPackageCost = model.AdditionalWorkPackageCost;
            plan.FeatureGroupTimeSpent = model.FeatureGroupTimeSpent;
            await _context.SaveChangesAsync();
            return OperationResult<bool>.Success(true);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "PlanRepository.Edit", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> Toggle(Guid userId, Guid id)
    {
        try
        {
            var plan = await _context.Plans.SingleAsync(i => i.Id == id);
            plan.Enabled = !plan.Enabled;
            await _context.SaveChangesAsync();
            return OperationResult<bool>.Success(true);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "PlanRepository.Delete", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> Create(Guid userId, PlanDto model)
    {
        try
        {
            var plan = new Plan
            {
                Days = model.Days,
                Description = model.Description,
                Enabled = model.Enabled,
                Order = model.Order,
                Picture = model.Picture,
                Project = model.Project,
                Title = model.Title,
                Type = model.Type,
                Unit = model.Unit,
                Users = model.Users,
                AttachmentSize = model.AttachmentSize,
                CanExtend = model.CanExtend,
                ComplexGroup = model.ComplexGroup,
                DiskSpace = model.DiskSpace,
                FeatureBlocking = model.FeatureBlocking,
                FeatureCalendar = model.FeatureCalendar,
                FeatureChat = model.FeatureChat,
                FeatureFiles = model.FeatureFiles,
                FeatureKartabl = model.FeatureKartabl,
                FeatureObjectives = model.FeatureObjectives,
                FeaturePayments = model.FeaturePayments,
                FeatureRelated = model.FeatureRelated,
                FeatureReports = model.FeatureReports,
                FeatureSeasons = model.FeatureSeasons,
                FeatureShift = model.FeatureShift,
                FeatureTree = model.FeatureTree,
                FeatureVote = model.FeatureVote,
                FeatureWbs = model.FeatureWbs,
                OneTime = model.OneTime,
                PlanCost = model.PlanCost,
                SimpleGroup = model.SimpleGroup,
                WorkPackage = model.WorkPackage,
                AdditionalProjectCost = model.AdditionalProjectCost,
                AdditionalSpaceCost = model.AdditionalSpaceCost,
                AdditionalUserCost = model.AdditionalUserCost,
                FeatureComplexGroup = model.FeatureComplexGroup,
                FeatureCustomField = model.FeatureCustomField,
                FeatureRoadMap = model.FeatureRoadMap,
                FeatureSubTask = model.FeatureSubTask,
                FeatureTimeOff = model.FeatureTimeOff,
                FeatureTimeSpent = model.FeatureTimeSpent,
                FeatureTimeValue = model.FeatureTimeValue,
                AdditionalComplexGroupCost = model.AdditionalComplexGroupCost,
                AdditionalSimpleGroupCost = model.AdditionalSimpleGroupCost,
                AdditionalWorkPackageCost = model.AdditionalWorkPackageCost,
                FeatureGroupTimeSpent = model.FeatureGroupTimeSpent
            };

            await _context.Plans.AddAsync(plan);
            await _context.SaveChangesAsync();
            return OperationResult<bool>.Success(true);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "PlanRepository.Create", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<GridResult<PlanDto>>> List(Guid userId, GridFilter model)
    {
        try
        {
            var query = _context.Plans.OrderBy(i => i.Order);
            return await DbHelper.GetPaginatedData(query, tuple =>
            {
                var (items, startIndex) = tuple;
                return items.Select((i, index) =>
                {
                    var vm = i.ToDto();
                    vm.Index = startIndex + index + 1;
                    return vm;
                }).ToArray();
            }, model.Page, model.PageSize);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "PlanRepository.List", e);
            return OperationResult<GridResult<PlanDto>>.Failed();
        }
    }
}