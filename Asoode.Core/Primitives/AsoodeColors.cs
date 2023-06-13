using System.Collections.Generic;
using System.Linq;
using Asoode.Core.ViewModels.General;

namespace Asoode.Core.Primitives;

public static class AsoodeColors
{
    public static Color Default => Plate.First();

    public static List<Color> Plate => new()
    {
        new() { Dark = true, Title = "gray", Value = "#999999" },
        new() { Dark = false, Title = "pink", Value = "#ee6285" },
        new() { Dark = false, Title = "green", Value = "#74d68c" },
        new() { Dark = false, Title = "lightBlue", Value = "#50a9dd" },
        new() { Dark = true, Title = "darkBlue", Value = "#2c338d" },
        new() { Dark = false, Title = "red", Value = "#b33634" },
        new() { Dark = false, Title = "purple", Value = "#b977f7" },
        new() { Dark = false, Title = "yellow", Value = "#f2d600" },
        new() { Dark = false, Title = "orange", Value = "#ff9f1a" }
    };
}