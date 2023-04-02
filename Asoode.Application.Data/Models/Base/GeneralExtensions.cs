using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Data.Models.Base
{
    public static class GeneralExtensions
    {
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
}