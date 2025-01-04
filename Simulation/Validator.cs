using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation;

public static class Validator
{
    public static string Shortener(string value, int min, int max, char placeholder)
    {
        value = value?.Trim() ?? string.Empty;
        if (value.Length > max)
            value = value[..max].TrimEnd();

        if (value.Length < min)
            value = value.PadRight(min, placeholder);

        if (!string.IsNullOrEmpty(value))
            value = char.ToUpper(value[0]) + value.Substring(1);

        return value;
    }
}
