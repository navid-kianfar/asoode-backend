using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asoode.Core.Contracts.General;
using Asoode.Core.Contracts.Logging;
using Asoode.Core.Contracts.Membership;
using Asoode.Core.Primitives;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.Membership.Order;
using Asoode.Data.Contexts;
using Asoode.Data.Models;
using Asoode.Data.Models.Base;
using Asoode.Data.Models.Junctions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Z.EntityFramework.Plus;

namespace Asoode.Business.Membership
{
    internal class OrderBiz : IOrderBiz
    {
        private const int VALUE_ADDED_RATE = 9;
        private readonly string Domain;
        private readonly IServiceProvider _serviceProvider;

        public OrderBiz(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            Domain = configuration["Setting:Domain"];
        }

        public async Task<OperationResult<OrderDiscountResultViewModel>> CheckDiscount(Guid userId,
            CheckDiscountViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var now = DateTime.UtcNow;

                    var user = await unit.Users.SingleOrDefaultAsync(i => i.Id == userId);
                    if (user == null || user.IsLocked || user.Blocked)
                        return OperationResult<OrderDiscountResultViewModel>.Rejected();

                    var discount = await unit.Discounts.SingleOrDefaultAsync(i => i.Code == model.Code);

                    if (discount == null)
                        return OperationResult<OrderDiscountResultViewModel>
                            .Success(new OrderDiscountResultViewModel {Invalid = true});

                    if (discount.PlanId.HasValue && discount.PlanId != model.PlanId)
                        return OperationResult<OrderDiscountResultViewModel>
                            .Success(new OrderDiscountResultViewModel {InvalidPlan = true});

                    if (discount.ForUser.HasValue && discount.ForUser.Value != userId)
                        return OperationResult<OrderDiscountResultViewModel>
                            .Success(new OrderDiscountResultViewModel {Invalid = true});

                    if (discount.EndAt.HasValue && discount.EndAt.Value > now)
                    {
                        return OperationResult<OrderDiscountResultViewModel>.Success(new OrderDiscountResultViewModel
                        {
                            Expired = true
                        });
                    }

                    if (discount.StartAt.HasValue && discount.StartAt.Value < now)
                    {
                        return OperationResult<OrderDiscountResultViewModel>.Success(new OrderDiscountResultViewModel
                        {
                            Expired = true
                        });
                    }

                    if (discount.MaxUsage != -1)
                    {
                        var alreadyUsed = await unit.Orders
                            .Where(i => i.DiscountId == discount.Id)
                            .Select(i => i.UserId)
                            .ToArrayAsync();
                        var numOfUsage = alreadyUsed.Count(i => i == userId);
                        if (discount.MaxUsage == alreadyUsed.Length)
                            return OperationResult<OrderDiscountResultViewModel>
                                .Success(new OrderDiscountResultViewModel {Invalid = true});
                        if (discount.MaxUsage == numOfUsage)
                            return OperationResult<OrderDiscountResultViewModel>
                                .Success(new OrderDiscountResultViewModel {AlreadyUsed = true});
                    }

                    var result = new OrderDiscountResultViewModel {Success = true, Id = discount.Id};
                    if (discount.MaxUnit == -1 || model.Amount < discount.MaxUnit)
                    {
                        result.Amount = model.Amount;
                        return OperationResult<OrderDiscountResultViewModel>.Success(result);
                    }

                    result.Amount = discount.MaxUnit;
                    return OperationResult<OrderDiscountResultViewModel>.Success(result);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<OrderDiscountResultViewModel>.Failed();
            }
        }

        public async Task<OperationResult<Guid>> Order(Guid userId, RequestOrderViewModel model)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var now = DateTime.UtcNow;

                    var user = await unit.Users.SingleOrDefaultAsync(i => i.Id == userId);
                    if (user == null || user.IsLocked || user.Blocked)
                        return OperationResult<Guid>.Rejected();

                    var existingOrder = await unit.Orders
                        .SingleOrDefaultAsync(i => i.UserId == userId && i.Status == OrderStatus.Pending);

