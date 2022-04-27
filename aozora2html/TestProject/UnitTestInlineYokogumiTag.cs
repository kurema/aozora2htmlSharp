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

public static class UnitTestInlineYokogumiTag
{
    [Fact]
    public static void TestNew()
    {
        var tag = new InlineYokogumi("aaa");
        Assert.True(tag is InlineYokogumi);
        Assert.True(tag is ReferenceMentioned);
        Assert.True(tag is Inline);
        Assert.True(tag is IHtmlProvider);
    }

    [Fact]
    public static void TestToHtml()
    {
        var tag = new InlineYokogumi("テスト");
        Assert.Equal("<span class=\"yokogumi\">テスト</span>", tag.to_html());
    }
}

