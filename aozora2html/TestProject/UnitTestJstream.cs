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
        Assert.Equal('a',stream.ReadChar());
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
}

