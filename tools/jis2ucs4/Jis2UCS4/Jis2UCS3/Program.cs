using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System;
using System.Text.Json;

byte[] bytes = new byte[2 * 94 * 94 * 2 + 2 - BlankEnd + BlankStart - 16];

bytes[0] = 0xFE;
bytes[1] = 0xFF;

{
    using var sr = new StreamReader("jis2ucs.yml");
    List<(int, byte[])> tooLongs = new();

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

        int pos = (((num1 - 1) * 94 + (num2 - 1)) * 94 + num3) * 2;
        if (matches.Length == 2)
        {
            if (matches[0].Groups[1].Value.Length != 4 || matches[1].Groups[1].Value.Length != 4) Console.Error.WriteLine("Unexpected char #1.");
            var tooAdd = new byte[6];
            tooAdd[0] = Convert.ToByte("A0", 16);
            tooAdd[1] = Convert.ToByte(matches[0].Groups[1].Value[..2], 16);
            tooAdd[2] = Convert.ToByte(string.Concat("A", matches[0].Groups[1].Value.AsSpan(2, 1)), 16);
            tooAdd[3] = Convert.ToByte(string.Concat(matches[0].Groups[1].Value.AsSpan(3, 1), matches[1].Groups[1].Value.AsSpan(0, 1)), 16);
            tooAdd[4] = Convert.ToByte(string.Concat("A", matches[1].Groups[1].Value.AsSpan(1, 1)), 16);
            tooAdd[5] = Convert.ToByte(matches[1].Groups[1].Value.Substring(2, 2), 16);
            tooLongs.Add((pos, tooAdd));
            bytes[VirtualPosToRealPos(pos)] = 0xFF;
            bytes[VirtualPosToRealPos(pos + 1)] = 0xFF;
        }
        else if (matches[0].Groups[1].Value.Length == 5)
        {
            //ASCIIの01～0Fは全部制御コードなので実際は何の問題もない。
            if (matches[0].Groups[1].Value[0] != '2') Console.Error.WriteLine("Unexpected char #2.");
            var tooAdd = new byte[4];
            tooAdd[0] = Convert.ToByte(string.Concat("A", matches[0].Groups[1].Value.AsSpan(0, 1)), 16);
            tooAdd[1] = Convert.ToByte(matches[0].Groups[1].Value.Substring(1, 2), 16);
            tooAdd[2] = Convert.ToByte(string.Concat("A", matches[0].Groups[1].Value.AsSpan(3, 1)), 16);
            tooAdd[3] = Convert.ToByte(string.Concat(matches[0].Groups[1].Value.AsSpan(4, 1), "0"), 16);
            tooLongs.Add((pos, tooAdd));
            bytes[VirtualPosToRealPos(pos)] = 0xFF;
            bytes[VirtualPosToRealPos(pos + 1)] = 0xFF;
        }
        else if (matches[0].Groups[1].Length == 4)
        {
            bytes[VirtualPosToRealPos(pos)] = Convert.ToByte(matches[0].Groups[1].Value[..2], 16);
            bytes[VirtualPosToRealPos(pos + 1)] = Convert.ToByte(matches[0].Groups[1].Value.Substring(2, 2), 16);
        }
        else
        {
            Console.Error.WriteLine("Unexpected char #3.");
        }
    }

    for (int i = 1; i < bytes.Length / 2; i++)
    {
        int pos = i * 2;
        var (posL, entry) = tooLongs[0];
        for (int j = 0; j < entry.Length; j++)
        {
            if (bytes[RealPosToVirtualPos(pos + j)] != 0)
            {
                //Console.Error.WriteLine($"Not suitable. Continue. {pos:X}");
                goto next;
            }
        }
        checked
        {
            if ((i >> 8) > (0xFF - 0xB0))
            {
                Console.Error.WriteLine($"Pointer size not enogh. {i:X}");
            }
            bytes[VirtualPosToRealPos(posL)] = (byte)((i >> 8) + 0xB0);
            bytes[VirtualPosToRealPos(posL + 1)] = (byte)(i & 0xFF);
            //Console.WriteLine($"{bytes[posL]:X2}{bytes[posL + 1]:X2}");
        }
        for (int j = 0; j < entry.Length; j++)
        {
            bytes[VirtualPosToRealPos(pos + j)] = entry[j];
        }
        i += entry.Length / 2 - 1;
        tooLongs.RemoveAt(0);
        if (tooLongs.Count == 0) break;
        next:;
    }
    if (tooLongs.Count != 0) Console.Error.WriteLine($"Not enough space. {tooLongs.Count}");

    using var fs = new FileStream("jis2ucs.bin", FileMode.Create);
    using var bw = new BinaryWriter(fs);
    bw.Write(bytes);
}

