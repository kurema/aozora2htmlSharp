using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System;
using System.Text.Json;

ushort[] shorts = new ushort[2 * 94 * 94 * 2 + 2];

shorts[0] = 0xFFFE;//実際はUTF16文章ではない。

using var sr = new StreamReader("jis2ucs.yml");

while (true)
{
    //var input = Console.In.ReadLine();
    var input = sr.ReadLine();
    if (input is null) break;
    var match = RegexJIS().Match(input);
    if (!match.Success) continue;
    var num1 = int.Parse(match.Groups[1].Value);
    var num2 = int.Parse(match.Groups[2].Value);
    var num3 = int.Parse(match.Groups[3].Value);

    if (!(num1 is 1 or 2) || !(num2 is > 0 and <= 94) || !(num3 is > 0 and <= 94)) Console.Error.WriteLine($"Unexpected kuten. {num1}-{num2}-{num3}");

    var matches = RegexUnicode().Matches(match.Groups[4].Value).ToArray();
    if (matches.Length > 2) Console.Error.WriteLine("Too many chars.");
    if (matches.Length == 0) Console.Error.WriteLine("No chars.");

    ushort result1 = 0xFFFF;
    ushort result2 = 0xFFFF;

    if (matches.Length == 2)
    {
        if (matches[0].Groups[1].Value.Length != 4 || matches[1].Groups[1].Value.Length != 4) Console.Error.WriteLine("Unexpected char #1.");
        result1 = Convert.ToUInt16(matches[0].Groups[1].Value, 16);
        result2 = Convert.ToUInt16(matches[1].Groups[1].Value, 16);

    }
    else if (matches[0].Groups[1].Value.Length == 5)
    {
        //ASCIIの01～0Fは全部制御コードなので実際は何の問題もない。
        if (matches[0].Groups[1].Value[0] != '2') Console.Error.WriteLine("Unexpected char #2.");
        result1 = Convert.ToUInt16(new string(matches[0].Groups[1].Value[0], 1), 16);
        result2 = Convert.ToUInt16(matches[0].Groups[1].Value.Substring(1), 16);
    }
    else if (matches[0].Groups[1].Length == 4)
    {
        result1 = 0;
        result2 = Convert.ToUInt16(matches[0].Groups[1].Value, 16);
    }
    else
    {
        Console.Error.WriteLine("Unexpected char #3.");
    }
    shorts[(((num1 - 1) * 94 + (num2 - 1)) * 94 + num3) * 2] = result1;
    shorts[(((num1 - 1) * 94 + (num2 - 1)) * 94 + num3) * 2 + 1] = result2;
}

using var fs = new FileStream("jis2ucs.bin", FileMode.OpenOrCreate);
using var bw = new BinaryWriter(fs);
var bytes = shorts.SelectMany(x => BitConverter.GetBytes(x)).ToArray();
bw.Write(bytes);

partial class Program
{
    [GeneratedRegex(@"^:(\d+)\-(\d+)\-(\d+):\s*""(.+)""$")]
    private static partial Regex RegexJIS();

    [GeneratedRegex(@"&#x([a-fA-F0-9]+)\;")]
    private static partial Regex RegexUnicode();
}