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

public static class UnitTestKeigakomiTag
{
    [Fact]
    public static void TestNew()
    {
        var parser = Helper.GetAozora2HtmlPlaceholder();
        var tag = new Keigakomi(parser, 2);
        Assert.True(tag is Keigakomi);
        Assert.True(tag is Block);
        Assert.True(tag is IMultiline);
        Assert.True(tag is IHtmlProvider);
    }

    [Fact]
    public static void TestToHtml1()
    {
        var parser = Helper.GetAozora2HtmlPlaceholder();
        var tag = new Keigakomi(parser);

        Assert.Equal("<div class=\"keigakomi\" style=\"border: solid 1px\">", tag.to_html());
        Assert.Equal("</div>", tag.close_tag());
    }

    [Fact]
    public static void TestToHtml2()
    {
        var parser = Helper.GetAozora2HtmlPlaceholder();
        var tag = new Keigakomi(parser, 2);

        Assert.Equal("<div class=\"keigakomi\" style=\"border: solid 2px\">", tag.to_html());
        Assert.Equal("</div>", tag.close_tag());
    }
}

