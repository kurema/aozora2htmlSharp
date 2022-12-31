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

public static class UnitTestJstream
{
    [Fact]
    public static void TestNewError()
    {
        using var sr = new System.IO.StringReader("aaa\nbbb\n");
        string message = "";
        try
        {
            //kurema:こちらでは指定しないとCRLFチェックはしません。
            var stream = new Jstream(sr, true);
        }
        catch (Aozora.Exceptions.UseCRLFException e)
        {
            message = e.Message;
        }
        //kurema:こちらでは処理を終了しないのでメッセージが変わっています。
        Assert.Equal("改行コードを、「CR+LF」にあらためてください", message);
    }

    [Fact]
    public static void TestReadChar()
    {
        using var sr = new System.IO.StringReader("aあ５\r\n％\\b\r\n");
        var stream = new Jstream(sr);
        Assert.Equal('a', stream.ReadChar());
        Assert.Equal('あ', stream.ReadChar());
        Assert.Equal('５', stream.ReadChar());
        Assert.Equal('\n', stream.ReadChar());//kurema:char?型なので\n。\r\nではない。
        Assert.Equal('％', stream.ReadChar());
        Assert.Equal('\\', stream.ReadChar());
        Assert.Equal('b', stream.ReadChar());
        Assert.Equal('\n', stream.ReadChar());
        Assert.Null(stream.ReadChar());//kurema:endcharではない。
        Assert.Null(stream.ReadChar());//kurema:何度もread_charするとnullが複数回出る。
    }

    [Fact]
    public static void TestPeekChar()
    {
        using var sr = new System.IO.StringReader("aあ５\r\n％\\b\r\n");
        var stream = new Jstream(sr);
        Assert.Equal('a', stream.PeekChar(0));
        Assert.Equal('あ', stream.PeekChar(1));
        Assert.Equal('５', stream.PeekChar(2));
        Assert.Equal('\n', stream.PeekChar(3));
        //改行文字以降は正しい値を保証しない
        Assert.Equal('a', stream.ReadChar());
        Assert.Equal('あ', stream.PeekChar(0));
        Assert.Equal('あ', stream.ReadChar());
        Assert.Equal('５', stream.ReadChar());
        Assert.Equal('\n', stream.ReadChar());
        Assert.Equal('％', stream.PeekChar(0));
        Assert.Equal('\\', stream.PeekChar(1));
        Assert.Equal('b', stream.PeekChar(2));
        Assert.Equal('\n', stream.PeekChar(3));
    }

    [Fact]
    public static void TestReadTo()
    {
        //kurema:以下は原文から追加されたテストです。
        {
            using var sr = new System.IO.StringReader("aあ５\r\n％\\bc\r\n");
            var stream = new Jstream(sr);
            Assert.Equal("aあ５\r\n％\\", stream.ReadTo('b'));
            Assert.Equal(2, stream.Line);
        }
        {
            using var sr = new System.IO.StringReader("a\r\nあ\n５\r％\\bc\r\n");
            var stream = new Jstream(sr, false);
            Assert.Equal("a\r\nあ\r\n５\r\n％\\", stream.ReadTo('b'));
            Assert.Equal(4, stream.Line);
        }
    }

    [Fact]
    public static void TestReplaceReturnCode()
    {
        {
            var (result, line, replaced) = Jstream.ReplaceReturnCode("a\rb\nc\r\nd", Jstream.CRLF);
            Assert.Equal("a\r\nb\r\nc\r\nd", result.ToString());
            Assert.Equal(3, line);
            Assert.True(replaced);
        }
        {
            var (result, line, replaced) = Jstream.ReplaceReturnCode("a\rb\nc\r\nd", Jstream.LF);
            Assert.Equal("a\nb\nc\nd", result.ToString());
            Assert.Equal(3, line);
            Assert.True(replaced);
        }
        {
            var (result, line, replaced) = Jstream.ReplaceReturnCode("a\r\nb\r\nc\r\nd", Jstream.CRLF);
            Assert.Equal("a\r\nb\r\nc\r\nd", result.ToString());
            Assert.Equal(3, line);
            Assert.False(replaced);
        }
        {
            var (result, line, replaced) = Jstream.ReplaceReturnCode("a\nb\nc\nd", Jstream.LF);
            Assert.Equal("a\nb\nc\nd", result.ToString());
            Assert.Equal(3, line);
            Assert.False(replaced);
        }
        {
            var (result, line, replaced) = Jstream.ReplaceReturnCode("a\nb\nc\nd", "/");
            Assert.Equal("a/b/c/d", result.ToString());
            Assert.Equal(3, line);
            Assert.True(replaced);
        }
    }
}