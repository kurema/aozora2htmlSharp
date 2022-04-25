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

public static class UnitTestGaijiTag
{
    public const string gaiji_dir = "g_dir/";

    [Fact]
    public static void TestNew()
    {
        var egt = new EmbedGaiji(Helper.GetAozora2HtmlPlaceholder(), "foo", "1-2-3", "name", gaiji_dir);

        //kurema:以下3行は原文にはありません。
        Assert.True(egt is EmbedGaiji);
        Assert.True(egt is Gaiji);
        Assert.True(egt is IHtmlProvider);

        //kurema:to_sは他だとNewじゃなくtest_to_s()みたいな名前の関数で実行されてますが、まぁ何でも良いです。
        Assert.Equal("<img src=\"g_dir/foo/1-2-3.png\" alt=\"※(name)\" class=\"gaiji\" />", egt.to_html());
    }

    [Fact]
    public static void TestNewUnEmbed()
    {
        var egt = new UnEmbedGaiji("テストtest");

        //kurema:以下3行は原文にはありません。
        Assert.True(egt is UnEmbedGaiji);
        Assert.True(egt is Gaiji);
        Assert.True(egt is IHtmlProvider);

        Assert.Equal("<span class=\"notes\">［テストtest］</span>", egt.to_html());
    }

    [Fact]
    public static void TestEscaped()
    {
        var egt = new UnEmbedGaiji("テストtest");

        Assert.False(egt.escaped);
    }

    [Fact]
    public static void TestEscape()
    {
        var egt = new UnEmbedGaiji("テストtest");

        egt.escape();
        Assert.True(egt.escaped);
    }

    [Fact]
    public static void TestJisx0213()
    {
        var parser = Helper.GetAozora2HtmlPlaceholder();
        parser.use_jisx0214_embed_gaiji = true;
        var egt = new EmbedGaiji(parser, "foo", "1-06-75", "snowman", gaiji_dir);
        Assert.Equal("&#x2603;", egt.to_html());
    }

    [Fact]
    public static void TestUnicode()
    {
        var parser = Helper.GetAozora2HtmlPlaceholder();
        parser.use_unicode_embed_gaiji = true;

        {
            var egt = new EmbedGaiji(parser, "foo", "1-06-75", "snowman", gaiji_dir, "2603");
            Assert.Equal("&#x2603;", egt.to_html());
        }

        {
            //kurema:
            //フラグを間違えているケースを想定してテストを追加しました。
            //codeを検証するようになればここで例外が出たりするのは正常です。その場合は以下を削除してください。
            var egt = new EmbedGaiji(parser, "foo", "1-06-75", "snowman", gaiji_dir, "2604");
            Assert.Equal("&#x2604;", egt.to_html());
        }

    }


}
