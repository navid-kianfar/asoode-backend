using Asoode.Core.ViewModels.Admin;

namespace Asoode.Data.Models.Base;

public static class ErrorExtensions
{
    public static ErrorViewModel ToViewModel(this ErrorLog error)
    {
        return new ErrorViewModel
        {
            Description = error.Description,
            Id = error.Id,
            CreatedAt = error.CreatedAt,
            ErrorBody = error.ErrorBody,
            UpdatedAt = error.UpdatedAt
        };
    }
}