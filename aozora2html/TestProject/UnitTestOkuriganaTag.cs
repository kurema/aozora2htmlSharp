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

public static class UnitTestOkuriganaTag
{
    [Fact]
    public static void TestOkuriganaNew()
    {
        var tag = new Okurigana("aaa");
        Assert.True(tag is Okurigana);//kurema:これ要る？
        Assert.True(tag is Inline);
    }

    [Fact]
    public static void TestToHtml()
    {
        var tag = new Okurigana("テスト");
        Assert.Equal("<sup class=\"okurigana\">テスト</sup>", tag.to_html());
    }
}
