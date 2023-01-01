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

public static class UnitTestAozora2Html
{
    [Fact]
    public static void TestNew()
    {
        var parser = Helper.GetAozora2HtmlPlaceholder();
#pragma warning disable IDE0150 // 型のチェックよりも 'null 値' チェックを優先する
        Assert.True(parser is Aozora2Html);
#pragma warning restore IDE0150 // 型のチェックよりも 'null 値' チェックを優先する
        Assert.True(parser is INewMidashiIdProvider);
    }

    [Fact]
    public static void TestLineNumber()
    {
        //kurema:原文では実ファイルとStringIOで別にテストをしていたが、C#でやる必要はないと思う。
        using var sr = new System.IO.StringReader("a\r\nb\r\nc\r\n");
        var parser = new Aozora2Html(new Jstream(sr), new OutputDummy(), null, null, null);

        Assert.Equal(0, parser.LineNumber);
        Assert.Equal('a', parser.ReadChar());
        Assert.Equal(1, parser.LineNumber);
        Assert.Equal('\n', parser.ReadChar());
        Assert.Equal(1, parser.LineNumber);
        Assert.Equal('b', parser.ReadChar());
        Assert.Equal(2, parser.LineNumber);
        Assert.Equal('\n', parser.ReadChar());
        Assert.Equal(2, parser.LineNumber);
        Assert.Equal('c', parser.ReadChar());
        Assert.Equal(3, parser.LineNumber);
    }

    [Fact]
    public static void TestReadLine()
    {
        using var sr = new System.IO.StringReader("ab\r\nc\r\n");
        var parser = new Aozora2Html(new Jstream(sr), new OutputDummy(), null, null, null);
        Assert.Equal("ab", parser.ReadLine().ToString());
    }

    [Fact]
    public static void TestCharType()
    {
        Assert.Equal(CharType.Kanji, new EmbedGaiji(Helper.GetAozora2HtmlPlaceholder(), "foo", "1-2-3", "name", string.Empty).CharType);
        Assert.Equal(CharType.Kanji, new UnEmbedGaiji("foo").CharType);
        Assert.Equal(CharType.Hankaku, new Accent(Helper.GetAozora2HtmlPlaceholder(), "123", "abc", string.Empty).CharType);
        Assert.Equal(CharType.Else, new Okurigana("abc").CharType);
        Assert.Equal(CharType.Else, new InlineKeigakomi("abc").CharType);
        Assert.Equal(CharType.Katankana, new DakutenKatakana(1, "abc", string.Empty).CharType);

        Assert.Equal(CharType.Hiragana, Utils.GetCharType('あ'));
        Assert.Equal(CharType.Hiragana, Utils.GetCharType('っ'));
        Assert.Equal(CharType.Katankana, Utils.GetCharType('ヴ'));
        Assert.Equal(CharType.Katankana, Utils.GetCharType('ー'));
        Assert.Equal(CharType.Zenkaku, Utils.GetCharType('Ａ'));
        Assert.Equal(CharType.Zenkaku, Utils.GetCharType('ｗ'));
        Assert.Equal(CharType.Hankaku, Utils.GetCharType('z'));
        Assert.Equal(CharType.Kanji, Utils.GetCharType('漢'));
        Assert.Equal(CharType.HankakuTerminate, Utils.GetCharType('!'));
        Assert.Equal(CharType.Else, Utils.GetCharType('？'));
        Assert.Equal(CharType.Else, Utils.GetCharType('Å'));
    }

    [Fact]
    public static void TestReadChar()
    {
        using var sr = new System.IO.StringReader("／＼\r\n");
        var parser = new Aozora2Html(new Jstream(sr), new OutputDummy(), null, null, null);
        var @char = parser.ReadChar();
        Assert.Equal('／', @char);
        Assert.Equal(Aozora2Html.KU, @char);
    }

    [Fact]
    public static void TestIllegalCharCheck()
    {
        var output = new OutputString();
        Utils.IllegalCharCheck('#', 123, output);
        Assert.Equal("警告(123行目):1バイトの「#」が使われています\r\n", output.ToString());
    }

    [Fact]
    public static void TestIllegalCharCheckSharp()
    {
        var output = new OutputString();
        Utils.IllegalCharCheck('♯', 123, output);
        Assert.Equal("警告(123行目):注記記号の誤用の可能性がある、「♯」が使われています\r\n", output.ToString());
    }

