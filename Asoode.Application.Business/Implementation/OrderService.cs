using Asoode.Application.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.Membership.Order;
using Asoode.Shared.Abstraction.Dtos.Payment;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Abstraction.Helpers;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contracts;

namespace Asoode.Application.Business.Implementation;

internal class OrderService : IOrderService
{
    private readonly ILoggerService _loggerService;
    private readonly IOrderRepository _repository;

    public OrderService(ILoggerService loggerService, IOrderRepository repository)
    {
        _loggerService = loggerService;
        _repository = repository;
    }

    public async Task<OperationResult<OrderDiscountResultDto>> CheckDiscount(Guid userId, CheckDiscountDto model)
    {
        try
        {
            var now = DateTime.UtcNow;

            var user = await _repository.GetUser(userId);
            if (user == null || user.IsLocked || user.Blocked)
                return OperationResult<OrderDiscountResultDto>.Rejected();

            var discount = await _repository.GetDiscount(model.Code);

            if (discount == null)
                return OperationResult<OrderDiscountResultDto>
                    .Success(new OrderDiscountResultDto { Invalid = true });

            if (discount.PlanId.HasValue && discount.PlanId != model.PlanId)
                return OperationResult<OrderDiscountResultDto>
                    .Success(new OrderDiscountResultDto { InvalidPlan = true });

            if (discount.ForUser.HasValue && discount.ForUser.Value != userId)
                return OperationResult<OrderDiscountResultDto>
                    .Success(new OrderDiscountResultDto { Invalid = true });

            if (discount.EndAt.HasValue && discount.EndAt.Value > now)
                return OperationResult<OrderDiscountResultDto>.Success(new OrderDiscountResultDto
                {
                    Expired = true
                });

            if (discount.StartAt.HasValue && discount.StartAt.Value < now)
                return OperationResult<OrderDiscountResultDto>.Success(new OrderDiscountResultDto
                {
                    Expired = true
                });

            if (discount.MaxUsage != -1)
            {
                var numOfUsage = await _repository.DidUserAlreadyUsedDiscount(userId, discount.Id);
                if (discount.MaxUsage == numOfUsage)
                    return OperationResult<OrderDiscountResultDto>
                        .Success(new OrderDiscountResultDto { AlreadyUsed = true });
            }

            var result = new OrderDiscountResultDto { Success = true, Id = discount.Id };
            if (discount.MaxUnit == -1 || model.Amount < discount.MaxUnit)
            {
                result.Amount = model.Amount;
                return OperationResult<OrderDiscountResultDto>.Success(result);
            }

            result.Amount = discount.MaxUnit;
            return OperationResult<OrderDiscountResultDto>.Success(result);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "OrderService.CheckDiscount", e);
            return OperationResult<OrderDiscountResultDto>.Failed();
        }
    }

    public async Task<OperationResult<Guid>> Order(Guid userId, RequestOrderDto model)
    {
        try
        {
            var now = DateTime.UtcNow;

            var user = await _repository.GetUser(userId);
            if (user == null || user.IsLocked || user.Blocked)
                return OperationResult<Guid>.Rejected();

            await _repository.CancelAnyPendingOrder(userId);
            var userPlan = await _repository.GetUserPlan(userId);

            var order = new OrderDto()
            {
                Status = OrderStatus.Pending,
                Duration = model.Duration,
                OrderType = model.Type
            };
            double totalAmount = 0;
            var plans = await _repository.PlansList(model.PlanId);

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
                    spaceCost = (userPlan.Space - freePlan.DiskSpace) / 1024 / 1024 / 1024 *
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
                            totalAmount = totalAmount - totalAmount / 10;
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
                    var remainPercent = 100 - remainDuration * 100 / totalDuration;

                    var additionalSpace = (int)model.DiskSpace == 0
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
                            totalAmount = totalAmount - totalAmount / 10;
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
                                totalAmount = totalAmount - totalAmount / 10;
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

                        additionalSpace = (int)model.DiskSpace == 0
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
                                totalAmount = totalAmount - totalAmount / 10;
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
                var op = await CheckDiscount(userId, new CheckDiscountDto
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

            if ((int)order.TotalAmount < 100) throw new Exception("Total amount can not be less than 100");

            if (EnvironmentHelper.IsDevelopment())
                order.AppliedDiscount = order.TotalAmount;

            order.PaymentAmount = order.TotalAmount - (order.AppliedDiscount ?? 0);
            // order.ValueAdded = order.PaymentAmount / 100 * VALUE_ADDED_RATE;
            // order.PaymentAmount += order.ValueAdded;
            order.UseWallet = model.UseWallet;

            // TODO: fix this
            // await _serviceProvider.GetService<IPostmanBiz>()
            //     .OrderCreated(basedOn.Title, user.ToDto(true), order);

            await _repository.Store(order);
            return OperationResult<Guid>.Success(order.Id);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "OrderService.Order", e);
            return OperationResult<Guid>.Failed();
        }
    }

    public async Task<OperationResult<string>> Pay(Guid orderId)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "OrderService.Pay", e);
            return OperationResult<string>.Failed();
        }
    }

    public async Task<Stream?> Pdf(Guid id)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "OrderService.Pdf", e);
            return null;
        }
    }
}