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

public static class UnitTestImgTag
{
    [Fact]
    public static void TestNew()
    {
        var tag = new Img("foo.png", "img1", "alt img1", "40", "50");
        Assert.True(tag is Img);
        Assert.True(tag is Inline);
        Assert.True(tag is IHtmlProvider);
    }

    [Fact]
    public static void TestToHtml()
    {
        var tag = new Img("foo.png", "img1", "alt img1", "40", "50");
        Assert.Equal("<img class=\"img1\" width=\"40\" height=\"50\" src=\"foo.png\" alt=\"alt img1\" />",tag.to_html());
    }
}
