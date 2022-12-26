using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Aozora.External;

public static partial class YamlValues
{
    public static string? JIS2UCS(string code)
    {
        var codes = code.Split('-').Select(a => int.TryParse(a, out int b) ? b : -1).ToArray();
        if (codes.Length < 3) return null;
        return JIS2UCS(codes[0], codes[1], codes[2]);
    }
}