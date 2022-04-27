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

public static class UnitTestMultilineYokogumiTag
{
    [Fact]
    public static void TestNew()
    {
        var tag = new MultilineYokogumi(Helper.GetAozora2HtmlPlaceholder());
        Assert.True(tag is MultilineYokogumi);
        Assert.True(tag is Block);
        Assert.True(tag is IMultiline);
        Assert.True(tag is IHtmlProvider);
        Assert.True(tag is IClosable);
    }

    [Fact]
    public static void TestToHtml()
    {
        var tag = new MultilineYokogumi(Helper.GetAozora2HtmlPlaceholder());
        Assert.Equal("<div class=\"yokogumi\">", tag.to_html());
        Assert.Equal("</div>", tag.close_tag());
    }
}

