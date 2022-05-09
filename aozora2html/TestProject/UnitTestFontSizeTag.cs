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

public static class UnitTestFontSizeTag
{
    [Fact]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0150:型のチェックよりも 'null 値' チェックを優先する", Justification = "<保留中>")]
    public static void TestNew()
    {
        var tag = new FontSize(Helper.GetAozora2HtmlPlaceholder(), 1, Aozora2Html.IndentTypeKey.dai);
        Assert.True(tag is FontSize);
        Assert.True(tag is Block);
        Assert.True(tag is IMultiline);
        Assert.True(tag is IHtmlProvider);
    }

    [Fact]
    public static void TestToHtml0()
    {
        string? error_msg = null;
        try
        {
            _ = new FontSize(Helper.GetAozora2HtmlPlaceholder(), 0, Aozora2Html.IndentTypeKey.sho);
        }
        catch (Aozora.Exceptions.InvalidFontSizeException e)
        {
            error_msg = e.Message;
        }
        Assert.Equal("文字サイズの指定が不正です", error_msg);
    }

    [Fact]
    public static void TestToHtml1()
    {
        var tag = new FontSize(Helper.GetAozora2HtmlPlaceholder(), 1, Aozora2Html.IndentTypeKey.dai);
        Assert.Equal("<div class=\"dai1\" style=\"font-size: large;\">", tag.ToHtml());
    }

    [Fact]
    public static void TestToHtml2()
    {
        var tag = new FontSize(Helper.GetAozora2HtmlPlaceholder(), 2, Aozora2Html.IndentTypeKey.dai);
        Assert.Equal("<div class=\"dai2\" style=\"font-size: x-large;\">", tag.ToHtml());
    }

    [Fact]
    public static void TestToHtml3()
    {
        var tag = new FontSize(Helper.GetAozora2HtmlPlaceholder(), 3, Aozora2Html.IndentTypeKey.sho);
        Assert.Equal("<div class=\"sho3\" style=\"font-size: xx-small;\">", tag.ToHtml());
    }

}
