using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aozora.Helpers;

public static partial class YamlValues
{
    public static string? Jisx0213ToUnicode(string code)
    {
        var codes = code.Split('-').Select(a => int.TryParse(a, out int b) ? b : -1).ToArray();
        if (codes.Length < 3) return null;
        return Jisx0213ToUnicode(codes[0], codes[1], codes[2]);
    }
}
