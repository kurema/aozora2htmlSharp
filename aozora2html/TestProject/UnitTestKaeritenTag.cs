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

public static class UnitTestKaeritenTag
{
    [Fact]
    public static void TestNew()
    {
        var tag = new Kaeriten("aaa");
        Assert.True(tag is Kaeriten);
        Assert.True(tag is Kunten);
        Assert.True(tag is Inline);
        Assert.True(tag is IHtmlProvider);
    }

    [Fact]
    public static void TestToHtml()
    {
        var tag = new Kaeriten("テスト");
        Assert.Equal("<sub class=\"kaeriten\">テスト</sub>", tag.to_html());
    }
}