                    if (existingOrder != null)
                    {
                        existingOrder.Status = OrderStatus.Canceled;
                        existingOrder.UpdatedAt = now;
                    }

                    var userPlan = await unit.UserPlanInfo
                        .Where(i => i.UserId == userId)
                        .OrderByDescending(i => i.CreatedAt)
                        .FirstAsync();

                    var order = new Order
                    {
                        Status = OrderStatus.Pending,
                        Duration = model.Duration,
                        OrderType = model.Type
                    };
                    double totalAmount = 0;
                    var plans = await unit.Plans
                        .Where(i => i.Id == model.PlanId || i.Type == PlanType.Custom || i.Type == PlanType.Free)
                        .AsNoTracking()
                        .ToArrayAsync();

                    var freePlan = plans.First(i => i.Type == PlanType.Free);
                    var customPlan = plans.First(i => i.Type == PlanType.Custom);
                    var basedOn = plans.FirstOrDefault(i => i.Id == model.PlanId);
                    if (basedOn == null || !basedOn.Enabled) return OperationResult<Guid>.Rejected();

                    int usersCost, simpleGroupsCost, complexGroupCost, projectCost, packagesCost;
                    double spaceCost;

                    switch (model.Type)
                    {
                        case OrderType.Renew:
                            usersCost = (userPlan.Users - freePlan.Users) * basedOn.AdditionalUserCost;
                            spaceCost = ((userPlan.Space - freePlan.DiskSpace) / 1024 / 1024 / 1024) *
                                        basedOn.AdditionalSpaceCost;
                            simpleGroupsCost = (userPlan.SimpleGroup - freePlan.SimpleGroup) *
                                               basedOn.AdditionalSimpleGroupCost;
                            complexGroupCost = (userPlan.ComplexGroup - freePlan.ComplexGroup) *
                                               basedOn.AdditionalComplexGroupCost;
                            projectCost = (userPlan.Project - freePlan.Project) * basedOn.AdditionalProjectCost;
                            packagesCost = (userPlan.WorkPackage - freePlan.WorkPackage) *
                                           basedOn.AdditionalWorkPackageCost;

                            totalAmount = usersCost +
                                          spaceCost +
                                          simpleGroupsCost +
                                          complexGroupCost +
                                          projectCost +
                                          packagesCost;

                            switch (model.Duration)
                            {
                                case OrderDuration.Monthly:
                                    order.Days = 30;
                                    break;
                                case OrderDuration.Season:
                                    order.Days = 90;
                                    totalAmount = totalAmount * 3;
                                    break;
                                case OrderDuration.HalfYear:
                                    order.Days = 180;
                                    totalAmount = totalAmount * 6;
                                    break;
                                case OrderDuration.Yearly:
                                    order.Days = 365;
                                    totalAmount = totalAmount * 12;
                                    totalAmount = totalAmount - (totalAmount / 10);
                                    break;
                            }

                            order.ExpireAt = DateTime.UtcNow.AddHours(72);
                            order.Project = userPlan.Project;
                            order.Type = userPlan.Type;
                            order.Unit = userPlan.Unit;
                            order.Users = userPlan.Users;
                            order.AttachmentSize = userPlan.AttachmentSize;
                            order.CanExtend = userPlan.CanExtend;
                            order.ComplexGroup = userPlan.ComplexGroup;
                            order.DiskSpace = userPlan.Space;
                            order.OneTime = userPlan.OneTime;
                            order.PlanCost = userPlan.PlanCost;
                            order.SimpleGroup = userPlan.SimpleGroup;
                            order.WorkPackage = userPlan.WorkPackage;
                            order.AdditionalUserCost = userPlan.AdditionalUserCost;
                            order.AdditionalComplexGroupCost = userPlan.AdditionalComplexGroupCost;
                            order.AdditionalSimpleGroupCost = userPlan.AdditionalSimpleGroupCost;
                            order.AdditionalProjectCost = userPlan.AdditionalProjectCost;
                            order.AdditionalWorkPackageCost = userPlan.AdditionalWorkPackageCost;
                            order.AdditionalSpaceCost = userPlan.AdditionalSpaceCost;
                            order.FeatureCustomField = userPlan.FeatureCustomField;
                            order.FeatureTimeSpent = userPlan.FeatureTimeSpent;
                            order.FeatureTimeValue = userPlan.FeatureTimeValue;
                            order.FeatureTimeOff = userPlan.FeatureTimeOff;
                            order.FeatureShift = userPlan.FeatureShift;
                            order.FeatureReports = userPlan.FeatureReports;
                            order.FeaturePayments = userPlan.FeaturePayments;
                            order.FeatureChat = userPlan.FeatureChat;
                            order.FeatureFiles = userPlan.FeatureFiles;
                            order.FeatureWbs = userPlan.FeatureWbs;
                            order.FeatureRoadMap = userPlan.FeatureRoadMap;
                            order.FeatureTree = userPlan.FeatureTree;
                            order.FeatureObjectives = userPlan.FeatureObjectives;
                            order.FeatureSeasons = userPlan.FeatureSeasons;
                            order.FeatureVote = userPlan.FeatureVote;
                            order.FeatureSubTask = userPlan.FeatureSubTask;
                            order.FeatureCalendar = userPlan.FeatureCalendar;
                            order.FeatureKartabl = userPlan.FeatureKartabl;
                            order.FeatureBlocking = userPlan.FeatureBlocking;
                            order.FeatureRelated = userPlan.FeatureRelated;
                            order.FeatureComplexGroup = userPlan.FeatureComplexGroup;
                            order.FeatureGroupTimeSpent = userPlan.FeatureGroupTimeSpent;
                            order.TotalAmount = totalAmount;
                            order.PlanId = userPlan.PlanId;
                            order.UserId = userId;
                            break;
                        case OrderType.Patch:
                            // extending existing plan
                            if (!userPlan.CanExtend || !userPlan.ExpireAt.HasValue)
                                return OperationResult<Guid>.Rejected();
                            if (userPlan.Duration != model.Duration) return OperationResult<Guid>.Rejected();

                            var totalDuration = (userPlan.ExpireAt.Value - userPlan.CreatedAt).TotalMilliseconds;
                            var remainDuration = (DateTime.UtcNow - userPlan.CreatedAt).TotalMilliseconds;
                            var remainPercent = 100 - (remainDuration * 100 / totalDuration);

                            var additionalSpace = (int) model.DiskSpace == 0
                                ? 0
                                : (model.DiskSpace - userPlan.Space) / 1024 / 1024 / 1024;
                            var additionalUser = model.Users == 0 ? 0 : model.Users - userPlan.Users;
                            var additionalSimpleGroup =
                                model.SimpleGroup == 0 ? 0 : model.SimpleGroup - userPlan.SimpleGroup;
                            var additionalComplexGroup = model.ComplexGroup == 0
                                ? 0
                                : model.ComplexGroup - userPlan.ComplexGroup;
                            var additionalProject =
                                model.Project == 0 ? 0 : model.Project - userPlan.Project;
                            var additionalWorkPackage =
                                model.WorkPackage == 0 ? 0 : model.WorkPackage - userPlan.WorkPackage;

                            usersCost = additionalUser * userPlan.AdditionalUserCost;
                            spaceCost = additionalSpace * userPlan.AdditionalSpaceCost;
                            simpleGroupsCost = additionalSimpleGroup * userPlan.AdditionalSimpleGroupCost;
                            complexGroupCost = additionalComplexGroup * userPlan.AdditionalComplexGroupCost;
                            projectCost = additionalProject * userPlan.AdditionalProjectCost;
                            packagesCost = additionalWorkPackage * userPlan.AdditionalWorkPackageCost;

                            totalAmount = usersCost +
                                          spaceCost +
                                          simpleGroupsCost +
                                          complexGroupCost +
                                          projectCost +
                                          packagesCost;

                            switch (model.Duration)
                            {
                                case OrderDuration.Monthly:
                                    order.Days = 30;
                                    break;
                                case OrderDuration.Season:
                                    order.Days = 90;
                                    totalAmount = totalAmount * 3;
                                    break;
                                case OrderDuration.HalfYear:
                                    order.Days = 180;
                                    totalAmount = totalAmount * 6;
                                    break;
                                case OrderDuration.Yearly:
                                    order.Days = 365;
                                    totalAmount = totalAmount * 12;
                                    totalAmount = totalAmount - (totalAmount / 10);
                                    break;
                            }

                            totalAmount = Math.Round(totalAmount * remainPercent / 100);

                            order.ExpireAt = DateTime.UtcNow.AddHours(72);

                            order.Type = customPlan.Type;
                            order.PlanId = customPlan.Id;
                            order.Unit = basedOn.Unit;
                            order.TotalAmount = totalAmount;
                            order.UserId = userId;
                            order.Users = additionalUser;
                            order.SimpleGroup = additionalSimpleGroup;
                            order.ComplexGroup = additionalComplexGroup;
                            order.Project = additionalProject;
                            order.WorkPackage = additionalWorkPackage;
                            order.DiskSpace = Math.Round(additionalSpace) * 1024 * 1024 * 1024;
                            break;
                        case OrderType.Change:
                            if (basedOn.Type != PlanType.Custom)
                            {
                                // Change of plan, every thing according to selected plan
                                if (basedOn.Users < userPlan.Users) return OperationResult<Guid>.Rejected();
                                if (basedOn.DiskSpace < userPlan.Space) return OperationResult<Guid>.Rejected();
                                if (basedOn.SimpleGroup < userPlan.SimpleGroup)
                                    return OperationResult<Guid>.Rejected();
                                if (basedOn.ComplexGroup < userPlan.ComplexGroup)
                                    return OperationResult<Guid>.Rejected();
                                if (basedOn.Project < userPlan.Project)
                                    return OperationResult<Guid>.Rejected();
                                if (basedOn.WorkPackage < userPlan.WorkPackage)
                                    return OperationResult<Guid>.Rejected();

                                totalAmount = basedOn.PlanCost;
                                switch (model.Duration)
                                {
                                    case OrderDuration.Monthly:
                                        order.Days = 30;
                                        break;
                                    case OrderDuration.Season:
                                        order.Days = 90;
                                        totalAmount = totalAmount * 3;
                                        break;
                                    case OrderDuration.HalfYear:
                                        order.Days = 180;
                                        totalAmount = totalAmount * 6;
                                        break;
                                    case OrderDuration.Yearly:
                                        order.Days = 365;
                                        totalAmount = totalAmount * 12;
                                        totalAmount = totalAmount - (totalAmount / 10);
                                        break;
                                }

                                order.ExpireAt = DateTime.UtcNow.AddHours(72);
                                order.Project = basedOn.Project;
                                order.Type = basedOn.Type;
                                order.Unit = basedOn.Unit;
                                order.Users = basedOn.Users;
                                order.AttachmentSize = basedOn.AttachmentSize;
                                order.CanExtend = basedOn.CanExtend;
                                order.ComplexGroup = basedOn.ComplexGroup;
                                order.DiskSpace = basedOn.DiskSpace;
                                order.OneTime = basedOn.OneTime;
                                order.PlanCost = basedOn.PlanCost;
                                order.SimpleGroup = basedOn.SimpleGroup;
                                order.WorkPackage = basedOn.WorkPackage;
                                order.AdditionalUserCost = basedOn.AdditionalUserCost;
                                order.AdditionalComplexGroupCost = basedOn.AdditionalComplexGroupCost;
                                order.AdditionalSimpleGroupCost = basedOn.AdditionalSimpleGroupCost;
                                order.AdditionalProjectCost = basedOn.AdditionalProjectCost;
                                order.AdditionalWorkPackageCost = basedOn.AdditionalWorkPackageCost;
                                order.AdditionalSpaceCost = basedOn.AdditionalSpaceCost;
                                order.FeatureCustomField = basedOn.FeatureCustomField;
                                order.FeatureTimeSpent = basedOn.FeatureTimeSpent;
                                order.FeatureTimeValue = basedOn.FeatureTimeValue;
                                order.FeatureTimeOff = basedOn.FeatureTimeOff;
                                order.FeatureShift = basedOn.FeatureShift;
                                order.FeatureReports = basedOn.FeatureReports;
                                order.FeaturePayments = basedOn.FeaturePayments;
                                order.FeatureChat = basedOn.FeatureChat;
                                order.FeatureFiles = basedOn.FeatureFiles;
                                order.FeatureWbs = basedOn.FeatureWbs;
                                order.FeatureRoadMap = basedOn.FeatureRoadMap;
                                order.FeatureTree = basedOn.FeatureTree;
                                order.FeatureObjectives = basedOn.FeatureObjectives;
                                order.FeatureSeasons = basedOn.FeatureSeasons;
                                order.FeatureVote = basedOn.FeatureVote;
                                order.FeatureSubTask = basedOn.FeatureSubTask;
                                order.FeatureCalendar = basedOn.FeatureCalendar;
                                order.FeatureKartabl = basedOn.FeatureKartabl;
                                order.FeatureBlocking = basedOn.FeatureBlocking;
                                order.FeatureRelated = basedOn.FeatureRelated;
                                order.FeatureComplexGroup = basedOn.FeatureComplexGroup;
                                order.FeatureGroupTimeSpent = basedOn.FeatureGroupTimeSpent;
                                order.TotalAmount = totalAmount;
                                order.PlanId = basedOn.Id;
                                order.UserId = userId;
                            }
                            else
                            {
                                // Change of plan, every thing according to user
                                if (model.Users < userPlan.Users) model.Users = userPlan.Users;
                                if (model.DiskSpace < userPlan.Space) model.DiskSpace = userPlan.Space;
                                if (model.SimpleGroup < userPlan.SimpleGroup) model.SimpleGroup = userPlan.SimpleGroup;
                                if (model.ComplexGroup < userPlan.ComplexGroup)
                                    model.ComplexGroup = userPlan.ComplexGroup;
                                if (model.Project < userPlan.Project) model.Project = userPlan.Project;
                                if (model.WorkPackage < userPlan.WorkPackage) model.WorkPackage = userPlan.WorkPackage;

                                additionalSpace = (int) model.DiskSpace == 0
                                    ? 0
                                    : (model.DiskSpace - userPlan.Space) / 1024 / 1024 / 1024;
                                additionalUser = model.Users == 0 ? 0 : model.Users - userPlan.Users;
                                additionalSimpleGroup =
                                    model.SimpleGroup == 0 ? 0 : model.SimpleGroup - userPlan.SimpleGroup;
                                additionalComplexGroup = model.ComplexGroup == 0
                                    ? 0
                                    : model.ComplexGroup - userPlan.ComplexGroup;
                                additionalProject =
                                    model.Project == 0 ? 0 : model.Project - userPlan.Project;
                                additionalWorkPackage =
                                    model.WorkPackage == 0 ? 0 : model.WorkPackage - userPlan.WorkPackage;

                                if (userPlan.Type == PlanType.Free)
                                {
                                    usersCost = additionalUser * basedOn.AdditionalUserCost;
                                    spaceCost = additionalSpace * basedOn.AdditionalSpaceCost;
                                    simpleGroupsCost = additionalSimpleGroup * basedOn.AdditionalSimpleGroupCost;
                                    complexGroupCost = additionalComplexGroup * basedOn.AdditionalComplexGroupCost;
                                    projectCost = additionalProject * basedOn.AdditionalProjectCost;
                                    packagesCost = additionalWorkPackage * basedOn.AdditionalWorkPackageCost;
                                }
                                else
                                {
                                    usersCost = additionalUser * userPlan.AdditionalUserCost;
                                    spaceCost = additionalSpace * userPlan.AdditionalSpaceCost;
                                    simpleGroupsCost = additionalSimpleGroup * userPlan.AdditionalSimpleGroupCost;
                                    complexGroupCost = additionalComplexGroup * userPlan.AdditionalComplexGroupCost;
                                    projectCost = additionalProject * userPlan.AdditionalProjectCost;
                                    packagesCost = additionalWorkPackage * userPlan.AdditionalWorkPackageCost;
                                }
                                
                                totalAmount = usersCost +
                                              spaceCost +
                                              simpleGroupsCost +
                                              complexGroupCost +
                                              projectCost +
                                              packagesCost;

                                switch (model.Duration)
                                {
                                    case OrderDuration.Monthly:
                                        order.Days = 30;
                                        break;
                                    case OrderDuration.Season:
                                        order.Days = 90;
                                        break;
                                    case OrderDuration.HalfYear:
                                        order.Days = 180;
                                        break;
                                    case OrderDuration.Yearly:
                                        totalAmount = totalAmount * 12;
                                        totalAmount = totalAmount - (totalAmount / 10);
                                        order.Days = 365;
                                        break;
                                }

                                order.ExpireAt = DateTime.UtcNow.AddHours(72);
                                order.Type = basedOn.Type;
                                order.Unit = basedOn.Unit;
                                order.AttachmentSize = basedOn.AttachmentSize;
                                order.CanExtend = basedOn.CanExtend;

                                order.Users = additionalUser;
                                order.SimpleGroup = additionalSimpleGroup;
                                order.ComplexGroup = additionalComplexGroup;
                                order.Project = additionalProject;
                                order.WorkPackage = additionalWorkPackage;
                                order.DiskSpace = Math.Round(additionalSpace) * 1024 * 1024 * 1024;

                                order.OneTime = basedOn.OneTime;
                                order.PlanCost = totalAmount;
                                order.AdditionalUserCost = basedOn.AdditionalUserCost;
                                order.AdditionalComplexGroupCost = basedOn.AdditionalComplexGroupCost;
                                order.AdditionalSimpleGroupCost = basedOn.AdditionalSimpleGroupCost;
                                order.AdditionalProjectCost = basedOn.AdditionalProjectCost;
                                order.AdditionalWorkPackageCost = basedOn.AdditionalWorkPackageCost;
                                order.AdditionalSpaceCost = basedOn.AdditionalSpaceCost;

                                order.FeatureCustomField = basedOn.FeatureCustomField;
                                order.FeatureTimeSpent = basedOn.FeatureTimeSpent;
                                order.FeatureTimeValue = basedOn.FeatureTimeValue;
                                order.FeatureTimeOff = basedOn.FeatureTimeOff;
                                order.FeatureShift = basedOn.FeatureShift;
                                order.FeatureReports = basedOn.FeatureReports;
                                order.FeaturePayments = basedOn.FeaturePayments;
                                order.FeatureChat = basedOn.FeatureChat;
                                order.FeatureFiles = basedOn.FeatureFiles;
                                order.FeatureWbs = basedOn.FeatureWbs;
                                order.FeatureRoadMap = basedOn.FeatureRoadMap;
                                order.FeatureTree = basedOn.FeatureTree;
                                order.FeatureObjectives = basedOn.FeatureObjectives;
                                order.FeatureSeasons = basedOn.FeatureSeasons;
                                order.FeatureVote = basedOn.FeatureVote;
                                order.FeatureSubTask = basedOn.FeatureSubTask;
                                order.FeatureCalendar = basedOn.FeatureCalendar;
                                order.FeatureKartabl = basedOn.FeatureKartabl;
                                order.FeatureBlocking = basedOn.FeatureBlocking;
                                order.FeatureRelated = basedOn.FeatureRelated;
                                order.FeatureComplexGroup = basedOn.FeatureComplexGroup;
                                order.FeatureGroupTimeSpent = basedOn.FeatureGroupTimeSpent;
                                order.TotalAmount = totalAmount;
                                order.PlanId = basedOn.Id;
                                order.UserId = userId;
                            }
                            break;
                    }

