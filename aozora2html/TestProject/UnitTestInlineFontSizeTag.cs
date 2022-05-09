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

public static class UnitTestInlineFontSizeTag
{
    [Fact]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0150:型のチェックよりも 'null 値' チェックを優先する", Justification = "<保留中>")]
    public static void TestNew()
    {
        var tag = new InlineFontSize("aa", 1, Aozora2Html.IndentTypeKey.dai);
        Assert.True(tag is InlineFontSize);
        Assert.True(tag is ReferenceMentioned);
        Assert.True(tag is Inline);
        Assert.True(tag is IHtmlProvider);
    }

    [Fact]
    public static void TestToHtml1()
    {
        var tag = new InlineFontSize("テスト", 1, Aozora2Html.IndentTypeKey.dai);
        Assert.Equal("<span class=\"dai1\" style=\"font-size: large;\">テスト</span>", tag.ToHtml());
    }

    [Fact]
    public static void TestToHtml2()
    {
        var tag = new InlineFontSize("テスト", 2, Aozora2Html.IndentTypeKey.sho);
        Assert.Equal("<span class=\"sho2\" style=\"font-size: x-small;\">テスト</span>", tag.ToHtml());
    }

    [Fact]
    public static void TestToHtml3()
    {
        var tag = new InlineFontSize("テスト", 3, Aozora2Html.IndentTypeKey.sho);
        Assert.Equal("<span class=\"sho3\" style=\"font-size: xx-small;\">テスト</span>", tag.ToHtml());
    }
}
