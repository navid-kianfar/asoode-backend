using Asoode.Core.ViewModels.Admin;
using Asoode.Core.ViewModels.General;

namespace Asoode.Data.Models.Base;

public static class GeneralExtensions
{
    public static DiscountViewModel ToViewModel(this Discount discount)
    {
        return new DiscountViewModel
        {
            Code = discount.Code,
            Description = discount.Description,
            Id = discount.Id,
            Percent = discount.Percent,
            Title = discount.Title,
            Unit = discount.Unit,
            CreatedAt = discount.CreatedAt,
            EndAt = discount.EndAt,
            ForUser = discount.ForUser,
            MaxUnit = discount.MaxUnit,
            MaxUsage = discount.MaxUsage,
            StartAt = discount.StartAt,
            UpdatedAt = discount.UpdatedAt,
            PlanId = discount.PlanId
        };
    }

    public static TaskLogViewModel ToViewModel(this ActivityLog log)
    {
        return new TaskLogViewModel
        {
            Description = log.Description,
            Type = log.Type,
            RecordId = log.RecordId,
            UserId = log.UserId
        };
    }
}