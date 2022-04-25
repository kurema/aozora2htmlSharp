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

public static class UnitTestDecorateTag
{
    [Fact]
    public static void TestNew()//kurema:コピペが簡単になるように名前を変更しました。他も同様。
    {
        var tag = new Decorate("テスト", "foo", "span");//kurema:原文の引数が明らかに変な型だったので変更しました。
        Assert.True(tag is Decorate);
        Assert.True(tag is Inline);
        Assert.True(tag is IHtmlProvider);
    }

    [Fact]
    public static void TestToHtml()
    {
        var tag = new Decorate("テスト", "foo", "span");

        Assert.Equal("<span class=\"foo\">テスト</span>", tag.to_html());
    }
}
