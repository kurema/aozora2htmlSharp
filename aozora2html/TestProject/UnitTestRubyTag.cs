﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using Aozora;
using Aozora.Helpers;
using Aozora.Helpers.Tag;

namespace TestProject;

public static class UnitTestRubyTag
{
    [Fact]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0150:型のチェックよりも 'null 値' チェックを優先する", Justification = "<保留中>")]
    public static void TestNew()
    {
        var tag = new Ruby("aaa", "bb");
        Assert.True(tag is Ruby);
        Assert.True(tag is Inline);
        Assert.True(tag is ReferenceMentioned);
        Assert.True(tag is IHtmlProvider);
    }

    [Fact]
    public static void TestToHtml()
    {
        var tag = new Ruby("テスト", "てすと");
        Assert.Equal("<ruby><rb>テスト</rb><rp>（</rp><rt>てすと</rt><rp>）</rp></ruby>", tag.ToHtml());
    }
}