    [Fact]
    public static void TestIllegalCharCheckNotJis()
    {
        var output = new OutputString();
        Utils.IllegalCharCheck('①', 123, output);
        Assert.Equal("警告(123行目):JIS外字「①」が使われています\r\n", output.ToString());
    }

    [Fact]
    public static void TestIllegalCharCheckOk()
    {
        var output = new OutputString();
        Utils.IllegalCharCheck('あ', 123, output);
        Assert.Equal("", output.ToString());
    }

    [Fact]
    public static void TestConvertJanapeseNumber()
    {
        Assert.Equal("3字下げ", Utils.ConvertJapaneseNumber("三字下げ"));
        Assert.Equal("10字下げ", Utils.ConvertJapaneseNumber("十字下げ"));
        Assert.Equal("12字下げ", Utils.ConvertJapaneseNumber("十二字下げ"));
        Assert.Equal("20字下げ", Utils.ConvertJapaneseNumber("二十字下げ"));
        Assert.Equal("20字下げ", Utils.ConvertJapaneseNumber("二〇字下げ"));
        Assert.Equal("23字下げ", Utils.ConvertJapaneseNumber("二十三字下げ"));
        Assert.Equal("2字下げ", Utils.ConvertJapaneseNumber("２字下げ"));
    }

    [Fact]
    public static void TestKuten2Png()
    {
        var parser = Helper.GetAozora2HtmlPlaceholder();
        Assert.Equal("<img src=\"../../../gaiji/1-84/1-84-77.png\" alt=\"※(「てへん＋劣」、第3水準1-84-77)\" class=\"gaiji\" />"
            , parser.KutenToPng("＃「てへん＋劣」、第3水準1-84-77").ToHtml());
        Assert.Equal("<img src=\"../../../gaiji/1-02/1-02-22.png\" alt=\"※(二の字点、1-2-22)\" class=\"gaiji\" />"
            , parser.KutenToPng("＃二の字点、1-2-22").ToHtml());
        Assert.Equal("<img src=\"../../../gaiji/1-06/1-06-57.png\" alt=\"※(ファイナルシグマ、1-6-57)\" class=\"gaiji\" />"
            , parser.KutenToPng("＃ファイナルシグマ、1-6-57").ToHtml());
        Assert.Equal("＃「口＋世」、151-23"
            , parser.KutenToPng("＃「口＋世」、151-23").ToHtml());
    }

    [Fact]
    public static void TestTerpri()
    {
        Assert.True(new TextBuffer().Terpri());
        Assert.True(new TextBuffer("").Terpri());
        Assert.True(new TextBuffer("a").Terpri());
        var tag = new MultilineMidashi(new Helper.MidashiIdProviderPlaceholder(0, true), "小", Utils.MidashiType.normal);
        Assert.False(new TextBuffer(new[] { new BufferItemTag(tag) }).Terpri());
        Assert.False(new TextBuffer(new[] { new BufferItemTag(tag), new BufferItemTag(tag) }).Terpri());
        Assert.False(new TextBuffer(new IBufferItem[] { new BufferItemTag(tag), new BufferItemString("") }).Terpri());
        Assert.False(new TextBuffer(new IBufferItem[] { new BufferItemString(""), new BufferItemTag(tag), new BufferItemString("") }).Terpri());
        Assert.True(new TextBuffer(new IBufferItem[] { new BufferItemTag(tag), new BufferItemString("a") }).Terpri());
        Assert.True(new TextBuffer(new IBufferItem[] { new BufferItemString("a"), new BufferItemTag(tag) }).Terpri());
    }

    [Fact]
    public static void TestNewMidashiId()
    {
        var parser = Helper.GetAozora2HtmlPlaceholder();
        var midashi_id = parser.GenerateNewMidashiId(1);
        Assert.Equal(midashi_id + 1, parser.GenerateNewMidashiId(1));
        Assert.Equal(midashi_id + 2, parser.GenerateNewMidashiId("小"));
        Assert.Equal(midashi_id + 12, parser.GenerateNewMidashiId("中"));
        Assert.Equal(midashi_id + 112, parser.GenerateNewMidashiId("大"));
        bool error = false;
        try
        {
            parser.GenerateNewMidashiId("？");
        }
        catch
        {
            error = true;
        }
        Assert.True(error);
    }

    [Fact]
    public static void TestMultiply()
    {
        Assert.Equal("x&nbsp;x&nbsp;x&nbsp;x&nbsp;x", Aozora2Html.GetMultipliedText("x", 5));
    }

