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

public static class UnitTestTextFragment
{
    [Fact]
    public static void TestMemory()
    {
        Assert.Null(TextFragmentMemory.Empty.Char);
        Assert.True(TextFragmentMemory.Empty.TryGetAppended(new TextFragmentMemory("Test".AsMemory()), out var fragment2));
        Assert.Equal("Test", fragment2?.ToString());
        Assert.False(fragment2?.TryGetAppended(new TextFragmentMemory("Test".AsMemory()), out _));

        var mem = "<p>Hello, World!</p>".AsMemory();
        var f1= new TextFragmentMemory(mem, 3, 5);
        var f2= new TextFragmentMemory(mem, 9, 1);
        var f3=new TextFragmentMemory(mem, 10, 5);
        Assert.False(f1.TryGetAppended(f2, out _));
        Assert.True(f2.TryGetAppended(f3, out var f4));
        Assert.Equal(" World", f4?.ToString());
    }

    [Fact]
    public static void TestChar()
    {
        Assert.True(TextFragmentMemory.Empty.TryGetAppended(new TextFragmentChar('a'), out var fragment2));
        Assert.Equal("a", fragment2?.ToString());
        Assert.False(fragment2?.TryGetAppended(new TextFragmentMemory("Test".AsMemory()), out _));
    }

    [Fact]
    public static void TestBuilder()
    {
        var builder = new TextFragmentStringBuilder();
        Assert.True(builder.TryGetAppended(TextFragmentMemory.Empty, out _));
        Assert.False(builder.TryGetAppended(new TextFragmentChar(' '), out _));
        var mem = "<p>Hello, World!</p>".AsMemory();
        builder.Append(new TextFragmentMemory(mem, 3, 5));
        builder.Append(new TextFragmentMemory(mem, 9, 1));
        builder.Append(new TextFragmentMemory(mem, 10, 5));
        Assert.Equal("Hello World", builder.ToString());
    }
}
