﻿using System;
using System.Collections.Generic;
using System.Text;


namespace Aozora.Helpers;

public static partial class YamlValues
{
    public static (string? @class, string? tag) CommandTable(string key) => key switch
    {
        "傍点" => ("sesame_dot", "em"),
        "白ゴマ傍点" => ("white_sesame_dot", "em"),
        "丸傍点" => ("black_circle", "em"),
        "白丸傍点" => ("white_circle", "em"),
        "黒三角傍点" => ("black_up-pointing_triangle", "em"),
        "白三角傍点" => ("white_up-pointing_triangle", "em"),
        "二重丸傍点" => ("bullseye", "em"),
        "蛇の目傍点" => ("fisheye", "em"),
        "ばつ傍点" => ("saltire", "em"),
        "傍線" => ("underline_solid", "em"),
        "二重傍線" => ("underline_double", "em"),
        "鎖線" => ("underline_dotted", "em"),
        "破線" => ("underline_dashed", "em"),
        "波線" => ("underline_wave", "em"),
        "太字" => ("futoji", "span"),
        "斜体" => ("shatai", "span"),
        "下付き小文字" => ("subscript", "sub"),
        "上付き小文字" => ("superscript", "sup"),
        "行右小書き" => ("superscript", "sup"),
        "行左小書き" => ("subscript", "sub"),
        _ => (null, null),
    };

}
