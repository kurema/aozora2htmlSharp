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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0150:型のチェックよりも 'null 値' チェックを優先する", Justification = "<保留中>")]
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
        Assert.Equal("<div class=\"yokogumi\">", tag.ToHtml());
        Assert.Equal("</div>", tag.CloseTag());
    }
}

