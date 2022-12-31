using Aozora.Helpers;
using Xunit;

namespace TestProject;

public static class UnitTestJIS2UCS
{
    [Fact]
    public static void TestConvert()
    {
        Assert.Null(YamlValues.Jisx0213ToUnicode("3-01-01"));
        Assert.Null(YamlValues.Jisx0213ToUnicode("2-94-94"));
        Assert.Null(YamlValues.Jisx0213ToUnicode("1-04-92"));
        Assert.Equal("&#x3000;", YamlValues.Jisx0213ToUnicode("1-01-01"));
        Assert.Equal("&#x2000B;", YamlValues.Jisx0213ToUnicode("1-14-02")); // サロゲートペア
        Assert.Equal("&#x289BA;", YamlValues.Jisx0213ToUnicode("2-90-81")); // サロゲートペア
    }
}