    [Fact]
    public static void TestApplyMidashi()
    {
        var parser = Helper.GetAozora2HtmlPlaceholder();
        var midashi = parser.ApplyMidashi("中見出し");
        Assert.Equal("<h4 class=\"naka-midashi\"><a class=\"midashi_anchor\" id=\"midashi10\">", midashi.ToHtml());
        midashi = parser.ApplyMidashi("大見出し");
        Assert.Equal("<h3 class=\"o-midashi\"><a class=\"midashi_anchor\" id=\"midashi110\">", midashi.ToHtml());
    }

    [Fact]
    public static void TestDetectCommandMode()
    {
        Assert.Equal(Aozora2Html.IndentTypeKey.jisage, Aozora2Html.DetectCommandMode("字下げ終わり"));
        Assert.Equal(Aozora2Html.IndentTypeKey.chitsuki, Aozora2Html.DetectCommandMode("地付き終わり"));
        Assert.Equal(Aozora2Html.IndentTypeKey.midashi, Aozora2Html.DetectCommandMode("中見出し終わり"));
        Assert.Equal(Aozora2Html.IndentTypeKey.futoji, Aozora2Html.DetectCommandMode("ここで太字終わり"));
    }

    [Fact]
    public static void TestTcy()
    {
        using var sr = new System.IO.StringReader("［＃ここから５字下げ］\r\n底本： test\r\n");
        var parser = new Aozora2Html(new Jstream(sr), new OutputDummy(), null, null, null);
        string message = string.Empty;
        try
        {
            parser.ParseBody();
            parser.ParseBody();
            parser.ParseBody();
            parser.GeneralOutput();
        }
        catch (Aozora.Exceptions.TerminateInStyleException e)
        {
            message = e.Message;
        }
        Assert.Equal("字下げ中に本文が終了しました", message);
    }

    [Fact]
    public static void TestEndingCheck()
    {
        using var sr = new System.IO.StringReader("本文\r\n\r\n底本：test\r\n");
        var output = new OutputString();
        var parser = new Aozora2Html(new Jstream(sr), output, null, null, null);
        string message = string.Empty;
        try
        {
            parser.ParseBody();
            parser.ParseBody();
            parser.ParseBody();
            parser.ParseBody();
            parser.ParseBody();
        }
        catch (Aozora.Exceptions.AozoraException e)
        {
            message = e.Message;
        }
        Assert.Equal("本文<br />\r\n<br />\r\n</div>\r\n<div class=\"bibliographical_information\">\r\n<hr />\r\n<br />\r\n", output.ToString());
    }

    [Fact]
    public static void TestInvalidClosing()
    {
        using var sr = new System.IO.StringReader("［＃ここで太字終わり］\r\n");
        var parser = new Aozora2Html(new Jstream(sr), new OutputDummy(), null, null, null);
        string message = string.Empty;
        try
        {
            parser.ParseBody();
        }
        catch (Aozora.Exceptions.AozoraException e)
        {
            message = e.GetMessageAozora();
        }
        Assert.Equal("エラー(0行目):太字を閉じようとしましたが、太字中ではありません. \r\n処理を停止します", message);
    }

    [Fact]
    public static void TestInvalidNest()
    {
        using var sr = new System.IO.StringReader("［＃太字］［＃傍線］あ［＃太字終わり］\r\n");
        var parser = new Aozora2Html(new Jstream(sr), new OutputDummy(), null, null, null);
        string message = string.Empty;
        try
        {
            parser.ParseBody();
            parser.ParseBody();
            parser.ParseBody();
            parser.ParseBody();
            parser.ParseBody();
            parser.ParseBody();
            parser.ParseBody();
        }
        catch (Aozora.Exceptions.AozoraException e)
        {
            message = e.GetMessageAozora();
        }
        Assert.Equal("エラー(0行目):太字を終了しようとしましたが、傍線中です. \r\n処理を停止します", message);
    }

    [Fact]
    public static void TestCommandDo()
    {
        using var sr = new System.IO.StringReader("［＃ここから太字］\r\nテスト。\r\n［＃ここで太字終わり］\r\n");
        var output = new OutputString();
        var parser = new Aozora2Html(new Jstream(sr), output, null, null, null);
        string message = string.Empty;
        try
        {
            for (int i = 0; i < 9; i++) parser.ParseBody();
        }
        catch (Aozora.Exceptions.AozoraException e)
        {
            message = e.GetMessageAozora();
        }
        Assert.Equal("<div class=\"futoji\">\r\nテスト。<br />\r\n</div>\r\n", output.ToString());
    }
}

