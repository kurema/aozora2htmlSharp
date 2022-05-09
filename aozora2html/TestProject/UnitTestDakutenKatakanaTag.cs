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

public static class UnitTestDakutenKatakanaTag
{
    private const string Gaiji_dir = "g_dir";

    [Fact]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0150:型のチェックよりも 'null 値' チェックを優先する", Justification = "<保留中>")]
    public static void TestNew()
    {
        var tag = new DakutenKatakana(1, "ア", Gaiji_dir);
        Assert.True(tag is DakutenKatakana);
        Assert.True(tag is Inline);
        Assert.True(tag is IHtmlProvider);
    }

    [Fact]
    public static void TestToHtml()
    {
        var tag = new DakutenKatakana(1, "ア", Gaiji_dir);
        Assert.Equal("<img src=\"g_dir/1-07/1-07-81.png\" alt=\"※(濁点付き片仮名「ア」、1-07-81)\" class=\"gaiji\" />", tag.ToHtml());
    }
}
