using Xunit;
using Aozora;

namespace TestProject;

public static class UnitTestJstreamString
{
    [Fact]
    public static void TestNewError()
    {
        string message = "";
        try
        {
            //kurema:こちらでは指定しないとCRLFチェックはしません。
            var stream = new JstreamString("aaa\nbbb\n", true);
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
        var stream = new JstreamString("aあ５\r\n％\\b\r\n");
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
        var stream = new JstreamString("aあ５\r\n％\\b\r\n");
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
        {
            var stream = new JstreamString("aあ５\r\n％\\bc\r\n");
            Assert.Equal("aあ５\r\n％\\", stream.ReadTo('b').ToString());
            Assert.Equal(2, stream.Line);
        }
        {
            var stream = new JstreamString("a\r\nあ\n５\r％\\bc\r\n",false);
            Assert.Equal("a\r\nあ\r\n５\r\n％\\", stream.ReadTo('b').ToString());
            Assert.Equal(4, stream.Line);
        }
    }
}