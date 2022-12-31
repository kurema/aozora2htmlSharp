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
#if NET7_0_OR_GREATER
        return HtmlEncodeUnicode(Jisx0213ToUnicode(codes[0], codes[1], codes[2]));
#else
        return Jisx0213ToUnicode(codes[0], codes[1], codes[2]);
#endif
    }

#if NET7_0_OR_GREATER
    public static string? HtmlEncodeUnicode(string? text)
    {
        if (text is null) return null;
        var sb = new StringBuilder();
        //kurema:.NET Standard 2.0にはEnumerateRunes()がないのでサロゲートペアを適切に復元するのは面倒。
        foreach (var item in text.EnumerateRunes())
        {
            sb.Append($"&#x{item.Value.ToString("X4")};");
        }
        //if (input.Length > 1) Console.Error.WriteLine($"Length: {input.Length} {sb.ToString()} {input}");
        return sb.ToString();
    }
#endif
}


