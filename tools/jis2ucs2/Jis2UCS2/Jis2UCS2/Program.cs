﻿using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System;

Console.WriteLine("""
using System;
using System.Collections.Generic;
using System.Text;

namespace Aozora.External;

public static partial class YamlValues
{
    public static string? JIS2UCS(int num1, int num2, int num3)
    {
        switch (num1)
        {
""");

int lastnum1 = -1, lastnum2 = -1;
while (true)
{
    var input = await Console.In.ReadLineAsync();
    if (input is null) break;
    var match = RegexJIS().Match(input);
    if (!match.Success) continue;
    var num1 = int.Parse(match.Groups[1].Value);
    var num2 = int.Parse(match.Groups[2].Value);
    var num3 = int.Parse(match.Groups[3].Value);
    if (lastnum1 != num1)
    {
        if (lastnum1 != -1)
        {
            Console.WriteLine($"                }}");
            Console.WriteLine($"                break;");
            Console.WriteLine($"            }}");
            Console.WriteLine($"            break;");
        }
        Console.WriteLine($"            case {num1}:");
        Console.WriteLine($"                switch (num2)");
        Console.WriteLine($"                {{");
        Console.WriteLine($"                    case {num2}:");
        Console.WriteLine($"                    switch (num3)");
        Console.WriteLine($"                    {{");
    }
    else if (lastnum2 != num2)
    {
        if (lastnum1 != -1)
        {
            Console.WriteLine($"                }}");
            Console.WriteLine($"                break;");
        }
        Console.WriteLine($"                    case {num2}:");
        Console.WriteLine($"                    switch (num3)");
        Console.WriteLine($"                    {{");
    }
    //Console.WriteLine($"                        case {num3}: return \"{Convert2UnicodeCS(match.Groups[4].Value)}\";");
    Console.WriteLine($"                        case {num3}: return \"{(match.Groups[4].Value)}\"; // {input}");

    lastnum1 = num1;
    lastnum2 = num2;
}

Console.WriteLine($"                }}");
Console.WriteLine($"                break;");
Console.WriteLine($"            }}");
Console.WriteLine($"            break;");
Console.WriteLine($"        }}");
Console.WriteLine($"        return null;");
Console.WriteLine($"    }}");
Console.WriteLine($"}}");


partial class Program
{
    [GeneratedRegex(@"^:(\d+)\-(\d+)\-(\d+):\s*""(.+)""$")]
    private static partial Regex RegexJIS();


    [GeneratedRegex(@"&#x([a-fA-F0-9]+)\;")]
    private static partial Regex RegexUnicode();

    private static string Convert2UnicodeCS(string input)
    {
        var matches = RegexUnicode().Matches(input);
        var result = new StringBuilder();
        foreach (Match item in matches)
        {
            var uni = $@"\u{item.Groups[1].Value}";
            result.Append(uni);
        }

        {
            string retra = Regex.Replace(Escape(Regex.Unescape(result.ToString())), @"\\u([a-z0-9A-Z]+)", "&#$1;");
            if (retra != input)
            {
                Console.Error.WriteLine($@"""{retra}"" != ""{input}""");
            }

        }
        return result.ToString();
    }

    private static string Escape(string input)
    {
        var sb=new StringBuilder();
        foreach(var item in input.EnumerateRunes())
        {
            sb.Append($"&#x{item.Value.ToString("x4").ToUpperInvariant()};");
        }
        return sb.ToString();
    }
}