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

public static class UnitTestTagParser
{
    [Fact]
    public static void TestParseKatakana()
    {
        using var sr = new System.IO.StringReader("テスト！あいうえお\r\n");
        var stream = new Jstream(sr);
        var (command, _) = new TagParser(stream, '！', new(), new(), new OutputDummy()).processTag();
        Assert.Equal("テスト", command);
    }

    [Fact]
    public static void TestParseBouten()
    {
        using var sr = new System.IO.StringReader("腹がへっても［＃「腹がへっても」に傍点］、ひもじゅうない［＃「ひもじゅうない」に傍点］とかぶりを振っている…\r\n");
        var stream = new Jstream(sr);
        var (command, _) = new TagParser(stream, '…', new(), new(), new OutputDummy()).processTag();
        Assert.Equal("<em class=\"sesame_dot\">腹がへっても</em>、<em class=\"sesame_dot\">ひもじゅうない</em>とかぶりを振っている", command);
    }

    [Fact]
    public static void TestParseGaijiA()
    {
        using var sr = new System.IO.StringReader("※［＃「口＋世」、ページ数-行数］…\r\n");
        var stream = new Jstream(sr);
        var (command, _) = new TagParser(stream, '…', new(), new(), new OutputDummy(), gaiji_dir: "g_dir/").processTag();
        Assert.Equal("※<span class=\"notes\">［＃「口＋世」、ページ数-行数］</span>", command);
    }

    [Fact]
    public static void TestParseGaijiB()
    {
        using var sr = new System.IO.StringReader("※［＃二の字点、1-2-22］…\r\n");
        var stream = new Jstream(sr);
        var (command, _) = new TagParser(stream, '…', new(), new(), new OutputDummy(), gaiji_dir: "g_dir/").processTag();
        Assert.Equal("<img src=\"g_dir/1-02/1-02-22.png\" alt=\"※(二の字点、1-2-22)\" class=\"gaiji\" />", command);
    }

    [Fact]
    public static void TestParseGaijiKaeri()
    {
        using var sr = new System.IO.StringReader("自［＃二］女王國［＃一］東度［＃レ］海千餘里。…\r\n");
        var stream = new Jstream(sr);
        var (command, _) = new TagParser(stream, '…', new(), new(), new OutputDummy(), gaiji_dir: "g_dir/").processTag();
        Assert.Equal("自<sub class=\"kaeriten\">二</sub>女王國<sub class=\"kaeriten\">一</sub>東度<sub class=\"kaeriten\">レ</sub>海千餘里。", command);
    }

    [Fact]
    public static void TestParseGaijiJisx0213()
    {
        using var sr = new System.IO.StringReader("※［＃「てへん＋劣」、第3水準1-84-77］…\r\n");
        var stream = new Jstream(sr);
        var (command, _) = new TagParser(stream, '…', new(), new(), new OutputDummy(), gaiji_dir: "g_dir/") { use_jisx0214_embed_gaiji = true }.processTag();
        Assert.Equal("&#x6318;", command);
    }
}

