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

public static class UnitTestMidashiTag
{
    [Fact]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0150:型のチェックよりも 'null 値' チェックを優先する", Justification = "<保留中>")]
    public static void TestNew()
    {
        var tag = new Midashi(new Helper.MidashiIdProviderPlaceholder(2, true), "テスト見出し", "小", Utils.MidashiType.normal);
        Assert.True(tag is Midashi);
        Assert.True(tag is Inline);
        Assert.True(tag is IHtmlProvider);
    }

    [Fact]
    public static void TestToHtml()
    {
        var tag = new Midashi(new Helper.MidashiIdProviderPlaceholder(2, true), "テスト見出し", "小", Utils.MidashiType.normal);

        Assert.Equal("<h5 class=\"ko-midashi\"><a class=\"midashi_anchor\" id=\"midashi2\">テスト見出し</a></h5>", tag.ToHtml());
    }

    [Fact]
    public static void TestToHtmlMado()
    {
        var tag = new Midashi(new Helper.MidashiIdProviderPlaceholder(2, true), "テスト見出し", "小", Utils.MidashiType.mado);

        Assert.Equal("<h5 class=\"mado-ko-midashi\"><a class=\"midashi_anchor\" id=\"midashi2\">テスト見出し</a></h5>", tag.ToHtml());
    }

    [Fact]
    public static void TestToHtmlMidashi()
    {
        string error = "";
        try
        {
            var tag = new Midashi(new Helper.MidashiIdProviderPlaceholder(2, true), "テスト見出し", "あ", Utils.MidashiType.mado);
        }
        catch (Aozora.Exceptions.UndefinedHeaderException e)
        {
            error = e.Message;
        }
        Assert.Equal("未定義な見出しです", error);
    }
}
