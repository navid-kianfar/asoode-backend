namespace Asoode.Application.Data.Models.Base
{
    public static class OrderExtensions
    {
        public static TransactionViewModel ToViewModel(this Transaction transaction)
        {
            return new TransactionViewModel
            {
                Amount = transaction.Amount,
                Detail = transaction.Detail,
                Id = transaction.Id,
                Status = transaction.Status,
                ApprovedAt = transaction.ApprovedAt,
                CreatedAt = transaction.CreatedAt,
                ExternalId = transaction.ExternalId,
                OrderId = transaction.OrderId,
                ReferenceNumber = transaction.ReferenceNumber,
                TrackingCode = transaction.TrackingCode,
                UpdatedAt = transaction.UpdatedAt,
            };
        }
        
        public static OrderViewModel ToViewModel(this Order order)
        {
            return new OrderViewModel
            {
                ExpireAt = order.ExpireAt,
                Type = order.Type,
                Unit = order.Unit,
                AttachmentSize = order.AttachmentSize,
                CanExtend = order.CanExtend,
                Users = order.Users,
                SimpleGroup = order.SimpleGroup,
                ComplexGroup = order.ComplexGroup,
                Project = order.Project,
                WorkPackage = order.WorkPackage,
                DiskSpace =order.DiskSpace,
                OneTime = order.OneTime,
                PlanCost = order.PlanCost,
                AdditionalUserCost = order.AdditionalUserCost,
                AdditionalComplexGroupCost = order.AdditionalComplexGroupCost,
                AdditionalSimpleGroupCost = order.AdditionalSimpleGroupCost,
                AdditionalProjectCost = order.AdditionalProjectCost,
                AdditionalWorkPackageCost = order.AdditionalWorkPackageCost,
                AdditionalSpaceCost = order.AdditionalSpaceCost,
                FeatureCustomField = order.FeatureCustomField,
                FeatureTimeSpent = order.FeatureTimeSpent,
                FeatureTimeValue = order.FeatureTimeValue,
                FeatureTimeOff = order.FeatureTimeOff,
                FeatureShift = order.FeatureShift,
                FeatureReports = order.FeatureReports,
                FeaturePayments = order.FeaturePayments,
                FeatureChat = order.FeatureChat,
                FeatureFiles = order.FeatureFiles,
                FeatureWbs = order.FeatureWbs,
                FeatureRoadMap = order.FeatureRoadMap,
                FeatureTree = order.FeatureTree,
                FeatureObjectives = order.FeatureObjectives,
                FeatureSeasons = order.FeatureSeasons,
                FeatureVote = order.FeatureVote,
                FeatureSubTask = order.FeatureSubTask,
                FeatureCalendar = order.FeatureCalendar,
                FeatureKartabl = order.FeatureKartabl,
                FeatureBlocking = order.FeatureBlocking,
                FeatureRelated = order.FeatureRelated,
                FeatureComplexGroup = order.FeatureComplexGroup,
                FeatureGroupTimeSpent = order.FeatureGroupTimeSpent,
                TotalAmount = order.TotalAmount,
                PlanId = order.Id,
                UserId = order.UserId,
                Automated = order.Automated,
                Days = order.Days,
                Duration = order.Duration,
                Id = order.Id,
                Status = order.Status,
                AppliedDiscount = order.AppliedDiscount,
                CreatedAt = order.CreatedAt,
                DiscountId = order.DiscountId,
                OrderType = order.OrderType,
                PaymentAmount = order.PaymentAmount,
                UpdatedAt = order.UpdatedAt,
                UseWallet = order.UseWallet,
                ValueAdded = order.ValueAdded,
            };
        }
    }
}