{
    using var sr2 = new StreamReader("jis2ucs.yml");

    while (true)
    {
        var input = sr2.ReadLine();
        if (input is null) break;
        var match = RegexJIS().Match(input);
        if (!match.Success) continue;
        var num1 = int.Parse(match.Groups[1].Value);
        var num2 = int.Parse(match.Groups[2].Value);
        var num3 = int.Parse(match.Groups[3].Value);

        if (!(num1 is 1 or 2) || !(num2 is > 0 and <= 94) || !(num3 is > 0 and <= 94)) Console.Error.WriteLine($"Unexpected kuten. {num1}-{num2}-{num3}");

        var matches = RegexUnicode().Matches(match.Groups[4].Value).ToArray();
        int pos = (((num1 - 1) * 94 + (num2 - 1)) * 94 + num3) * 2;
        pos = VirtualPosToRealPos(pos);

        if (bytes[pos] is >= 0xA0 and <= 0xAF)
        {
            Console.Error.WriteLine($"This should not be empty! {input} {pos:X4} {bytes[pos]:X2}{bytes[pos + 1]:X2}");
        }
        else if (bytes[pos] is >= 0xA0 and <= 0xEF)
        {
            int posRef = (((bytes[pos] - 0xB0) << 8) + bytes[pos + 1]) * 2;
            posRef = VirtualPosToRealPos(posRef);
            if (bytes[posRef] == 0xA0)
            {
                string result1 = $"{bytes[posRef + 1]:X2}{bytes[posRef + 2] & 0x0F:X}{bytes[posRef + 3] >> 4:X}";
                string result2 = $"{bytes[posRef + 3] & 0x0F:X}{bytes[posRef + 4] & 0x0F:X}{bytes[posRef + 5]:X2}";
                if (result1 != matches[0].Groups[1].Value || result2 != matches[1].Groups[1].Value)
                {
                    var span = bytes.AsSpan(posRef);
                    Console.Error.WriteLine($"Wrong code {input} {result1} {result2}");
                }
            }
            else if (bytes[posRef] == 0xA2)
            {
                string result = $"{bytes[posRef] & 0xF:X}{bytes[posRef + 1]:X2}{bytes[posRef + 2] & 0xF:X}{bytes[posRef + 3] >> 4:X}";
                if (result != matches[0].Groups[1].Value)
                {
                    Console.Error.WriteLine($"Wrong code {input} {result}");
                }
            }
            else
            {
                Console.Error.WriteLine($"Unexpected referenced value. {bytes[posRef]:X2}");
            }
        }
        else
        {
            string result = $"{bytes[pos]:X2}{bytes[pos + 1]:X2}";
            if (result != matches[0].Groups[1].Value)
            {
                Console.Error.WriteLine($"Wrong code {input} {result}");
            }
        }
    }
}


partial class Program
{
    [GeneratedRegex(@"^:(\d+)\-(\d+)\-(\d+):\s*""(.+)""$")]
    private static partial Regex RegexJIS();

    [GeneratedRegex(@"&#x([a-fA-F0-9]+)\;")]
    private static partial Regex RegexUnicode();

    const int BlankStart = 0x50B6;
    const int BlankEnd = 0x7D94;
    static int RealPosToVirtualPos(int pos)
    {
        if (pos >= BlankStart) return pos + (BlankEnd - BlankStart);
        else return pos;
    }

    static int VirtualPosToRealPos(int pos)
    {
        if (pos is >= BlankStart and <= BlankEnd)
        {
            throw new ArgumentOutOfRangeException();
        }
        if (pos >= BlankStart) return pos - (BlankEnd - BlankStart);
        else return pos;
    }
}