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

public static class UnitTestRubyParse
{
    [Fact]
    public static void TestParseRuby1()
    {
        const string src = "青空文庫《あおぞらぶんこ》\r\n";
        const string expected = "<ruby><rb>青空文庫</rb><rp>（</rp><rt>あおぞらぶんこ</rt><rp>）</rp></ruby><br />\r\n";
        var parsed = Helper.ParseText(src);
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public static void TestParseRuby1b()
    {
        const string src = "身装《みなり》\r\n";
        const string expected = "<ruby><rb>身装</rb><rp>（</rp><rt>みなり</rt><rp>）</rp></ruby><br />\r\n";
        var parsed = Helper.ParseText(src);
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public static void TestParseRuby2()
    {
        const string src = "霧の｜ロンドン警視庁《スコットランドヤード》\r\n";
        const string expected = "霧の<ruby><rb>ロンドン警視庁</rb><rp>（</rp><rt>スコットランドヤード</rt><rp>）</rp></ruby><br />\r\n";
        var parsed = Helper.ParseText(src);
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public static void TestParseRuby2b()
    {
        const string src = "いかにも最｜猛者《もさ》のように\r\n";
        const string expected = "いかにも最<ruby><rb>猛者</rb><rp>（</rp><rt>もさ</rt><rp>）</rp></ruby>のように<br />\r\n";
        var parsed = Helper.ParseText(src);
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public static void TestParseRuby3()
    {
        const string src = "〆切《しめきり》を逃れるために、市ヶ谷《いちがや》から転々《てんてん》と、居を移した。\r\n";
        const string expected = "<ruby><rb>〆切</rb><rp>（</rp><rt>しめきり</rt><rp>）</rp></ruby>を逃れるために、<ruby><rb>市ヶ谷</rb><rp>（</rp><rt>いちがや</rt><rp>）</rp></ruby>から<ruby><rb>転々</rb><rp>（</rp><rt>てんてん</rt><rp>）</rp></ruby>と、居を移した。<br />\r\n";
        var parsed = Helper.ParseText(src);
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public static void TestParseRuby4()
    {
        const string src = "水鉢を置いた※［＃「木＋靈」、第3水準1-86-29］子窓《れんじまど》の下には\r\n";
        const string expected = "水鉢を置いた<ruby><rb><img src=\"../../../gaiji/1-86/1-86-29.png\" alt=\"※(「木＋靈」、第3水準1-86-29)\" class=\"gaiji\" />子窓</rb><rp>（</rp><rt>れんじまど</rt><rp>）</rp></ruby>の下には<br />\r\n";
        var parsed = Helper.ParseText(src);
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public static void TestParseRuby5()
    {
        const string src = "それが彼の 〔E'tude〕《エチュード》 だった。\r\n";
        const string expected = "それが彼の <ruby><rb><img src=\"../../../gaiji/1-09/1-09-32.png\" alt=\"※(アキュートアクセント付きE)\" class=\"gaiji\" />tude</rb><rp>（</rp><rt>エチュード</rt><rp>）</rp></ruby> だった。<br />\r\n";
        var parsed = Helper.ParseText(src);
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public static void TestParseRuby6()
    {
        const string src = "青空文庫［＃「青空文庫」の左に「あおぞらぶんこ」のルビ］\r\n";
        const string expected = "青空文庫<span class=\"notes\">［＃「青空文庫」の左に「あおぞらぶんこ」のルビ］</span><br />\r\n";
        var parsed = Helper.ParseText(src);
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public static void TestParseRuby7()
    {
        const string src = "青空文庫《あおぞらぶんこ》［＃「青空文庫」の左に「aozora bunko」のルビ］\r\n";
        const string expected = "<ruby><rb>青空文庫</rb><rp>（</rp><rt>あおぞらぶんこ</rt><rp>）</rp></ruby><span class=\"notes\">［＃「青空文庫」の左に「aozora bunko」のルビ］</span><br />\r\n";
        var parsed = Helper.ParseText(src);
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public static void TestParseRuby8()
    {
        const string src = "大空文庫［＃「大空文庫」に「ママ」の注記］\r\n";
        const string expected = "<ruby><rb>大空文庫</rb><rp>（</rp><rt>ママ</rt><rp>）</rp></ruby><br />\r\n";
        var parsed = Helper.ParseText(src);
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public static void TestParseRuby9()
    {
        const string src = "大空文庫［＃「大空文庫」の左に「ママ」の注記］\r\n";
        const string expected = "大空文庫<span class=\"notes\">［＃「大空文庫」の左に「ママ」の注記］</span><br />\r\n";
        var parsed = Helper.ParseText(src);
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public static void TestParseRuby10()
    {
        const string src = "大空文庫《あおぞらぶんこ》［＃「大空文庫」の左に「ママ」の注記］\r\n";
        const string expected = "<ruby><rb>大空文庫</rb><rp>（</rp><rt>あおぞらぶんこ</rt><rp>）</rp></ruby><span class=\"notes\">［＃「大空文庫」の左に「ママ」の注記］</span><br />\r\n";
        var parsed = Helper.ParseText(src);
        Assert.Equal(expected, parsed);
    }

    [Fact]
    public static void TestParseRuby11()
    {
        const string src = "大空文庫《あおぞらぶんこ》［＃「大空文庫」に「ママ」の注記］\r\n";
        string error = "";
        try
        {
            var parsed = Helper.ParseText(src);
        }
        catch (Aozora.Exceptions.DontUseDoubleRubyException e)
        {
            error=e.Message;
        }
        Assert.Equal("同じ箇所に2つのルビはつけられません", error);
    }

    [Fact]
    public static void TestParseRuby12()
    {
        const string src = "［＃注記付き］名※［＃二の字点、1-2-22］［＃「（銘々）」の注記付き終わり］\r\n";
        const string expected = "<ruby><rb>名<img src=\"../../../gaiji/1-02/1-02-22.png\" alt=\"※(二の字点、1-2-22)\" class=\"gaiji\" /></rb><rp>（</rp><rt>（銘々）</rt><rp>）</rp></ruby><br />\r\n";
        var parsed = Helper.ParseText(src);
        Assert.Equal(expected, parsed);
    }

}

