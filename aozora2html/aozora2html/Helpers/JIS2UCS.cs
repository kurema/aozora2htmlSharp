using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Aozora.Helpers;

public static partial class YamlValues
{
    public static string? Jisx0213ToUnicode(string code)
    {
        var codes = code.Split('-').Select(a => int.TryParse(a, out int b) ? b : -1).ToArray();
        return Jisx0213ToUnicode(codes);
    }

    public static string? Jisx0213ToUnicode(int men, int ku, int ten)
    {
        return Jisx0213ToUnicode(new int[] { men, ku, ten });
    }

    private static string? Jisx0213ToUnicode(int[] codes)
    {
        static int VirtualPosToRealPos(int pos)
        {
            //kurema:
            //デカい空白領域を省略する簡易的な圧縮。
            //この値はバイナリに埋め込みたいが、設計上微妙。末尾なら8バイト、先頭なら4バイト必要。どちらも美しくない。
            const int BlankStart = 0x50AA;//kurema:2面16区79点
            const int BlankEnd = 0x7D96;

            switch (pos)
            {
                case >= BlankStart and < BlankEnd:
                    return -1;
                case >= BlankStart:
                    return pos - (BlankEnd - BlankStart);
                default:
                    return pos;
            }
        }

        if (codes.Length < 3) return null;
        if (!(codes[0] is 1 or 2) || !(codes[1] is > 0 and <= 94) || !(codes[2] is > 0 and <= 94)) return null;
        var dic = Jisx0213ToUnicodeBytes;
        if (dic is null) return null;

        int pos = (((codes[0] - 1) * 94 + (codes[1] - 1)) * 94 + codes[2]) * 2;//kurema:ここで-2を加えると2バイト節約できる。その代わりこの領域をヘッダにした。
        pos = VirtualPosToRealPos(pos);
        if (pos < 0) return null;
        if (pos >= dic.Count) return null;

        const string resultHeader = "&#x";
        const string resultFooter = ";";

        if (dic[pos] is >= 0xA0 and <= 0xAF || (dic[pos] == 0x00 && dic[pos + 1] == 0x00))
        {
            return null;
        }
        else if (dic[pos] is >= 0xA0 and <= 0xEF)
        {
            int posRef = (((dic[pos] - 0xB0) << 8) + dic[pos + 1]) * 2;
            posRef = VirtualPosToRealPos(posRef);
            if (posRef < 0) return null;
            if (dic[posRef] == 0xA0)
            {
                return $"{resultHeader}{dic[posRef + 1]:X2}{dic[posRef + 2] & 0x0F:X}{dic[posRef + 3] >> 4:X}{resultFooter}" + $"{resultHeader}{dic[posRef + 3] & 0x0F:X}{dic[posRef + 4] & 0x0F:X}{dic[posRef + 5]:X2}{resultFooter}";
            }
            else if ((dic[posRef] & 0xF0) == 0xA0)//kurema:0xA1-0xAFの場合
            {
                return $"{resultHeader}{dic[posRef] & 0xF:X}{dic[posRef + 1]:X2}{dic[posRef + 3]:X2}{resultFooter}";
            }
            else return null;
        }
        else
        {
            return $"{resultHeader}{dic[pos]:X2}{dic[pos + 1]:X2}{resultFooter}";
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
            using var s1 = Assembly.GetAssembly(typeof(YamlValues))?.GetManifestResourceStream("Aozora.jis2ucs.bin");
            if (s1 is null) throw new Exceptions.FailedToLoadJIS2UCSException();
            var result = new byte[s1.Length];
            s1.Seek(0, SeekOrigin.Begin);
            s1.Read(result, 0, (int)s1.Length);
            return new ReadOnlyCollection<byte>(result);
        }
        catch
        {
            return null;
        }
    }

}


