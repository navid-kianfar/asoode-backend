using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.Discount;
using Asoode.Shared.Abstraction.Dtos.Payment;
using Asoode.Shared.Abstraction.Dtos.Plan;
using Asoode.Shared.Abstraction.Dtos.User;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Contexts;
using Asoode.Shared.Database.Contracts;
using Asoode.Shared.Database.Tables;
using Microsoft.EntityFrameworkCore;

namespace Asoode.Shared.Database.Repositories;

internal class OrderRepository : IOrderRepository
{
    private readonly ILoggerService _loggerService;
    private readonly PremiumDbContext _context;

    public OrderRepository(ILoggerService loggerService, PremiumDbContext context)
    {
        _loggerService = loggerService;
        _context = context;
    }
    
    public async Task<UserDto?> GetUser(Guid userId)
    {
        try
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == userId);
            return user?.ToDto();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "OrderRepository.GetUser", e);
            return null;
        }
    }

    public async Task<DiscountDto?> GetDiscount(string code)
    {
        try
        {
            var discount = await _context.Discounts
                .AsNoTracking()
                .SingleOrDefaultAsync(i => i.Code == code);
            return discount?.ToDto();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "OrderRepository.GetDiscount", e);
            return null;
        }
    }

    public async Task<int> DidUserAlreadyUsedDiscount(Guid userId, Guid discountId)
    {
        try
        {
            return await _context.Orders
                .Where(i => i.DiscountId == discountId && i.UserId == userId)
                .CountAsync();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "OrderRepository.DidUserAlreadyUsedDiscount", e);
            return -1;
        }
    }

    public async Task<PlanDto[]> PlansList(Guid planId)
    {
        try
        {
            var plans = await _context.Plans
                .Where(i => i.Id == planId || i.Type == PlanType.Custom || i.Type == PlanType.Free)
                .AsNoTracking()
                .ToArrayAsync();

            return plans.Select(p => p.ToDto()).ToArray();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "OrderRepository.PlansList", e);
            return Array.Empty<PlanDto>();
        }
    }

    public async Task Store(OrderDto order)
    {
        try
        {
            await _context.Orders.AddAsync(new Order
            {
                FeatureCustomField = order.FeatureCustomField,
                Id = order.Id,
                Type = order.Type,
                AttachmentSize = order.AttachmentSize,
                CanExtend = order.CanExtend,
                ComplexGroup = order.ComplexGroup,
                Users = order.Users,
                Days = order.Days,
                Status = order.Status,
                Project = order.Project,
                FeatureBlocking = order.FeatureBlocking,
                FeatureCalendar = order.FeatureCalendar,
                FeatureChat = order.FeatureChat,
                FeatureFiles = order.FeatureFiles,
                FeatureKartabl = order.FeatureKartabl,
                Unit = order.Unit,
                FeatureObjectives = order.FeatureObjectives,
                FeaturePayments = order.FeaturePayments,
                FeatureRelated = order.FeatureRelated,
                FeatureReports = order.FeatureReports,
                FeatureSeasons = order.FeatureSeasons,
                FeatureShift = order.FeatureShift,
                FeatureTree = order.FeatureTree,
                FeatureVote = order.FeatureVote,
                FeatureWbs = order.FeatureWbs,
                DiscountId = order.DiscountId,
                ExpireAt = order.ExpireAt,
                OneTime = order.OneTime,
                SimpleGroup = order.SimpleGroup,
                CreatedAt = order.CreatedAt,
                PlanCost = order.PlanCost,
                PlanId = order.PlanId,
                UserId = order.UserId,
                WorkPackage = order.WorkPackage,
                UpdatedAt = order.UpdatedAt,
                AdditionalProjectCost = order.AdditionalProjectCost,
                AdditionalSpaceCost = order.AdditionalSpaceCost,
                AdditionalUserCost = order.AdditionalUserCost,
                FeatureComplexGroup = order.FeatureComplexGroup,
                FeatureRoadMap = order.FeatureRoadMap,
                FeatureSubTask = order.FeatureSubTask,
                FeatureTimeOff = order.FeatureTimeOff,
                FeatureTimeSpent = order.FeatureTimeSpent,
                FeatureTimeValue = order.FeatureTimeValue,
                AdditionalComplexGroupCost = order.AdditionalComplexGroupCost,
                AdditionalSimpleGroupCost = order.AdditionalSimpleGroupCost,
                AdditionalWorkPackageCost = order.AdditionalWorkPackageCost,
                FeatureGroupTimeSpent = order.FeatureGroupTimeSpent,
                Automated = order.Automated,
                Duration = order.Duration,
                AppliedDiscount = order.AppliedDiscount,
                DeletedAt = null,
                DiskSpace = order.DiskSpace,
                OrderType = order.OrderType,
                PaymentAmount = order.PaymentAmount,
                TotalAmount = order.TotalAmount,
                UseWallet = order.UseWallet,
                ValueAdded = order.ValueAdded,
            });
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "OrderRepository.Store", e);
        }
    }

    public async Task<UserPlanInfoDto?> GetUserPlan(Guid userId)
    {
        try
        {
            var plan = await _context.UserPlanInfo
                .Where(i => i.UserId == userId)
                .AsNoTracking()
                .OrderByDescending(i => i.CreatedAt)
                .FirstOrDefaultAsync();
            return plan?.ToDto();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "OrderRepository.GetUserPlan", e);
            return null;
        }
    }

    public async Task CancelAnyPendingOrder(Guid userId)
    {
        try
        {
            await _context.Orders
                .Where(i => i.UserId == userId && i.Status == OrderStatus.Pending)
                .ExecuteUpdateAsync(i => 
                    i.SetProperty(p => p.Status, OrderStatus.Canceled)
                    .SetProperty(p => p.UpdatedAt, DateTime.UtcNow)
                );
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "OrderRepository.CancelAnyPendingOrder", e);
        }
    }
}