                    if (!string.IsNullOrEmpty(model.DiscountCode))
                    {
                        var op = await CheckDiscount(userId, new CheckDiscountViewModel
                        {
                            Amount = totalAmount,
                            Code = model.DiscountCode,
                            PlanId = model.PlanId
                        });
                        if (op.Status == OperationResultStatus.Success && op.Data.Success)
                        {
                            order.DiscountId = op.Data.Id;
                            order.AppliedDiscount = op.Data.Amount;
                        }
                    }

                    if ((int) order.TotalAmount < 100) throw new Exception("Total amount can not be less than 100");

                    if(_serviceProvider.GetService<IServerInfo>().IsDevelopment)
                        order.AppliedDiscount = order.TotalAmount;
                    
                    order.PaymentAmount = order.TotalAmount - (order.AppliedDiscount ?? 0);
                    // order.ValueAdded = order.PaymentAmount / 100 * VALUE_ADDED_RATE;
                    // order.PaymentAmount += order.ValueAdded;
                    order.UseWallet = model.UseWallet;

                    await _serviceProvider.GetService<IPostmanBiz>()
                        .OrderCreated(basedOn.Title, user.ToViewModel(true), order.ToViewModel());

                    await unit.Orders.AddAsync(order);
                    await unit.SaveChangesAsync();
                    return OperationResult<Guid>.Success(order.Id);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<Guid>.Failed();
            }
        }

        public async Task<OperationResult<string>> Pay(Guid orderId)
        {
            try
            {
                var op = await _serviceProvider.GetService<IPaymentBiz>().PayByPayPing(orderId);
                if (op.Status != OperationResultStatus.Success)
                    return OperationResult<string>.Success($"https://{Domain}/?invalidOrder=true");
                return OperationResult<string>.Success(op.Data);
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<string>.Failed();
            }
        }

        public async Task<OperationResult<Tuple<bool, string>>> PayPingCallBack(string transId, Guid paymentId)
        {
            var callbackUrl = $"https://panel.{Domain}/payment/failed/{paymentId}";
            var failedOp = OperationResult<Tuple<bool, string>>
                .Success(new Tuple<bool, string>(false, callbackUrl));
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var found = await (
                        from transaction in unit.Transaction
                        join order in unit.Orders on transaction.OrderId equals order.Id
                        join user in unit.Users on order.UserId equals user.Id
                        where transaction.Id == paymentId && transaction.Status == TransactionStatus.Pending
                        select new {Transaction = transaction, Order = order, User = user}
                    ).SingleOrDefaultAsync();
                    if (found == null) return failedOp;

                    var verify = await _serviceProvider
                        .GetService<IPaymentBiz>()
                        .VerifyPayPing(found.Transaction.Amount, transId);

                    var now = DateTime.UtcNow;

                    found.Order.UpdatedAt = now;
                    found.Transaction.UpdatedAt = now;
                    if (!verify.Data.Success)
                    {
                        found.Transaction.Status = TransactionStatus.Canceled;
                        found.Transaction.Detail = verify.Data.Message;
                        await unit.SaveChangesAsync();
                        return failedOp;
                    }

                    found.Transaction.ApprovedAt = now;
                    found.Transaction.Status = TransactionStatus.Success;
                    found.Order.Status = OrderStatus.Success;

                    await _serviceProvider.GetService<IPaymentBiz>().ConfirmOrder(unit, found.Order);

                    await _serviceProvider.GetService<IPostmanBiz>()
                        .OrderPaid(found.User.ToViewModel(), found.Order.ToViewModel());
                    
                    return OperationResult<Tuple<bool, string>>
                        .Success(new Tuple<bool, string>(true,
                            $"https://panel.{Domain}/payment/success/{paymentId}"));
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<Tuple<bool, string>>.Failed();
            }
        }

        public async Task<Stream> Pdf(Guid id)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var order = await unit.Orders.AsNoTracking().SingleOrDefaultAsync(i => i.Id == id);
                    if (order == null || order.Status == OrderStatus.Canceled) return null;
                    var serverInfo = _serviceProvider.GetService<IServerInfo>();
                    var pdf = order.Status == OrderStatus.Pending
                        ? $"{serverInfo.ContentRootPath}/pdf/created/{order.Id}.pdf"
                        : $"{serverInfo.ContentRootPath}/pdf/paid/{order.Id}.pdf";
                    return !File.Exists(pdf) ? null : File.Open(pdf, FileMode.Open);
                }
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return null;
            }
        }
    }
}