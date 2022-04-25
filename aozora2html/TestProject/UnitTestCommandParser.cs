﻿using System;
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

    [Fact]
    public static void TestParseCommand3()
    {
        var src = "〔Sito^t qu'on le touche il re'sonne.〕\r\n";
        var parsed = ParseText(src);
        const string expected = "Sit<img src=\"../../../gaiji/1-09/1-09-74.png\" alt=\"※(サーカムフレックスアクセント付きO小文字)\" class=\"gaiji\" />t q<img src=\"../../../gaiji/1-09/1-09-79.png\" alt=\"※(アキュートアクセント付きU小文字)\" class=\"gaiji\" />on le touche il r<img src=\"../../../gaiji/1-09/1-09-63.png\" alt=\"※(アキュートアクセント付きE小文字)\" class=\"gaiji\" />sonne.<br />\r\n";
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public static void TestParseCommand4()
    {
        var src = "presqu'〔i^le〕\r\n";
        var parsed = ParseText(src);
        const string expected = "presqu'<img src=\"../../../gaiji/1-09/1-09-68.png\" alt=\"※(サーカムフレックスアクセント付きI小文字)\" class=\"gaiji\" />le<br />\r\n";
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public static void TestParseCommand5()
    {
        var src = "［二十歳の 〔E'tude〕］\r\n";
        var parsed = ParseText(src);
        const string expected = "［二十歳の <img src=\"../../../gaiji/1-09/1-09-32.png\" alt=\"※(アキュートアクセント付きE)\" class=\"gaiji\" />tude］<br />\r\n";
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public static void TestParseCommand6()
    {
        var src = "責［＃「責」に白ゴマ傍点］空文庫\r\n";
        var parsed = ParseText(src);
        const string expected = "<em class=\"white_sesame_dot\">責</em>空文庫<br />\r\n";
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public static void TestParseCommand7()
    {
        var src = "［＃丸傍点］青空文庫で読書しよう［＃丸傍点終わり］。\r\n";
        var parsed = ParseText(src);
        const string expected = "<em class=\"black_circle\">青空文庫で読書しよう</em>。<br />\r\n";
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public static void TestParseCommand8()
    {
        var src = "この形は傍線［＃「傍線」に傍線］と書いてください。\r\n";
        var parsed = ParseText(src);
        const string expected = "この形は<em class=\"underline_solid\">傍線</em>と書いてください。<br />\r\n";
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public static void TestParseCommand9()
    {
        var src = "［＃左に鎖線］青空文庫で読書しよう［＃左に鎖線終わり］。\r\n";
        var parsed = ParseText(src);
        const string expected = "<em class=\"overline_dotted\">青空文庫で読書しよう</em>。<br />\r\n";
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public static void TestParseCommand10()
    {
        var src = "「クリス、宇宙航行委員会が選考［＃「選考」は太字］するんだ。きみは志願できない。待つ［＃「待つ」は太字］んだ」\r\n";
        var parsed = ParseText(src);
        const string expected = "「クリス、宇宙航行委員会が<span class=\"futoji\">選考</span>するんだ。きみは志願できない。<span class=\"futoji\">待つ</span>んだ」<br />\r\n";
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public static void TestParseCommand11()
    {
        var src = "Which, teaching us, hath this exordium: Nothing from nothing ever yet was born.［＃「Nothing from nothing ever yet was born.」は斜体］\r\n";
        var parsed = ParseText(src);
        const string expected = "Which, teaching us, hath this exordium: <span class=\"shatai\">Nothing from nothing ever yet was born.</span><br />\r\n";
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public static void TestParseCommandWarichu()
    {
        var src = "［＃割り注］価は四百円であった。［＃割り注終わり］\r\n";
        var parsed = ParseText(src);
        const string expected = "<span class=\"warichu\">（価は四百円であった。）</span><br />\r\n";
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public static void TestParseCommandWarichu2()
    {
        var src = "飽海郡南平田村大字飛鳥［＃割り注］東は字大林四三七［＃改行］西は字神内一一一ノ一［＃割り注終わり］\r\n";
        var parsed = ParseText(src);
        const string expected = "飽海郡南平田村大字飛鳥<span class=\"warichu\">（東は字大林四三七<span class=\"notes\">［＃改行］</span>西は字神内一一一ノ一）</span><br />\r\n";
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public static void TestParseCommandUnicode()
    {
        var src = "※［＃「衄のへん＋卩」、U+5379、287-2］\r\n";
        var parsed = ParseText(src, a => a.use_unicode_embed_gaiji = true);
        const string expected = "&#x5379;<br />\r\n";
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public static void TestParseCommandTeisei1()
    {
        var src = "吹喋［＃「喋」に「ママ」の注記］\r\n";
        var parsed = ParseText(src, a => a.use_unicode_embed_gaiji = true);
        const string expected = "吹<ruby><rb>喋</rb><rp>（</rp><rt>ママ</rt><rp>）</rp></ruby><br />\r\n";
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public static void TestParseCommandTeisei2()
    {
        var src = "紋附だとか［＃「紋附だとか」は底本では「絞附だとか」］\r\n";
        var parsed = ParseText(src, a => a.use_unicode_embed_gaiji = true);
        const string expected = "紋附だとか<span class=\"notes\">［＃「紋附だとか」は底本では「絞附だとか」］</span><br />\r\n";
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public static void TestParseCommandTeisei3()
    {
        var src = "私は籠《ざる》［＃ルビの「ざる」は底本では「さる」］をさげ\r\n";
        var parsed = ParseText(src, a => a.use_unicode_embed_gaiji = true);
        const string expected = "私は<ruby><rb>籠</rb><rp>（</rp><rt>ざる</rt><rp>）</rp></ruby><span class=\"notes\">［＃ルビの「ざる」は底本では「さる」］</span>をさげ<br />\r\n";
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public static void TestParseCommandTeisei4()
    {
        var src = "広場へに［＃「広場へに」はママ］店でもだそう。\r\n";
        var parsed = ParseText(src, a => a.use_unicode_embed_gaiji = true);
        const string expected = "広場へに<span class=\"notes\">［＃「広場へに」はママ］</span>店でもだそう。<br />\r\n";
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public static void TestParseCommandTeisei5()
    {
        var src = "お湯《ゆう》［＃ルビの「ゆう」はママ］\r\n";
        var parsed = ParseText(src, a => a.use_unicode_embed_gaiji = true);
        const string expected = "お<ruby><rb>湯</rb><rp>（</rp><rt>ゆう</rt><rp>）</rp></ruby><span class=\"notes\">［＃ルビの「ゆう」はママ］</span><br />\r\n";
        Assert.Equal(expected, parsed);
    }

    public static string? ParseText(string input, Action<Aozora2Html>? action = null)
    {
        using var sr = new System.IO.StringReader(input);
        var stream = new Jstream(sr);
        var output = new OutputString();
        var parser = new Aozora2Html(stream, output, null, null, null) { section = Aozora2Html.SectionKind.tail };
        action?.Invoke(parser);
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
