using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Asoode.Core.Contracts.General;
using Asoode.Core.Contracts.Logging;
using Asoode.Core.Primitives;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.Payment.PayPing;
using Asoode.Data.Contexts;
using Asoode.Data.Models;
using Asoode.Data.Models.Base;
using Asoode.Data.Models.Junctions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;

namespace Asoode.Business.General
{
    internal class PaymentBiz : IPaymentBiz
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly bool Development;
        private readonly string VerifyUrl;
        private readonly string ForwardUrl;
        private readonly string Callback;
        private readonly string TestCallback;
        private readonly string Token;
        private readonly string TestToken;
        private readonly string Domain;

        public PaymentBiz(IServiceProvider serviceProvider, IConfiguration configuration, IServerInfo serverInfo)
        {
            _serviceProvider = serviceProvider;
            Development = serverInfo.IsDevelopment;
            VerifyUrl = configuration["Setting:PaymentGateWay:PayPing:VerifyUrl"];
            ForwardUrl = configuration["Setting:PaymentGateWay:PayPing:Forward"];
            Callback = configuration["Setting:PaymentGateWay:PayPing:Callback"];
            TestCallback = configuration["Setting:PaymentGateWay:PayPing:TestCallback"];
            Token = configuration["Setting:PaymentGateWay:PayPing:Id"];
            TestToken = configuration["Setting:PaymentGateWay:PayPing:TestId"];
            Domain = configuration["Setting:Domain"];
        }

        #region GateWay

