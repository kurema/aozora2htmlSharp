using Xunit;

namespace TestProject;

public static class UnitTestAccentParser
{
    [Fact]
    public static void TestNew()
    {
        var str = "〔e'tiquette〕\r\n";
        using var sr = new System.IO.StringReader(str);
        var stream = new Aozora.Jstream(sr);
        var output = new Aozora.Helpers.OutputString();
        var parsed = new Aozora.Helpers.AccentParser(stream, '〕', new(), new(), output, gaiji_dir: "g_dir/").ProcessAccent().ToHtml();
        Assert.Equal(@"〔<img src=""g_dir/1-09/1-09-63.png"" alt=""※(アキュートアクセント付きE小文字)"" class=""gaiji"" />tiquette", parsed);
    }

    [Fact]
    public static void TestInvalid()
    {
        var str = "〔e'tiquette\r\n";
        using var sr = new System.IO.StringReader(str);
        var stream = new Aozora.Jstream(sr);
        var output = new Aozora.Helpers.OutputString();
        var warn = new Aozora.Helpers.OutputString();
        var parsed = new Aozora.Helpers.AccentParser(stream, '〕', new(), new(), output, warnChannel: warn, gaiji_dir: "g_dir/").ProcessAccent().ToHtml();
        //kurema:元のテストでは行末は"\n"でしたが、こちらでは"\r\n"にしています。
        Assert.Equal("警告(1行目):アクセント分解の亀甲括弧の始めと終わりが、行中で揃っていません\r\n", warn.ToString());
    }

    [Fact]
    public static void TestUseJisx0213()
    {
        var str = "〔e'tiquette〕\r\n";
        using var sr = new System.IO.StringReader(str);
        var stream = new Aozora.Jstream(sr);
        var output = new Aozora.Helpers.OutputString();
        var parsed = new Aozora.Helpers.AccentParser(stream, '〕', new(), new(), output, gaiji_dir: "g_dir/") { UseJisx0213Accent = true }.ProcessAccent().ToHtml();
        Assert.Equal(@"〔&#x00E9;tiquette", parsed);
    }
}
