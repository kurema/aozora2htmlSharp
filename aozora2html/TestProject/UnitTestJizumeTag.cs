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

public static class UnitTestJizumeTag
{
    [Fact]
    public static void TestNew()
    {
        var tag = new Jizume(Helper.GetAozora2HtmlPlaceholder(), 50);
        Assert.True(tag is Jizume);
        Assert.True(tag is Block);
        Assert.True(tag is IMultiline);
        Assert.True(tag is IHtmlProvider);
    }

    [Fact]
    public static void TestToHtml()
    {
        var tag = new Jizume(Helper.GetAozora2HtmlPlaceholder(), 50);
        Assert.Equal("<div class=\"jizume_50\" style=\"width: 50em\">", tag.to_html());
        Assert.Equal("</div>", tag.close_tag());
    }
}

