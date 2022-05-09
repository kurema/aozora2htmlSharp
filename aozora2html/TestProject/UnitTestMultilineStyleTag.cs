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

public static class UnitTestMultilineStyleTag
{
    [Fact]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0150:型のチェックよりも 'null 値' チェックを優先する", Justification = "<保留中>")]
    public static void TestNew()
    {
        var tag = new MultilineStyle(Helper.GetAozora2HtmlPlaceholder(), "style1");
        Assert.True(tag is MultilineStyle);
        Assert.True(tag is Block);
        Assert.True(tag is IMultiline);
        Assert.True(tag is IHtmlProvider);
        Assert.True(tag is IClosable);
    }

    [Fact]
    public static void TestToHtml()
    {
        var tag = new MultilineStyle(Helper.GetAozora2HtmlPlaceholder(), "s1");
        Assert.Equal("<div class=\"s1\">", tag.ToHtml());
        Assert.Equal("</div>", tag.CloseTag());
    }
}

