using Aozora.Helpers;
using Xunit;

namespace TestProject;

public static class UnitTestJIS2UCS
{
    [Fact]
    public static void TestConvert()
    {
        Assert.Null(YamlValues.Jisx0213ToHtmlEntity("3-01-01"));
        Assert.Null(YamlValues.Jisx0213ToHtmlEntity("2-94-87"));
        Assert.Null(YamlValues.Jisx0213ToHtmlEntity("2-94-94"));
        Assert.Null(YamlValues.Jisx0213ToHtmlEntity("1-04-92"));
        Assert.Null(YamlValues.Jisx0213ToHtmlEntity("2-16-01"));
        Assert.Null(YamlValues.Jisx0213ToHtmlEntity("2-16-78"));// kurema:空白期間直前
        Assert.Null(YamlValues.Jisx0213ToHtmlEntity("2-16-79"));// kurema:空白期間開始
        Assert.Null(YamlValues.Jisx0213ToHtmlEntity("2-77-94"));// kurema:空白期間終了
        Assert.Equal("&#x3000;", YamlValues.Jisx0213ToHtmlEntity("1-01-01"));
        Assert.Equal("&#x30AB;&#x309A;", YamlValues.Jisx0213ToHtmlEntity("1-05-87"));//kurema:2文字
        Assert.Equal("&#x0391;", YamlValues.Jisx0213ToHtmlEntity("1-06-01"));//kurema:0始まり
        Assert.Equal("&#x2000B;", YamlValues.Jisx0213ToHtmlEntity("1-14-02")); // kurema:サロゲートペア
        Assert.Equal("&#x289BA;", YamlValues.Jisx0213ToHtmlEntity("2-90-81")); // kurema:サロゲートペア
        Assert.Equal("&#x6B9B;", YamlValues.Jisx0213ToHtmlEntity("2-78-01")); // kurema:空白期間直後
        Assert.Equal("&#x2A6B2;", YamlValues.Jisx0213ToHtmlEntity("2-94-86")); // kurema:最終
    }

    [Fact]
    public static void TestConvert2()
    {
        Assert.Null(YamlValues.Jisx0213ToString("3-01-01"));
        Assert.Null(YamlValues.Jisx0213ToString("2-94-87"));
        Assert.Null(YamlValues.Jisx0213ToString("2-94-94"));
        Assert.Null(YamlValues.Jisx0213ToString("1-04-92"));
        Assert.Null(YamlValues.Jisx0213ToString("2-16-01"));
        Assert.Null(YamlValues.Jisx0213ToString("2-16-78"));// kurema:空白期間直前
        Assert.Null(YamlValues.Jisx0213ToString("2-16-79"));// kurema:空白期間開始
        Assert.Null(YamlValues.Jisx0213ToString("2-77-94"));// kurema:空白期間終了
        Assert.Equal("\u3000", YamlValues.Jisx0213ToString("1-01-01"));
        Assert.Equal("\u30AB\u309A", YamlValues.Jisx0213ToString("1-05-87"));//kurema:2文字
        Assert.Equal("\u0391", YamlValues.Jisx0213ToString("1-06-01"));//kurema:0始まり
        Assert.Equal(char.ConvertFromUtf32(0x2000B), YamlValues.Jisx0213ToString("1-14-02")); // kurema:サロゲートペア
        Assert.Equal(char.ConvertFromUtf32(0x289BA), YamlValues.Jisx0213ToString("2-90-81")); // kurema:サロゲートペア
        Assert.Equal("\u6B9B", YamlValues.Jisx0213ToString("2-78-01")); // kurema:空白期間直後
        Assert.Equal(char.ConvertFromUtf32(0x2A6B2), YamlValues.Jisx0213ToString("2-94-86")); // kurema:最終
    }

    [Fact]
    public static void TestReverse()
    {
        var rev = YamlValues.Jisx0213ReverseDictionary;
        Assert.Equal((1, 2, 56), rev["〘"]);
		Assert.Equal((1, 1, 58), rev["【"]);
		Assert.Equal((1, 5, 84), rev["ヴ"]);
	}
}