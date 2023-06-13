using Microsoft.EntityFrameworkCore;

namespace Asoode.Data.Contexts;

// ReSharper disable once InconsistentNaming
public static class _ContextOverrides
{
    public static void OnModelCreating(ModelBuilder builder)
    {
        foreach (var entity in builder.Model.GetEntityTypes()) entity.SetTableName(entity.GetTableName().ToLower());
    }
}