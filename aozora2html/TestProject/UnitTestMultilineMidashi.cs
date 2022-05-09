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

public static class UnitTestMultilineMidashi
{
    [Fact]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0150:型のチェックよりも 'null 値' チェックを優先する", Justification = "<保留中>")]
    public static void TestNew()
    {
        var tag = new MultilineMidashi(new Helper.MidashiIdProviderPlaceholder(2, true), "小", Utils.MidashiType.normal);
        Assert.True(tag is MultilineMidashi);
        Assert.True(tag is Block);
        Assert.True(tag is IMultiline);
        Assert.True(tag is IHtmlProvider);
        Assert.True(tag is IClosable);
    }

    [Fact]
    public static void TestToHtml()
    {
        var tag = new MultilineMidashi(new Helper.MidashiIdProviderPlaceholder(2, true), "小", Utils.MidashiType.normal);
        Assert.Equal("<h5 class=\"ko-midashi\"><a class=\"midashi_anchor\" id=\"midashi2\">", tag.ToHtml());
        Assert.Equal("</a></h5>", tag.CloseTag());
    }

    [Fact]
    public static void TestToHtmlChu()
    {
        var tag = new MultilineMidashi(new Helper.MidashiIdProviderPlaceholder(2, true), "中", Utils.MidashiType.dogyo);
        Assert.Equal("<h4 class=\"dogyo-naka-midashi\"><a class=\"midashi_anchor\" id=\"midashi2\">", tag.ToHtml());
        Assert.Equal("</a></h4>", tag.CloseTag());
    }

    [Fact]
    public static void TestToHtmlDai()
    {
        var tag = new MultilineMidashi(new Helper.MidashiIdProviderPlaceholder(2, true), "大", Utils.MidashiType.mado);
        Assert.Equal("<h3 class=\"mado-o-midashi\"><a class=\"midashi_anchor\" id=\"midashi2\">", tag.ToHtml());
        Assert.Equal("</a></h3>", tag.CloseTag());
    }

    [Fact]
    public static void TestUndefinedMidashi1()
    {
        string error = "";
        try
        {
            var _ = new MultilineMidashi(new Helper.MidashiIdProviderPlaceholder(2, true), "あ", Utils.MidashiType.mado);
        }
        catch (Aozora.Exceptions.UndefinedHeaderException e)
        {
            error = e.Message;
        }
        Assert.Equal("未定義な見出しです", error);
    }

    [Fact]
    public static void TestUndefinedMidashi2()
    {
        string error = "";
        try
        {
            //kurema:C#のenumは基本内部的にintなので範囲外の値も設定できます。
            var _ = new MultilineMidashi(new Helper.MidashiIdProviderPlaceholder(2, true), "大", (Utils.MidashiType)12345);
        }
        catch (Aozora.Exceptions.UndefinedHeaderException e)
        {
            error = e.Message;
        }
        Assert.Equal("未定義な見出しです", error);
    }
}

