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
        Assert.True(parser is Aozora2Html);
        Assert.True(parser is INewMidashiIdProvider);
    }

    [Fact]
    public static void TestLineNumber()
    {
        //kurema:原文では実ファイルとStringIOで別にテストをしていたが、C#でやる必要はないと思う。
        using var sr = new System.IO.StringReader("a\r\nb\r\nc\r\n");
        var parser = new Aozora2Html(new Jstream(sr), new OutputDummy(), null, null, null);

        Assert.Equal(0, parser.line_number);
        Assert.Equal('a', parser.read_char());
        Assert.Equal(1, parser.line_number);
        Assert.Equal('\n', parser.read_char());
        Assert.Equal(1, parser.line_number);
        Assert.Equal('b', parser.read_char());
        Assert.Equal(2, parser.line_number);
        Assert.Equal('\n', parser.read_char());
        Assert.Equal(2, parser.line_number);
        Assert.Equal('c', parser.read_char());
        Assert.Equal(3, parser.line_number);
    }

    [Fact]
    public static void TestReadLine()
    {
        using var sr = new System.IO.StringReader("ab\r\nc\r\n");
        var parser = new Aozora2Html(new Jstream(sr), new OutputDummy(), null, null, null);
        Assert.Equal("ab", parser.read_line());
    }

    [Fact]
    public static void TestCharType()
    {
        Assert.Equal(CharType.Kanji, new EmbedGaiji(Helper.GetAozora2HtmlPlaceholder(), "foo", "1-2-3", "name", string.Empty).char_type);
        Assert.Equal(CharType.Kanji, new UnEmbedGaiji("foo").char_type);
        Assert.Equal(CharType.Hankaku, new Accent(Helper.GetAozora2HtmlPlaceholder(), "123", "abc", string.Empty).char_type);
        Assert.Equal(CharType.Else, new Okurigana("abc").char_type);
        Assert.Equal(CharType.Else, new InlineKeigakomi("abc").char_type);
        Assert.Equal(CharType.Katankana, new DakutenKatakana(1, "abc", string.Empty).char_type);

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
        var @char = parser.read_char();
        Assert.Equal('／', @char);
        Assert.Equal(Aozora2Html.KU, @char);
    }

    [Fact]
    public static void TestIllegalCharCheck()
    {
        var output = new OutputString();
        var result = Utils.illegal_char_check('#', 123, output);
        Assert.Equal("警告(123行目):1バイトの「#」が使われています\r\n", output.ToString());
    }

    [Fact]
    public static void TestIllegalCharCheckSharp()
    {
        var output = new OutputString();
        var result = Utils.illegal_char_check('♯', 123, output);
        Assert.Equal("警告(123行目):注記記号の誤用の可能性がある、「♯」が使われています\r\n", output.ToString());
    }

    [Fact]
    public static void TestIllegalCharCheckNotJis()
    {
        var output = new OutputString();
        var result = Utils.illegal_char_check('①', 123, output);
        Assert.Equal("警告(123行目):JIS外字「①」が使われています\r\n", output.ToString());
    }

    [Fact]
    public static void TestIllegalCharCheckOk()
    {
        var output = new OutputString();
        var result = Utils.illegal_char_check('あ', 123, output);
        Assert.Equal("", output.ToString());
    }

    [Fact]
    public static void TestConvertJanapeseNumber()
    {
        Assert.Equal("3字下げ", Utils.convert_japanese_number("三字下げ"));
        Assert.Equal("10字下げ", Utils.convert_japanese_number("十字下げ"));
        Assert.Equal("12字下げ", Utils.convert_japanese_number("十二字下げ"));
        Assert.Equal("20字下げ", Utils.convert_japanese_number("二十字下げ"));
        Assert.Equal("20字下げ", Utils.convert_japanese_number("二〇字下げ"));
        Assert.Equal("23字下げ", Utils.convert_japanese_number("二十三字下げ"));
        Assert.Equal("2字下げ", Utils.convert_japanese_number("２字下げ"));
    }

    [Fact]
    public static void TestKuten2Png()
    {
        var parser = Helper.GetAozora2HtmlPlaceholder();
        Assert.Equal("<img src=\"../../../gaiji/1-84/1-84-77.png\" alt=\"※(「てへん＋劣」、第3水準1-84-77)\" class=\"gaiji\" />"
            , parser.kuten2png("＃「てへん＋劣」、第3水準1-84-77").to_html());
        Assert.Equal("<img src=\"../../../gaiji/1-02/1-02-22.png\" alt=\"※(二の字点、1-2-22)\" class=\"gaiji\" />"
            , parser.kuten2png("＃二の字点、1-2-22").to_html());
        Assert.Equal("<img src=\"../../../gaiji/1-06/1-06-57.png\" alt=\"※(ファイナルシグマ、1-6-57)\" class=\"gaiji\" />"
            , parser.kuten2png("＃ファイナルシグマ、1-6-57").to_html());
        Assert.Equal("＃「口＋世」、151-23"
            , parser.kuten2png("＃「口＋世」、151-23").to_html());
    }

    [Fact]
    public static void TestTerpri()
    {
        Assert.True(new TextBuffer().terpri());
        Assert.True(new TextBuffer("").terpri());
        Assert.True(new TextBuffer("a").terpri());
        var tag = new MultilineMidashi(new Helper.MidashiIdProviderPlaceholder(0, true), "小", Utils.MidashiType.normal);
        Assert.False(new TextBuffer(new[] { new BufferItemTag(tag) }).terpri());
        Assert.False(new TextBuffer(new[] { new BufferItemTag(tag), new BufferItemTag(tag) }).terpri());
        Assert.False(new TextBuffer(new IBufferItem[] { new BufferItemTag(tag), new BufferItemString("") }).terpri());
        Assert.False(new TextBuffer(new IBufferItem[] { new BufferItemString(""), new BufferItemTag(tag), new BufferItemString("") }).terpri());
        Assert.True(new TextBuffer(new IBufferItem[] { new BufferItemTag(tag), new BufferItemString("a") }).terpri());
        Assert.True(new TextBuffer(new IBufferItem[] { new BufferItemString("a"), new BufferItemTag(tag) }).terpri());
    }

    [Fact]
    public static void TestNewMidashiId()
    {
        var parser = Helper.GetAozora2HtmlPlaceholder();
        var midashi_id = parser.new_midashi_id(1);
        Assert.Equal(midashi_id + 1, parser.new_midashi_id(1));
        Assert.Equal(midashi_id + 2, parser.new_midashi_id("小"));
        Assert.Equal(midashi_id + 12, parser.new_midashi_id("中"));
        Assert.Equal(midashi_id + 112, parser.new_midashi_id("大"));
        bool error = false;
        try
        {
            parser.new_midashi_id("？");
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
        Assert.Equal("x&nbsp;x&nbsp;x&nbsp;x&nbsp;x", Aozora2Html.multiply("x", 5));
    }

    [Fact]
    public static void TestApplyMidashi()
    {
        var parser = Helper.GetAozora2HtmlPlaceholder();
        var midashi = parser.apply_midashi("中見出し");
        Assert.Equal("<h4 class=\"naka-midashi\"><a class=\"midashi_anchor\" id=\"midashi10\">", midashi.to_html());
        midashi = parser.apply_midashi("大見出し");
        Assert.Equal("<h3 class=\"o-midashi\"><a class=\"midashi_anchor\" id=\"midashi110\">", midashi.to_html());
    }

    [Fact]
    public static void TestDetectCommandMode()
    {
        Assert.Equal(Aozora2Html.IndentTypeKey.jisage, Aozora2Html.detect_command_mode("字下げ終わり"));
        Assert.Equal(Aozora2Html.IndentTypeKey.chitsuki, Aozora2Html.detect_command_mode("地付き終わり"));
        Assert.Equal(Aozora2Html.IndentTypeKey.midashi, Aozora2Html.detect_command_mode("中見出し終わり"));
        Assert.Equal(Aozora2Html.IndentTypeKey.futoji, Aozora2Html.detect_command_mode("ここで太字終わり"));
    }

    [Fact]
    public static void TestTcy()
    {
        using var sr = new System.IO.StringReader("［＃ここから５字下げ］\r\n底本： test\r\n");
        var parser = new Aozora2Html(new Jstream(sr), new OutputDummy(), null, null, null);
        string message = string.Empty;
        try
        {
            parser.parse_body();
            parser.parse_body();
            parser.parse_body();
            parser.general_output();
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
            parser.parse_body();
            parser.parse_body();
            parser.parse_body();
            parser.parse_body();
            parser.parse_body();
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
            parser.parse_body();
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
            parser.parse_body();
            parser.parse_body();
            parser.parse_body();
            parser.parse_body();
            parser.parse_body();
            parser.parse_body();
            parser.parse_body();
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
            for (int i = 0; i < 9; i++) parser.parse_body();
        }
        catch (Aozora.Exceptions.AozoraException e)
        {
            message = e.GetMessageAozora();
        }
        Assert.Equal("<div class=\"futoji\">\r\nテスト。<br />\r\n</div>\r\n", output.ToString());
    }
}

