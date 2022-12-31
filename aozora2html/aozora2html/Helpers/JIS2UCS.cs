using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Aozora.Helpers;

public static partial class YamlValues
{
    public static string? Jisx0213ToUnicode(string code)
    {
        var codes = code.Split('-').Select(a => int.TryParse(a, out int b) ? b : -1).ToArray();
        if (codes.Length < 3) return null;
        if (!(codes[0] is 1 or 2) || !(codes[1] is > 0 and <= 94) || !(codes[2] is > 0 and <= 94)) return null;
        if (Jisx0213ToUnicodeBytes is null) return null;
        var dic = Jisx0213ToUnicodeBytes;
        int basepos = (((codes[0] - 1) * 94 + (codes[1] - 1)) * 94 + codes[2]) * 4;
        if (basepos + 4 >= Jisx0213ToUnicodeBytes.Count) return null;
        var entry = (dic[basepos], dic[basepos + 1], dic[basepos + 2], dic[basepos + 3]);
        if (entry.Item1 == 0 && entry.Item2 == 0)
        {
            if (entry.Item3 == 0 && entry.Item4 == 0) return null;
            return $"&#x{entry.Item4.ToString("X2")}{entry.Item3.ToString("X2")};";
        }
        if (entry.Item2==0 && entry.Item1 <= 0xF)
        {
            return $"&#x{entry.Item1.ToString("X")}{entry.Item4.ToString("X2")}{entry.Item3.ToString("X2")};";
        }
        {
            return $"&#x{entry.Item2.ToString("X2")}{entry.Item1.ToString("X2")};&#x{entry.Item4.ToString("X2")}{entry.Item3.ToString("X2")};";
        }
    }

    private static ReadOnlyCollection<byte>? _Jisx0213ToUnicodeBytes;
    public static ReadOnlyCollection<byte>? Jisx0213ToUnicodeBytes { get { return _Jisx0213ToUnicodeBytes ??= LoadJisx0213ToUnicodeBytes(); } }

    private static ReadOnlyCollection<byte>? LoadJisx0213ToUnicodeBytes()
    {
        //kurema:これは単一ファイル構成でも使えるのか？
        //kurema:Resourceと同じ仕組みなんだから使えるに決まってるわ。
        try
        {
            using var s1 = Assembly.GetAssembly(typeof(YamlValues)).GetManifestResourceStream("Aozora.jis2ucs.gz");
            if (s1 is null) throw new Exceptions.FailedToLoadJIS2UCSException();
            using var sgz = new System.IO.Compression.GZipStream(s1, System.IO.Compression.CompressionMode.Decompress);
            using var ms = new MemoryStream();
            sgz.CopyTo(ms);
            var result = new byte[ms.Length];
            ms.Seek(0, SeekOrigin.Begin);
            ms.Read(result, 0, (int)ms.Length);
            return new ReadOnlyCollection<byte>(result);
        }
        catch
        {
            return null;
        }
    }

}


