namespace Asoode.Application.Data.Models.Base
{
    public static class MarketerExtensions
    {
        public static MarketerViewModel ToViewModel(this Marketer marketer)
        {
            return new MarketerViewModel
            {
                Code = marketer.Code,
                Description = marketer.Description,
                Enabled = marketer.Enabled,
                Fixed = marketer.Fixed,
                Id = marketer.Id,
                Percent = marketer.Percent,
                Title = marketer.Title,
                CreatedAt = marketer.CreatedAt,
                UpdatedAt = marketer.UpdatedAt
            };
        }
    }
}