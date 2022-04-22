using System;
using System.Collections.Generic;
using System.Text;


namespace Aozora.Helpers;

public static partial class YamlValues
{
    public static string[]? CommandTable(string key) => key switch
    {
        "傍点" => new string[] { "sesame_dot", "em", },
        "白ゴマ傍点" => new string[] { "white_sesame_dot", "em", },
        "丸傍点" => new string[] { "black_circle", "em", },
        "白丸傍点" => new string[] { "white_circle", "em", },
        "黒三角傍点" => new string[] { "black_up-pointing_triangle", "em", },
        "白三角傍点" => new string[] { "white_up-pointing_triangle", "em", },
        "二重丸傍点" => new string[] { "bullseye", "em", },
        "蛇の目傍点" => new string[] { "fisheye", "em", },
        "ばつ傍点" => new string[] { "saltire", "em", },
        "傍線" => new string[] { "underline_solid", "em", },
        "二重傍線" => new string[] { "underline_double", "em", },
        "鎖線" => new string[] { "underline_dotted", "em", },
        "破線" => new string[] { "underline_dashed", "em", },
        "波線" => new string[] { "underline_wave", "em", },
        "太字" => new string[] { "futoji", "span", },
        "斜体" => new string[] { "shatai", "span", },
        "下付き小文字" => new string[] { "subscript", "sub", },
        "上付き小文字" => new string[] { "superscript", "sup", },
        "行右小書き" => new string[] { "superscript", "sup", },
        "行左小書き" => new string[] { "subscript", "sub", },
        _ => null,
    };

}
