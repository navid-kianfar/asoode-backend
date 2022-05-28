namespace Asoode.Application.Core.Helpers
{
    public static class GuidExtensions
    {
        public static string ToShortUniqueId(this Guid guid)
        {
            return guid.ToString().GetHashCode().ToString("x");
        }
    }
}