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

public static class UnitTestHeader
{
    [Fact]
    public static void TestHeaderToHtml()
    {
        var header = new Header(new[] { "../../aozora.css" });
        header.push("武装せる市街");
        header.push("黒島伝治");
        var actual = header.to_html();
        const string expected = "<?xml version=\"1.0\" encoding=\"Shift_JIS\"?>\r\n<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\"\r\n    \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"ja\" >\r\n<head>\r\n\t<meta http-equiv=\"Content-Type\" content=\"text/html;charset=Shift_JIS\" />\r\n\t<meta http-equiv=\"content-style-type\" content=\"text/css\" />\r\n\t<link rel=\"stylesheet\" type=\"text/css\" href=\"../../aozora.css\" />\r\n\t<title>黒島伝治 武装せる市街</title>\r\n\t<script type=\"text/javascript\" src=\"../../jquery-1.4.2.min.js\"></script>\r\n  <link rel=\"Schema.DC\" href=\"http://purl.org/dc/elements/1.1/\" />\r\n\t<meta name=\"DC.Title\" content=\"武装せる市街\" />\r\n\t<meta name=\"DC.Creator\" content=\"黒島伝治\" />\r\n\t<meta name=\"DC.Publisher\" content=\"青空文庫\" />\r\n</head>\r\n<body>\r\n<div class=\"metadata\">\r\n<h1 class=\"title\">武装せる市街</h1>\r\n<h2 class=\"author\">黒島伝治</h2>\r\n<br />\r\n<br />\r\n</div>\r\n<div id=\"contents\" style=\"display:none\"></div><div class=\"main_text\">";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public static void TestBuildTitle()
    {
        var header = new Header(new[] { "../../aozora.css" });
        header.push("武装せる市街");
        header.push("黒島伝治");
        var header_info = header.build_header_info();
        var actual = header.build_title(header_info);
        const string expected = "<title>黒島伝治 武装せる市街</title>";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public static void TestBuildTitle2()
    {
        var header = new Header(new[] { "../../aozora.css" });
        header.push("スリーピー・ホローの伝説");
        header.push("故ディードリッヒ・ニッカボッカーの遺稿より");
        header.push("ワシントン・アーヴィング　Washington Irving");
        header.push("吉田甲子太郎訳");
        var header_info = header.build_header_info();
        var actual = header.build_title(header_info);
        const string expected = "<title>ワシントン・アーヴィング　Washington Irving 吉田甲子太郎訳 スリーピー・ホローの伝説 故ディードリッヒ・ニッカボッカーの遺稿より</title>";
        Assert.Equal(expected, actual);
    }
}
