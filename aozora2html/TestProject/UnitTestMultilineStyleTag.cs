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
        Assert.Equal("<div class=\"s1\">", tag.to_html());
        Assert.Equal("</div>", tag.close_tag());
    }
}

