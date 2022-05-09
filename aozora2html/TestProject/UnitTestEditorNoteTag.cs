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

public static class UnitTestEditorNoteTag
{
    [Fact]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0150:型のチェックよりも 'null 値' チェックを優先する", Justification = "<保留中>")]
    public static void TestNew()
    {
        var tag = new EditorNote("注記のテスト");
        Assert.True(tag is EditorNote);
        Assert.True(tag is Inline);
        Assert.True(tag is IHtmlProvider);
    }

    [Fact]
    public static void TestToHtml()
    {
        var tag = new EditorNote("注記のテスト");
        Assert.Equal("<span class=\"notes\">［＃注記のテスト］</span>", tag.ToHtml());
    }
}