        private async Task<T> CallApi<T>(string apiUrl, object value, bool isToken = false) where T : class
        {
            var url = new Uri(apiUrl);
            var baseHost = $"{url.Scheme}://{url.Authority}";
            var method = $"{url.AbsolutePath}";
            var client = new RestClient(baseHost) {Timeout = 20000};
            var request = new RestRequest(method, Method.POST);
            if (isToken) request.AddObject(value);
            else
            {
                client.AddDefaultHeader("Authorization",
                    Development ? $"bearer {TestToken}" : $"bearer {Token}");
                request.AddJsonBody(value);
            }

            var response = await client.ExecuteAsync<T>(request);
            if (!method.Contains("verify")) return response.Data;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = new ResponseVerifyViewModel {Success = true};
                return result as T;
            }
            else
            {
                var result = new ResponseVerifyViewModel {Message = response.Content};
                return result as T;
            }
        }

        public async Task<OperationResult<string>> PayByPayPing(Guid orderId)
        {
            try
            {
                using (var unit = _serviceProvider.GetService<AccountDbContext>())
                {
                    var order = await unit.Orders.SingleOrDefaultAsync(i =>
                        i.Id == orderId && i.Status == OrderStatus.Pending);
                    if (order == null || order.ExpireAt < DateTime.UtcNow)
                        return OperationResult<string>.Rejected();

                    if ((int)order.PaymentAmount == 0) return await ConfirmOrder(unit, order);

                    var user = await unit.Users.AsNoTracking()
                        .SingleOrDefaultAsync(i => i.Id == order.UserId);

                    var paymentId = Guid.NewGuid();

                    var paymentRequest = await CallApi<PaymentResult>(ForwardUrl, new RequestPayment
                    {
                        Amount = (int) (Development ? 1000 : order.PaymentAmount),
                        Description = "",
                        PayerIdentity = order.UserId.ToString(),
                        PayerName = user.FullName,
                        ReturnUrl = (Development ? TestCallback : Callback),
                        ClientRefId = paymentId.ToString()
                    });

                    if (paymentRequest == null || string.IsNullOrEmpty(paymentRequest.Code)) return null;
                    var payment = new Transaction
                    {
                        Id = paymentId,
                        Status = TransactionStatus.Pending,
                        Amount = order.PaymentAmount,
                        OrderId = orderId,
                        ReferenceNumber = paymentRequest.Code,
                    };
                    await unit.Transaction.AddAsync(payment);
                    await unit.SaveChangesAsync();
                    var ret = $"https://api.payping.ir/v1/pay/gotoipg/{paymentRequest.Code}";
                    return OperationResult<string>.Success(ret);
                }
            }
            catch (Exception e)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(e);
                return null;
            }
        }

        public async Task<OperationResult<ResponseVerifyViewModel>> VerifyPayPing(double amount, string transId)
        {
            try
            {
                var verifyBody = new RequestVerify
                {
                    Amount = (int) (Development ? 1000 : amount),
                    RefId = transId
                };
                var result = await CallApi<ResponseVerifyViewModel>(VerifyUrl, verifyBody);
                return OperationResult<ResponseVerifyViewModel>.Success(result);
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return null;
            }
        }

        #endregion

        public async Task<OperationResult<string>> ConfirmOrder(object unitObj, object orderObj)
        {
            AccountDbContext unit = (AccountDbContext) unitObj;
            Order order = (Order) orderObj;
            var now = DateTime.UtcNow;

            order.UpdatedAt = now;
            order.Status = OrderStatus.Success;

            var user = await unit.Users.SingleAsync(i => i.Id == order.UserId);

            var oldPlan = await unit.UserPlanInfo
                .Where(i => i.UserId == order.UserId)
                .OrderByDescending(i => i.CreatedAt)
                .FirstAsync();

            var plan = await unit.Plans
                .AsNoTracking()
                .SingleAsync(i => i.Id == order.PlanId);

            bool hasExpireDate = oldPlan.ExpireAt.HasValue && oldPlan.ExpireAt.Value > now;
            bool mustRefund = order.OrderType == OrderType.Change && hasExpireDate && oldPlan.Type != PlanType.Free;

            UserPlanInfo userPlan;
            switch (order.OrderType)
            {
                case OrderType.Renew:
                    userPlan = new UserPlanInfo
                    {
                        ExpireAt = hasExpireDate
                            ? oldPlan.ExpireAt.Value.AddDays(order.Days)
                            : now.AddDays(order.Days),
                        SimpleGroup = oldPlan.SimpleGroup,
                        WorkPackage = oldPlan.WorkPackage,
                        ComplexGroup = oldPlan.ComplexGroup,
                        Users = oldPlan.Users,
                        Space = oldPlan.Space,
                        Project = oldPlan.Project,
                        Description = plan.Description,
                        Picture = plan.Picture,
                        Title = plan.Title,
                        UsedProject = oldPlan.UsedProject,
                        UsedSpace = oldPlan.UsedSpace,
                        UsedUser = oldPlan.UsedUser,
                        UsedComplexGroup = oldPlan.UsedComplexGroup,
                        UsedSimpleGroup = oldPlan.UsedSimpleGroup,
                        UsedWorkPackage = oldPlan.UsedWorkPackage,
                        PlanId = order.PlanId,
                        OrderId = order.Id,
                        Enabled = true,
                        Days = order.Days,
                        Type = order.Type,
                        Unit = order.Unit,
                        Duration = order.Duration,
                        UserId = order.UserId,
                        CanExtend = order.CanExtend,
                        OneTime = order.OneTime,
                        PlanCost = order.PlanCost,
                        AttachmentSize = order.AttachmentSize,

                        AdditionalSpaceCost = order.AdditionalSpaceCost,
                        AdditionalProjectCost = order.AdditionalProjectCost,
                        AdditionalUserCost = order.AdditionalUserCost,
                        AdditionalComplexGroupCost = order.AdditionalComplexGroupCost,
                        AdditionalSimpleGroupCost = order.AdditionalSimpleGroupCost,
                        AdditionalWorkPackageCost = order.AdditionalWorkPackageCost,
                        FeatureComplexGroup = order.FeatureComplexGroup,
                        FeatureCustomField = order.FeatureCustomField,
                        FeatureRoadMap = order.FeatureRoadMap,
                        FeatureSubTask = order.FeatureSubTask,
                        FeatureTimeOff = order.FeatureTimeOff,
                        FeatureTimeSpent = order.FeatureTimeSpent,
                        FeatureTimeValue = order.FeatureTimeValue,
                        FeatureGroupTimeSpent = order.FeatureGroupTimeSpent,
                        FeatureBlocking = order.FeatureBlocking,
                        FeatureCalendar = order.FeatureCalendar,
                        FeatureChat = order.FeatureChat,
                        FeatureFiles = order.FeatureFiles,
                        FeatureKartabl = order.FeatureKartabl,
                        FeatureObjectives = order.FeatureObjectives,
                        FeaturePayments = order.FeaturePayments,
                        FeatureRelated = order.FeatureRelated,
                        FeatureReports = order.FeatureReports,
                        FeatureSeasons = order.FeatureSeasons,
                        FeatureShift = order.FeatureShift,
                        FeatureTree = order.FeatureTree,
                        FeatureVote = order.FeatureVote,
                        FeatureWbs = order.FeatureWbs,
                    };
                    break;
                case OrderType.Patch:
                    userPlan = new UserPlanInfo
                    {
                        Space = oldPlan.Space + order.DiskSpace,
                        Project = oldPlan.Project + order.Project,
                        Users = oldPlan.Users + order.Users,
                        ComplexGroup = oldPlan.ComplexGroup + order.ComplexGroup,
                        SimpleGroup = oldPlan.SimpleGroup + order.SimpleGroup,
                        WorkPackage = oldPlan.WorkPackage + order.WorkPackage,

                        Description = plan.Description,
                        Picture = plan.Picture,
                        Title = plan.Title,
                        ExpireAt = oldPlan.ExpireAt,
                        UsedProject = oldPlan.UsedProject,
                        UsedSpace = oldPlan.UsedSpace,
                        UsedUser = oldPlan.UsedUser,
                        UsedComplexGroup = oldPlan.UsedComplexGroup,
                        UsedSimpleGroup = oldPlan.UsedSimpleGroup,
                        UsedWorkPackage = oldPlan.UsedWorkPackage,
                        PlanId = order.PlanId,
                        OrderId = order.Id,
                        Type = order.Type,
                        Unit = order.Unit,
                        Duration = order.Duration,
                        Enabled = true,
                        Days = (int) (now - oldPlan.CreatedAt).TotalDays,
                        UserId = oldPlan.UserId,
                        CanExtend = oldPlan.CanExtend,
                        OneTime = oldPlan.OneTime,
                        PlanCost = oldPlan.PlanCost,
                        AttachmentSize = oldPlan.AttachmentSize,
                        FeatureComplexGroup = oldPlan.FeatureComplexGroup,
                        FeatureCustomField = oldPlan.FeatureCustomField,
                        FeatureRoadMap = oldPlan.FeatureRoadMap,
                        FeatureSubTask = oldPlan.FeatureSubTask,
                        FeatureTimeOff = oldPlan.FeatureTimeOff,
                        FeatureTimeSpent = oldPlan.FeatureTimeSpent,
                        FeatureTimeValue = oldPlan.FeatureTimeValue,
                        FeatureGroupTimeSpent = oldPlan.FeatureGroupTimeSpent,
                        FeatureBlocking = oldPlan.FeatureBlocking,
                        FeatureCalendar = oldPlan.FeatureCalendar,
                        FeatureChat = oldPlan.FeatureChat,
                        FeatureFiles = oldPlan.FeatureFiles,
                        FeatureKartabl = oldPlan.FeatureKartabl,
                        FeatureObjectives = oldPlan.FeatureObjectives,
                        FeaturePayments = oldPlan.FeaturePayments,
                        FeatureRelated = oldPlan.FeatureRelated,
                        FeatureReports = oldPlan.FeatureReports,
                        FeatureSeasons = oldPlan.FeatureSeasons,
                        FeatureShift = oldPlan.FeatureShift,
                        FeatureTree = oldPlan.FeatureTree,
                        FeatureVote = oldPlan.FeatureVote,
                        FeatureWbs = oldPlan.FeatureWbs,
                        AdditionalSpaceCost = oldPlan.AdditionalSpaceCost,
                        AdditionalProjectCost = oldPlan.AdditionalProjectCost,
                        AdditionalUserCost = oldPlan.AdditionalUserCost,
                        AdditionalComplexGroupCost = oldPlan.AdditionalComplexGroupCost,
                        AdditionalSimpleGroupCost = oldPlan.AdditionalSimpleGroupCost,
                        AdditionalWorkPackageCost = oldPlan.AdditionalWorkPackageCost,
                    };
                    break;
                case OrderType.Change:
                    if (mustRefund)
                    {
                        var planOrder = await unit.Orders
                            .AsNoTracking()
                            .SingleAsync(o => o.Id == oldPlan.OrderId.Value);

                        var timeSpan = planOrder.CreatedAt.AddDays(planOrder.Days) - planOrder.CreatedAt;
                        var priceByHours = Math.Round(((planOrder.PaymentAmount * 1000000) / timeSpan.TotalHours));

                        var gap = (now - planOrder.CreatedAt).TotalHours;
                        var calculatedBalance = Math.Round(gap * priceByHours / 1000000);
                        if (calculatedBalance > 0)
                        {
                            user.Wallet += calculatedBalance;
                            await unit.Wallet.AddAsync(new Wallet
                            {
                                Amount = calculatedBalance,
                                Unit = planOrder.Unit,
                                Type = WalletType.Order,
                                OrderId = order.Id,
                                UserId = order.UserId
                            });
                        }
                    }

                    userPlan = new UserPlanInfo
                    {
                        SimpleGroup = order.SimpleGroup,
                        WorkPackage = order.WorkPackage,
                        ComplexGroup = order.ComplexGroup,
                        Users = order.Users,
                        Space = order.DiskSpace,
                        Project = order.Project,

                        ExpireAt = now.AddDays(order.Days),
                        Description = plan.Description,
                        Picture = plan.Picture,
                        Title = plan.Title,
                        UsedProject = oldPlan.UsedProject,
                        UsedSpace = oldPlan.UsedSpace,
                        UsedUser = oldPlan.UsedUser,
                        UsedComplexGroup = oldPlan.UsedComplexGroup,
                        UsedSimpleGroup = oldPlan.UsedSimpleGroup,
                        UsedWorkPackage = oldPlan.UsedWorkPackage,
                        PlanId = order.PlanId,
                        OrderId = order.Id,
                        Enabled = true,
                        Days = order.Days,
                        Type = order.Type,
                        Unit = order.Unit,
                        Duration = order.Duration,
                        UserId = order.UserId,
                        CanExtend = order.CanExtend,
                        OneTime = order.OneTime,
                        PlanCost = order.PlanCost,
                        AttachmentSize = order.AttachmentSize,

                        AdditionalSpaceCost = order.AdditionalSpaceCost,
                        AdditionalProjectCost = order.AdditionalProjectCost,
                        AdditionalUserCost = order.AdditionalUserCost,
                        AdditionalComplexGroupCost = order.AdditionalComplexGroupCost,
                        AdditionalSimpleGroupCost = order.AdditionalSimpleGroupCost,
                        AdditionalWorkPackageCost = order.AdditionalWorkPackageCost,
                        FeatureComplexGroup = order.FeatureComplexGroup,
                        FeatureCustomField = order.FeatureCustomField,
                        FeatureRoadMap = order.FeatureRoadMap,
                        FeatureSubTask = order.FeatureSubTask,
                        FeatureTimeOff = order.FeatureTimeOff,
                        FeatureTimeSpent = order.FeatureTimeSpent,
                        FeatureTimeValue = order.FeatureTimeValue,
                        FeatureGroupTimeSpent = order.FeatureGroupTimeSpent,
                        FeatureBlocking = order.FeatureBlocking,
                        FeatureCalendar = order.FeatureCalendar,
                        FeatureChat = order.FeatureChat,
                        FeatureFiles = order.FeatureFiles,
                        FeatureKartabl = order.FeatureKartabl,
                        FeatureObjectives = order.FeatureObjectives,
                        FeaturePayments = order.FeaturePayments,
                        FeatureRelated = order.FeatureRelated,
                        FeatureReports = order.FeatureReports,
                        FeatureSeasons = order.FeatureSeasons,
                        FeatureShift = order.FeatureShift,
                        FeatureTree = order.FeatureTree,
                        FeatureVote = order.FeatureVote,
                        FeatureWbs = order.FeatureWbs,
                    };
                    
                    if (order.Type == PlanType.Custom)
                    {
                        userPlan.SimpleGroup = order.SimpleGroup + oldPlan.SimpleGroup;
                        userPlan.WorkPackage = order.WorkPackage + oldPlan.WorkPackage;
                        userPlan.ComplexGroup = order.ComplexGroup + oldPlan.ComplexGroup;
                        userPlan.Users = order.Users + oldPlan.Users;
                        userPlan.Space = order.DiskSpace + oldPlan.Space;
                        userPlan.Project = order.Project + oldPlan.Project;
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            oldPlan.Enabled = false;
            oldPlan.ExpireAt = now;
            oldPlan.UpdatedAt = now;

            var groups = await unit.Groups.Where(i => i.PlanInfoId == oldPlan.Id).ToArrayAsync();
            var projects = await unit.Projects.Where(i => i.PlanInfoId == oldPlan.Id).ToArrayAsync();
            var members = await unit.PlanMembers.Where(i => i.PlanId == oldPlan.Id).ToArrayAsync();

            foreach (var grp in groups) grp.PlanInfoId = userPlan.Id;
            foreach (var prj in projects) prj.PlanInfoId = userPlan.Id;
            foreach (var mbr in members) mbr.PlanId = userPlan.Id;

            await _serviceProvider.GetService<IPostmanBiz>()
                .OrderPaid(user.ToViewModel(), order.ToViewModel());

            await unit.UserPlanInfo.AddAsync(userPlan);
            await unit.SaveChangesAsync();

            return OperationResult<string>.Success($"https://panel.{Domain}");
        }
    }
}