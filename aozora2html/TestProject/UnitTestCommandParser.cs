using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using Aozora;
using Aozora.Helpers;
using Aozora.Helpers.Tag;

namespace TestProject;

public static class UnitTestCommandParser
{
    [Fact]
    public static void TestParseCommand1()
    {
        var src = "デボルド―※［＃濁点付き片仮名ワ、1-7-82］ルモオル\r\n";
        var parsed = ParseText(src);
        const string expected = "デボルド―<img src=\"../../../gaiji/1-07/1-07-82.png\" alt=\"※(濁点付き片仮名ワ、1-7-82)\" class=\"gaiji\" />ルモオル<br />\r\n";
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public static void TestParseCommand2()
    {
        var src = "繁雑な日本の 〔e'tiquette〕 も、\r\n";
        var parsed = ParseText(src);
        const string expected = "繁雑な日本の <img src=\"../../../gaiji/1-09/1-09-63.png\" alt=\"※(アキュートアクセント付きE小文字)\" class=\"gaiji\" />tiquette も、<br />\r\n";
        Assert.Equal(expected, parsed);
    }

    public static string? ParseText(string input)
    {
        using var sr = new System.IO.StringReader(input);
        var stream = new Jstream(sr);
        var output = new OutputString();
        var parser = new Aozora2Html(stream, output, null, null, null) { section = Aozora2Html.SectionKind.tail };
        try
        {
            while (true) parser.parse();
        }
        catch (Aozora.Exceptions.TerminateException)
        {
        }

        return output.ToString();
    }
}
